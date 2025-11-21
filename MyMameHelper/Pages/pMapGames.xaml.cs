using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
                    _OrpheanRoms= value;
                    OnPropertyChanged();
                }
            }
        }


        private void Add_OrpheanRom(CT_Rom rom)
        {
            var tmpOrphean = new List<CT_Rom>(OrpheanRoms);
            tmpOrphean.Add(rom);
            OrpheanRoms = tmpOrphean;
        }


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




        /// <summary>
        /// Liste des jeux à updater
        /// </summary>
        public List<CT_Rom> RomsToUpdate { get; set; } = new List<CT_Rom>();



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
        #endregion


        #region Games
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

        #endregion






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
                GamesMapped = _Tmp;


                // Charement asynchrone des jeux
                /*aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();*/
                //OnPropertyChanged("Games");
            }
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

            var tmp2 =new List<CT_Rom>(OrpheanRoms);            
            for (int i = 0; i < SelectedRoms.Count(); i++)
            {
                var rom = SelectedRoms[i];

                // On enlève de la liste des orphelins
                //OrpheanRoms.Remove(SelectedOrpheanRom);
                tmp2.Remove(rom);

            }
            OrpheanRoms = tmp2;
            
        }

        #endregion Roms


        /// <summary>
        /// Sauvegarde des roms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Seules les roms sont à sauvegarder car la pk est située à ce niveau
        /// </remarks>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (SQLite_Op sqOp = new SQLite_Op())
            {
                // Update des roms
                sqOp.Update_Roms(RomsToUpdate);


                // Update des jeux



            }

        }


        #region Obsolete ?
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

                var aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadGames);
                aLoad.ShowDialog();

            }
        }

        private void GameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox grid = (ListBox)sender;
            //SelectedRoms = grid.SelectedItems.Cast<CT_Game>().ToList();
        }

        private void RomsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox grid = (ListBox)sender;

            SelectedRoms = grid.SelectedItems.Cast<CT_Rom>().ToList();
            Debug.WriteLine($"Selected Roms: {SelectedRoms.Count}");
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



        #endregion


    }
}
