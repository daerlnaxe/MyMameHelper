using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Constructeur
    {
        public uint ID { get; set; }
        public string Nom { get; set; }

        public CT_Constructeur()
        {

        }

        public CT_Constructeur(string n)
        {
            Nom = n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dico"></param>
        /// <returns></returns>
        public static CT_Constructeur Result2Class(Dictionary<string, object> dico)
        {
            CT_Constructeur cTC = new CT_Constructeur();

            cTC.ID = Trans.GetUInt("ID", dico);
            cTC.Nom = Trans.GetString("Nom", dico);

            return cTC;
        }

    }
}
