using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.GetGizmos))]
public static class Building_NutrientPasteDispenser_GetGizmos
{
    public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values)
    {
        foreach (var value in values)
        {
            yield return value;
        }

        foreach (var hopper in NPDHarmony.allExtraHoppers)
        {
            var designatorBuild = BuildCopyCommandUtility.FindAllowedDesignator(hopper);
            if (designatorBuild != null)
            {
                yield return designatorBuild;
            }
        }
    }
}