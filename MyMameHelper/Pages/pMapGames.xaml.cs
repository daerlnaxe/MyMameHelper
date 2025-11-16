using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pMapGames.xaml
    /// </summary>
    public partial class pMapGames : Page, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Liste des roms à mapper
        /// </summary>
        private List<CT_Game> _RomsToMap = new List<CT_Game>();
        public List<CT_Game> RomsToMap
        {
            get => _RomsToMap;
            set
            {
                _RomsToMap = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Roms Sélectionnées
        /// </summary>
        /// <remarks>
        /// Change en fonction de l'event sur le datagrid
        /// </remarks>
        public List<CT_Game> SelectedRoms { get; set; }



        public ObservableCollection<CT_Rom> OrpheanRoms { get; set; } = new ObservableCollection<CT_Rom>();

        private CT_Rom _SelectedOrpheanRom;
        public CT_Rom SelectedOrpheanRom 
        {
            get => _SelectedOrpheanRom;
            set
            {
                if(value != _SelectedOrpheanRom)
                {
                    _SelectedOrpheanRom = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Liste des jeux
        /// </summary>
        private List<CT_Game> _Games;
        public List<CT_Game> Games
        {
            get => _Games;
            set
            {
                if (_Games != value)
                {
                    _Games = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Jeu Selectionné dans la ListBox
        /// </summary>
        private CT_Game _SelectedGame;
        public CT_Game SelectedGame
        {
            get => _SelectedGame;
            set
            {
                if (value != _SelectedGame)
                {
                    _SelectedGame = value;
                    OnPropertyChanged();
                }

            }
        }






        private string _GameToAdd;
        public string GameToAdd
        {
            get => _GameToAdd;
            set
            {
                if (Games.FirstOrDefault(x => x.Game_Name == value) == null)
                {
                    //Games.Add(value);
                    Add_GameOnDB(value);
                }

                _GameToAdd = "";
                OnPropertyChanged();

            }
        }



        public pMapGames()
        {
            InitializeComponent();

            DataContext = this;
        }

        //
        //private List<Map_RomGame> _Tmp;
        private List<CT_Game> _Tmp;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Chargement 
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                /*Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Developers, all: true));
                /*Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));*/

                //
                //RomsToMap.ChangeContent = sqReq.AffRoms_List();

                //
                // System.Data.SQLite.SQLiteDataReader tmp = sqReq.AffGames_SQL(null, null);

                // Chargement asynchrone des roms
                AsyncWindowProgress aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadMapGames);
                aLoad.ShowDialog();
                RomsToMap = _Tmp;


                // Charement asynchrone des jeux
                /*aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();*/
                //OnPropertyChanged("Games");
            }
        }


        /// <summary>
        /// Récupère en base uniquement les jeux (Games)
        /// </summary>
        /// <param name="window"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AsyncLoadGames(AsyncWindowProgress aLoad)
        {
            List<CT_Game> games = new List<CT_Game>();

            aLoad.AsyncMessage("Loading Roms...");
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                SQLiteDataReader reader = sqReq.SimpleGames_SQL();

                if (reader.HasRows)
                {
                    //dicCol = Get_Poss(reader);

                    while (reader.Read())
                    {
                        CT_Game mr = new CT_Game();
                        mr.ID = Trans.GetUInt("ID", reader);
                        mr.Game_Name = Trans.GetString("Game_Name", reader);


                        //Ag.Game_Name = Trans.GetString("Game_Name", reader);
                        games.Add(mr);
                    }
                }
            }
            ;

            Games = games;
        }



        /// <summary>
        /// Récupère en base les valeurs avec liaison des deux tables
        /// </summary>
        /// <param name="aLoad"></param>
        private void AsyncLoadMapGames(AsyncWindowProgress aLoad)
        {
            aLoad.AsyncMessage("Loading Roms...");
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                _Tmp = sqReq.QueryGameWithRoms();


            }
        }


        /// <summary>
        /// Ajoute un jeu dans la base de données, sans linker les roms.
        /// </summary>
        /// <param name="value"></param>
        private void Add_GameOnDB(string value)
        {
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                sqReq.Insert_Game(
                    new CT_Game()
                    {
                        Game_Name = value
                    }
                );

                var aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();

            }
        }

        private void RomsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox grid = (ListBox)sender;
            SelectedRoms = grid.SelectedItems.Cast<CT_Game>().ToList();
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            SelectedGame = (CT_Game)item.DataContext;

        }




        private void Assign_Game(object sender, RoutedEventArgs e)
        {

            throw new Exception("deprecated ? ");
            for (int i = 0; i < Games.Count; i++)
            {
                var game = Games[i];

                //game.Game_Name = Gam.Game_Name;
                //                game.ID = GameSelected.ID;
            }
        }


        #region Roms
        private void RemoveRom_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;

            List<CT_Rom> tmp = new List<CT_Rom>();
            for (int i = 0; i < SelectedGame.Roms.Count; i++)
            {
                //foreach (var rom in SelectedGame.Roms)
                {
                    CT_Rom rom = SelectedGame.Roms[i];
                    if (rom.ID != uint.Parse(bt.Tag.ToString()))
                    {
                        tmp.Add(rom);
                    }
                    else
                    {
                        OrpheanRoms.Add(rom);
                    }
                }
            }
            SelectedGame.Roms = tmp;
        }

        private void AddRom_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new List<CT_Rom> (SelectedGame.Roms);
            
            //
            tmp.Add(SelectedOrpheanRom);




            SelectedGame.Roms = tmp;    


            OrpheanRoms.Remove(SelectedOrpheanRom); 
        }
        #endregion Roms


    }
}
