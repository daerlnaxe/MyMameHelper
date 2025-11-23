using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Developer
    {
        public uint ID { get; set; }
        public string Nom { get; set; }

        public CT_Developer()
        {

        }

        public CT_Developer(string n)
        {
            Nom = n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dico"></param>
        /// <returns></returns>
        public static CT_Developer Result2Class(Dictionary<string, object> dico)
        {
            CT_Developer cTC = new CT_Developer();

            cTC.ID = Trans.GetUInt("ID", dico);
            cTC.Nom = Trans.GetString("Nom", dico);

            return cTC;
        }

    }
}
