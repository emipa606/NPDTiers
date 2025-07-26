using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NutrientPasteTiers;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.TryDispenseFood))]
public static class Building_NutrientPasteDispenser_TryDispenseFood
{
    public static bool Prefix(ref Thing __result, Building_NutrientPasteDispenser __instance,
        ref List<IntVec3> ___cachedAdjCellsCardinal)
    {
        if (!__instance.def.HasModExtension<NutrientPasteCustom>())
        {
            return true;
        }

        if (!__instance.CanDispenseNow)
        {
            __result = null;
            return false;
        }

        var list = new List<ThingDef>();
        var ingredientList = __instance.def.GetModExtension<NutrientPasteCustom>().ingredientList;
        var empty = !__instance.def.GetModExtension<NutrientPasteCustom>().ingredientList.Any();
        if (!empty)
        {
            var nutritionLeft = new float[ingredientList.Count];
            for (var i = 0; i < nutritionLeft.Length; i++)
            {
                nutritionLeft[i] = ingredientList[i].nutritionCost;
            }

            for (;;)
            {
                var thing = __instance.def.GetModExtension<NutrientPasteCustom>()
                    .FindNextIngredientInHopper(___cachedAdjCellsCardinal, __instance, nutritionLeft);
                if (thing is null)
                {
                    break;
                }

                var index = ingredientList.FindIndex(x => x.thingDef == thing.def);
                var num2 = Mathf.Min(thing.stackCount,
                    Mathf.CeilToInt(nutritionLeft[index] / thing.GetStatValue(StatDefOf.Nutrition)));
                nutritionLeft[index] -= num2 * thing.GetStatValue(StatDefOf.Nutrition);
                list.Add(thing.def);
                thing.SplitOff(num2);
                if (!nutritionLeft.Any(x => x > 0f))
                {
                    goto Block_3;
                }
            }
        }
        else
        {
            var num = __instance.def.building.nutritionCostPerDispense - 0.0001f;
            for (;;)
            {
                var thing = __instance.FindFeedInAnyHopper();
                if (thing is null)
                {
                    break;
                }

                var num2 = Mathf.Min(thing.stackCount,
                    Mathf.CeilToInt(num / thing.GetStatValue(StatDefOf.Nutrition)));
                num -= num2 * thing.GetStatValue(StatDefOf.Nutrition);
                list.Add(thing.def);
                thing.SplitOff(num2);
                if (num <= 0f)
                {
                    goto Block_3;
                }
            }
        }

        Log.Error("Did not find enough food in hoppers while trying to dispense.");
        __result = null;
        return false;

        Block_3:
        __instance.def.building.soundDispense.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map));
        var thing2 = ThingMaker.MakeThing(__instance.def.GetModExtension<NutrientPasteCustom>().customMeal);
        var compIngredients = thing2.TryGetComp<CompIngredients>();
        foreach (var ingredient in list)
        {
            if (!__instance.def.GetModExtension<NutrientPasteCustom>().mysteryIngredients)
            {
                compIngredients.RegisterIngredient(ingredient);
            }
        }

        __result = thing2;
        return false;
    }
}