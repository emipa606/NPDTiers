using System;
using System.Xml;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace NutrientPasteTiers
{
    public sealed class IngredientAndCostClass : IExposable
    {
        public IngredientAndCostClass()
        { 
        }

        public IngredientAndCostClass(ThingDef thingDef, float nutritionCost)
        {
            if(nutritionCost < 0)
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

        public string Summary
        {
            get
            {
                return this.nutritionCost + "x " + ((this.thingDef is null) ? "null" : this.thingDef.label);
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<ThingDef>(ref this.thingDef, "thingDef");
            Scribe_Values.Look<float>(ref this.nutritionCost, "nutritionCost", 0.1f, false);
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if(xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("Misconfigured IngredientAndCostClass: " + xmlRoot.OuterXml, false);
                return;
            }
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
            this.nutritionCost = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "(", this.nutritionCost, "x ",
                (this.thingDef is null) ? "null" : this.thingDef.defName,
                ")"
            });
        }

        public override int GetHashCode()
        {
            return (int)this.thingDef.shortHash + (int)(this.nutritionCost*10) << 16;
        }

        public static implicit operator IngredientAndCostClass(IngredientAndCost t)
        {
            return new IngredientAndCostClass(t.ThingDef, t.NutritionCost);
        }

        public ThingDef thingDef;

        public float nutritionCost;
    }
}
