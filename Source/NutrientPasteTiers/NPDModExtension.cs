using RimWorld;
using Verse;
using System.Collections.Generic;

namespace NutrientPasteTiers
{
    public class NutrientPasteCustom : DefModExtension
    {
        public ThingDef customMeal;

        public bool mysteryIngredients = false;

        public List<IngredientAndCostClass> ingredientList = new List<IngredientAndCostClass>();

        public Thing FindNextIngredientInHopper(List<IntVec3> cachedCells, Building_NutrientPasteDispenser instance, float[] nutrition)
        {
            foreach(IntVec3 c in cachedCells)
            {
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = c.GetThingList(instance.Map);
                foreach(Thing t in thingList)
                {
                    if(Building_NutrientPasteDispenser.IsAcceptableFeedstock(t.def) && this.ingredientList.Any(x => x.thingDef == t.def)
                        && nutrition[ingredientList.FindIndex(x => x.thingDef == t.def)] > 0f )
                    {
                        thing = t;
                    }
                    if (t.def == ThingDefOf.Hopper || t.def.thingClass == typeof(NPDHopper_Storage))
                    {
                        thing2 = t;
                    }
                }
                if (!(thing is null) && !(thing2 is null))
                {
                    return thing;
                }            
            }
            return null;
        }
    }
}
