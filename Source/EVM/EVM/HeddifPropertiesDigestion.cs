// Verse.HediffCompProperties_SeverityPerDay
using Verse;
// Verse.HediffComp_SeverityPerDay
using UnityEngine;

namespace EVM
{
    public class HediffComp_SeverityWhileInside : HediffComp_SeverityModifierBase
    {
        public float CurrentSeverityPerDay;

        private HediffCompProperties_SeverityWhileInside Props => (HediffCompProperties_SeverityWhileInside)props;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (Props.showHoursToRecover && SeverityChangePerDay() < 0f)
                {
                    return Mathf.RoundToInt(parent.Severity / Mathf.Abs(SeverityChangePerDay()) * 24f) + (string)"LetterHour".Translate();
                }
                return null;
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                if (Props.showDaysToRecover && SeverityChangePerDay() < 0f)
                {
                    return "DaysToRecover".Translate((parent.Severity / Mathf.Abs(SeverityChangePerDay())).ToString("0.0")).Resolve();
                }
                return null;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            CurrentSeverityPerDay = SeverityChangePerDay();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref CurrentSeverityPerDay, "CurrentSeverityPerDay", 0f, false);
        }

        public override float SeverityChangePerDay()
        {
            float num = 0;
            if (this.Pawn.Spawned)
            {
                num = Props.severityWhileOut;
            }
            else
            {
                num = Props.severityPerDay;
            }

            return num;
        }
    }




    public class HediffCompProperties_SeverityWhileInside : HediffCompProperties
    {
        public float severityPerDay;

        public bool showDaysToRecover;

        public bool showHoursToRecover;

        public float mechanitorFactor = 1f;

        public float severityWhileOut;
        

        public float reverseSeverityChangeChance;

        public FloatRange severityPerDayRange = FloatRange.Zero;

        public float minAge;

        public HediffCompProperties_SeverityWhileInside()
        {
            compClass = typeof(HediffComp_SeverityWhileInside);
        }

        public float CalculateSeverityPerDay()
        {
            return severityPerDay;
        }
    }
}