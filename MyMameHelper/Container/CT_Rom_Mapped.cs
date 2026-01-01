using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    /// <summary>
    /// Représente la version enrichie avec le mapping, et non la version existante en base de donnée
    /// </summary>
    public class CT_Rom_Mapped: CT_Rom
    {
        /// <summary>
        /// Liaison avec les jeux
        /// </summary>
        /// <remarks>
        /// Ajout le 15/11/2025 pour changement
        /// </remarks>
        private CT_Game _Game;// = new CT_Game();
        public CT_Game Game
        {
            get => _Game;
            set
            {
                if (value != _Game)
                {
                    _Game = value;
                    NotifyPropertyChanged();
                }

            }
        }


        #region Machine
        public uint? Machine_Id { get; set; }
        
        private CT_Machine _Machine;// = new CT_Machine();
        #endregion Machine







        public CT_Rom_Mapped() { }
        public CT_Rom_Mapped(CT_Rom rom) : base(rom)
        {
        }



        public CT_Machine Machine
        {
            get => _Machine;
            set
            {
                if (value != _Machine)
                {
                    _Machine = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Localisation de l'archive sur le disque dur
        /// </summary>
        public string FilePath { get; set; }
    }
}
