using System;
using Verse;

namespace NutrientPasteTiers
{
    public struct IngredientAndCost : IEquatable<IngredientAndCost>, IExposable
    {
        public IngredientAndCost(ThingDef thingDef, float nutritionCost)
        {
            if (nutritionCost < 0)
            {
                Log.Warning(string.Concat(new object[]
                {
                    "Tried to set nutrition cost of ", thingDef.defName,
                    " to a value less than 0."
                }));
                nutritionCost = 0f;
            }

            this.thingDef = thingDef;
            this.nutritionCost = nutritionCost;
        }

        public ThingDef ThingDef => thingDef;

        public float NutritionCost => nutritionCost;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref thingDef, "thingDef");
            Scribe_Values.Look(ref nutritionCost, "nutritionCost");
        }

        public IngredientAndCost WithCost(float newCost)
        {
            return new IngredientAndCost(thingDef, newCost);
        }

        public override bool Equals(object obj)
        {
            return obj is IngredientAndCost cost && Equals(cost);
        }

        public bool Equals(IngredientAndCost other)
        {
            return this == other;
        }

        public static bool operator ==(IngredientAndCost a, IngredientAndCost b)
        {
            return a.thingDef == b.thingDef && a.nutritionCost == b.nutritionCost;
        }

        public static bool operator !=(IngredientAndCost a, IngredientAndCost b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Gen.HashCombine((int)(nutritionCost * 10), thingDef);
        }

        public override string ToString()
        {
            return string.Concat("(", nutritionCost, "x ", thingDef == null ? "null" : thingDef.defName, ")");
        }

        public static implicit operator IngredientAndCost(IngredientAndCostClass t)
        {
            if (t == null)
            {
                return new IngredientAndCost(null, 0);
            }

            return new IngredientAndCost(t.thingDef, t.nutritionCost);
        }

        private ThingDef thingDef;

        private float nutritionCost;
    }
}