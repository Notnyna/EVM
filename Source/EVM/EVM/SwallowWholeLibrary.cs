using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using System.Diagnostics;

namespace EVM
{
    internal class SwallowWholeLibrary: Mod
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static Settings settings;

        public SwallowWholeLibrary(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard tabListing = new Listing_Standard()
            {
                ColumnWidth = (float)Math.Round((double)((inRect.width - 34f) / 3f))
            };
            tabListing.Begin(inRect);

            List<TabRecord> tabRecords = new List<TabRecord>();

            tabRecords.Add(new TabRecord(SettingTabs.ModOptions.GetDescription(), delegate()
            {
                settings.tab = SettingTabs.ModOptions;
            }, settings.tab == SettingTabs.ModOptions));

            tabRecords.Add(new TabRecord(SettingTabs.AnimalMawSize.GetDescription(), delegate ()
            {
                settings.tab = SettingTabs.AnimalMawSize;
            }, settings.tab == SettingTabs.AnimalMawSize));

            if (ModLister.BiotechInstalled)
            {
                tabRecords.Add(new TabRecord(SettingTabs.XenotypeMawSize.GetDescription(), delegate ()
                {
                    settings.tab = SettingTabs.XenotypeMawSize;
                }, settings.tab == SettingTabs.XenotypeMawSize));
            }

            if (ModLister.HasActiveModWithName("Humanoid Alien Races"))
            {
                tabRecords.Add(new TabRecord(SettingTabs.AlienMawSize.GetDescription(), delegate ()
                {
                    settings.tab = SettingTabs.AlienMawSize;
                }, settings.tab == SettingTabs.AlienMawSize));
            }

            Rect rect = inRect.AtZero();
            rect.yMin += tabListing.MaxColumnHeightSeen;
            rect.yMin += 42f;
            rect.height -= 42f;
            Widgets.DrawMenuSection(rect);
            TabDrawer.DrawTabs<TabRecord>(rect, tabRecords, 200f);
            //Rect viewRect = new Rect(0f, 0f, inRect.width - 30f, nutritionGainOptions.Length * 24 + 24 * 4);
            Rect innerRect = rect.GetInnerRect();

            switch (settings.tab)
            {
                case SettingTabs.ModOptions:
                    NutritionGainOptions[] nutritionGainOptions = (NutritionGainOptions[])Enum.GetValues(typeof(NutritionGainOptions));

                    Listing_Standard listing_Standard = new Listing_Standard();
                    listing_Standard.Begin(innerRect);

                    listing_Standard.Label("Nutrition Gain Moment");
                    foreach (NutritionGainOptions nutritionGainOption in nutritionGainOptions)
                    {
                        if(listing_Standard.RadioButton(nutritionGainOption.GetDescription(), settings.nutritionGainOption == (int)nutritionGainOption))
                        {
                            settings.nutritionGainOption = (int)nutritionGainOption;
                        }
                    }

                    listing_Standard.Gap(12f);

                    listing_Standard.CheckboxLabeled("Swallow Ignores Size", ref settings.swallowIgnoresSize, 1f);
                    listing_Standard.CheckboxLabeled("Swallow Ignores Mass", ref settings.swallowIgnoresMass, 1f);
                    listing_Standard.CheckboxLabeled("Swallow Ignores Contents", ref settings.swallowIgnoresContents, 1f);
                    listing_Standard.CheckboxLabeled("Debug Options", ref settings.debugOptions, 1f);
                    
                    if (float.TryParse(listing_Standard.TextEntryLabeled("Default Maw Size", settings.DefaultMawSize.ToString()), out float defaultMawSize))
                    {
                        settings.DefaultMawSize = defaultMawSize;
                    }
                    if (float.TryParse(listing_Standard.TextEntryLabeled("Swallow speed factor", settings.swallowSpeed.ToString()), out float SwallowSpeed))
                    {
                        settings.swallowSpeed = SwallowSpeed;
                    }
                    listing_Standard.CheckboxLabeled("Predators Swallow", ref settings.predatorsSwallow, 1f);

                    listing_Standard.End();
                    break;
                case SettingTabs.AnimalMawSize:
                    Rect outRect = new Rect(innerRect.x, innerRect.y, innerRect.width, innerRect.height);
                    Rect viewRect = new Rect(0f, 0f, innerRect.width - 30f, settings.mawList.Count * 24 + 24);
                    Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

                    Listing_Standard listing_Standard2 = new Listing_Standard();
                    listing_Standard2.Begin(viewRect);

                    listing_Standard2.Label("How big can the prey of a creature be compared to its own size.");

                    foreach (SettingsAnimal animal in settings.mawList.OrderBy(a => a.name))
                    {
                        float.TryParse(listing_Standard2.TextEntryLabeled(animal.name, animal.preySize.ToString()), out animal.preySize);
                    }

                    listing_Standard2.End();
                    Widgets.EndScrollView();
                    break;
                case SettingTabs.XenotypeMawSize:
                    Rect outRect3 = new Rect(innerRect.x, innerRect.y, innerRect.width, innerRect.height);
                    Rect viewRect3 = new Rect(0f, 0f, innerRect.width - 30f, settings.xenotypes.Count * 24 + 24);
                    Widgets.BeginScrollView(outRect3, ref scrollPosition, viewRect3);

                    Listing_Standard listing_Standard3 = new Listing_Standard();
                    listing_Standard3.Begin(viewRect3);

                    listing_Standard3.Label("How big can the prey of a creature be compared to its own size.");

                    foreach (XenotypeUnifier xenotype in settings.xenotypes.OrderBy(a => a.ToString()))
                    {
                        float.TryParse(listing_Standard3.TextEntryLabeled(xenotype.ToString(), xenotype.preySize.ToString()), out xenotype.preySize);
                    }

                    listing_Standard3.End();
                    Widgets.EndScrollView();
                    break;
                case SettingTabs.AlienMawSize:
                    Listing_Standard listing_Standard4 = new Listing_Standard();
                    listing_Standard4.Begin(innerRect);

                    listing_Standard4.Label("todo");

                    listing_Standard4.End();
                    break;
                default: break;
            }

            tabListing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Swallow Whole Library";
        }
    }
}
