using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EVM
{
    [DefOf]
    public static class InternalDefOf
    {
        public static HediffDef EVM_PreyContainer;
        public static HediffDef EVM_Digested = DefDatabase<HediffDef>.GetNamed("EVM_Digested");
        public static HediffDef EVM_Heartburn = DefDatabase<HediffDef>.GetNamed("EVM_Heartburn");
        
        public static JobDef EVM_Eat;
        public static JobDef EVM_Regurgitate;

        public static AbilityDef EVM_AbilityVore = DefDatabase<AbilityDef>.GetNamed("EVM_AbilityVore");

        public static TaleDef EVM_VoreTale;
        public static ThoughtDef EVM_VoreGeneric = DefDatabase<ThoughtDef>.GetNamed("EVM_VoreGeneric");
        public static ThoughtDef EVM_VoreMasochist = DefDatabase<ThoughtDef>.GetNamed("EVM_VoreMasochist");
        public static ThoughtDef EVM_VoreGenericPersonal = DefDatabase<ThoughtDef>.GetNamed("EVM_VoreGenericPersonal");
        public static ThoughtDef EVM_VoreMasochistPersonal = DefDatabase<ThoughtDef>.GetNamed("EVM_VoreMasochistPersonal");
        public static SoundDef EVM_Gokun = DefDatabase<SoundDef>.GetNamed("EVM_Gokun");
        public static SoundDef EVM_Mog = DefDatabase<SoundDef>.GetNamed("EVM_Mog");
        public static SoundDef EVM_Resist = DefDatabase<SoundDef>.GetNamed("EVM_Resist");
        public static SoundDef EVM_Juice = DefDatabase<SoundDef>.GetNamed("EVM_Juice");
        public static RecipeDef EVM_RescuePrey = DefDatabase<RecipeDef>.GetNamed("EVM_RescuePrey");

        public static GeneCategoryDef EVM_DigestionWorker;

        // Core
        public static DamageArmorCategoryDef Blunt;

        public static BodyPartDef Stomach;
        //public static BodyPartDef Jaw;

        public static PawnCapacityDef Metabolism;
    }
}
