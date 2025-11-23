using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Game_Mapped : CT_Game
    {
        private List<CT_Rom> _RomList = new List<CT_Rom>();
        /// <summary>
        /// Clones
        /// </summary>
        public List<CT_Rom> Roms
        {
            get => _RomList;
            set
            {
                if (_RomList != value)
                {
                    _RomList = value;
                    NotifyPropertyChanged();

                }

            }
        }

        /// <summary>
        /// Machine
        /// </summary>
        private CT_Machine _Machine;
        public CT_Machine Machine
        {
            get { return _Machine; }
            set
            {
                if (value != _Machine)
                {
                    _Machine = value;
                    base.NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Developer
        /// </summary>
        private CT_Machine _Developer;
        public CT_Machine Developer
        {
            get { return _Developer; }
            set
            {
                if (value != _Developer)
                {
                    _Developer = value;
                    base.NotifyPropertyChanged();
                }
            }
        }


        public CT_Game_Mapped()
        {
        }
        public CT_Game_Mapped(CT_Game game) : base(game)
        {
        }

        public CT_Game_Mapped(CT_Game_Mapped game):base(game)
        {
           /*ID = game.ID;
            Game_Name = game.Game_Name;
            Description = game.Description;*/
            //SourceFile = game.SourceFile;

            Machine = game.Machine;

            /*Aff_Machine = game.Aff_Machine;*/

            //Unwanted = game.Unwanted;
            Developer = game.Developer;
            //  Aff_Developer = game.Aff_Developer;
            //Genre_Id = game.Genre_Id;
            //Aff_Genre = game.Aff_Genre;
            //Rate = game.Rate;

            foreach (var rom in game.Roms)
                Roms.Add(new CT_Rom(rom));
        }


    }
}
