﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NutrientPasteTiers
{
    public class NPDHopper_Storage : Building, ISlotGroupParent, IStoreSettingsParent, IHaulDestination
    {
        private List<IntVec3> cachedOccupiedCells;

        private StorageSettings settings;


        private readonly SlotGroup slotGroup;

        public NPDHopper_Storage()
        {
            slotGroup = new SlotGroup(this);
        }

        public bool StorageTabVisible => true;

        public bool IgnoreStoredThingsBeauty => def.building.ignoreStoredThingsBeauty;

        public SlotGroup GetSlotGroup()
        {
            return slotGroup;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public StorageSettings GetStoreSettings()
        {
            return settings;
        }

        public virtual void Notify_ReceivedThing(Thing newItem)
        {
            if (Faction == Faction.OfPlayer && !(newItem.def.storedConceptLearnOpportunity is null))
            {
                LessonAutoActivator.TeachOpportunity(newItem.def.storedConceptLearnOpportunity,
                    OpportunityType.GoodToKnow);
            }
        }

        public virtual void Notify_LostThing(Thing newItem)
        {
        }

        public virtual IEnumerable<IntVec3> AllSlotCells()
        {
            foreach (var c in GenAdj.CellsOccupiedBy(this))
            {
                yield return c;
            }
        }

        public List<IntVec3> AllSlotCellsList()
        {
            if (cachedOccupiedCells == null)
            {
                cachedOccupiedCells = AllSlotCells().ToList();
            }

            return cachedOccupiedCells;
        }

        public string SlotYielderLabel()
        {
            return LabelCap;
        }

        public bool Accepts(Thing t)
        {
            return settings.AllowedToAccept(t);
        }

        public override void PostMake()
        {
            base.PostMake();
            settings = new StorageSettings(this);
            if (!(def.building.defaultStorageSettings is null))
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            cachedOccupiedCells = null;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref settings, "settings", this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            foreach (var g2 in StorageSettingsClipboard.CopyPasteGizmosFor(settings))
            {
                yield return g2;
            }
        }
    }
}