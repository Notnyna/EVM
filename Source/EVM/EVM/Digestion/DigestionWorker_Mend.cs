using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_Mend : DigestionWorker
    {
        public override string GetDescription()
        {
            return "Being mended";
        }
        public override void ApplyDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            float mendingPool = swallowWholeProperties.baseDamage * base.GetDigestionEfficiancy(swallowWholeProperties);
            allMendingSoFar += mendingPool;

            if (mendingPool > 0)
            {
                foreach (Thing thing in innerContainer)
                {
                    if (!(thing is Corpse))
                    {
                        float mendAmmount = mendingPool;
                        float missingAmmount = thing.MaxHitPoints - thing.HitPoints;

                        if (missingAmmount < mendAmmount)
                        {
                            mendAmmount = missingAmmount;
                        }

                        thing.HitPoints += (int)mendAmmount;
                    }
                }
            }
        }

        public override float GetNutritionFromDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            float nutrition = 0f;

            if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.OnEating)
            {
                foreach (Thing thing in innerContainer)
                {
                    try
                    {
                        nutrition += thing.MaxHitPoints - thing.HitPoints;
                    }
                    catch
                    {
                        // don't care
                    }
                }
            }
            else if (SwallowWholeLibrary.settings.nutritionGainOption == (int)NutritionGainOptions.AfterDigestion)
            {
                nutrition = allMendingSoFar;
            }

            return nutrition;
        }

        public override float GetNutritionFromDigestionTick(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            return swallowWholeProperties.baseDamage * base.GetDigestionEfficiancy(swallowWholeProperties);
        }

        public float allMendingSoFar = 0f;

        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref allMendingSoFar, "EVM_HealingVoreWorker_HealingPoolCount");
        }
    }
}
