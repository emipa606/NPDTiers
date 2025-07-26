using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser),
    nameof(Building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers))]
public static class Building_NutrientPasteDispenser_HasEnoughFeedstockInHoppers
{
    public static bool Prefix(ref bool __result,
        Building_NutrientPasteDispenser __instance, ref List<IntVec3> ___cachedAdjCellsCardinal)
    {
        if (__instance.def.GetModExtension<NutrientPasteCustom>() is null)
        {
            return true;
        }

        ___cachedAdjCellsCardinal ??= (from c in GenAdj.CellsAdjacentCardinal(__instance)
            where c.InBounds(__instance.Map)
            select c).ToList();

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

                    if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage) ||
                        t.def.building?.isHopper == true)
                    {
                        thing2 = t;
                    }
                }

                if (thing is not null && thing2 is not null)
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

                if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage) ||
                    t.def.building?.isHopper == true)
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
}