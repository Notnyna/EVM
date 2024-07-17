using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM.Digestion
{
    public class DigestionWorker_JoyPred : DigestionWorker
    {
        public override string GetDescription()
        {
            return "Being kept as joy object";
        }
        public override void ApplyDigestion(SwallowWholeProperties swallowWholeProperties, ThingOwner innerContainer)
        {
            Need_Joy joyNeed = swallowWholeProperties.pred.needs?.TryGetNeed<Need_Joy>();

            if (joyNeed != null)
            {
                joyNeed.GainJoy(3.5E-05f * 530, JoyKindDefOf.Gluttonous);
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
