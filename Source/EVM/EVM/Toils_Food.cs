using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace EVM
{
    public static class Toils_Food
    {
        public static Toil SwallowWhole(SwallowWholeProperties swallowWholeProperties)
        {
            Toil toil = ToilMaker.MakeToil("Vore");
            toil.initAction = delegate ()
            {
                if (swallowWholeProperties.pred == null)
                {
                    swallowWholeProperties.pred = toil.actor;
                }
                if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("Vore toil for : " + toil.actor); }
                if (swallowWholeProperties.IsValid(true))
                {
                    if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("Vore valid"); }
                    PreyContainer preyContainer = (PreyContainer)swallowWholeProperties.pred.health.AddHediff(InternalDefOf.EVM_PreyContainer, swallowWholeProperties.pred.RaceProps.body.GetPartsWithDef(swallowWholeProperties.digestiveTracks[swallowWholeProperties.trackId].track[swallowWholeProperties.trackStage])[0]);
                    preyContainer.swallowWholeProperties = swallowWholeProperties;

                    Pawn pred = swallowWholeProperties.pred as Pawn;
                    Pawn preyP = swallowWholeProperties.prey as Pawn;
                    //Events if not dead thing
                    if (preyP != null)
                    {
                        Find.BattleLog.Add(new BattleLogEntry_Event(preyP, RulePackDefOf.Event_DevourerConsumeLeap, pred));
                        if (preyP.Faction != null && (int)preyP.def.race.intelligence >= 1)
                        {//voring animals don't scare people whuh?
                            TraitDef Maso = DefDatabase<TraitDef>.GetNamed("Masochist");
                            if (preyP.story.traits.HasTrait(Maso))
                            {
                                preyP.needs.mood.thoughts.memories.TryGainMemory(InternalDefOf.EVM_VoreMasochistPersonal, pred);
                            }
                            else
                            {
                                preyP.needs.mood.thoughts.memories.TryGainMemory(InternalDefOf.EVM_VoreGenericPersonal, pred);
                            }
                            TaleRecorder.RecordTale(InternalDefOf.EVM_VoreTale, preyP, pred);
                            foreach (Pawn bystander in pred.Map.mapPawns.AllHumanlikeSpawned.Where(x => x != pred && x != preyP && !x.Downed && !x.Suspended))
                            {
                                if (bystander.CanSee(pred))
                                {
                                    if (bystander.story.traits.HasTrait(Maso))
                                    {
                                        bystander.needs.mood.thoughts.memories.TryGainMemory(InternalDefOf.EVM_VoreMasochist, pred);
                                    }
                                    else
                                    {
                                        bystander.needs.mood.thoughts.memories.TryGainMemory(InternalDefOf.EVM_VoreGeneric, pred);
                                    }
                                }
                            }
                        }
                    }

                    //Reduce moving after vore
                    float struggle = Utils.GetBodysize(swallowWholeProperties.prey);
                    //twice the size double the time
                    struggle = struggle / pred.BodySize;
                    struggle = 1200 * struggle * SwallowWholeLibrary.settings.swallowSpeed;
                    pred.stances.stagger.StaggerFor((int)struggle, 0.3f);

                    //Add sound *****Eating/Gulp******
                    Map map = pred.Map;
                    SoundDef sound = InternalDefOf.EVM_Gokun;
                    sound?.PlayOneShot(new TargetInfo(pred.Position, map, false));

                    if (preyP != null && preyP.Faction !=null)//== Faction.OfPlayer)
                    { //hardcoded messages, yummy
                        if (preyP.Name != null)
                        {
                            Messages.Message(preyP.Name + " has been devoured alive!", pred, MessageTypeDefOf.NeutralEvent, false);
                        }
                        else
                        {
                            Messages.Message(preyP + " has been devoured alive!", pred, MessageTypeDefOf.NeutralEvent, false);
                        }
                    }

                    Utils.SwallowWhole(preyContainer, swallowWholeProperties.prey);
                    preyContainer.UpdateLabel();
                    preyContainer.CalculateMass();
                }
            };

            return toil;
        }

        public static Toil Regurgitate(Thing food)
        {
            Toil toil = ToilMaker.MakeToil("Regurgitate");
            toil.initAction = delegate ()
            {
                GenPlace.TryPlaceThing(food, toil.actor.Position, toil.actor.MapHeld, ThingPlaceMode.Near, null, null, default(Rot4));
                //Add sound *****Out******
                Map map = toil.actor.Map;
                SoundDef sound = InternalDefOf.EVM_Gokun;
                sound?.PlayOneShot(new TargetInfo(toil.actor.Position, map, false));
            };

            return toil;
        }
    }
}
