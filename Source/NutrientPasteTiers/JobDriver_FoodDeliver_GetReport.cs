using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(JobDriver_FoodDeliver), nameof(JobDriver_FoodDeliver.GetReport))]
public static class JobDriver_FoodDeliver_GetReport
{
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
}