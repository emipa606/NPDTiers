using HarmonyLib;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.DispensableDef),
    MethodType.Getter)]
public static class Building_NutrientPasteDispenser_DispensableDef
{
    public static bool Prefix(ref ThingDef __result, Building_NutrientPasteDispenser __instance)
    {
        if (!__instance.def.HasModExtension<NutrientPasteCustom>())
        {
            return true;
        }

        __result = __instance.def.GetModExtension<NutrientPasteCustom>().customMeal;
        return false;
    }
}