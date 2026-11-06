using HarmonyLib;
using RimWorld;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Alert_PasteDispenserNeedsHopper), nameof(Alert_PasteDispenserNeedsHopper.GetReport))]
public static class Alert_PasteDispenserNeedsHopper_GetReport
{
    public static bool Prefix(ref AlertReport __result)
    {
        __result = AlertReport.CulpritsAre(NPDHarmony.GetBadDispensers);
        return false;
    }
}