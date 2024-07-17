using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace EVM
{
    internal class JobDriver_SwallowWhole : JobDriver
    {
        private Thing OtherPawn
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        private Corpse OtherCorpse
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing as Corpse;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            SwallowWholeProperties swallowWholeProperties = null;

            foreach (SwallowWholeProperties vp in SwallowWholeProperties.passer)
            {
                if (vp.pred == this.pawn && vp.prey == OtherPawn)
                {
                    swallowWholeProperties = vp;
                    break;
                }
            }

            if (swallowWholeProperties != null)
            {
                SwallowWholeProperties.passer.Remove(swallowWholeProperties);
            }
            if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("Vore jobdriver for : " + pawn); }
            if (pawn.HostileTo(OtherPawn))
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }
            else {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            }
            float struggle = Utils.GetBodysize(OtherPawn);
            struggle = struggle / pawn.BodySize;
            struggle = 1200 * struggle * SwallowWholeLibrary.settings.swallowSpeed;

            if (struggle > 0) {
                //yield return Toils_General.Wait((int)struggle).PlaySustainerOrSound(InternalDefOf.EVM_Juice, 1 / pawn.BodySize).FailOnDespawnedOrNull(TargetIndex.A);
                Toils_ReserveAttackTarget.TryReserve(TargetIndex.A); //What does this do?
                if (OtherPawn is Pawn preyP)
                {
                    preyP.stances.stagger.StaggerFor((int)struggle, 0.17f);
                    if (preyP.Faction != null)
                    {
                        if (preyP.Name != null)
                        {
                            Messages.Message(preyP.Name + " is being devoured alive!", pawn, MessageTypeDefOf.NeutralEvent, false);
                        }
                        else
                        {
                            Messages.Message(preyP + " is being devoured alive!", pawn, MessageTypeDefOf.NeutralEvent, false);
                        }
                    }
                }
                yield return Toils_General.WaitWith(TargetIndex.A, (int)struggle, true)
                    .PlaySustainerOrSound(InternalDefOf.EVM_Juice, 1 / pawn.BodySize)
                    .FailOnDespawnedOrNull(TargetIndex.A);
             } //struggle end - in the time the pawn could have died or downed?

            if (OtherPawn.Spawned) { //taking into account multiple vores?
                if (OtherCorpse != null) {
                    pawn.CurJob.SetTarget(TargetIndex.A, OtherCorpse);
                    if (SwallowWholeLibrary.settings.debugOptions)  {
                        Log.Message("Vore jobdriver got corpse");
                    }
                }
                if (SwallowWholeLibrary.settings.debugOptions) {
                    Log.Message("Vore jobdriver reached end");
                }
                swallowWholeProperties = Utils.GetSwallowWholePropertiesFromTags(this.pawn, OtherPawn);
                yield return Toils_Food.SwallowWhole(swallowWholeProperties);
             }
            //EndJobWith(JobCondition.Succeeded);
            yield break;
        }
    }
}


/*Toil preyDownedDead = ToilMaker.MakeToil("EVM_preyDead");
preyDownedDead.initAction = delegate
{
    Pawn pred = preyDownedDead.actor;
    LocalTargetInfo preyd = this.job.GetTarget(TargetIndex.A);
    foreach (Thing lostprey in preyd.Cell.GetThingList(pred.Map)) //I am brute
    {
        Pawn lostpp = lostprey as Pawn;
        Corpse lostpc = lostprey as Corpse;
        //actor.CurJob.SetTarget(TargetIndex.A, corpse);
    }
};.JumpIfDespawnedOrNull(TargetIndex.A, preyDownedDead);  }*/
