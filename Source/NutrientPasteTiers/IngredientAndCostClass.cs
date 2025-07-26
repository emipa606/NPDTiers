using System.Xml;
using Verse;

namespace NutrientPasteTiers;

public sealed class IngredientAndCostClass : IExposable
{
    public float nutritionCost;

    public ThingDef thingDef;

    public IngredientAndCostClass()
    {
    }

    private IngredientAndCostClass(ThingDef thingDef, float nutritionCost)
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

    public string Summary => nutritionCost + "x " + (thingDef is null ? "null" : thingDef.label);

    public void ExposeData()
    {
        Scribe_Defs.Look(ref thingDef, "thingDef");
        Scribe_Values.Look(ref nutritionCost, "nutritionCost", 0.1f);
    }

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        if (xmlRoot.ChildNodes.Count != 1)
        {
            Log.Error("Misconfigured IngredientAndCostClass: " + xmlRoot.OuterXml);
            return;
        }

        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
        nutritionCost = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
    }

    public override string ToString()
    {
        return string.Concat("(", nutritionCost, "x ", thingDef is null ? "null" : thingDef.defName, ")");
    }

    public override int GetHashCode()
    {
        return (thingDef.shortHash + (int)(nutritionCost * 10)) << 16;
    }

    public static implicit operator IngredientAndCostClass(IngredientAndCost t)
    {
        return new IngredientAndCostClass(t.ThingDef, t.NutritionCost);
    }
}