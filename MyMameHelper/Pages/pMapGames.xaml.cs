using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
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



        private List<Map_RomGame> _RomsToMap = new List<Map_RomGame>();
        public List<Map_RomGame> RomsToMap
        {
            get => _RomsToMap;
            set
            {
                _RomsToMap = value;
                OnPropertyChanged();
            }
        }


        public List<Map_RomGame> SelectedRoms {  get; set; }


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


        public CT_Game GameSelected { get; set; }



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


        private List<Map_RomGame> _Tmp;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Chargement 
            using (SQLite_Req sqReq = new SQLite_Req())
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
                RomsToMap = _Tmp.ToList();

                aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();
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
            using (SQLite_Req sqReq = new SQLite_Req())
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
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                _Tmp = sqReq.Build4Game_List();


            }
        }


        /// <summary>
        /// Ajoute un jeu dans la base de données, sans linker les roms.
        /// </summary>
        /// <param name="value"></param>
        private void Add_GameOnDB(string value)
        {
            using (SQLite_Req sqReq = new SQLite_Req())
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
            DataGrid grid = (DataGrid)sender;
            SelectedRoms = grid.SelectedItems.Cast<Map_RomGame>().ToList();


        }

        private void Assign_Game(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Games.Count; i++) 
            { 
                var game = Games[i];

                game.Game_Name = GameSelected.Game_Name;
                game.ID = GameSelected.ID;
            }
        }
    }
}
