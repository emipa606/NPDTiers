using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.FindFeedInAnyHopper))]
public static class Building_NutrientPasteDispenser_FindFeedInAnyHopper
{
    public static bool Prefix(Building_NutrientPasteDispenser __instance, ref Thing __result,
        ref List<IntVec3> ___cachedAdjCellsCardinal)
    {
        ___cachedAdjCellsCardinal ??= (from c in GenAdj.CellsAdjacentCardinal(__instance)
            where c.InBounds(__instance.Map)
            select c).ToList();

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
}