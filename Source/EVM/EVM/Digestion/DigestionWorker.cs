using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public abstract class DigestionWorker: IExposable
    {
        public virtual string GetDescription() { return "Unknown digestion type"; }
        public virtual float GetDigestionEfficiancy(SwallowWholeProperties swallowWholeProperties)
        {
            if (!swallowWholeProperties.pred.health.capacities.CapableOf(InternalDefOf.Metabolism))
            {
                return 0;
            }

            return swallowWholeProperties.digestionEfficiancy * swallowWholeProperties.pred.health.capacities.GetLevel(InternalDefOf.Metabolism);
        }

        public abstract void ApplyDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer);

        public virtual void Struggle(Thing struggler, SwallowWholeProperties swallowWholeProperties)
        {
            if (struggler is Pawn pawn)
            {
                if (!(pawn.Downed || pawn.Dead))
                {
                    int meleeLvl = 0;

                    if (pawn.skills != null)
                    {
                        SkillRecord melee = pawn.skills.GetSkill(SkillDefOf.Melee);
                        if (melee != null)
                        {
                            if (meleeLvl < melee.Level)
                            {
                                meleeLvl = melee.Level;
                            }
                        }
                    }
                    
                    VerbEntry verbEntry = pawn.meleeVerbs.GetUpdatedAvailableVerbsList(false).RandomElement<VerbEntry>();
                    float damage = verbEntry.verb.verbProps.AdjustedMeleeDamageAmount(verbEntry.verb, pawn) * meleeLvl / 10 * swallowWholeProperties.pred.GetStatValue(StatDefOf.IncomingDamageFactor, true, -1);
                    DamageDef damageDef = verbEntry.verb.verbProps.meleeDamageDef;
                    
                    int rand = Enumerable.Range(1, 100).RandomElement<int>();
                    int armorValue = 0;
                    
                    if (swallowWholeProperties.armorValues.ContainsKey(damageDef.armorCategory))
                    {
                        armorValue = swallowWholeProperties.armorValues[damageDef.armorCategory];
                    }
                    
                    if (rand < armorValue / 2)
                    {
                        return;
                    }
                    else if (rand <= armorValue)
                    {
                        damage /= 2;
                    }
                    BodyPartRecord a = swallowWholeProperties.pred.RaceProps.body.GetPartsWithDef(swallowWholeProperties.digestiveTracks[swallowWholeProperties.trackId].track[swallowWholeProperties.trackStage])[0];
                    float partHealth = swallowWholeProperties.pred.health.hediffSet.GetPartHealth(a);
                    if (partHealth <= damage)
                    {
                        if (SwallowWholeLibrary.settings.debugOptions) {
                            Log.Message(pawn + " should able to escape!");
                        }
                        swallowWholeProperties.escape = true;
                    } // Prevent prey from erasing themselves into memory leaks
                    else
                    {
                        swallowWholeProperties.pred.TakeDamage(new DamageInfo(
                              damageDef,
                              damage,
                              500f,
                              -1,
                              struggler,
                              a,
                              null,
                              DamageInfo.SourceCategory.ThingOrUnknown,
                              swallowWholeProperties.pred,
                              false,
                              false
                          ));
                    }
                }
            }
        }

        public abstract float GetNutritionFromDigestionTick(SwallowWholeProperties voreProperties, ThingOwner innerContainer);
        
        public abstract float GetNutritionFromDigestion(SwallowWholeProperties voreProperties, ThingOwner innerContainer);

        public virtual void ExposeData()
        {
            
        }
    }
}
