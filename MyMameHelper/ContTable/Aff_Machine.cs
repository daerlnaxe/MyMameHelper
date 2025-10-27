using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class Aff_Machine: CT_Machine
    {
        public string Constructeur { get; set; }

        public static Aff_Machine Result2Class(Dictionary<string, object> dico)
        {
            Aff_Machine cTC = new Aff_Machine();

            cTC.ID = Trans.GetUInt("ID", dico);
            cTC.Nom = Trans.GetString("Nom", dico);
            cTC.Constructeur = Trans.GetString("Constructeur", dico);
            cTC.Revision = Trans.GetString("Revision", dico);
            cTC.Year = Trans.GetUShort("Year", dico);

            return cTC;
        }
    }
}
