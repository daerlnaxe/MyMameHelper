using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class Aff_Game : CT_Game
    { 

        private string _Aff_Machine;
        public string Aff_Machine
        {
            get { return _Aff_Machine; }
            set
            {
                if (value != _Aff_Machine)
                {
                    _Aff_Machine = value;
                    base.NotifyPropertyChanged();
                }
            }
        }

        private string _Aff_Developer;
        public string Aff_Developer
        {
            get { return _Aff_Developer; }
            set
            {
                if (value != _Aff_Developer)
                {
                    _Aff_Developer = value;
                    base.NotifyPropertyChanged();
                }
            }
        }

        private string _Aff_Genre;
        public string Aff_Genre
        {
            get { return _Aff_Genre; }
            set
            {
                if (value != _Aff_Genre)
                {
                    _Aff_Genre = value;
                    base.NotifyPropertyChanged();
                }
            }
        }

        public List<CT_Rom> Aff_Roms = new List<CT_Rom>();

        public Aff_Game() { }

        /*
        public Aff_Game(string archiveName, string gameName): base(archiveName, gameName)
        {


        }*/
        
        public Aff_Game(CT_Game game):base(game)
        {
            
        }
                
        public Aff_Game(Aff_Game game) 
        {
            ID = game.ID;
            Game_Name = game.Game_Name;
            Description = game.Description;
            //SourceFile = game.SourceFile;
            Machine = game.Machine;
            Aff_Machine = game.Aff_Machine;
            Unwanted = game.Unwanted;
            Developer = game.Developer;
            Aff_Developer = game.Aff_Developer;
            Genre = game.Genre;
            Aff_Genre = game.Aff_Genre;
            Rate = game.Rate;

            foreach (var rom in game.Roms)
                Roms.Add(new CT_Rom(rom));
        }

        public new static Aff_Game Result2Class(Dictionary<string, object> dico)
        {
            Aff_Game aGame = new Aff_Game(CT_Game.Result2Class(dico));
            return aGame;
        }
    }
}
