using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EVM
{
    internal class SettingsAnimal: IExposable
    {
        public string name;
        public string defName;
        public float preySize;
        //public bool voreAbility;


        public SettingsAnimal(string name, string defname, float preySize)
        {
            this.name = name;
            this.defName = defname;
            this.preySize = preySize;
        }

        public SettingsAnimal(string name, string defname)
        {
            this.name = name;
            this.defName = defname;
            this.preySize = SwallowWholeLibrary.settings.DefaultMawSize;
        }

        public SettingsAnimal(string defname, float preySize)
        {
            this.name = defname;
            this.defName = defname;
            this.preySize = preySize;
        }

        public SettingsAnimal(string defname)
        {
            this.name = defname;
            this.defName = defname;
            this.preySize = SwallowWholeLibrary.settings.DefaultMawSize;
        }

        // Mandatory
        public SettingsAnimal()
        {
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref name, "EVM_AnimalSettings_Name");
            Scribe_Values.Look<string>(ref defName, "EVM_AnimalSettings_DefName");
            Scribe_Values.Look<float>(ref preySize, "EVM_Swallow_AnimalSettings_PreySize");
        }
    }
}
