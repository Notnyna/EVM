using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace EVM
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    internal class EVM_FloatMenuMakerMap_Patch
    {
        [HarmonyPostfix]
        public static void AddRightClickOptions(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts) 
        {
            foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns(), true, null))
            {
                if ( (Pawn)localTargetInfo.Thing == pawn) { 

                    if (pawn.health.hediffSet.HasHediff<PreyContainer>()) {
                        // Regurgitate
                        List<PreyContainer> preyContainers = new List<PreyContainer>();
                        pawn.health.hediffSet.GetHediffs<PreyContainer>(ref preyContainers);

                        foreach (PreyContainer preyContainer in preyContainers)
                        {
                            foreach (Thing thing in preyContainer.innerContainer)
                            {
                                opts.Add(new FloatMenuOption("Regurgitate " + thing.LabelShort, delegate ()
                                {
                                    if (preyContainer.swallowWholeProperties.baseDamage > 0f)
                                    {
                                        pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Vomit));
                                    }

                                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Regurgitate, thing));
                                }));
                            }
                        }
                    }
                }
            }
            
            if (SwallowWholeLibrary.settings.debugOptions)
            {
                // For every Pawn in the map
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns(), true, null))
                {
                    Pawn food = (Pawn)localTargetInfo.Thing;

                    // Except the selected pawn
                    if (food != pawn)
                    {
                        // Vore
                        SwallowWholeProperties swallowWholeProperties = Utils.GetSwallowWholePropertiesFromTags(pawn, food);

                        for (int i = 0; i < swallowWholeProperties.digestiveTracks.Count; ++i)
                        {
                            swallowWholeProperties.trackId = i;

                            opts.Add(new FloatMenuOption("Eat " + food.LabelShort + " (" + swallowWholeProperties.digestiveTracks[i].purpose + ")", delegate () {
                                SwallowWholeProperties.passer.Add(swallowWholeProperties);
                                pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Eat, food));
                            }));
                        }
                    }/* Handled above?
                    else
                    {
                        // Regurgitate
                        List<PreyContainer> preyContainers = new List<PreyContainer>();
                        pawn.health.hediffSet.GetHediffs<PreyContainer>(ref preyContainers);

                        foreach (PreyContainer preyContainer in preyContainers)
                        {
                            foreach (Thing thing in preyContainer.innerContainer)
                            {
                                opts.Add(new FloatMenuOption("Regurgitate " + thing.LabelShort, delegate () {
                                    if (preyContainer.swallowWholeProperties.baseDamage > 0f)
                                    {
                                        pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Vomit));
                                    }

                                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.EVM_Regurgitate, thing));
                                }));
                            }
                        }
                    }*/
                }
            }
        }
    }
}
