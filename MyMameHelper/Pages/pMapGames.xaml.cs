using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using PProp = MyMameHelper.Properties.Settings;


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


        #region  Roms
        /// <summary>
        /// Roms Sélectionnées
        /// </summary>
        /// <remarks>
        /// Change en fonction de l'event sur le datagrid
        /// </remarks>
        public List<CT_Rom> SelectedRoms { get; set; }


        /// <summary>
        /// Orphean Roms, when you remove a rom from a game
        /// </summary>
        private List<CT_Rom> _OrpheanRoms = new List<CT_Rom>();
        public List<CT_Rom> OrpheanRoms
        {
            get => _OrpheanRoms;
            set
            {
                if (value != _OrpheanRoms)
                {
                    _OrpheanRoms = value;
                    OnPropertyChanged();
                    OnPropertyChanged("FilteredOrpheanRoms");
                }
            }
        }


       // private List<CT_Rom> _FilteredOrpheanRoms = new List<CT_Rom>();
        public List<CT_Rom> FilteredOrpheanRoms
        {
            get
            {
                var filteredOrpheanRoms = new List<CT_Rom>();

                if (string.IsNullOrEmpty(OrpheanRomsFilter) ) 
                {
                    return _OrpheanRoms;
                }

                foreach (CT_Rom rom in _OrpheanRoms)
                {
                    if (rom.Description.Contains(OrpheanRomsFilter))
                    {
                        filteredOrpheanRoms.Add(rom);
                    }
                }
                return filteredOrpheanRoms;
                
            }
         /*   set
            {
                if (value != _FilteredOrpheanRoms)
                {
                    _FilteredOrpheanRoms = value;
                    OnPropertyChanged();
                }
            }*/
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rom"></param>
        private void Add_OrpheanRom(CT_Rom rom)
        {
            var tmpOrphean = new List<CT_Rom>(OrpheanRoms);
            tmpOrphean.Add(rom);
            OrpheanRoms = tmpOrphean;
        }

        
        /// <summary>
        /// Filtre pour les roms orphelines
        /// </summary>
        public string OrpheanRomsFilter { get; set; }


        private List<CT_Rom> _RomsToUpdate=new List<CT_Rom>();

        /// <summary>
        /// Liste des jeux à updater
        /// </summary>
        public List<CT_Rom> RomsToUpdate
        {
            get => _RomsToUpdate;
            set
            {
                if (_RomsToUpdate != value)
                {
                    _RomsToUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Ajouts aux roms à updater en vérifiant qu'elle n'est pas déjà présente
        /// </summary>
        /// <param name=""></param>
        private void Add_RomToUpdate(CT_Rom rom2update)
        {
            bool isPresent = false;
            for (int i = 0; i < RomsToUpdate.Count; i++)
                if (RomsToUpdate[i] == rom2update)
                {
                    isPresent = true;
                    break;
                }


            if (!isPresent)
            {
                RomsToUpdate.Add(rom2update);
                OnPropertyChanged("RomsToUpdate");
            }
        }
        #endregion Roms


        #region Games

        /// <summary>
        /// Liste des jeux à mapper
        /// </summary>
        private List<CT_Game> _GamesMapped = new List<CT_Game>();
        public List<CT_Game> GamesMapped
        {
            get => _GamesMapped;
            set
            {
                _GamesMapped = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Jeu que l'on ajoutera à la liste
        /// </summary>
        private string _NewGame;
        public string NewGame
        {
            get => _NewGame;
            set
            {
                if (GamesMapped.FirstOrDefault(x => x.Game_Name == value) == null)
                {
                    //Games.Add(value);
                    //Add_GameOnDB(value);


                    _NewGame = value;
                    OnPropertyChanged();
                }
            }
        }

        /*
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
        }*/


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


        //
        //private List<Map_RomGame> _Tmp;
        private List<CT_Game> _TmpGames;



        #endregion Games


        /// <summary>
        /// Constructeur
        /// </summary>
        public pMapGames()
        {
            InitializeComponent();

            DataContext = this;
        }



        #region Loading
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

                // Chargement asynchrone des jeux et des roms en relation
                Load_MapGames();


                // Chargement asynchrone des roms orphelines
                Load_OrpheanRoms();


                // Charement asynchrone des jeux
                /*aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();*/
                //OnPropertyChanged("Games");
            }
        }


        /// <summary>
        /// Construit et lance l'asyncloadmapGames
        /// </summary>
        private void Load_MapGames()
        {
            // Chargement asynchrone des Jeux et des roms associées
            AsyncWindowProgress aLoad = new AsyncWindowProgress();
            aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoad_MapGames);
            aLoad.ShowDialog();
            GamesMapped = _TmpGames;
        }

        /// <summary>
        /// Récupère en base les valeurs avec liaison des deux tables
        /// </summary>
        /// <param name="aLoad"></param>
        private void AsyncLoad_MapGames(AsyncWindowProgress aLoad)
        {
            aLoad.AsyncMessage("Loading Games and mappel Roms...");
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                _TmpGames = sqReq.QueryGameWithRoms();


            }
        }

        /// <summary>
        /// Construit et lance l'asyncloadorphanroms
        /// </summary>
        private void Load_OrpheanRoms()
        {
            // Chargement asynchrone des Jeux et des roms associées
            AsyncWindowProgress aLoad = new AsyncWindowProgress();
            // Chargement asynchrone des Roms isolées
            aLoad = new AsyncWindowProgress();
            aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoad_OrpheanRoms);
            aLoad.ShowDialog();
        }

        private void AsyncLoad_OrpheanRoms(AsyncWindowProgress aLoad)
        {
            aLoad.AsyncMessage("Loading Orphean Roms...");

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                var selOrphean = new Obj_Select(table: PProp.Default.T_Roms, new string[] { "ID", "Archive_Name", "Game_Id", "Description" });
                selOrphean.AddConds(new SqlCond("Game_Id", eWhere.Is, "null"));
                List<CT_Rom> tmp = sqReq.GetListOf<CT_Rom>(CT_Rom.Result2Class, selOrphean);
                OrpheanRoms = tmp;
            }
        }

        #endregion


        #region UI

        /// <summary>
        /// Actions quand le curseur de la souris passe sur un element de la listbox des jeux
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //ListBoxItem item = (ListBoxItem)sender;
            //SelectedGame = (CT_Game)item.DataContext;

        }

        private void ListBoxItem_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            SelectedGame = (CT_Game)item.DataContext;
        }
        #endregion


        #region Roms
        /// <summary>
        /// Enlève la rom du jeu depuis l'itemscontrol du jeu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveRom_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            CT_Rom romToRemove = (CT_Rom)bt.DataContext;

            // On ajoute aux roms orphelines
            //OrpheanRoms.Add(romToRemove);
            Add_OrpheanRom(romToRemove);


            // On ajoute la rom à la liste des roms qui sont à updater
            Add_RomToUpdate(romToRemove);

            // 2️⃣ Remonter dans le VisualTree pour trouver le parent correspondant au jeu
            DependencyObject parent = VisualTreeHelper.GetParent(bt);
            while (parent != null && !(parent is ListBoxItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent == null)
            {
                return;
            }

            CT_Game gameParent = (CT_Game)((ListBoxItem)parent).DataContext;


            // On enlève de la liste du jeu sélectionné
            List<CT_Rom> tmp = new List<CT_Rom>();
            // Parcours des roms
            for (int i = 0; i < gameParent.Roms.Count; i++)
            {
                CT_Rom currRom = gameParent.Roms[i];

                // On lève l'association du jeu                
                currRom.Game = null;

                //foreach (var rom in SelectedGame.Roms)

                //if (rom.ID != uint.Parse(bt.Tag.ToString()))                
                if (currRom != romToRemove)
                    tmp.Add(currRom);
                //              else                
                //                    OrpheanRoms.Add(rom);
            }
            gameParent.Roms = tmp;
        }


        /// <summary>
        /// Ajout d'une rom à un jeu (par la combobox des roms orphelines)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRom_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGame == null)
            {
                MessageBox.Show("Select a game before, please.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // Récupération de la liste des roms du jeu
            var tmp = new List<CT_Rom>(SelectedGame.Roms);


            for (int i = 0; i < SelectedRoms.Count(); i++)
            {
                var rom = SelectedRoms[i];

                // Ajout de la rom
                //tmp.Add(SelectedOrpheanRom);
                tmp.Add(rom);

                // On ajoute la rom à la liste des roms qui sont à updater
                //Add_RomToUpdate(SelectedOrpheanRom);
                Add_RomToUpdate(rom);

                // Liaison de la rom au jeu
                //SelectedOrpheanRom.Game = SelectedGame;
                rom.Game = SelectedGame;
            }

            // Transmission pour signaler un changement
            SelectedGame.Roms = tmp;

            var tmp2 = new List<CT_Rom>(OrpheanRoms);
            for (int i = 0; i < SelectedRoms.Count(); i++)
            {
                var rom = SelectedRoms[i];

                // On enlève de la liste des orphelins
                //OrpheanRoms.Remove(SelectedOrpheanRom);
                tmp2.Remove(rom);

            }
            OrpheanRoms = tmp2;

        }


        /// <summary>
        /// Déclenché quand on sélectionne des roms en cliquant dessus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RomsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox grid = (ListBox)sender;

            SelectedRoms = grid.SelectedItems.Cast<CT_Rom>().ToList();
            Debug.WriteLine($"Selected Roms: {SelectedRoms.Count}");
        }


        /// <summary>
        /// Sauvegarde des roms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Seules les roms sont à sauvegarder car la pk est située à ce niveau
        /// </remarks>
        private void SaveRoms_Click(object sender, RoutedEventArgs e)
        {
            using (SQLite_Op sqOp = new SQLite_Op())
            {
                // Update des roms
                sqOp.Update_Roms(RomsToUpdate);


                // Reset des roms à updater
                RomsToUpdate = new List<CT_Rom>();
            }

        }


        #endregion Roms


        #region Games
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Can_AddGame(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = !string.IsNullOrEmpty(NewGame);

        }

        /// <summary>
        /// Ajoute un jeu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_AddGame(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show($"Do you want to add {NewGame} to the database ?", "Add Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Add_GameOnDB(NewGame);

                Load_MapGames();
            }

            /*  // Ajout à la liste de tous les jeux à afficher
              Add_GameToMap(new CT_Game()
              {
                  Game_Name = NewGame
              }
              );*/
        }

        /// <summary>
        /// Ajoute un jeu dans la base de données, sans linker les roms.
        /// </summary>
        /// <param name="value"></param>
        private void Add_GameOnDB(string value)
        {
            using (SQLite_Op sqOP = new SQLite_Op())
            {
                sqOP.Insert_Game(
                    new CT_Game()
                    {
                        Game_Name = value
                    }
                );


            }
        }


        private void Can_RemoveGame(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Enlever un jeu de la base
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_RemoveGame(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Game game = (CT_Game)e.Parameter;


            if (MessageBox.Show($"Do you want to remove {game.Game_Name} from the database ?", "Remove Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SqlCond[] sqlCond = new SqlCond[1];

                using (SQLite_Op sqOp = new SQLite_Op())
                {
                    sqOp.Delete_Game(new SqlCond[] { new SqlCond("ID", eWhere.Equal, game.ID) });

                    // Update de toutes les roms qui sont en lien avec le jeu
                    sqOp.Update_MassiveRoms(new List<SQL_Element>() { new SQL_Element(typeof(string), "Game_Id", null) }, new SqlCond[] { new SqlCond("Game_Id", eWhere.Equal, game.ID) });
                }


                GamesMapped.Remove(game);
                GamesMapped = new List<CT_Game>(GamesMapped);
            }
        }








        #endregion

        #region Obsolete ?


        /*
        private void GameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox grid = (ListBox)sender;
            //SelectedRoms = grid.SelectedItems.Cast<CT_Game>().ToList();
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



        private void Add_GameToMap(CT_Game game)
        {
            var tmpGamesToMapp = new List<CT_Game>(GamesMapped);
            tmpGamesToMapp.Add(game);
            GamesMapped = tmpGamesToMapp;

        }


        */


        /*
        private CT_Rom _SelectedOrpheanRom;
        public CT_Rom SelectedOrpheanRom
        {
            get => _SelectedOrpheanRom;
            set
            {
                if (value != _SelectedOrpheanRom)
                {
                    _SelectedOrpheanRom = value;
                    OnPropertyChanged();
                }
            }
        }
        */
        #endregion


        /// <summary>
        /// Change le contenu de la liste des orpheans lorsque le filtre change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterChanged(object sender, TextChangedEventArgs e)
        {           
            TextBox textBox = (TextBox)sender;
            string filter = textBox.Text;
            
            Debug.WriteLine($"Filterd Changed '{filter}'");

            OrpheanRomsFilter = filter;
            OnPropertyChanged("FilteredOrpheanRoms");

            /*
            var filteredOrpheanRoms = new             List<CT_Rom>();
            foreach (CT_Rom rom in _OrpheanRoms)
            {
                if (rom.Description.Contains(filter))
                {
                    filteredOrpheanRoms.Add(rom);
                }
            }
            _OrpheanRoms= filteredOrpheanRoms;
            */
        }
    }
}
