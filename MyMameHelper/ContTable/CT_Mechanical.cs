using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Mechanical : M_TypeRom
    {
        public uint ID { get; set; }

        public string Meca_Name { get; set; }

        internal static CT_Mechanical Result2Class(Dictionary<string, object> dico)
        {

            CT_Mechanical meca = new CT_Mechanical();

            meca.ID = Trans.GetUInt("ID", dico);

            meca.Meca_Name = Trans.GetString("Meca_Name", dico);


            return meca;

        }
    }
}
