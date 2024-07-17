using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_JoyPrey : DigestionWorker
    {
        public override string GetDescription()
        {
            return "Being held in pleasure";
        }
        public override void ApplyDigestion(SwallowWholeProperties voreProperties, ThingOwner innerContainer)
        {
            foreach (Thing thing in innerContainer)
            {
                if (thing is Pawn pawn)
                {
                    Need_Joy joyNeed = pawn.needs?.TryGetNeed<Need_Joy>();

                    if (joyNeed != null) 
                    {
                        joyNeed.GainJoy(3.5E-05f * 530, JoyKindDefOf.Meditative);
                    }
                }
            }
        }

        public override float GetNutritionFromDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            return 0f;
        }

        public override float GetNutritionFromDigestionTick(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            return 0f;
        }
    }
}
