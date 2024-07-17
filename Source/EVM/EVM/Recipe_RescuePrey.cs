// RimWorld.Recipe_RemoveBodyPart
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
namespace EVM
{
    public class Recipe_RescuePrey : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> notMissingParts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null);
            pawn.health.hediffSet.HasHediff(InternalDefOf.EVM_PreyContainer);
            foreach (BodyPartRecord item in notMissingParts)
            {
                if (pawn.health.hediffSet.HasHediff(InternalDefOf.EVM_PreyContainer, item))
                {
                    yield return item;
                }
            }

        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                { //kill the predator on fail?
                    //pawn.Kill(new DamageInfo());
                    DamagePart(pawn, part);
                }
                //TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                //Rescued me thought for all?
                pawn.health.hediffSet.GetFirstHediffMatchingPart<PreyContainer>(part).FreePreys();
            }
            DamagePart(pawn, part);
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }

        public virtual void DamagePart(Pawn pawn, BodyPartRecord part)
        {
            pawn.TakeDamage(new DamageInfo(DamageDefOf.SurgicalCut, 6f, 999f, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true, QualityCategory.Normal, true));
        }

        public virtual void ApplyThoughts(Pawn pawn, Pawn billDoer)
        {
            if (pawn.Dead)
            { //Justified - tried to rescue contents?
                ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, billDoer, PawnExecutionKind.GenericHumane);
            }
        }
    }
}