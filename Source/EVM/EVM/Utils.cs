using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using EVM.Digestion;

namespace EVM
{
    public static class Utils
    {
        /// <summary>
        /// Gets all the BodyPartExtensions and returns a VoreProperties with them
        /// </summary>
        /// <param name="pred">the pawn from who we get the properties</param>
        /// <param name="prey">not useful, but part of SwallowWholeProperties, so might as well assign it now</param>
        /// <param name="trackId">digestive track id in use</param>
        /// <param name="trackStage">id pointing to stomach in track pointed by trackId</param>
        /// <returns></returns>
        public static SwallowWholeProperties GetSwallowWholePropertiesFromTags(Pawn pred, Thing prey = null, int trackId = 0, int trackStage = 0, bool struggle = true)
        {
            SwallowWholeProperties swallowWholeProperties = new SwallowWholeProperties(pred, prey);
            swallowWholeProperties.trackId = trackId;
            swallowWholeProperties.trackStage = trackStage;
            swallowWholeProperties.struggle = struggle;
            
            // Maw
            //List<BodyPartRecord> jaws = pred.RaceProps.body.GetPartsWithDef(InternalDefOf.Jaw);
            //BodyPartExtension bodyPartExtension = jaws[0].def.GetModExtension<BodyPartExtension>();

            //if (jaws.Count > 0)
            //{
            //    if (bodyPartExtension != null)
            //    {
            //        if (bodyPartExtension.mawSize != -1)
            //        {
            //            voreProperties.mawSize = bodyPartExtension.mawSize;
            //        }
            //    }
            //}
            swallowWholeProperties.mawSize = SwallowWholeLibrary.settings.DefaultMawSize;

            if (pred.RaceProps.Animal)
            {
                IEnumerable<float> matches = from m 
                                             in SwallowWholeLibrary.settings.mawList
                                             where m.defName == pred.kindDef.defName
                                             select m.preySize;

                if (matches.Count() > 0)
                {
                    swallowWholeProperties.mawSize = matches.First();
                }
            } 
            else if (pred.RaceProps.Humanlike)
            {
                //Log.Message(pred.Name.ToStringFull);
                //Log.Message(pred.genes);
                //Log.Message(pred.genes.Xenotype);
                //Log.Message(pred.genes.Xenotype.defName);
                foreach (XenotypeUnifier xenotypeUnifier in SwallowWholeLibrary.settings.xenotypes)
                {
                    //Log.Message("You'd better not be null");
                    if (pred.genes.Xenotype != null)
                    {
                        if (xenotypeUnifier.ToString() == pred.genes.Xenotype.defName)
                        {
                            swallowWholeProperties.mawSize = xenotypeUnifier.preySize;
                            break;
                        }
                    }
                    else if (pred.genes.CustomXenotype != null)
                    {
                        if (xenotypeUnifier.ToString() == pred.genes.CustomXenotype.name)
                        {
                            swallowWholeProperties.mawSize = xenotypeUnifier.preySize;
                            break;
                        }
                    }
                }

                //IEnumerable<float> matches = from x
                //                             in SwallowWholeLibrary.settings.xenotypes
                //                             where x.ToString() == pred.genes.Xenotype.defName || 
                //                                x.ToString() == pred.genes.CustomXenotype.name
                //                             select x.preySize;
                    
                //if (matches.Count() > 0)
                //{
                //    swallowWholeProperties.mawSize = matches.First();
                //}
            }
            //Log.Message("DT");
            // Digestive Tracks
            BodyPartExtension bodyPartExtension = pred.RaceProps.body.GetModExtension<BodyPartExtension>();
            if (bodyPartExtension != null) 
            { 
                if (bodyPartExtension.digestiveTracks != null)
                {
                    swallowWholeProperties.digestiveTracks = bodyPartExtension.digestiveTracks;
                }
            }
            //Log.Message("Stomach");
            // Stomach
            if (trackId < swallowWholeProperties.digestiveTracks.Count)
            {
                if (trackStage < swallowWholeProperties.digestiveTracks[trackId].track.Count)
                {
                    bodyPartExtension = swallowWholeProperties.digestiveTracks[trackId].track[trackStage].GetModExtension<BodyPartExtension>();

                    if (bodyPartExtension != null)
                    {
                        if (bodyPartExtension.baseDamage != -1)
                        {
                            swallowWholeProperties.baseDamage = bodyPartExtension.baseDamage;
                        }

                        if (bodyPartExtension.digestionEfficiancy != -1)
                        {
                            swallowWholeProperties.digestionEfficiancy = bodyPartExtension.digestionEfficiancy;
                        }

                        if (bodyPartExtension.digestionDamageType != null)
                        {
                            swallowWholeProperties.digestionDamageType = bodyPartExtension.digestionDamageType;
                        }

                        if (bodyPartExtension.comfort != -1)
                        {
                            swallowWholeProperties.comfort = bodyPartExtension.comfort;
                        }

                        if (bodyPartExtension.armorValues != null)
                        {
                            swallowWholeProperties.armorValues = bodyPartExtension.armorValues;
                        }

                        if (bodyPartExtension.canDigest != null)
                        {
                            swallowWholeProperties.canDigest = bodyPartExtension.canDigest;
                        }

                        if (bodyPartExtension.deadline != -1)
                        {
                            swallowWholeProperties.deadline = bodyPartExtension.deadline;
                        }

                        if (bodyPartExtension.digestionWorker != null)
                        {
                            if (Activator.CreateInstance(bodyPartExtension.digestionWorker) is DigestionWorker digestionWorker)
                            {
                                swallowWholeProperties.digestionWorker = digestionWorker;
                            }
                        }

                        if (bodyPartExtension.grantsNutrition != false)
                        {
                            swallowWholeProperties.grantsNutrition = bodyPartExtension.grantsNutrition;
                        }

                        if (bodyPartExtension.nutritionCost != 0)
                        {
                            swallowWholeProperties.nutritionCost = bodyPartExtension.nutritionCost;
                        }
                    }
                }
                else
                {
                    //Log.Error("[EVM.Utils.GetSwallowWholePropertiesFromTags]: trackStage out of bounds");
                }
            }
            else
            {
                //Log.Error("[EVM.Utils.GetSwallowWholePropertiesFromTags]: trackId out of bounds");
            }
            //Log.Message("End");
            return swallowWholeProperties;
        }

        public static bool SwallowWhole(PreyContainer preyContainer, Thing thing)
        {
            thing.DeSpawnOrDeselect(DestroyMode.Vanish);
            if (thing is Pawn pawn)
            {
                if (!pawn.Dead) {
                    pawn.health.AddHediff(InternalDefOf.EVM_Digested, null, null, null);
                }
            }
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, preyContainer.innerContainer, thing.stackCount, true);
                return true;
            }
            else
            {
                return preyContainer.innerContainer.TryAdd(thing, true);
            }
        }

        //Is this even neccesary? I have a feeling I am missing something
        public static float GetBodysize(Thing thing) 
        {
            if (thing is Pawn p)
            {
                return p.BodySize;
            }
            else if (thing is Corpse c)
            {
                return c.InnerPawn.BodySize;
            }
            else { //mass, CE bulk or some other?
                return 0.5f;
            }
        }

        public static bool Dodged(Pawn prey) {
            //RimWorld.Verb_MeleeAttack
            //assume prey is not a rock
            Thing thing = prey as Thing;
            float num = prey.GetStatValue(StatDefOf.MeleeDodgeChance, true, -1);
            if (thing.def.category == ThingCategory.Pawn && !prey.Downed)
            {
                if (prey.GetPosture() != PawnPosture.Standing) { return false; }
            }
            Stance_Busy stance_Busy = prey.stances.curStance as Stance_Busy;
            if (stance_Busy != null && stance_Busy.verb != null && !stance_Busy.verb.verbProps.IsMeleeAttack)
            {
                return false;
            }

            if (ModsConfig.IdeologyActive)
            {
                if (DarknessCombatUtility.IsOutdoorsAndLit(prey))
                {
                    num += prey.GetStatValue(StatDefOf.MeleeDodgeChanceOutdoorsLitOffset, true, -1);
                }
                else if (DarknessCombatUtility.IsOutdoorsAndDark(prey))
                {
                    num += prey.GetStatValue(StatDefOf.MeleeDodgeChanceOutdoorsDarkOffset, true, -1);
                }
                else if (DarknessCombatUtility.IsIndoorsAndDark(prey))
                {
                    num += prey.GetStatValue(StatDefOf.MeleeDodgeChanceIndoorsDarkOffset, true, -1);
                }
                else if (DarknessCombatUtility.IsIndoorsAndLit(prey))
                {
                    num += prey.GetStatValue(StatDefOf.MeleeDodgeChanceIndoorsLitOffset, true, -1);
                }
            }
            if (Rand.Chance(num))
            {
                return true;
            }
            return false;

        }

        // Predicates
        public static bool IsStoneOrMetal(Thing thing)
        {
            if (thing.def != null)
            {
                if (thing.def.stuffProps != null)
                {
                    if (thing.def.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic) ||
                        thing.def.stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
                    {
                        return true;
                    }
                }
            }

            if (thing is Pawn pawn)
            {
                if (pawn.RaceProps != null)
                {
                    if (pawn.RaceProps.IsMechanoid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
