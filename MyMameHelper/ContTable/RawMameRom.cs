using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class RawMameRom
    {
        public uint ID { get; set; }

        public string Name { get; set; }
        public string Source_File { get; set; }
        public string Rom_Of { get; set; }
        public string Clone_Of { get; set; }
        public string Sample_Of { get; set; }

        #region type
        public bool Is_Bios { get; set; }
        public bool Is_Mechanical { get; set; }
        #endregion

        public string Description { get; set; }

        public string Year { get; set; }

        public string Manufacturer { get; set; }

        public static RawMameRom Result2Class(Dictionary<string, object> dico)
        {
            RawMameRom ctR = new RawMameRom();
            ctR.ID = Trans.GetUInt("ID", dico);
            ctR.Name = Trans.GetString("Name", dico);
            ctR.Source_File = Trans.GetString("Source_File", dico);
            ctR.Rom_Of = Trans.GetString("Rom_Of", dico);
            ctR.Clone_Of = Trans.GetString("Clone_Of", dico);
            ctR.Sample_Of = Trans.GetString("Sample_Of", dico);

            ctR.Is_Bios = Trans.GetBool("Is_Bios", dico);
            ctR.Is_Mechanical = Trans.GetBool("Is_Mechanical", dico);

            ctR.Description = Trans.GetString("Description", dico);
            ctR.Year = Trans.GetString("Year", dico);
            ctR.Manufacturer = Trans.GetString("Manufacturer", dico);

            return ctR;
        }

    }
}
