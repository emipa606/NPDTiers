using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.AdjacentReachableHopper))]
public static class Building_NutrientPasteDispenser_AdjacentReachableHopper
{
    public static bool Prefix(Pawn reacher, Building_NutrientPasteDispenser __instance,
        ref Building __result,
        List<IntVec3> ___cachedAdjCellsCardinal)
    {
        if (!__instance.def.HasModExtension<NutrientPasteCustom>())
        {
            return true;
        }

        ___cachedAdjCellsCardinal ??= (from c in GenAdj.CellsAdjacentCardinal(__instance)
            where c.InBounds(__instance.Map)
            select c).ToList();

        foreach (var c in ___cachedAdjCellsCardinal)
        {
            var edifice = c.GetEdifice(__instance.Map);

            if (edifice is null ||
                edifice.def != ThingDefOf.Hopper && edifice.def.thingClass != typeof(NPDHopper_Storage) &&
                edifice.def.building?.isHopper == false ||
                !reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly))
            {
                continue;
            }

            __result = edifice;
            return false;
        }

        return true;
    }
}