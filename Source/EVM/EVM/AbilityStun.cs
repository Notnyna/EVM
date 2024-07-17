// RimWorld.CompAbilityEffect_GiveHediff
using RimWorld;
using Verse;
using Verse.Sound;

namespace EVM
{
    public class CompAbilityEffect_MeleeAmbushHediff : CompAbilityEffect
    {
        public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (Props.ignoreSelf && target.Pawn == parent.pawn)
            {
                return;
            }
            if (!Props.onlyApplyToSelf && Props.applyToTarget)
            {
                ApplyInner(target.Pawn, parent.pawn);
            }
            if (!Props.applyToSelf && !Props.onlyApplyToSelf)
            {
                return;
            }
            ApplyInner(parent.pawn, target.Pawn);
        }

        protected void ApplyInner(Pawn target, Pawn other)
        {
            //Are corpses pawns?
            if (target != null)
            {
                //Cannot stun someone already spooked
                Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef, false);
                if (firstHediffOfDef != null) { return; }

                Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, target, target.health.hediffSet.GetBrain());

                float stun_severity = 0;
                float mult = 0;
                float psy = 0;

                //To implement - if target is looking at predator, can dodge, otherwise simulating ambush
                //or somehow use vanilla verb suprise property
                if (Utils.Dodged(target))
                {
                    other.stances.stagger.StaggerFor(120, 0.17f);
                    target.stances.stagger.StaggerFor(60, 0.3f);
                    //Add sound *****Dodged******
                    Map map = other.Map;
                    MoteMaker.ThrowText(target.DrawPos, map, "TextMote_Dodge".Translate(), 1.9f);
                    SoundDef sound = InternalDefOf.EVM_Resist;
                    sound?.PlayOneShot(new TargetInfo(other.Position, map, false));
                    return;
                }
                //Compare melee skill 
                stun_severity = target.skills.GetSkill(SkillDefOf.Melee).Level;
                if (stun_severity > 0)
                {
                    stun_severity = other.skills.GetSkill(SkillDefOf.Melee).Level / stun_severity;
                }
                else {
                    stun_severity = other.skills.GetSkill(SkillDefOf.Melee).Level;
                }
                mult = other.GetStatValue(StatDefOf.MeleeDamageFactor, true, -1);
                mult = mult - target.GetStatValue(StatDefOf.IncomingDamageFactor, true, -1);

                psy = target.GetStatValue(StatDefOf.PsychicSensitivity, true, -1);
                psy = psy*(other.GetStatValue(StatDefOf.PsychicSensitivity, true, -1)-psy);
                stun_severity = (stun_severity/2) + (stun_severity * mult) + psy;

                if (stun_severity <= 1)
                {
                    MoteMaker.ThrowText(target.DrawPos, target.Map, "Resisted".Translate(), -1f);
                }
                hediff.Severity = stun_severity;
                target.health.AddHediff(hediff, null, null, null);
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (parent.pawn.Faction == Faction.OfPlayer)
            {
                return false;
            }
            return target.Pawn != null;
        }
    }
}