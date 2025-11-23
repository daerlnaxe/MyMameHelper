using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Machine 
    {
        /*: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

        public uint ID { get; set; }

        //public string _Nom;
        public string Nom {  get; set; }
        /*{
            get => _Nom;
            set
            {
                _Nom = value; 
               // NotifyPropertyChanged();
            }
        }*/

        public string Revision { get; set; }
        public uint IDConstructeur
        {
            get;
            set;
        }
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

        public static CT_Machine Result2Class(SQLiteDataReader reader)
        {
            CT_Machine cTC = new CT_Machine();

            cTC.ID = Trans.GetUInt("ID", reader);
            cTC.Nom = Trans.GetString("Nom", reader);
            cTC.Revision = Trans.GetString("Revision", reader);
            cTC.IDConstructeur = Trans.GetUInt("Constructeur", reader);
            cTC.Year = Trans.GetUInt("Year", reader);

            return cTC;
        }
    }
}
