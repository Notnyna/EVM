using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_Heal: DigestionWorker_Tend
    {
        public override string GetDescription()
        {
            return "Being healed inside";
        }
        public override float Heal(Pawn pawn, float healingPool)
        {
            healingPool = base.Heal(pawn, healingPool);

            while (healingPool > 0f)
            {
                IEnumerable<Hediff> healableParts = GetHealables(pawn);

                if (healableParts.Count() < 1)
                {
                    break;
                }

                Hediff luckyOne = healableParts.RandomElement<Hediff>();
                float healAmount = 1f; 

                if (healingPool < healAmount)
                {
                    healAmount = healingPool;
                }

                if (luckyOne.Severity < healingPool)
                {
                    healAmount = luckyOne.Severity;
                }

                luckyOne.Heal(healAmount);
            }

            return healingPool;
        }

        public virtual IEnumerable<Hediff> GetHealables(Pawn pawn)
        {
            foreach(Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury && !hediff.IsPermanent())
                {
                    yield return hediff;
                }
            }

            yield break;
        }

        public override float GetNutritionFromDigestion(SwallowWholeProperties voreProperties, ThingOwner innerContainer)
        {
            float nutrition = base.GetNutritionFromDigestion(voreProperties, innerContainer);

            if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.OnEating)
            {
                foreach (Thing thing in innerContainer)
                {
                    if (thing is Pawn pawn)
                    { 
                        foreach (Hediff hediff in GetHealables(pawn))
                        {
                            nutrition += hediff.Severity;
                        }
                    }
                }
            }

            return nutrition;
        }
    }
}
