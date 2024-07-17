using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_Regenerate: DigestionWorker_HealScars
    {
        public override string GetDescription()
        {
            return "Being regenerated";
        }
        public Hediff chosen = null;
        public float progress = 0;

        public override float Heal(Pawn pawn, float healingPool)
        {
            healingPool = base.Heal(pawn, healingPool);
            bool needToChoose = false;

            do
            {
                if (chosen == null)
                {
                    needToChoose = true;
                }
                else if (!pawn.health.hediffSet.hediffs.Contains<Hediff>(chosen))
                {
                    needToChoose = true;
                }

                IEnumerable<Hediff> missingParts = from h
                                                   in pawn.health.hediffSet.hediffs
                                                   where h is Hediff_MissingPart
                                                   select h;

                if (needToChoose)
                {
                    if (missingParts.Count() > 0)
                    {
                        chosen = missingParts.RandomElement<Hediff>();
                        needToChoose = false;
                    }
                }

                if (missingParts.Count() > 0)
                {
                    progress += healingPool;
                    healingPool = 0;

                    float threshold = chosen.Part.def.GetMaxHealth(pawn);
                    if (progress >= threshold)
                    {
                        healingPool = progress - threshold;
                        progress = 0;
                        pawn.health.RestorePart(chosen.Part);
                        chosen = null;
                        needToChoose = true;
                    }
                }
            } while (healingPool > 0 && needToChoose == true);

            return healingPool;
        }

        public override float GetNutritionFromDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            float nutrition = base.GetNutritionFromDigestion(swallowWholeProperties, innerContainer);

            if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.OnEating)
            {
                foreach (Thing thing in innerContainer)
                {
                    if (thing is Pawn pawn)
                    {
                        foreach (Hediff hediff in from h
                                                  in pawn.health.hediffSet.hediffs
                                                  where h is Hediff_MissingPart
                                                  select h)
                        {
                            nutrition += hediff.Severity;
                        }
                    }
                }
            }

            return nutrition;
        }

        public override void ExposeData()
        {
            Scribe_References.Look<Hediff>(ref chosen, "EVM_DigestionWorker_Regenerate_Hediff");
            Scribe_Values.Look<float>(ref progress, "EVM_DigestionWorker_Regenerate_Progress");
        }
    }
}
