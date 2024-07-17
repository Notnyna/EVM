using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_HealScars: DigestionWorker_Heal
    {
        public override string GetDescription()
        {
            return "Being healed with scars";
        }
        public bool healScars = false;

        public override float Heal(Pawn pawn, float healingPool)
        {
            healingPool = base.Heal(pawn, healingPool);
            healScars = true;
            healingPool = base.Heal(pawn, healingPool);

            return healingPool;
        }

        public override IEnumerable<Hediff> GetHealables(Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury)
                {
                    if (hediff.IsPermanent() && !healScars) 
                    {
                        continue;
                    }

                    yield return hediff;
                }
            }

            yield break;
        }
    }
}
