using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;
using EVM.Digestion;

namespace EVM
{
    [StaticConstructorOnStartup]
    public static class OnStartUp
    {
        static OnStartUp() 
        {
            // Harmony
            Harmony harmony = new Harmony("com.evm");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            float defaultMaw = SwallowWholeLibrary.settings.defaultMawSize;
            Log.Message("startup achieved, defaultmaw: " + defaultMaw);
            // SettingAnimals
            foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (pawnKindDef.RaceProps.Animal)
                {
                    bool found = false;
                    foreach (SettingsAnimal animal in SwallowWholeLibrary.settings.mawList)
                    {
                        if (pawnKindDef.defName == animal.defName)
                        {
                            found = true;
                            if (!SwallowWholeLibrary.settings.predatorsSwallow) {
                                break;
                            }//Logic to add abilities to animals with bigger than default maws
                            if (animal.preySize > defaultMaw)   {
                                if (SwallowWholeLibrary.settings.debugOptions)
                                {
                                    Log.Message("Adding vore ability: " +pawnKindDef.defName);
                                }
                                pawnKindDef.RaceProps.overrideShouldHaveAbilityTracker = true;
                                if (pawnKindDef.abilities == null) {
                                    pawnKindDef.abilities = new List<AbilityDef>()
                                    {
                                        InternalDefOf.EVM_AbilityVore
                                    };
                                } else  {
                                    pawnKindDef.abilities.Add(InternalDefOf.EVM_AbilityVore);
                                }
                            }
                            break;
                        }
                    }

                    if (!found)
                    {
                        SettingsAnimal settingsAnimal = new SettingsAnimal(pawnKindDef.label, pawnKindDef.defName);

                        foreach (SettingsAnimal exception in SwallowWholeLibrary.settings.animalMawException)
                        {
                            if (settingsAnimal.defName == exception.defName)
                            {
                                settingsAnimal.preySize = exception.preySize;
                                break;
                            }
                        }

                        SwallowWholeLibrary.settings.mawList.Add(settingsAnimal);
                    }
                    


                }
            }
            
            // SettingXenotypes
            foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
            {
                bool found = false;
                foreach (XenotypeUnifier xenotypeUnifier in SwallowWholeLibrary.settings.xenotypes)
                {
                    if (xenotypeDef.defName == xenotypeUnifier.defName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    XenotypeUnifier xenotypeUnifier = new XenotypeUnifier(xenotypeDef.defName, xenotypeDef.label);
                    
                    foreach (XenotypeUnifier exception in SwallowWholeLibrary.settings.xenotypeMawException)
                    {
                        if (xenotypeUnifier.ToString() == exception.ToString())
                        {
                            xenotypeUnifier.preySize += exception.preySize;
                            break;
                        }
                    }
                    
                    SwallowWholeLibrary.settings.xenotypes.Add(xenotypeUnifier);
                }
            }
            
            // SettingHAR

            //if (ModLister.BiotechInstalled)
            //{
            //    Make Digestion Worker Genes
            //    int displayOrder = 10;

            //    foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(s => s.GetTypes())
            //    .Where(myType => myType.IsClass &&
            //        !myType.IsAbstract &&
            //        myType.IsSubclassOf(typeof(DigestionWorker))))
            //    {
            //        string rawName = type.ToString().Remove(0, type.ToString().IndexOf('_') + 1);

            //        DefDatabase<GeneDef>.Add(new GeneDef()
            //        {
            //            defName = "EVM_" + rawName + "Stomach",
            //            label = rawName + " Stomach",
            //            description = "Autogenerated gene based on DigestionWorker",
            //            iconPath = "UI/Icons/Genes/EVM_" + rawName + "Stomach",
            //            displayCategory = InternalDefOf.EVM_DigestionWorker,
            //            displayOrderInCategory = ++displayOrder,
            //            biostatCpx = 2,
            //            biostatMet = 1,
            //            selectionWeight = 0
            //        });
            //    }
            //}
        }
    }
}
