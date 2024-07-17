// RimWorld.CompAbilityEffect_ConsumeLeap
using RimWorld;
using Verse;
using Verse.Sound;

namespace EVM
{

    public class CompProperties_Vore : CompProperties_AbilityEffect
    {
        public CompProperties_Vore()
        {
            compClass = typeof(CompAbilityPounce);
        }
    }


    public class CompAbilityPounce : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //Target can evade
            if (target.Pawn != null)
            {
                if (Utils.Dodged(target.Pawn))
                {
                    target.Pawn.stances.stagger.StaggerFor(60, 0.3f);
                    parent.pawn.stances.stagger.StaggerFor(95, 0.17f);
                    //Add sound *****Dodged******
                    Map map = parent.pawn.Map;
                    MoteMaker.ThrowText(target.Pawn.DrawPos, map, "TextMote_Dodge".Translate(), 1.9f);
                    SoundDef sound = InternalDefOf.EVM_Resist;
                    sound?.PlayOneShot(new TargetInfo(target.Pawn.Position, map, false));
                }
                else
                {
                    //target.Pawn.stances.CancelBusyStanceHard();

                    int stun = (int)(600*(Utils.GetBodysize(target.Thing)/parent.pawn.BodySize));
                    target.Pawn.stances.stunner.StunFor(stun, null, false, true, false);
                    //target.Pawn.CurJob.collideWithPawns = false; //this is unknown territoy
                    parent.pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Eat, target));
                }
            }
            else { //Target is not live pawn
                parent.pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Eat, target));
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            SwallowWholeProperties swallowWholeProperties = Utils.GetSwallowWholePropertiesFromTags(parent.pawn, target.Pawn);
            if (swallowWholeProperties.IsValid(true))
            {
                return true;
            }
            return false;
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            SwallowWholeProperties swallowWholeProperties = Utils.GetSwallowWholePropertiesFromTags(parent.pawn, target.Thing);
            if (!swallowWholeProperties.IsValid(throwMessages)) {
                return false;
            }
            return base.Valid(target, throwMessages);
        }
    }



}