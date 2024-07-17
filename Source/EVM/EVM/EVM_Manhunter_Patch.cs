using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace EVM
{
    [HarmonyPatch(typeof(JobGiver_Manhunter), "TryGiveJob")]
    internal class EVM_JobGiver_Manhunter_Patch
    {
        [HarmonyPostfix]
        static void ManhuntVore(ref Job __result, Pawn pawn) {
            //if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("EVM manhunt patch reached init"); }
            if (__result == null) { return; }
            if (__result.targetA == null) { return; }
            Pawn prey = __result.targetA.Pawn;
            if (prey == null) { return; }
            Ability vore= null;
            try   { //is this better?
               vore = pawn.abilities.GetAbility(InternalDefOf.EVM_AbilityVore);
            }
            catch { return; }
            //if (SwallowWholeLibrary.settings.debugOptions) { Log.Message("EVM manhunt reached end"); }
            if (vore != null & vore.CanCast) {
                    __result = vore.GetJob(prey, prey);
            }
        }


    }
}
