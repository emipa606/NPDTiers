using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace NutrientPasteTiers;

[StaticConstructorOnStartup]
internal static class NPDHarmony
{
    public static readonly IEnumerable<ThingDef> allExtraHoppers;

    static NPDHarmony()
    {
        allExtraHoppers =
            DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                def.building?.isHopper == true && def != ThingDefOf.Hopper);
        new Harmony("rimworld.smashphil.npdtiers").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static List<Thing> GetBadDispensers
    {
        get
        {
            var dispensers = new List<Thing>();
            foreach (var map in Find.Maps)
            {
                foreach (var dispenser in map.listerBuildings.allBuildingsColonist)
                {
                    if (!dispenser.def.IsFoodDispenser)
                    {
                        continue;
                    }

                    var success = false;
                    foreach (var c in GenAdj.CellsAdjacentCardinal(dispenser))
                    {
                        if (!c.InBounds(map))
                        {
                            continue;
                        }

                        Thing building = c.GetEdifice(dispenser.Map);
                        if (building is null || building.def != ThingDefOf.Hopper &&
                            building.def.thingClass != typeof(NPDHopper_Storage) &&
                            building.def.building?.isHopper == false)
                        {
                            continue;
                        }

                        success = true;
                        break;
                    }

                    if (!success)
                    {
                        dispensers.Add(dispenser);
                    }
                }
            }

            return dispensers;
        }
    }

    public static IEnumerable<Thing> GetCompRottableThings(CompRottable instance)
    {
        return instance.parent?.Map?.thingGrid.ThingsAt(instance.parent.Position);
    }

    public static int TicksUntilRotAtSetTemp(CompRottable instance, float temp)
    {
        temp = Mathf.RoundToInt(temp);
        return instance.TicksUntilRotAtTemp(temp);
    }
}

//Building