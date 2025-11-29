using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Logique d'interaction pour pWorkPage.xaml
    /// </summary>
    public partial class pUpdateGames : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly RoutedCommand Select_AllCmd = new RoutedCommand("Select All", typeof(pUpdateGames));


        #region Game
        /// <summary>
        /// Liste de référence prise dans la bdd
        /// </summary>
        public MyObservableCollection<CT_Game_Mapped> DbGames { get; set; } = new MyObservableCollection<CT_Game_Mapped>();

        /// <summary>
        /// Liste des jeux en cours de modification
        /// </summary>
        public MyObservableCollection<CT_Game_Mapped> GamesToModify { get; set; } = new MyObservableCollection<CT_Game_Mapped>();

        /// <summary>
        /// Liste des jeux à updater
        /// </summary>
        public List<CT_Game_Mapped> GamesToUpdate { get; set; } = new List<CT_Game_Mapped>();
        #endregion Game


        /// <summary>
        /// Stockage des jeux pour permetre de revenir sur les modifications
        /// </summary>
        //private List<Aff_Game> _TempGame { get; set; } = new List<Aff_Game>();

        //  public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();



        #region Machine
        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();
        private uint _SelectedMachineID;
        public uint SelectedMachineID
        {
            get => _SelectedMachineID;
            set
            {
                if (value != _SelectedMachineID)
                {
                    _SelectedMachineID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion Machine



        /// <summary>
        /// Unwanted
        /// </summary>
        public bool? _CheckedUnwanted = false;
        public bool? CheckedUnwanted
        {
            get => _CheckedUnwanted;
            set
            {
                if (_CheckedUnwanted != value)
                {
                    _CheckedUnwanted = value;
                    NotifyPropertyChanged();
                }
            }
        }


        #region Developer
        /* Désactivé pour le moment
        public MyObservableCollection<CT_Developer> Developers { get; set; } = new MyObservableCollection<CT_Developer>();
        private uint _DeveloperID;
        public uint DeveloperID
        {
            get => _DeveloperID;
            set
            {
                if (value != _DeveloperID)
                {
                    _DeveloperID = value;
                    NotifyPropertyChanged();
                }
            }
        }*/
        #endregion Developer


        #region Genre
        public MyObservableCollection<CT_Genre> Genres { get; set; } = new MyObservableCollection<CT_Genre>();

        private uint _SelectedGenreID;
        public uint SelectedGenreID
        {
            get => _SelectedGenreID;
            set
            {
                if (value != _SelectedGenreID)
                {
                    _SelectedGenreID = value;
                    NotifyPropertyChanged();
                }
            }
        }


        #endregion Genre





        #region Mechanicals
        /// <summary>
        /// Mahjong
        /// </summary>
        public bool? _CheckedMahjong = false;
        public bool? CheckedMahjong
        {
            get => _CheckedMahjong;
            set
            {
                if (_CheckedMahjong != value)
                {
                    _CheckedMahjong = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Quizz
        /// </summary>
        public bool? _CheckedQuizz = false;
        public bool? CheckedQuizz
        {
            get => _CheckedQuizz;
            set
            {
                if (_CheckedQuizz != value)
                {
                    _CheckedQuizz = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Fruit
        /// </summary>
        public bool? _CheckedFruit = false;
        public bool? CheckedFruit
        {
            get => _CheckedFruit;
            set
            {
                if (_CheckedFruit != value)
                {
                    _CheckedFruit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion


        // private Aff_Game _LeftSelected;
        /*public Aff_Game LeftSel
        {
            get { return _LeftSelected; }
            set
            {
                if (value != _LeftSelected)
                {
                    _LeftSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Aff_Game _RightSelected;
        public Aff_Game RightSel
        {
            get { return _RightSelected; }
            set
            {
                if (value != _RightSelected)
                {
                    _RightSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }
        */


        public pUpdateGames()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            using (SQLite_OP sqReq = new SQLite_OP())
            {
                DbGames.ChangeContent = sqReq.QueryGame4Update(order: new SqlOrder("Game_Name"));

                // Liste des constructeurs ?? 
                //Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Constructeurs, all: true));

                // Liste des développeurs désactivé pour le moment
                //Developers.ChangeContent = sqReq.GetListOf<CT_Developer>(CT_Developer.Result2Class, new Obj_Select(table: PProp.Default.T_Developers, all: true));
                // Liste des Machines
                Machines.ChangeContent = sqReq.GetListOf<CT_Machine>(CT_Machine.Result2Class, new Obj_Select(table: PProp.Default.T_Machines, all: true));
                // Liste des genres
                Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, new Obj_Select(table: PProp.Default.T_Genres, all: true));
            }

        }

        private void Select_All(object sender, ExecutedRoutedEventArgs e)
        {
            dg2Organize.SelectAll();
        }

        private void AllwaysTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CbConstructeur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int idConstruct = Convert.ToInt32(cb.SelectedValue);

            using (SQLite_OP sqReq = new SQLite_OP())
            {
                var osel = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                osel.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                osel.AddOrders(new SqlOrder("Nom"));

                Machines.ChangeContent = sqReq.GetListOf(CT_Machine.Result2Class, osel);
            }
        }

        #region Simulation
        raoul wd1;

        public static readonly RoutedUICommand AddCmd = new RoutedUICommand("Simulate", "AddCmd", typeof(pUpdateGames));

        private void Can_Simulate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DbGames.Count > 0;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="selectedGames"></param>
        /// <param name="gToAdd"></param>
        private void AddGamesToSave(CT_Machine machine, IList selectedGames, ObservableCollection<Aff_Game> gToAdd)
        {
            int old_percent = 0;
            for (int i = 0; i < selectedGames.Count; i++)
            {
                CT_Game game = (CT_Game)selectedGames[i];
                Aff_Game affG = new Aff_Game(game);

                if (machine != null)
                {
                    affG.Aff_Machine = machine.Nom;
                    affG.Machine = Convert.ToUInt32(machine.ID);
                }

                Dispatcher.BeginInvoke((Action)delegate () { gToAdd.Add(affG); });

                //    GamesToOrganize.Remove(game);
                int percent = i * 50 / selectedGames.Count;
                if (old_percent != percent)
                {
                    old_percent = percent;
                    wd1.Progress_Value = percent;
                }
            }
        }*/

        /*
    private ObservableCollection<Aff_Game> Asynctest2(ObservableCollection<Aff_Game> copyGames, IList selectedGames)
    {
        int old_percent = 0;
        for (int i = 0; i < selectedGames.Count; i++)
        {

            Aff_Game game = (Aff_Game)selectedGames[i];

            copyGames.Remove(game);

            int percent = 50 + (i * 50) / selectedGames.Count;
            if (old_percent != percent)
            {
                old_percent = percent;
                wd1.Progress_Value = percent;
            }

        }
        wd1.CloseByAsync();

        return copyGames;
    }*/

        #endregion

        /*
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            wd1.Progress_Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GamesSelected = (ObservableCollection<Aff_Game>)e.Result;
            if (e.Cancelled == true)
            {
                Debug.WriteLine("Canceled !");
            }
            else if (e.Error != null)
            {
                Debug.WriteLine("Error: " + e.Error.Message);
            }
            else
            {
                Debug.WriteLine("Done !");
            }
            wd1.Close();
        }
        */


        #region Updater dans la table Games
        private void Can_Update(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToUpdate.Count > 0;
        }

        private void UpdateGames(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateDbGames<CT_Game_Mapped> sDb = new UpdateDbGames<CT_Game_Mapped>();
            sDb.Update_GamesTable(GamesToUpdate);

            using (SQLite_OP sqReq = new SQLite_OP())
            {
                DbGames.ChangeContent = sqReq.QueryGame4Update(order: new SqlOrder("Game_Name"));
            }
            //GamesToModify.Clear();
            GamesToModify.RemoveSilentRange(GamesToUpdate);

            for (int i = 0; i < GamesToModify.Count; i++)
            {
                for (int j = 0; j < DbGames.Count; j++)
                {
                    if (GamesToModify[i].ID == DbGames[j].ID)
                    {                        
                        DbGames.RemoveAt(j);
                        break;
                    }
                }
            }

            GamesToUpdate.Clear();

        }
        #endregion

        #region Datagrid Gauche
        private void Can_AddGame(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = FilterIsActive & Games.Count > 0;
            e.CanExecute = DbGames.Count > 0;
        }


        private void Ex_AddGame(object sender, ExecutedRoutedEventArgs e)
        {
            //CT_Machine machine = (CT_Machine)cbMachines.SelectedItem;

            List<CT_Game_Mapped> toModify = null;
            if (dg2Organize.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Game(s)", "", MessageBoxButton.OK);
            }

            //toModify =.Cast<Aff_Game>().ToList();
            List<CT_Game_Mapped> tmp = new List<CT_Game_Mapped>();
            for (int i = 0; i < dg2Organize.SelectedItems.Count; i++)
            {
                tmp.Add((CT_Game_Mapped)dg2Organize.SelectedItems[i]);
            }



            foreach (CT_Game_Mapped game in tmp)
            {
                /*if (GamesModified.FirstOrDefault(x => x.Game_Name.Equals(game.Game_Name)) != null)
                    continue;*/

                GamesToModify.AddSilent(new CT_Game_Mapped(game));
                DbGames.Remove(game);
            }
            // _TempGame.AddRange(toModify);
            // DbGames.RemoveSilentRange(toModify);

            GamesToModify.SignalChange();
            //DbGames.SignalChange();


            dg2Organize.SelectedIndex = -1;
            // DbGames.SignalChange();
        }
        #endregion

        #region Datagrid Droite

        private void Can_RemoveRight(object sender, CanExecuteRoutedEventArgs e)
        {/*
            e.CanExecute = GamesModified.Count > 0;*/
        }

        private void Ex_RemoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            IList items = dgRight.SelectedItems;
            List<Aff_Game> toDel = new List<Aff_Game>();
            for (int i = 0; i < items.Count; i++)
            {
                Aff_Game g = (Aff_Game)items[i];
                toDel.Add(g);
            }

            foreach (Aff_Game g in toDel)
            {
            /*    DbGames.Add(g);
                GamesModified.Remove(g);*/
            /*}*/
        }

        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToModify.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var g in GamesToModify)
            {
                DbGames.Add(g);
            }

            GamesToModify.Clear();
        }




        #endregion

        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            SearchPlus sp = new SearchPlus();
            if (sp.ShowDialog() == true)
            {
                //s  DbGames.ChangeContent = sp.GamesFound.ToList();
            }
        }

        private void CbMachines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region Change
        private void Can_Change(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToModify.Count > 0;
        }

        private void Ex_Change(object sender, ExecutedRoutedEventArgs e)
        {

            if (dgRight.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Game(s)");
                return;
            }

            for (int i = 0; i < dgRight.SelectedItems.Count; i++)
            {
                CT_Game_Mapped game = dgRight.SelectedItems[i] as CT_Game_Mapped;
                if (game is null)
                    continue;

                Trace.WriteLine(game.Game_Name);

                // Machines
                if (SelectedMachineID != 0)
                {
                    game.Machine_Id = SelectedMachineID;
                    game.Machine = Machines.FirstOrDefault(x => x.ID == SelectedMachineID);
                }

                // Unwanted
                game.Unwanted = CheckedUnwanted;

                /*foreach (CT_Rom rom in game.Roms)
                    rom.Unwanted = cbUnwanted.IsChecked;*/

                // Genre
                if (SelectedGenreID != 0)
                {
                    game.Genre_Id = SelectedGenreID;
                    game.Genre = Genres.FirstOrDefault(x => x.ID == SelectedGenreID);
                }

                /* Désactivé pour le moment
                //Developpeur
                if (ConstructorID != 0)
                {
                    game.Constructeur_ID = ConstructorID;
                    game.Constructeur = Constructors.FirstOrDefault(x => x.ID == ConstructorID);
                }*/


                if (cboxRate.SelectedItem != null)
                {
                    DictionaryEntry rate = (DictionaryEntry)cboxRate.SelectedItem;
                    game.Rate = Convert.ToUInt32(rate.Key); ;
                }

                // Mechanicals
                game.IsMahjong = CheckedMahjong;
                game.IsQuizz = CheckedQuizz;
                game.IsFruit = CheckedFruit;

                // Add to games requiring an update
                GamesToUpdate.Add(game);
            }

            //cbMachines.SelectedItem = null;
            // Raz
            SelectedMachineID = 0;
            CheckedUnwanted = false;
            SelectedGenreID = 0;
            // ConstructorID = 0; Désactivé pour le moment
            cboxRate.SelectedIndex = -1;
            CheckedMahjong = false;
            CheckedQuizz = false;
            CheckedFruit = false;
        }
        #endregion


        #region Edit
        private void Can_EditRom(object sender, CanExecuteRoutedEventArgs e)
        {
            // e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_EditRom(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            Aff_Game game = (Aff_Game)dgRight.SelectedItem;
            wGame window = new wGame();
            window.Game = new Aff_Game(game);
            window.SelIndexMachine = game.Machine;
            window.SelIndexGenre = game.Genre;

            if (window.ShowDialog() == true)
            {
                game = window.Game;
                /*
                UpdateDb<Aff_Game> sDb = new UpdateDb<Aff_Game>();
                sDb.Update_GamesTable(GamesModified);

                using (SQLite_Req sqReq = new SQLite_Req())
                {
                    DbGames.ChangeContent = sqReq.AffGames_List();
                }*/
            /*}*/
        }
        #endregion

        #region Delete
        private void Can_Delete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DbGames.Count > 0;
        }

        private void DeleteGames(object sender, ExecutedRoutedEventArgs e)
        {
            /* A revoir
            if (MessageBox.Show("Delete this games from database ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                List<Aff_Game> selected = dg2Organize.SelectedItems.Cast<Aff_Game>().ToList();

                using (SQLite_Req sqReq = new SQLite_Req())
                {
                    foreach (var game in selected)
                    {
                        SqlCond[] conditions = new SqlCond[] { new SqlCond(colonne: "ID", eWhere.Equal, game.ID) };
                        sqReq.Delete_Game(conditions);
                    }

                    DbGames.ChangeContent = sqReq.AffGames_List();
                }

            }
            */
        }



        #endregion


    }
}
