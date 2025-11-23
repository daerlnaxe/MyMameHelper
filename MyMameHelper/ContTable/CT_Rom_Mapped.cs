using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Rom_Mapped: CT_Rom
    {
        /// <summary>
        /// Liaison avec les jeux
        /// </summary>
        /// <remarks>
        /// Ajout le 15/11/2025 pour changement
        /// </remarks>
        private CT_Game _Game = new CT_Game();
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
    }
}
