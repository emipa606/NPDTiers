using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NutrientPasteTiers;

public class NutrientPasteCustom : DefModExtension
{
    public readonly List<IngredientAndCostClass> ingredientList = [];

    public readonly bool mysteryIngredients = false;
    public ThingDef customMeal;

    public Thing FindNextIngredientInHopper(List<IntVec3> cachedCells, Building_NutrientPasteDispenser instance,
        float[] nutrition)
    {
        foreach (var c in cachedCells)
        {
            Thing thing = null;
            Thing thing2 = null;
            var thingList = c.GetThingList(instance.Map);
            foreach (var t in thingList)
            {
                if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def) &&
                    ingredientList.Any(x => x.thingDef == t.def)
                    && nutrition[ingredientList.FindIndex(x => x.thingDef == t.def)] > 0f)
                {
                    thing = t;
                }

                if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage) ||
                    t.def.building?.isHopper == true)
                {
                    thing2 = t;
                }
            }

            if (thing is not null && thing2 is not null)
            {
                return thing;
            }
        }

        return null;
    }
}