using Verse;

namespace NutrientPasteTiers
{
    public class NPDTiersSettings : ModSettings
    {
        //List<Building_NutrientPasteDispenser> npdList = new List<Building_NutrientPasteDispenser>();

        public NPDModOption options;

        public override void ExposeData()
        {
            //Scribe_Collections.Look(ref npdList, "npdList", LookMode.Reference);
            //Scribe_Values(ref options, "options");
            base.ExposeData();
        }
    }
}