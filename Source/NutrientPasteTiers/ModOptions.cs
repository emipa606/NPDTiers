using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace NutrientPasteTiers
{
    public class NPDModOption
    {
        public static string label;

        public static bool ingredientsMatter;

        public static Color npdColor;

        public static IntVec2 size;

        public static string description;

        public static int workToBuild;

        public static int maxHp;

        public static int costToDispense;

        //need cost list

        public List<ResearchProjectDef> researchRequirements = new List<ResearchProjectDef>();
    }
}