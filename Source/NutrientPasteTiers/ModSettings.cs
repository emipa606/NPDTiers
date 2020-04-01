using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

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

    public class NPDTiersWindow : Mod
    {
        public static NPDTiersSettings settings;

        public NPDTiersWindow(ModContentPack content) : base(content)
        {
            settings = GetSettings<NPDTiersSettings>();
        }

        public static void DebugLog(string message = null, Exception e = null)
        {
            #if DEBUG
            if( !(message is null) )
            {
                Log.Warning("[ NPDTiers ] - " + message);
            }
            if(Prefs.DevMode && !(e is null))
            {
                Log.Error(string.Concat(new object[]
                {
                    "[ NPDTiers ] - Exception thrown: \n",
                    e,
                    "\n ----------- \n",
                    "FullStackTrace: \n",
                    e.StackTrace,
                    "\n ----------- \n",
                    "Data: \n",
                    e.Data
                }));
            }
            #endif
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            //listingStandard.CheckboxLabeled("DisableNPDExamples".Translate(), ref settings.disableExampleNPDs, "DisableNPDTooltip".Translate());
            //listingStandard.CheckboxLabeled("DisableIngredientCheck".Translate(), ref settings.mysteryMeat, "DisableIngredientTooltip".Translate());

            listingStandard.End();

            base.DoSettingsWindowContents(rect);
        }

        public override string SettingsCategory()
        {
            return "NutrientPasteTiersCategoryLabel".Translate();
        }
    }
}
