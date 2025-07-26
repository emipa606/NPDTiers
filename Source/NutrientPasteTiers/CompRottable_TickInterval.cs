using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(CompRottable), "TickInterval")]
public static class CompRottable_TickInterval
{
    private static bool Prefix(int delta, CompRottable __instance)
    {
        var things = NPDHarmony.GetCompRottableThings(__instance);

        if (things is null || !things.OfType<NPDHopper_Storage>().Any())
        {
            return true;
        }

        Thing t = things.OfType<NPDHopper_Storage>().First();
        foreach (var thing in things)
        {
            if (thing is not NPDHopper_Storage hopper)
            {
                continue;
            }

            var rotProgress = __instance.RotProgress;
            var hopperTemp = t.TryGetComp<CompPowerTrader>().PowerOn
                ? hopper.def.GetModExtension<HopperCustom>().setTemperature
                : __instance.parent.AmbientTemperature;
            var num = GenTemperature.RotRateAtTemperature(hopperTemp);
            __instance.RotProgress += num * delta;

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

            _ = Mathf.FloorToInt(rotProgress / GenDate.TicksPerDay) !=
                Mathf.FloorToInt(__instance.RotProgress / GenDate.TicksPerDay);
            //Should Take Rot Damage?
            return false;
        }

        return true;
    }
}