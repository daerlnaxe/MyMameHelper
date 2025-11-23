using MyMameHelper.Commands;
using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.Parsers;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Logique d'interaction pour AddGamesByTXT.xaml
    /// </summary>
    public partial class PrepareGames : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool _FilterIsActive;
        public bool FilterIsActive
        {
            get { return _FilterIsActive; }
            set
            {
                if (value != _FilterIsActive)
                {
                    _FilterIsActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _leftKFilter;
        public string LeftKFilter
        {
            get { return _leftKFilter; }
            set
            {
                if (!value.Equals(_leftKFilter))
                {
                    _leftKFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public MyObservableCollection<CT_Game> Games { get; set; } = new MyObservableCollection<CT_Game>();


        private ObservableCollection<CT_Game> DBGames = new ObservableCollection<CT_Game>();

        public MyObservableCollection<Aff_Game> SelectedGames { get; set; } = new MyObservableCollection<Aff_Game>();

        private object _gamesLock = new object();
        private object _selectLock = new object();

        private CT_Game _S4L;
        public CT_Game Selected4List
        {
            get { return _S4L; }
            set
            {
                if (value != _S4L)
                {
                    _S4L = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private string _LastGamesOrder;
        private string _LastSelectedOrder;
        private string GamesModes;
        private string SelectedModes;

        public PrepareGames()
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(Games, _gamesLock);
            BindingOperations.EnableCollectionSynchronization(SelectedGames, _selectLock);
            DataContext = this;
        }

        /*
         * 
         */
        #region lvGames
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.A && e.Key <= Key.Z)
                || (e.Key >= Key.D0 && e.Key <= Key.D9)
                || (e.Key >= Key.NumPad0 && e.Key<= Key.NumPad9)
                || e.Key == Key.Back
                || e.Key == Key.Space
                || e.Key == Key.Delete
                || e.Key == Key.Decimal
                || e.Key == Key.OemPeriod 
                || e.Key == Key.Subtract
                || e.Key == Key.Add
                ))
                return;

            char k = Methods.Keyboard.GetCharFromKey(e.Key);
            //
            if (LeftKFilter is null)
                LeftKFilter = string.Empty;

            //
            if (e.Key == Key.Back)
                LeftKFilter = LeftKFilter.Length > 0 ? LeftKFilter.Substring(0, LeftKFilter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                LeftKFilter = string.Empty;

            else
                LeftKFilter += k;


            Select_LeftGame();
        }

        private void Select_LeftGame()
        {
            throw new Exception("A revoir");
            /*
            if (GamesModes == "Mode Game")
                Selected4List = Games.FirstOrDefault(x => x.Game_Name.StartsWith(LeftKFilter, StringComparison.OrdinalIgnoreCase));
            else if (GamesModes == "Mode Archive")
                Selected4List = Games.FirstOrDefault(x => x.Parent_Name.StartsWith(LeftKFilter, StringComparison.OrdinalIgnoreCase));

            if (Selected4List != null)
            {
                lvGames.ScrollIntoView(lvGames.SelectedItem);
            }*/
        }

        private void RBGames_Checked(object sender, RoutedEventArgs e)
        {
            throw new Exception("A revoir");
            /*
            GamesModes = ((RadioButton)sender).Content.ToString();
            if(LeftKFilter != null)
                Select_LeftGame();*/
        }


        #endregion


        /*
         *
         */
        #region Lv selected 

        /*
        private void lvSelectedColumnHeader_Click(object sender, RoutedEventArgs e)
        {
        var colClicked = e.OriginalSource as GridViewColumnHeader;

        if (colClicked.Content.ToString() == "Game Name")
        {
            if (_LastSelectedOrder == "GN_Asc")
            {
                SelectedGames = new ObservableCollection<CT_Game>(SelectedGames.OrderByDescending(x => x.Game_Name));
                _LastSelectedOrder = "GN_Desc";
            }
            else
            {
                SelectedGames = new ObservableCollection<CT_Game>(SelectedGames.OrderBy(x => x.Game_Name));
                _LastSelectedOrder = "GN_Asc";
            }
        }

        if (colClicked.Content.ToString() == "Archive Name")
        {
            if (_LastSelectedOrder == "AN_Asc")
            {
                SelectedGames = new ObservableCollection<CT_Game>(SelectedGames.OrderByDescending(x => x.Archive_Name));
                _LastSelectedOrder = "AN_Desc";
            }
            else
            {
                SelectedGames = new ObservableCollection<CT_Game>(SelectedGames.OrderBy(x => x.Archive_Name));
                _LastSelectedOrder = "AN_Asc";

            }

        }
        }
        */
        private void RBSelectedGames_Checked(object sender, RoutedEventArgs e)
        {
            SelectedModes = ((RadioButton)sender).Content.ToString();
        }



        #endregion

        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            List<CT_Game> diff = new List<CT_Game>();

            foreach (CT_Game g in Games)
            {
                CT_Game found = null;
                foreach (CT_Game y in DBGames)
                {
                    if (g.Equals(y))
                    {
                        found = y;
                        break;
                    }
                }

                if (found == null)
                    diff.Add(g);
                else
                {
                    DBGames.Remove(found);
                }
            }

        }*/

        // search




        #region Execute


        /*
        private void Ex_Search(object sender, CanExecuteRoutedEventArgs e)
        {
            Search searchBox = new Search();
            searchBox.Games = Games;
            searchBox.ShowDialog();
            if (searchBox.DialogResult == true)
            {
                foreach (CT_Game g in searchBox.GamesFound)
                {
                    SelectedGames.Add(g);
                    Games.Remove(g);
                }
            }
        }*/

        #endregion

        #region Panneau de gauche
        //  public static readonly RoutedUICommand ResetLeftCmd = new RoutedUICommand("Reset", "ResetLeftCmd", typeof(UICommands));

        private void Can_ResetLeft(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Games.Count > 0;
        }

        private void Ex_ResetLeft(object sender, ExecutedRoutedEventArgs e)
        {
            Games.Clear();
        }

        // public static readonly RoutedUICommand AddGameCmd = new RoutedUICommand("Add Game", "AddGameCmd", typeof(UICommands));
        private void Can_AddGame(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = FilterIsActive & Games.Count > 0;
            e.CanExecute = FilterIsActive & Games.Count > 0;
        }

        // Le fait d'ajouter de nombreux jeux d'une liste à l'autre est gourmand en ressources
        private void Ex_AddGame(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            List<CT_Game> tempoGames = Games.ToList();
            for (int i = 0; i < lvGames.SelectedItems.Count; i++)
            {
                CT_Game g = (CT_Game)lvGames.SelectedItems[i];
                SelectedGames.Add(g);
                tempoGames.Remove(g);
            }
            Games.Clear();
            Games.AddRange(tempoGames);

            return;
            */

            AsyncWindowProgress pw = new AsyncWindowProgress();
            pw.go += new AsyncWindowProgress.AsyncAction(AsyncSwapL2R);
            pw.ShowDialog();
            return;

            throw new Exception("A revoir");

            // SelectedGames.Sort(x => x.Parent_Name, ListSortDirection.Ascending);
        }

        /// <summary>
        /// Passe des éléments d'une observable collection à une autre en task séparées + progress window
        /// </summary>
        private void AsyncSwapL2R(AsyncWindowProgress window)
        {
            IList items = null;
            List<CT_Game> games = Games.ToList();

            Dispatcher.Invoke((Action)delegate ()
            {
                items = lvGames.SelectedItems;
            }
            );

            if (items.Count == 0)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                throw new Exception("A revoir");
                /*
                CT_Game g = (CT_Game)items[i];

                window.AsyncUpProgressPercent(i * 100 / items.Count);
                SelectedGames.Add(new Aff_Game(g));
                games.Remove(g);*/
            }

            Games.Clear();
            Games.AddSilentRange(games);
        }

        #endregion

        #region Panneau de droite
        // public static readonly RoutedUICommand RemoveRightCmd = new RoutedUICommand("Reset", "RemoveRightCmd", typeof(UICommands));

        private void Can_RemoveRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedGames.Count > 0;
        }

        private void Ex_Remove(object sender, ExecutedRoutedEventArgs e)
        {
            /* IList items = lvSelected.SelectedItems;
             List<CT_Game> toDel = new List<CT_Game>();
             for (int i = 0; i < items.Count; i++)
             {
                 CT_Game g = (CT_Game)items[i];
                 toDel.Add(g);
             }

             foreach (CT_Game g in toDel)
             {
                 Games.Add(g);
                 SelectedGames.Remove(g);
             }
             */


            AsyncWindowProgress pw = new AsyncWindowProgress();
            pw.go += new AsyncWindowProgress.AsyncAction(AsyncSwapR2L);
            pw.ShowDialog();
            return;
            /*
            Stopwatch sw = new Stopwatch();
            sw.Start();

            lvGames.sor
            wait w1 = new wait();
            w1.Show();
            Games.Sort(x => x.Archive_Name, ListSortDirection.Ascending);
            Debug.WriteLine(sw.ElapsedMilliseconds);
            w1.Close();*/

        }

        private void AsyncSwapR2L(AsyncWindowProgress window)
        {
            IList items = null;
            List<Aff_Game> sel = SelectedGames.ToList();

            Dispatcher.Invoke((Action)delegate ()
            {
                items = lvSelected.SelectedItems;
            }
            );

            if (items.Count == 0)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                Aff_Game g = (Aff_Game)items[i];

                window.AsyncUpProgressPercent(i * 100 / items.Count);
                Games.Add(g);
                sel.Remove(g);
            }

            SelectedGames.Clear();
            SelectedGames.AddSilentRange(sel);
        }


        //public static readonly RoutedUICommand ResetRightCmd = new RoutedUICommand("Reset", "ResetRightCmd", typeof(UICommands));
        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedGames.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (CT_Game g in SelectedGames)
            {
                Games.Add(g);
            }

            SelectedGames.Clear();
        }
        #endregion


        #region Db

        private void Ex_LoadDb(object sender, ExecutedRoutedEventArgs e)
        {
            if (Games.Count > 0 && MessageBox.Show("The collection is not empty, load db ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Obj_Select objS = new Obj_Select(table: PProp.Default.T_TempRoms, all: true);
                Games.ChangeContent = sqReq.GetListOf<CT_Game>(CT_Game.Result2Class, objS);
            }

            if (Games.Count == 0)
            {
                MessageBox.Show("Database 'Vrac' is empty");
                return;
            }

            if (WorkWithDb.FilterGames(Games))
            {
                FilterIsActive = true;
                return;
            }

            FilterIsActive = false;
        }

        private void Can_SaveVrac(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = FilterIsActive && Games.Count > 0;
        }

        private void Ex_SaveVrac(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            SaveDb<CT_Game> sDb = new SaveDb<CT_Game>();
            sDb.VracTable(Games);
            */
        }
        #endregion


        #region Work
        private void Can_Work(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedGames.Count > 0;
        }


        /// <summary>
        /// Envoie à la page de données des traitement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_Work(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            pWorkPage pWork = new pWorkPage();
            pWork.GamesToOrganize = SelectedGames;
            NavigationService.Navigate(pWork);
            */
        }
        #endregion

        #region Search
        private void Can_Search(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = FilterIsActive & Games.Count > 0;
        }

        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            throw new Exception("A revoir");

            /*
            SearchMethod search = new SearchMethod();
            ObservableCollection<CT_Game> feed = search.CTGames(Games);

            SelectedGames.AddRange(feed.Select(x => new Aff_Game(x)));
            Games.RemoveRange(feed);*/
        }
        #endregion


        /// <summary>
        /// Chargement des jeux via un fichier txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_LoadFile(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (Directory.Exists(Properties.Settings.Default.MameFolder))
                openFileDialog.InitialDirectory = String.IsNullOrEmpty(Properties.Settings.Default.MameFolder) ? string.Empty : Properties.Settings.Default.MameFolder;

            if (openFileDialog.ShowDialog() != true)
                return;

            Games.ChangeContent = MameExportParser.Try_TxtParse(openFileDialog.FileName);

            if (WorkWithDb.FilterGames(Games))
            {
                FilterIsActive = true;
                return;
            }

            FilterIsActive = false;
        }
    }
}
