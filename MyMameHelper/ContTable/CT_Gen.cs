
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Gen : CT_Base
    {
        private uint? _ID;
        public uint? ID
        {
            get { return _ID; }
            set
            {
                if(value != _ID && value != null)
                {
                    _ID = value;
                    NotifyChange(new PropertyChangedEventArgs("ID"));
                }
            }
        }


        private string _Valeur;
        public string Valeur
        {
            get { return _Valeur; }
            set
            {
                if (value != _Valeur && value != null)
                {
                    _Valeur = value;
                    NotifyChange(new PropertyChangedEventArgs("Valeur"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;



        private CT_Gen Result2Class(Dictionary<string, object> dico)
        {
            throw new NotImplementedException();
        }
    }
}
