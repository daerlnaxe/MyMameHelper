using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Machine
    {
        public uint ID { get; set; }
        public string Nom { get; set; }
        public string Revision { get; set; }
        public uint IDConstructeur
        {
            get;
            set; }
        public uint Year { get; set; }

        public bool AllowCPath { get; set; }

        public CT_Machine(Aff_Machine machine)
        {
            this.ID = machine.ID;
            this.Nom = machine.Nom;
            this.Revision = machine.Revision;
            this.IDConstructeur = machine.IDConstructeur;
            this.Year = machine.Year;
            this.AllowCPath = machine.AllowCPath;
        }

        public CT_Machine()
        {

        }


        public static CT_Machine Result2Class(Dictionary<string, object> dico)
        {
            CT_Machine cTC = new CT_Machine();

            cTC.ID = Trans.GetUInt("ID", dico);
            cTC.Nom = Trans.GetString("Nom", dico);
            cTC.Revision = Trans.GetString("Revision", dico);
            cTC.IDConstructeur = Trans.GetUInt("Constructeur", dico);
            cTC.Year = Trans.GetUInt("Year", dico);

            return cTC;
        }
    }
}
