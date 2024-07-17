using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_Fatal : DigestionWorker
    {
        public override string GetDescription()
        {
            return "Being digested fatally";
        }
        public override void ApplyDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            float digestionEfficiancy = base.GetDigestionEfficiancy(swallowWholeProperties);
            
            if (digestionEfficiancy > 0)
            {
                // digest
                float damage = swallowWholeProperties.baseDamage * digestionEfficiancy;

                foreach (Thing thing in innerContainer)
                {
                    if (swallowWholeProperties.canDigest(thing))
                    {
                        bool justdied = true;

                        if (swallowWholeProperties.checkIsAlive())
                        {
                            justdied = false;
                        }

                        thing.TakeDamage(new DamageInfo(
                            swallowWholeProperties.digestionDamageType,
                            damage,
                            100f,
                            -1f,
                            swallowWholeProperties.pred,
                            null,
                            null,
                            DamageInfo.SourceCategory.ThingOrUnknown,
                            thing,
                            true,
                            false
                        ));
                        if ((!justdied) & (!swallowWholeProperties.checkIsAlive()))
                        { //Are pawns corpses? Do corpses hold pawns? What is this rimworld jungle.
                            Find.BattleLog.Add(new BattleLogEntry_Event(swallowWholeProperties.pred, RulePackDefOf.Event_DevourerDigestionCompleted, (Pawn)thing));
                            //swallowWholeProperties.prey = (Corpse)swallowWholeProperties.prey;
                            //Where to get the corpse?
                        }

                    }
                }
            }
        }

        public override float GetNutritionFromDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            float nutrition = 0f;

            foreach (Thing thing in innerContainer)
            {
                if (!swallowWholeProperties.canDigest(thing))
                {
                    Corpse corpse = null;

                    if (thing is Corpse maybe)
                    {
                        corpse = maybe;
                    }
                    else if (thing is Pawn pawn)
                    {
                        corpse = pawn.Corpse;
                    }

                    float thisNutrition = 0.9f;

                    try
                    {
                        thisNutrition = thing.GetStatValue(StatDefOf.Nutrition);
                    }
                    catch
                    {
                        // may not exist
                    }
                    nutrition += thisNutrition;
                }
            }

            return nutrition;
        }

        public override float GetNutritionFromDigestionTick(SwallowWholeProperties voreProperties, ThingOwner innerContainer)
        {
            float nutrition = 0f;

            foreach (Thing thing in innerContainer)
            {
                if (voreProperties.canDigest(thing))
                {
                    float factor = 1f;
                    float totalNutrition = 0.1f;
                    try   {
                        factor = thing.HitPoints / thing.MaxHitPoints;
                    }
                    catch  {
                        // thing may not use this hp system
                        return 0;
                    }

                    if (thing is Corpse)
                    {
                        totalNutrition = thing.GetStatValue(StatDefOf.Nutrition);
                        factor = 1;
                    }
                    else if (thing is Pawn pawn)
                    {
                        totalNutrition = pawn.GetStatValue(StatDefOf.MeatAmount)*0.05f;
                        factor = pawn.health.summaryHealth.SummaryHealthPercent;
                        factor = factor / 2; //Living pawns are twice as hard to digest
                    }
                    factor = factor * base.GetDigestionEfficiancy(voreProperties);
                    nutrition += totalNutrition * factor;
                    if (SwallowWholeLibrary.settings.debugOptions)
                    {
                        Log.Message("Nutrition gained? : " + totalNutrition.ToString() + " Factor: " + factor.ToString());
                    }
                }
            }

            return nutrition;
        }
    }
}
