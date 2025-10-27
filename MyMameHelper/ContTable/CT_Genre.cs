using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Genre
    {
        public uint ID { get; set; }
        public string Nom { get; set; }

        public static CT_Genre Result2Class(Dictionary<string, object> dico)
        {
            CT_Genre cTC = new CT_Genre();

            cTC.ID = Trans.GetUInt("ID", dico);
            cTC.Nom = Trans.GetString("Nom", dico);

            return cTC;
        }
    }
}
