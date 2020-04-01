using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using UnityEngine;
using UnityEngine.AI;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace NutrientPasteTiers
{
    public class NPDHopper_Storage : Building, ISlotGroupParent, IStoreSettingsParent, IHaulDestination
    {
        public NPDHopper_Storage()
        {
            this.slotGroup = new SlotGroup(this);
        }

        public bool StorageTabVisible
        {
            get
            {
                return true;
            }
        }

        public bool IgnoreStoredThingsBeauty
        {
            get
            {
                return this.def.building.ignoreStoredThingsBeauty;
            }
        }

        public SlotGroup GetSlotGroup()
        {
            return this.slotGroup;  
        }

        public StorageSettings GetParentStoreSettings()
        {
            return this.def.building.fixedStorageSettings;
        }

        public StorageSettings GetStoreSettings()
        {
            return this.settings;
        }

        public virtual void Notify_ReceivedThing(Thing newItem)
        {
            if(base.Faction == Faction.OfPlayer && !(newItem.def.storedConceptLearnOpportunity is null))
            {
                LessonAutoActivator.TeachOpportunity(newItem.def.storedConceptLearnOpportunity, OpportunityType.GoodToKnow);
            }
        }

        public virtual void Notify_LostThing(Thing newItem)
        {
        }
           
        public virtual IEnumerable<IntVec3> AllSlotCells()
        {
            foreach(IntVec3 c in GenAdj.CellsOccupiedBy(this))
            {
                yield return c;
            }
            yield break;
        }

        public List<IntVec3> AllSlotCellsList()
        {
            if (this.cachedOccupiedCells == null)
            {
                this.cachedOccupiedCells = this.AllSlotCells().ToList<IntVec3>();
            }
            return this.cachedOccupiedCells;
        }

        public string SlotYielderLabel()
        {
            return this.LabelCap;
        }

        public bool Accepts(Thing t)
        {
            return this.settings.AllowedToAccept(t);
        }

        public override void PostMake()
        {
            base.PostMake();
            this.settings = new StorageSettings(this);
            if( !(this.def.building.defaultStorageSettings is null) )
            {
                this.settings.CopyFrom(this.def.building.defaultStorageSettings);
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.cachedOccupiedCells = null;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<StorageSettings>(ref this.settings, "settings", new object[]
            {
                this
            });
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            foreach (Gizmo g2 in StorageSettingsClipboard.CopyPasteGizmosFor(this.settings))
            {
                yield return g2;
            }
            yield break;
        }


        private SlotGroup slotGroup;

        private StorageSettings settings;

        private List<IntVec3> cachedOccupiedCells;
    }
}