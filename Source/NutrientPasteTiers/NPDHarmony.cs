using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace NutrientPasteTiers
{
    [StaticConstructorOnStartup]
    internal static class NPDHarmony
    {
        static NPDHarmony()
        {
            var harmony = new Harmony("rimworld.smashphil.npdtiers");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //Building
            harmony.Patch(
                AccessTools.Property(typeof(Building_NutrientPasteDispenser),
                    nameof(Building_NutrientPasteDispenser.DispensableDef)).GetGetMethod(),
                new HarmonyMethod(typeof(NPDHarmony), nameof(DispensableDefCustom)));
            /*BUG FIX: CUSTOM NPDs DONT UPDATE PRISONER COLOR */
            /*FUTURE IMPLEMENTATION ... see if thingclass override works */
            harmony.Patch(
                AccessTools.Method(typeof(Building_NutrientPasteDispenser),
                    nameof(Building_NutrientPasteDispenser.TryDispenseFood)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(TryDispenseCustomFood)));
            harmony.Patch(
                AccessTools.Method(typeof(Building_NutrientPasteDispenser),
                    nameof(Building_NutrientPasteDispenser.FindFeedInAnyHopper)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(FindFeedInAnyCustomHopper)));
            harmony.Patch(
                AccessTools.Method(typeof(Building_NutrientPasteDispenser),
                    nameof(Building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(HasEnoughFeedstockInCustomHoppers)));

            //Jobs
            harmony.Patch(AccessTools.Method(typeof(JobDriver_FoodDeliver), nameof(JobDriver_FoodDeliver.GetReport)),
                null,
                new HarmonyMethod(typeof(NPDHarmony), nameof(GetReportModified)));

            //Custom Hoppers
            harmony.Patch(AccessTools.Method(typeof(CompRottable), "Tick"),
                new HarmonyMethod(typeof(NPDHarmony), nameof(HopperTick)));
            harmony.Patch(AccessTools.Method(typeof(CompRottable), nameof(CompRottable.CompInspectStringExtra)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(CompInspectStringCustomHopper)));
            harmony.Patch(
                AccessTools.Method(typeof(Building_NutrientPasteDispenser),
                    nameof(Building_NutrientPasteDispenser.AdjacentReachableHopper)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(AdjacentReachableHopperCustom)));
            harmony.Patch(
                AccessTools.Method(typeof(Alert_PasteDispenserNeedsHopper),
                    nameof(Alert_PasteDispenserNeedsHopper.GetReport)),
                new HarmonyMethod(typeof(NPDHarmony), nameof(BadDispenserReportModified)));
            harmony.Patch(
                AccessTools.Property(typeof(WorkGiver_CookFillHopper),
                    nameof(WorkGiver_CookFillHopper.PotentialWorkThingRequest)).GetGetMethod(),
                new HarmonyMethod(typeof(NPDHarmony), nameof(PotentialWorkThingRequest_CustomHoppers)));
        }

        public static List<Thing> GetBadDispensers
        {
            get
            {
                var dispensers = new List<Thing>();
                foreach (var map in Find.Maps)
                {
                    foreach (var dispenser in map.listerBuildings.allBuildingsColonist)
                    {
                        if (!dispenser.def.IsFoodDispenser)
                        {
                            continue;
                        }

                        var success = false;
                        foreach (var c in GenAdj.CellsAdjacentCardinal(dispenser))
                        {
                            if (!c.InBounds(map))
                            {
                                continue;
                            }

                            Thing building = c.GetEdifice(dispenser.Map);
                            if (building is null || building.def != ThingDefOf.Hopper &&
                                building.def.thingClass != typeof(NPDHopper_Storage))
                            {
                                continue;
                            }

                            success = true;
                            break;
                        }

                        if (!success)
                        {
                            dispensers.Add(dispenser);
                        }
                    }
                }

                return dispensers;
            }
        }

        public static bool DispensableDefCustom(ref ThingDef __result, Building_NutrientPasteDispenser __instance)
        {
            if (!__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                return true;
            }

            __result = __instance.def.GetModExtension<NutrientPasteCustom>().customMeal;
            return false;
        }

        public static bool TryDispenseCustomFood(ref Thing __result, Building_NutrientPasteDispenser __instance,
            ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (!__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                return true;
            }

            if (!__instance.CanDispenseNow)
            {
                __result = null;
                return false;
            }

            var list = new List<ThingDef>();
            var ingredientList = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList;
            var empty = !__instance.def.GetModExtension<NutrientPasteCustom>().ingredientList.Any();
            Log.Message("-" + empty);
            if (!empty)
            {
                var nutritionLeft = new float[ingredientList.Count];
                for (var i = 0; i < nutritionLeft.Length; i++)
                {
                    nutritionLeft[i] = ingredientList[i].nutritionCost;
                }

                for (;;)
                {
                    var thing = __instance.def.GetModExtension<NutrientPasteCustom>()
                        .FindNextIngredientInHopper(___cachedAdjCellsCardinal, __instance, nutritionLeft);
                    if (thing is null)
                    {
                        break;
                    }

                    var index = ingredientList.FindIndex(x => x.thingDef == thing.def);
                    var num2 = Mathf.Min(thing.stackCount,
                        Mathf.CeilToInt(nutritionLeft[index] / thing.GetStatValue(StatDefOf.Nutrition)));
                    nutritionLeft[index] -= num2 * thing.GetStatValue(StatDefOf.Nutrition);
                    list.Add(thing.def);
                    thing.SplitOff(num2);
                    if (!nutritionLeft.Any(x => x > 0f))
                    {
                        goto Block_3;
                    }
                }
            }
            else
            {
                Log.Message("2");
                var num = __instance.def.building.nutritionCostPerDispense - 0.0001f;
                for (;;)
                {
                    var thing = __instance.FindFeedInAnyHopper();
                    if (thing is null)
                    {
                        break;
                    }

                    var num2 = Mathf.Min(thing.stackCount,
                        Mathf.CeilToInt(num / thing.GetStatValue(StatDefOf.Nutrition)));
                    num -= num2 * thing.GetStatValue(StatDefOf.Nutrition);
                    list.Add(thing.def);
                    thing.SplitOff(num2);
                    if (num <= 0f)
                    {
                        goto Block_3;
                    }
                }
            }

            Log.Error("Did not find enough food in hoppers while trying to dispense.");
            __result = null;
            return false;

            Block_3:
            __instance.def.building.soundDispense.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map));
            var thing2 = ThingMaker.MakeThing(__instance.def.GetModExtension<NutrientPasteCustom>().customMeal);
            var compIngredients = thing2.TryGetComp<CompIngredients>();
            foreach (var ingredient in list)
            {
                if (!__instance.def.GetModExtension<NutrientPasteCustom>().mysteryIngredients)
                {
                    compIngredients.RegisterIngredient(ingredient);
                }
            }

            __result = thing2;
            return false;
        }

        private static bool HopperTick(int interval, CompRottable __instance)
        {
            var things = GetCompRottableThings(__instance);

            if (things is null || !things.OfType<NPDHopper_Storage>().Any())
            {
                return true;
            }

            Thing t = things.OfType<NPDHopper_Storage>().First();
            foreach (var thing in things)
            {
                if (!(thing is NPDHopper_Storage hopper))
                {
                    continue;
                }

                var rotProgress = __instance.RotProgress;
                var hopperTemp = t.TryGetComp<CompPowerTrader>().PowerOn
                    ? hopper.def.GetModExtension<HopperCustom>().setTemperature
                    : __instance.parent.AmbientTemperature;
                var num = GenTemperature.RotRateAtTemperature(hopperTemp);
                __instance.RotProgress += num * interval;

                if (__instance.Stage == RotStage.Rotting && __instance.PropsRot.rotDestroys)
                {
                    if (__instance.parent.IsInAnyStorage() && __instance.parent.SpawnedOrAnyParentSpawned)
                    {
                        Messages.Message(
                            "MessageRottedAwayInStorage".Translate(__instance.parent.Label, __instance.parent)
                                .CapitalizeFirst(),
                            new TargetInfo(__instance.parent.PositionHeld, __instance.parent.MapHeld),
                            MessageTypeDefOf.NegativeEvent);
                        LessonAutoActivator.TeachOpportunity(ConceptDefOf.SpoilageAndFreezers,
                            OpportunityType.GoodToKnow);
                    }

                    __instance.parent.Destroy();
                    return false;
                }

                var unused = Mathf.FloorToInt(rotProgress / 60000f) !=
                             Mathf.FloorToInt(__instance.RotProgress / 60000f);
                //Should Take Rot Damage?
                return false;
            }

            return true;
        }

        public static bool CompInspectStringCustomHopper(CompRottable __instance, ref string __result)
        {
            var things = GetCompRottableThings(__instance);

            if (!things.OfType<NPDHopper_Storage>().Any())
            {
                return true;
            }

            Thing t = things.OfType<NPDHopper_Storage>().First();
            var stringBuilder = new StringBuilder();
            var stage = __instance.Stage;
            if (stage != RotStage.Fresh)
            {
                if (stage != RotStage.Rotting)
                {
                    if (stage == RotStage.Dessicated)
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

            var num = __instance.PropsRot.TicksToRotStart - __instance.RotProgress;
            if (num > 0f)
            {
                var num2 = t.TryGetComp<CompPowerTrader>().PowerOn
                    ? Mathf.RoundToInt(things.OfType<NPDHopper_Storage>().First().def
                        .GetModExtension<HopperCustom>().setTemperature)
                    : __instance.parent.AmbientTemperature;
                var num3 = GenTemperature.RotRateAtTemperature(num2);
                var ticksUntilRotAtCurrentTemp = TicksUntilRotAtSetTemp(__instance, num2);
                stringBuilder.AppendLine();
                if (num3 < 0.001f)
                {
                    stringBuilder.Append("CurrentlyFrozen".Translate() + ".");
                }
                else if (num3 < 0.999f)
                {
                    stringBuilder.Append(
                        "CurrentlyRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) +
                        ".");
                }
                else
                {
                    stringBuilder.Append(
                        "NotRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) + ".");
                }
            }

            __result = stringBuilder.ToString();
            return false;
        }

        //Needs Work
        public static bool PotentialWorkThingRequest_CustomHoppers(ref ThingRequest __result)
        {
            var unused = Find.CurrentMap.listerBuildings.AllBuildingsColonistOfClass<NPDHopper_Storage>()
                .ToList();
            return true;
        }

        public static bool FindFeedInAnyCustomHopper(Building_NutrientPasteDispenser __instance, ref Thing __result,
            ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (___cachedAdjCellsCardinal is null)
            {
                ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                    where c.InBounds(__instance.Map)
                    select c).ToList();
            }

            foreach (var c in ___cachedAdjCellsCardinal)
            {
                Thing thing = null;
                Thing thing2 = null;
                var thingList = c.GetThingList(__instance.Map);
                foreach (var t in thingList)
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

                if (thing == null || thing2 == null)
                {
                    continue;
                }

                __result = thing;
                return false;
            }

            __result = null;
            return true;
        }

        public static bool HasEnoughFeedstockInCustomHoppers(ref bool __result,
            Building_NutrientPasteDispenser __instance, ref List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (__instance.def.GetModExtension<NutrientPasteCustom>() is null)
            {
                return true;
            }

            if (___cachedAdjCellsCardinal is null)
            {
                ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                    where c.InBounds(__instance.Map)
                    select c).ToList();
            }

            var num = 0f;
            var empty = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList is null ||
                        !__instance.def.GetModExtension<NutrientPasteCustom>().ingredientList.Any();
            if (!empty)
            {
                __result = false;
                var ingredientList = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList;
                var ingredientFulfilled = new bool[ingredientList.Count];
                var nutritionCost = new float[ingredientList.Count];
                foreach (var c in ___cachedAdjCellsCardinal)
                {
                    Thing thing = null;
                    Thing thing2 = null;
                    var thingList = c.GetThingList(__instance.Map);
                    foreach (var t in thingList)
                    {
                        if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def) &&
                            ingredientList.Any(x => x.thingDef == t.def) &&
                            nutritionCost[ingredientList.FindIndex(x => x.thingDef == t.def)] <= 0f)
                        {
                            thing = t;
                        }

                        if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage))
                        {
                            thing2 = t;
                        }
                    }

                    if (!(thing is null) && !(thing2 is null))
                    {
                        nutritionCost[ingredientList.FindIndex(x => x.thingDef == thing.def)] +=
                            thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition);
                        if (nutritionCost[ingredientList.FindIndex(x => x.thingDef == thing.def)] >=
                            ingredientList.Find(x => x.thingDef == thing.def).nutritionCost)
                        {
                            ingredientFulfilled[ingredientList.FindIndex(x => x.thingDef == thing.def)] = true;
                        }
                    }

                    if (!ingredientFulfilled.All(x => x))
                    {
                        continue;
                    }

                    __result = true;
                    return false;
                }

                return false;
            }

            foreach (var c in ___cachedAdjCellsCardinal)
            {
                Thing thing = null;
                Thing thing2 = null;
                var thingList = c.GetThingList(__instance.Map);
                foreach (var t in thingList)
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
                    num += thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition);
                }

                if (!(num >= __instance.def.building.nutritionCostPerDispense))
                {
                    continue;
                }

                __result = true;
                return false;
            }

            __result = false;
            return false;
        }

        public static bool AdjacentReachableHopperCustom(Pawn reacher, Building_NutrientPasteDispenser __instance,
            ref Building __result,
            List<IntVec3> ___cachedAdjCellsCardinal)
        {
            if (!__instance.def.HasModExtension<NutrientPasteCustom>())
            {
                return true;
            }

            if (___cachedAdjCellsCardinal is null)
            {
                ___cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(__instance)
                    where c.InBounds(__instance.Map)
                    select c).ToList();
            }

            foreach (var c in ___cachedAdjCellsCardinal)
            {
                var edifice = c.GetEdifice(__instance.Map);

                if (edifice is null ||
                    edifice.def != ThingDefOf.Hopper && edifice.def.thingClass != typeof(NPDHopper_Storage) ||
                    !reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly))
                {
                    continue;
                }

                __result = edifice;
                return false;
            }

            return true;
        }

        public static void GetReportModified(ref string __result, JobDriver_FoodDeliver __instance)
        {
            var targetBuilding = __instance.job.GetTarget(TargetIndex.A).Thing;
            var deliveree = (Pawn)__instance.job.targetB.Thing;
            if (targetBuilding is Building_NutrientPasteDispenser && deliveree != null)
            {
                __result = __instance.job.def.reportString.Replace("TargetA",
                        targetBuilding.def.GetModExtension<NutrientPasteCustom>().customMeal.label)
                    .Replace("TargetB", deliveree.LabelShort);
            }

            //__result = base.GetReport();
        }

        public static bool BadDispenserReportModified(ref AlertReport __result,
            Alert_PasteDispenserNeedsHopper __instance)
        {
            __result = AlertReport.CulpritsAre(GetBadDispensers);
            return false;
        }

        public static IEnumerable<Thing> GetCompRottableThings(CompRottable instance)
        {
            return instance.parent?.Map?.thingGrid.ThingsAt(instance.parent.Position);
        }

        public static int TicksUntilRotAtSetTemp(CompRottable instance, float temp)
        {
            temp = Mathf.RoundToInt(temp);
            return instance.TicksUntilRotAtTemp(temp);
        }
    }
}