using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Container
{
    internal class Cont_Machine
    {
        public string[] Names { get; }
        //public uint Constructor_Id { get; }

       // private CT_Machine _Machine = new CT_Machine();


        public uint? Year { get; set; }
        /*{
            get => _Machine.Year;
            internal set
            {
                _Machine.Year = value;
            }
        }*/
        public uint? FirstVersion {  get; set; }    
        /*{
            get => _Machine.FirstVersion;
            internal set
            {
                _Machine.FirstVersion = value;
            }
        }*/

        public string Category { get; set; }    
        /*{
            get => _Machine.Category;
            internal set
            {
                _Machine.Category = value;
            }
        }*/

        public Cont_Machine() { }


        public Cont_Machine(params string[] names)
        {

            this.Names = names;
            //this.Constructor_Id = contructor_id;
        }


        internal CT_Machine Get_Machine(string machineName)
        {
            for (int i = 0; i < Names.Length; i++)
            {
                if (Names[i].Equals(machineName))
                {
                    CT_Machine machine = new CT_Machine();
                    machine.Nom = Names[i];
                    //machine.IDConstructeur = this.Constructor_Id;
                    machine.Year = this.Year;
                    machine.FirstVersion = this.FirstVersion;   
                    machine.Category = this.Category;   
                    return machine;
                }

            }

            return null;


        }
    }
}
