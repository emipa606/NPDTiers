using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(CompRottable), nameof(CompRottable.CompInspectStringExtra))]
public static class CompRottable_CompInspectStringExtra
{
    public static bool Prefix(CompRottable __instance, ref string __result)
    {
        var things = NPDHarmony.GetCompRottableThings(__instance);

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
            var ticksUntilRotAtCurrentTemp = NPDHarmony.TicksUntilRotAtSetTemp(__instance, num2);
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
}