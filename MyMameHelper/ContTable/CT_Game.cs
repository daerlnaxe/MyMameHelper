using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class CT_Game : M_TypeRom, IEquatable<CT_Game>, INotifyPropertyChanged, iCT_Games
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public uint ID { get; set; }

        /// <summary>
        /// Game  can be modified by user
        /// </summary>
        public string Game_Name { get; set; }
               
        public uint? Machine { get; set; }

        private bool? _UnWanted =  false;
        public bool? Unwanted
        {
            get { return _UnWanted; }
            set
            {
                if (value != _UnWanted)
                {
                    _UnWanted = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public uint? Developer { get; set; }

        public uint? _Genre;

        public uint? Genre
        {
            get { return _Genre; }
            set
            {
                if(_Genre != value)
                {
                    _Genre = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool? IsMahjong { get; set; } = false;
        public bool? IsQuizz { get; set; } = false;

        public uint? _Rate;
        public uint? Rate
        {
            get { return _Rate; }
            set
            {
                if (_Rate != value)
                {
                    _Rate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CT_Game()
        {

        }

        public CT_Game(string description, string gameName, List<CT_Rom> roms=null)
        {
            this.Description = description;
            this.Game_Name = gameName;
            this.Roms = roms;
        }

        public CT_Game(CT_Game game)
        {
            ID = game.ID;
            Game_Name = game.Game_Name;
            Description = game.Description;
         //   SourceFile = game.SourceFile;
            Machine = game.Machine;
            Unwanted = game.Unwanted;
            Developer = game.Developer;
            Genre = game.Genre;
            IsMahjong = game.IsMahjong;
            IsQuizz = game.IsQuizz;
        }

        public bool Equals(CT_Game other)
        {
            if (other == null || Game_Name == null)
                return false;

            bool res = ID == other.ID && Game_Name.Equals(other.Game_Name);
            return res;
        }

        public static CT_Game Result2Class(Dictionary<string, object> dico)
        {
            CT_Game cTC = new CT_Game();

            cTC.ID = Trans.GetUInt("ID", dico);
            //cTC.Parent_Name = Trans.GetString("Archive_Name", dico);
            cTC.Game_Name = Trans.GetString("Game_Name", dico);           
            cTC.Description = Trans.GetString("Description", dico);           
            cTC.Machine = Trans.GetNullableUInt("Machine", dico);
            cTC.Unwanted = Trans.GetBool("Unwanted", dico);
            cTC.Genre = Trans.GetNullableUInt("Genre", dico);
            cTC.Developer = Trans.GetNullableUInt("Developer", dico);
            cTC.Rate = Trans.GetNullableUInt("Rate", dico);
            cTC.IsMahjong = Trans.GetBoolFalse("IsMahjong", dico);
            cTC.IsQuizz = Trans.GetBoolFalse("IsQuizz", dico);

            return cTC;
        }
    }
}
