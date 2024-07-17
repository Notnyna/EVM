using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using EVM.Digestion;
using Verse.Sound;

namespace EVM
{
    public class PreyContainer: HediffWithComps, IThingHolder
    {
        private string label, labelb;
        private BodyPartRecord a;
        public override bool TryMergeWith(Hediff other)
        {
            return false;
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
            remainingStageTime = swallowWholeProperties.deadline;

            if (swallowWholeProperties.pred == null)
            {
                swallowWholeProperties.pred = this.pawn;
            }

            if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.OnEating)
            {
                swallowWholeProperties.digestionWorker.GetNutritionFromDigestion(swallowWholeProperties, innerContainer);
            }
            a = swallowWholeProperties.pred.RaceProps.body.GetPartsWithDef(swallowWholeProperties.digestiveTracks[swallowWholeProperties.trackId].track[swallowWholeProperties.trackStage])[0];
            a.def.destroyableByDamage = false; //complete hack, makes all stomachs invulnerable?
            //Should patch: PreApplyDamage
            //prey is null at this point? Why?
            //CalculateMass();
            //UpdateLabel();
        }

        public void FreePreys()
        {
            foreach (Thing thing in innerContainer)
            {
                GenPlace.TryPlaceThing(thing, this.pawn.Position, this.pawn.MapHeld, ThingPlaceMode.Near, null, null, default(Rot4));
            }
        }

        public override void Tick()
        {
            base.Tick();
            
            if (Find.TickManager.TicksGame % 1000 == 0)
            {
                if (swallowWholeProperties.escape) {
                    swallowWholeProperties.pred.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Vomit));
                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Regurgitate, innerContainer.First()));
                    swallowWholeProperties.escape = false;
                }
                Hediff badShrooms = pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.EVM_Heartburn);
                if (badShrooms != null && badShrooms.Severity > 2.5)
                {
                    if (Rand.Chance(0.12f))
                    {
                        swallowWholeProperties.escape = true;
                        badShrooms.Severity -= 1;
                    }
                }
                // digestion
                swallowWholeProperties.digestionWorker.ApplyDigestion(swallowWholeProperties, innerContainer);
                if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.PerDigestionTick)
                {
                    Need_Food foodNeed = swallowWholeProperties.pred.needs?.TryGetNeed<Need_Food>();
                    if (foodNeed != null)
                    {
                        if (swallowWholeProperties.grantsNutrition)
                        {
                            foodNeed.CurLevel += swallowWholeProperties.digestionWorker.GetNutritionFromDigestionTick(swallowWholeProperties, innerContainer);
                        }

                        foodNeed.CurLevel -= swallowWholeProperties.nutritionCost;
                        //Add sound *****Digest****** Should vary by stomach but meh
                        Map map = this.pawn.Map;
                        SoundDef sound = InternalDefOf.EVM_Gokun;
                        sound?.PlayOneShot(new TargetInfo(this.pawn.Position, map, false));
                    }
                }

                if (swallowWholeProperties.struggle)
                {
                    foreach (Thing thing in innerContainer)
                    {
                        swallowWholeProperties.digestionWorker.Struggle(thing, swallowWholeProperties);
                    }
                }
                UpdateLabel();
            }

            // Sleep if you're not being digested
            if (swallowWholeProperties.baseDamage == 0 && !swallowWholeProperties.struggle)
            {
                foreach (Thing thing in innerContainer)
                {
                    if (thing is Pawn pawn)
                    {
                        Need_Rest restNeed = pawn.needs?.TryGetNeed<Need_Rest>();

                        if (restNeed != null)
                        {
                            Need_Comfort comfortNeed = pawn.needs?.TryGetNeed<Need_Comfort>();
                            if (comfortNeed != null)
                            {
                                comfortNeed.ComfortUsed(swallowWholeProperties.comfort);
                            }

                            restNeed.TickResting(1);
                        }
                    }
                }
            }
            // Move Along
            if (--remainingStageTime <= 0)
            {
                if (swallowWholeProperties.trackStage + 1 < swallowWholeProperties.digestiveTracks[swallowWholeProperties.trackId].track.Count)
                {
                    PreyContainer next = (PreyContainer)swallowWholeProperties.pred.health.AddHediff(InternalDefOf.EVM_PreyContainer, swallowWholeProperties.pred.RaceProps.body.GetPartsWithDef(swallowWholeProperties.digestiveTracks[swallowWholeProperties.trackId].track[swallowWholeProperties.trackStage + 1])[0]);
                    next.swallowWholeProperties = Utils.GetSwallowWholePropertiesFromTags(swallowWholeProperties.pred, swallowWholeProperties.prey, swallowWholeProperties.trackId, swallowWholeProperties.trackStage + 1, swallowWholeProperties.struggle);
                }
                else
                {
                    FreePreys();
                }
            }
            
            if (innerContainer.Count <= 0)
            {
                a.def.destroyableByDamage = true;
                swallowWholeProperties.pred.health.RemoveHediff(this);
            }
            
        }
        public void CalculateMass() {
            float massinc = 0;
            float contentsize = 0;
            foreach (Thing thing in innerContainer)
            {
                if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("Vored thing: " + thing); }
                massinc += thing.GetStatValue(StatDefOf.Mass); //Can a thing not have mass?
                contentsize += Utils.GetBodysize(thing); //You should take care what you ingest
            }
            if (!SwallowWholeLibrary.settings.swallowIgnoresContents) { 
             if (contentsize > pawn.BodySize * swallowWholeProperties.mawSize)  {

                    swallowWholeProperties.escape = true;
                }
            }
            //Clamp movespeed factor from 1 to 0.5
            contentsize = contentsize / (pawn.BodySize* swallowWholeProperties.mawSize);
            if (contentsize > 1) { contentsize = 0.5f; }
            else { contentsize = contentsize / 2; }
            if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("Final contained size: " + contentsize); }
            this.CurStage.statOffsets[0].value = massinc;
            this.CurStage.statFactors[0].value = 1-contentsize;
        }

        public void UpdateLabel()
        { //There is certainly a better way to do this, but this rimworld jungle code is making me sick
            //unbelievable how much of a struggle this is
            bool err = false;
            CalculateMass(); //For lack of better place to put in
            if (SwallowWholeLibrary.settings.debugOptions) { err = true; }
            if (swallowWholeProperties.prey == null || swallowWholeProperties == null) {
                Log.Message("PreyContainer: Should not happen?");
            }
            else if (swallowWholeProperties.prey is Corpse corp)
            {
                if (corp.Destroyed) { return;  }
                if (err) { Log.Message("Containing corpse"); }
                if (corp.InnerPawn.Name == null) { label = "Contains: " + swallowWholeProperties.prey; }
                else { label = "Contains: " + corp.InnerPawn.Name.ToString(); }
                labelb = corp.HitPoints.ToString();
            }
            else if (swallowWholeProperties.prey is Pawn pawn)
            {
                if (err) { Log.Message("Containing pawn"); }
                if (pawn.Name == null) { label = "Contains: " + swallowWholeProperties.prey; }
                else { label = "Contains: " + pawn.Name.ToString(); }
                labelb = (int)(pawn.health.summaryHealth.SummaryHealthPercent * 100) + "%" + (pawn.Downed ? " downed" : "");
            }
            else
            {
                if (err) {Log.Message("Containing wghat?"); }
                label = "Contains: " + swallowWholeProperties.prey + (swallowWholeProperties.prey.stackCount != 1 ? "x" + swallowWholeProperties.prey.stackCount : "");
                labelb = (int)(swallowWholeProperties.prey.HitPoints / swallowWholeProperties.prey.MaxHitPoints * 100) + "%";
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            FreePreys();
            base.Notify_PawnDied(dinfo, culprit);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public IThingHolder ParentHolder
        {
            get
            {
                return this.pawn;
            }
        }

        public override string LabelBase
        { 
            get
            {
                return label;
            }
        }

        public override string Description
        {
            get
            {
                return swallowWholeProperties.digestionWorker.GetDescription();
            }
        }

        public override string LabelInBrackets
        {
            get
            {
                return labelb;
            }
        }

        public ThingOwner innerContainer;
        public SwallowWholeProperties swallowWholeProperties = new SwallowWholeProperties();
        public int remainingStageTime;
    }
}
