using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using UnityEngine;
using UnityEngine.AI;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace NutrientPasteTiers
{
    [StaticConstructorOnStartup]
    internal static class NPDHarmony
    {
        static NPDHarmony()
        {
            var harmony = new Harmony("rimworld.smashphil.npdtiers");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            #region FunctionDefs
            //Building
            harmony.Patch(original: AccessTools.Property(type: typeof(Building_NutrientPasteDispenser), name: nameof(Building_NutrientPasteDispenser.DispensableDef)).GetGetMethod(),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(DispensableDefCustom)));
            /*BUG FIX: CUSTOM NPDs DONT UPDATE PRISONER COLOR */
            /*FUTURE IMPLEMENTATION ... see if thingclass override works */
            harmony.Patch(original: AccessTools.Method(type: typeof(Building_NutrientPasteDispenser), name: nameof(Building_NutrientPasteDispenser.TryDispenseFood)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(TryDispenseCustomFood)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Building_NutrientPasteDispenser), name: nameof(Building_NutrientPasteDispenser.FindFeedInAnyHopper)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(FindFeedInAnyCustomHopper)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Building_NutrientPasteDispenser), name: nameof(Building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(HasEnoughFeedstockInCustomHoppers)));

            //Jobs
            harmony.Patch(original: AccessTools.Method(type: typeof(JobDriver_FoodDeliver), name: nameof(JobDriver_FoodDeliver.GetReport)),
                prefix: null,
                postfix: new HarmonyMethod(typeof(NPDHarmony), nameof(GetReportModified)));

            //Custom Hoppers
            harmony.Patch(original: AccessTools.Method(type: typeof(CompRottable), name: "Tick"),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(HopperTick)));
            harmony.Patch(original: AccessTools.Method(type: typeof(CompRottable), name: nameof(CompRottable.CompInspectStringExtra)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(CompInspectStringCustomHopper)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Building_NutrientPasteDispenser), name: nameof(Building_NutrientPasteDispenser.AdjacentReachableHopper)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(AdjacentReachableHopperCustom)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Alert_PasteDispenserNeedsHopper), name: nameof(Alert_PasteDispenserNeedsHopper.GetReport)),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(BadDispenserReportModified)));
            harmony.Patch(original: AccessTools.Property(type: typeof(WorkGiver_CookFillHopper), name: nameof(WorkGiver_CookFillHopper.PotentialWorkThingRequest)).GetGetMethod(),
                prefix: new HarmonyMethod(typeof(NPDHarmony), nameof(PotentialWorkThingRequest_CustomHoppers)));
            #endregion #FunctionDefs
        }

        #region Prefixes
        public static bool DispensableDefCustom(ref ThingDef __result, Building_NutrientPasteDispenser __instance)
        {
            if(__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                __result = __instance.def.GetModExtension<NutrientPasteCustom>().customMeal;
                return false;
            }
            return true;
        }

        public static bool TryDispenseCustomFood(ref Thing __result, Building_NutrientPasteDispenser __instance, ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                if (!__instance.CanDispenseNow)
                {
                    __result = null;
                    return false;
                }
                List<ThingDef> list = new List<ThingDef>();
                List<IngredientAndCostClass> ingredientList = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList;
                bool empty = !__instance.def.GetModExtension<NutrientPasteCustom>().ingredientList.Any();
                Log.Message("-" + empty);
                if (!empty)
                {            
                    float[] nutritionLeft = new float[ingredientList.Count];
                    for (int i = 0; i < nutritionLeft.Length; i++)
                    {
                        nutritionLeft[i] = ingredientList[i].nutritionCost;
                    }
                    for (; ;)
                    {
                        Thing thing = __instance.def.GetModExtension<NutrientPasteCustom>().FindNextIngredientInHopper(___cachedAdjCellsCardinal, __instance, nutritionLeft);
                        if (thing is null) break;
                        int index = ingredientList.FindIndex(x => x.thingDef == thing.def);
                        int num2 = Mathf.Min(thing.stackCount, Mathf.CeilToInt(nutritionLeft[index] / thing.GetStatValue(StatDefOf.Nutrition, true)));
                        nutritionLeft[index] -= (float)num2 * thing.GetStatValue(StatDefOf.Nutrition, true);
                        list.Add(thing.def);
                        thing.SplitOff(num2);
                        if(!nutritionLeft.Any(x => x > 0f))
                        {
                            goto Block_3;
                        }
                    }
                }
                else
                {
                    Log.Message("2");
                    float num = __instance.def.building.nutritionCostPerDispense - 0.0001f;
                    for (; ; )
                    {
                        Thing thing = __instance.FindFeedInAnyHopper();
                        if (thing is null)
                        {
                            break;
                        }
                        int num2 = Mathf.Min(thing.stackCount, Mathf.CeilToInt(num / thing.GetStatValue(StatDefOf.Nutrition, true)));
                        num -= (float)num2 * thing.GetStatValue(StatDefOf.Nutrition, true);
                        list.Add(thing.def);
                        thing.SplitOff(num2);
                        if (num <= 0f)
                        {
                            goto Block_3;
                        }
                    }
                }
                
                Log.Error("Did not find enough food in hoppers while trying to dispense.", false);
                __result = null;
                return false;

                Block_3:
                __instance.def.building.soundDispense.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map, false));
                Thing thing2 = ThingMaker.MakeThing(__instance.def.GetModExtension<NutrientPasteCustom>().customMeal, null);
                CompIngredients compIngredients = thing2.TryGetComp<CompIngredients>();
                foreach(ThingDef ingredient in list)
                {
                    if(!__instance.def.GetModExtension<NutrientPasteCustom>().mysteryIngredients)
                    {
                        compIngredients.RegisterIngredient(ingredient);
                    }
                    
                }
                __result = thing2;
                return false;
            }
            return true;
        }

        private static bool HopperTick(int interval, CompRottable __instance)
        {
            IEnumerable<Thing> things = GetCompRottableThings(__instance);

            if (!(things is null) && things.OfType<NPDHopper_Storage>().Any())
            {
                Thing t = things.OfType<NPDHopper_Storage>().First();
                foreach (Thing thing in things)
                {
                    NPDHopper_Storage hopper = thing as NPDHopper_Storage;
                    if (!(hopper is null))
                    {
                        float rotProgress = __instance.RotProgress;
                        float hopperTemp = t.TryGetComp<CompPowerTrader>().PowerOn ? hopper.def.GetModExtension<HopperCustom>().setTemperature : __instance.parent.AmbientTemperature;
                        float num = GenTemperature.RotRateAtTemperature(hopperTemp);
                        __instance.RotProgress += num * (float)interval;

                        if(__instance.Stage == RotStage.Rotting && __instance.PropsRot.rotDestroys)
                        {
                            if(__instance.parent.IsInAnyStorage() && __instance.parent.SpawnedOrAnyParentSpawned)
                            {
                                Messages.Message("MessageRottedAwayInStorage".Translate(__instance.parent.Label, __instance.parent).CapitalizeFirst(), 
                                    new TargetInfo(__instance.parent.PositionHeld, __instance.parent.MapHeld, false), MessageTypeDefOf.NegativeEvent, true);
                                    LessonAutoActivator.TeachOpportunity(ConceptDefOf.SpoilageAndFreezers, OpportunityType.GoodToKnow);
                            }
                            __instance.parent.Destroy(DestroyMode.Vanish);
                            return false;
                        }
                        bool flag = Mathf.FloorToInt(rotProgress / 60000f) != Mathf.FloorToInt(__instance.RotProgress / 60000f);
                        //Should Take Rot Damage?
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CompInspectStringCustomHopper(CompRottable __instance, ref string __result)
        {
            IEnumerable<Thing> things = GetCompRottableThings(__instance);
            
            if(things.OfType<NPDHopper_Storage>().Any())
            {
                Thing t = things.OfType<NPDHopper_Storage>().First();
                StringBuilder stringBuilder = new StringBuilder();
                RotStage stage = __instance.Stage;
                if(stage != RotStage.Fresh)
                {
                    if(stage != RotStage.Rotting)
                    {
                        if(stage == RotStage.Dessicated)
                        {
                            stringBuilder.Append("RotStateDessicated".Translate() + ".");
                        }
                    }
                    else
                    {
                        stringBuilder.Append("RotStateRotting".Translate() + ".");
                    }
                }
                else
                {
                    stringBuilder.Append("RotStateFresh".Translate() + ".");
                }
                float num = (float)__instance.PropsRot.TicksToRotStart - __instance.RotProgress;
                if(num > 0f)
                {
                    float num2 = t.TryGetComp<CompPowerTrader>().PowerOn ? (float)Mathf.RoundToInt(things.OfType<NPDHopper_Storage>().First().def.GetModExtension<HopperCustom>().setTemperature) 
                        : __instance.parent.AmbientTemperature;
                    float num3 = GenTemperature.RotRateAtTemperature(num2);
                    int ticksUntilRotAtCurrentTemp = TicksUntilRotAtSetTemp(__instance, num2);
                    stringBuilder.AppendLine();
                    if(num3 < 0.001f)
                    {
                        stringBuilder.Append("CurrentlyFrozen".Translate() + ".");
                    }
                    else if(num3 < 0.999f)
                    {
                        stringBuilder.Append("CurrentlyRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) + ".");
                    }
                    else
                    {
                        stringBuilder.Append("NotRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) + ".");
                    }
                }
                __result = stringBuilder.ToString();
                return false;
            }
            return true;
        }

        //Needs Work
        public static bool PotentialWorkThingRequest_CustomHoppers(ref ThingRequest __result)
        {
            List<NPDHopper_Storage> allCustomHoppers = Find.CurrentMap.listerBuildings.AllBuildingsColonistOfClass<NPDHopper_Storage>().ToList();
            return true;
        }

        public static bool FindFeedInAnyCustomHopper(Building_NutrientPasteDispenser __instance, ref Thing __result, ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (___cachedAdjCellsCardinal is null)
            {
                ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                                               where c.InBounds(__instance.Map)
                                               select c).ToList<IntVec3>();
            }
            foreach (IntVec3 c in ___cachedAdjCellsCardinal)
            {
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = c.GetThingList(__instance.Map);
                foreach(Thing t in thingList)
                {
                    if(Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def))
                    {
                        thing = t;
                    }
                    if(t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage))
                    {
                        thing2 = t;
                    }
                }
                if(thing != null && thing2 != null)
                {
                    __result = thing;
                    return false;
                }
            }
            __result = null;
            return true;
        }

        public static bool HasEnoughFeedstockInCustomHoppers(ref bool __result, Building_NutrientPasteDispenser __instance, ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (!(__instance.def.GetModExtension<NutrientPasteCustom>() is null))
            {
                if (___cachedAdjCellsCardinal is null)
                {
                    ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                                                 where c.InBounds(__instance.Map)
                                                 select c).ToList<IntVec3>();
                }
                
                float num = 0f;
                bool empty = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList is null ||
                    !__instance.def.GetModExtension<NutrientPasteCustom>().ingredientList.Any();
                if (!empty)
                {
                    __result = false;
                    List<IngredientAndCostClass> ingredientList = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList;
                    bool[] ingredientFulfilled = new bool[ingredientList.Count];
                    float[] nutritionCost = new float[ingredientList.Count];
                    foreach (IntVec3 c in ___cachedAdjCellsCardinal)
                    {
                        Thing thing = null;
                        Thing thing2 = null;
                        List<Thing> thingList = c.GetThingList(__instance.Map);
                        foreach (Thing t in thingList)
                        {
                            if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def) && ingredientList.Any(x => x.thingDef == t.def) &&
                                nutritionCost[ingredientList.FindIndex(x => x.thingDef == t.def)] <= 0f)
                            {
                                thing = t;
                            }
                            if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage))
                            {
                                thing2 = t;
                            }
                        }
                        if(!(thing is null) && !(thing2 is null))
                        {
                            nutritionCost[ingredientList.FindIndex(x => x.thingDef == thing.def)] += (float)thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition, true);
                            if(nutritionCost[ingredientList.FindIndex(x => x.thingDef == thing.def)] >= ingredientList.Find(x => x.thingDef == thing.def).nutritionCost)
                            {
                                ingredientFulfilled[ingredientList.FindIndex(x => x.thingDef == thing.def)] = true;
                            }
                        }
                        
                        if(ingredientFulfilled.All(x => x))
                        {
                            __result = true;
                            return false;
                        }
                    }
                    return false;
                }
                foreach (IntVec3 c in ___cachedAdjCellsCardinal)
                {
                    Thing thing = null;
                    Thing thing2 = null;
                    List<Thing> thingList = c.GetThingList(__instance.Map);
                    foreach (Thing t in thingList)
                    {
                        if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def))
                        {
                            thing = t;
                        }
                        if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage))
                        {
                            thing2 = t;
                        }
                    }
                    if (thing != null && thing2 != null)
                    {
                        num += (float)thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition, true);
                    }
                    if (num >= __instance.def.building.nutritionCostPerDispense)
                    {
                        __result = true;
                        return false;
                    }
                }
                __result = false;
                return false;
            }
            return true;
        }

        public static bool AdjacentReachableHopperCustom(Pawn reacher, Building_NutrientPasteDispenser __instance, ref Building __result,
            List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                if (___cachedAdjCellsCardinal is null)
                {
                    ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                                                 where c.InBounds(__instance.Map)
                                                 select c).ToList<IntVec3>();
                }
                foreach (IntVec3 c in ___cachedAdjCellsCardinal)
                {
                    Building edifice = c.GetEdifice(__instance.Map);

                    if (!(edifice is null) && (edifice.def == ThingDefOf.Hopper || edifice.def.thingClass == typeof(NPDHopper_Storage)) &&
                        reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
                    {
                        __result = edifice;
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion Prefixes

        #region Postfixes
        public static void GetReportModified(ref string __result, JobDriver_FoodDeliver __instance)
        {
            Thing targetBuilding = __instance.job.GetTarget(TargetIndex.A).Thing;
            Pawn deliveree = (Pawn)__instance.job.targetB.Thing;
            if (targetBuilding is Building_NutrientPasteDispenser && deliveree != null)
            {
                __result = __instance.job.def.reportString.Replace("TargetA", targetBuilding.def.GetModExtension<NutrientPasteCustom>().customMeal.label)
                    .Replace("TargetB", deliveree.LabelShort);
            }
            //__result = base.GetReport();
        }

        public static bool BadDispenserReportModified(ref AlertReport __result, Alert_PasteDispenserNeedsHopper __instance)
        {
            __result = AlertReport.CulpritsAre(GetBadDispensers);
            return false;
        }

        #endregion Postfixes

        #region Transpilers

        #endregion Transpilers

        #region HelperFunctions
        public static IEnumerable<Thing> GetCompRottableThings(CompRottable instance)
        {
            return instance.parent?.Map?.thingGrid.ThingsAt(instance.parent.Position); ;
        }

        public static List<Thing> GetBadDispensers
        {
            get
            {
                var dispensers = new List<Thing>();
                foreach (Map map in Find.Maps)
                {
                    foreach (Building dispenser in map.listerBuildings.allBuildingsColonist)
                    {
                        if (dispenser.def.IsFoodDispenser)
                        {
                            bool success = false;
                            foreach (IntVec3 c in GenAdj.CellsAdjacentCardinal(dispenser))
                            {
                                if (c.InBounds(map))
                                {
                                    Thing building = c.GetEdifice(dispenser.Map);
                                    if (!(building is null) && (building.def == ThingDefOf.Hopper || (building.def.thingClass == typeof(NPDHopper_Storage))))
                                    {
                                        success = true;
                                        break;
                                    }
                                }
                            }
                            if(!success)
                            {
                                dispensers.Add(dispenser);
                            }
                        }
                    }
                }
                return dispensers;
            }
        }

        public static int TicksUntilRotAtSetTemp(CompRottable instance, float temp)
        {
            temp = (float)Mathf.RoundToInt(temp);
            return instance.TicksUntilRotAtTemp(temp);
        }
        #endregion HelperFunctions
    }
}
