using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Rom: INotifyPropertyChanged, iCT_Rom
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public uint ID { get; set; }
        public string Archive_Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        public string Year { get; set; }

        private uint _Manufacturer;
        /// <summary>
        /// Developpeur
        /// </summary>
        public uint Manufacturer
        {
            get { return _Manufacturer; }
            set
            {
                if(value != _Manufacturer)
                {
                    _Manufacturer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsParent { get; set; }

        private bool? _Unwanted;
        public bool? Unwanted
        {
            get { return _Unwanted; }
            set
            {
                if(value != _Unwanted)
                {
                    _Unwanted = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public uint Clone_Of { get; set; }

        // Fichier source pour l'émulation (en général similaire à la carte mère)
        public string SourceFile { get; set; }

        /// <summary>
        /// Clones
        /// </summary>
        // public List<CT_Rom> Roms { get; set; } = new List<CT_Rom>();

        // Affichages
        public string Aff_Manufacturer { get; set; }

        public string Aff_Clone_Of { get; set; }


        public CT_Rom(string name)
        {
            this.Archive_Name = name;
        }

        public CT_Rom()
        {

        }

        public CT_Rom(CT_Rom another)
        {
            this.ID = another.ID;
            this.Archive_Name = another.Archive_Name;
            this.Description = another.Description;
            this.Aff_Clone_Of = another.Aff_Clone_Of;
            this.Clone_Of = another.Clone_Of;
            this.Aff_Manufacturer = another.Aff_Manufacturer;
            this.Manufacturer = another.Manufacturer;
            this.Unwanted = another.Unwanted;
            this.Year = another.Year;
            this.IsParent = another.IsParent;
           
        }

        internal static CT_Rom Result2Class(Dictionary<string, object> dico)
        {
            CT_Rom rom = new CT_Rom();
            rom.ID = Trans.GetUInt("ID", dico);
            rom.Archive_Name = Trans.GetString("Archive_Name", dico);
            rom.Description = Trans.GetString("Description", dico);
            rom.IsParent = Trans.GetBool("IsParent", dico);
            rom.Clone_Of = Trans.GetUInt("Clone_Of", dico);
            rom.Unwanted = Trans.GetBool("Unwanted", dico);

            return rom;
        }
    }
}
