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
    public class CT_Game : M_GameType, IEquatable<CT_Game>, iCT_Games
    {


        public uint ID { get; set; }

        /// <summary>
        /// Game  can be modified by user
        /// </summary>
        public string Game_Name { get; set; }


        #region Machine

        

        private uint? _Machine_ID;

        public uint? Machine_Id
        {
            get => _Machine_ID;
            set
            {
                if (value != _Machine_ID)
                {
                    _Machine_ID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion Machine

        private bool? _UnWanted = false;
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

        #region Developer
        private uint? _Constructeur_ID;
        public uint? Constructeur_ID
        {
            get => _Constructeur_ID;
            set
            {
                if (value != _Constructeur_ID)
                {
                    _Constructeur_ID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion Developer


        #region Genre
        private uint? _Genre_Id;

        public uint? Genre_Id
        {
            get { return _Genre_Id; }
            set
            {
                if (_Genre_Id != value)
                {
                    _Genre_Id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion Genre


        private bool? _IsMahjong = false    ;
        public bool? IsMahjong 
        {
            get => _IsMahjong;
            set
            {
                if(value != _IsMahjong)
                {
                    _IsMahjong = value;
                    NotifyPropertyChanged();
                }

            }
        }


        private bool? _IsQuizz;
        public bool? IsQuizz 
        { 
            get=>_IsQuizz;
            set
            {
                if(_IsQuizz != value)
                {
                    _IsQuizz = value;
                    NotifyPropertyChanged();
                }
            } 
        }

       
        private bool? _IsFruit = false ;
        public bool? IsFruit
        {
            get => _IsFruit;
            set
            {
                if (value != _IsFruit)
                {
                    _IsFruit = value;
                    NotifyPropertyChanged();
                }
            }
        }


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

        public CT_Game(string description, string gameName)
        {
            this.Description = description;
            this.Game_Name = gameName;
        }

        public CT_Game(CT_Game game)
        {
            ID = game.ID;
            Game_Name = game.Game_Name;
            Description = game.Description;
            //   SourceFile = game.SourceFile;
            Machine_Id = game.Machine_Id;
            Unwanted = game.Unwanted;
            Constructeur_ID = game.Constructeur_ID;
            Genre_Id = game.Genre_Id;
            IsMahjong = game.IsMahjong;
            IsQuizz = game.IsQuizz;
            IsFruit = game.IsFruit;
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
            cTC.Machine_Id = Trans.GetNullableUInt("Machine", dico);
            cTC.Unwanted = Trans.GetBool("Unwanted", dico);
            cTC.Genre_Id = Trans.GetNullableUInt("Genre", dico);
            cTC.Constructeur_ID = Trans.GetNullableUInt("Constructor_Id", dico);
            cTC.Rate = Trans.GetNullableUInt("Rate", dico);
            cTC.IsMahjong = Trans.GetBoolFalse("IsMahjong", dico);
            cTC.IsQuizz = Trans.GetBoolFalse("IsQuizz", dico);
            cTC.IsFruit = Trans.GetBoolFalse("IsFruit", dico);

            return cTC;
        }
    }
}
