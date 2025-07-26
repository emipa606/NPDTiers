using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(WorkGiver_CookFillHopper), nameof(WorkGiver_CookFillHopper.PotentialWorkThingRequest),
    MethodType.Getter)]
public static class WorkGiver_CookFillHopper_PotentialWorkThingRequest
{
    //Needs Work
    public static void Prefix()
    {
        _ = Find.CurrentMap.listerBuildings.AllBuildingsColonistOfClass<NPDHopper_Storage>()
            .ToList();
    }
}