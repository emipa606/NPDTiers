using System;
using System.Xml;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

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
                }), false);
                nutritionCost = 0f;
            }
            this.thingDef = thingDef;
            this.nutritionCost = nutritionCost;
        }

        public ThingDef ThingDef
        {
            get
            {
                return this.thingDef;
            }
        }

        public float NutritionCost
        {
            get
            {
                return this.nutritionCost;
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<ThingDef>(ref this.thingDef, "thingDef");
            Scribe_Values.Look<float>(ref this.nutritionCost, "nutritionCost");
        }

        public IngredientAndCost WithCost(float newCost)
        {
            return new IngredientAndCost(this.thingDef, newCost);
        }

        public override bool Equals(object obj)
        {
            return obj is IngredientAndCost && this.Equals((IngredientAndCost)obj);
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
            return Gen.HashCombine<ThingDef>((int)(this.nutritionCost * 10), this.thingDef);
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "(",
                this.nutritionCost,
                "x ",
                (this.thingDef == null) ? "null" : this.thingDef.defName,
                ")"
            });
        }

        public static implicit operator IngredientAndCost(IngredientAndCostClass t)
        {
            if(t == null)
            {
                return new IngredientAndCost(null, 0);
            }
            return new IngredientAndCost(t.thingDef, t.nutritionCost);
        }

        private ThingDef thingDef;

        private float nutritionCost;
    }
}
