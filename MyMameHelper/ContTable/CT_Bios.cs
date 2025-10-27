using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Bios : M_TypeRom
    {
        public uint ID { get; set; }

        /// <summary>
        /// Archive Name
        /// </summary>
        public string Bios_Name { get; set; }

        internal static CT_Bios Result2Class(Dictionary<string, object> dico)
        {
            CT_Bios bios = new CT_Bios();
            bios.ID = Trans.GetUInt("ID", dico);
            bios.Bios_Name = Trans.GetString("Bios_Name", dico);

            return bios;
        }
    }
}
