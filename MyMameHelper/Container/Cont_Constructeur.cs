using MyMameHelper.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    /// <summary>
    /// Représente un constructeur et les machines associées
    /// </summary>
    internal class Cont_Constructeur
    {
        public uint? ID { get; set; }
        public string Constructeur { get; set; }


        //private List<string> _MachinesName { get; set; }

        public List<Cont_Machine> Machines { get; set; } = new List<Cont_Machine>();
        /*
        public List<CT_Machine> SetMachines
        {
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    var machine = value[i];
                    if(_MachinesName.Equals(machine.Nom.ToLower))
                    {

                    }
                }
            }
        }*/

        public Cont_Constructeur() { }

        public Cont_Constructeur(uint? id, string constructeur)
        {
            ID = id;
            Constructeur = constructeur;

        }
    }
}
