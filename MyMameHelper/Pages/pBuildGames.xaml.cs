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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pWorkPage.xaml
    /// </summary>
    public partial class pBuildGames : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string couille { get; set; } = "mes couilles";

        public MyObservableCollection<Aff_Game> GamesToOrganize { get; set; } = new MyObservableCollection<Aff_Game>();

        public MyObservableCollection<Aff_Game> GamesModified { get; set; } = new MyObservableCollection<Aff_Game>();

        List<CT_Game> dbGames = new List<CT_Game>();


        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();

        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();
        public MyObservableCollection<CT_Genre> Genres { get; set; } = new MyObservableCollection<CT_Genre>();

        private Aff_Game _LeftSelected;
        public Aff_Game LeftSel
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

        public pBuildGames()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Init();

        }

        private void Init()
        {
            bool ignored = false;

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true, orders: new SqlOrder("Nom")));
                Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, new Obj_Select(table: PProp.Default.T_Genres, all: true, orders: new SqlOrder("Nom")));
                dbGames = sqReq.Get_ListOf_Games(new Obj_Select(all: true));
            }

            for (int i = 0; i < GamesToOrganize.Count; i++)
            {
                Aff_Game game = GamesToOrganize[i];

                if (dbGames.FirstOrDefault(x => x.Game_Name.Equals(game.Game_Name)) != null)
                {
                    ignored = true;
                    GamesToOrganize.Remove(game);
                    i--;
                }
            }

            if (ignored)
                MessageBox.Show("Some games removed from list to save because they are already present in database");

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

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                Machines.ChangeContent = sqReq.GetListOf(
                   CT_Machine.Result2Class,
                   new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" }, conditions: new SqlCond[] { new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()) }, orders: new SqlOrder("Nom"))
                   );
            }
        }

        #region Mise en place
        /*
        public void SetGamesToOrganize(ObservableCollection<Aff_Game> collec)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (CT_Game g in collec)
                GamesToOrganize.Add(new Aff_Game(g));

            Debug.WriteLine(sw.ElapsedMilliseconds);
        }*/
        #endregion


        #region Simulation
        raoul wd1;



        private void Can_Simulate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToOrganize.Count > 0;
        }

        private void Ex_Simulate(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Machine machine = (CT_Machine)cbMachines.SelectedItem;
            CT_Genre genre = (CT_Genre)cboxGenres.SelectedItem;

            //List<Aff_Game> toModify = null;
            if (dgRight.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Game(s)");
            }

            foreach (Aff_Game selected in dgRight.SelectedItems)
            {
                foreach (Aff_Game destination in GamesModified)
                {
                    if (selected.Equals(destination))
                    {
                        if (machine != null)
                        {
                            destination.Aff_Machine = machine.Nom;
                            destination.Machine = Convert.ToUInt32(machine.ID);
                        }

                        destination.Unwanted = cbUnwanted.IsChecked;

                        if (genre != null)
                        {
                            destination.Aff_Genre = genre.Nom;
                            destination.Genre = genre.ID;
                        }

                        if (cboxRate.SelectedItem != null)
                        {
                            string rate = ((DictionaryEntry)cboxRate.SelectedItem).Key.ToString();
                            destination.Rate = Convert.ToUInt32(rate);
                        }

                    }
                }
            }

            cbMachines.SelectedItem = null;
            cboxGenres.SelectedItem = null;
            cboxRate.SelectedItem = null;
            cbUnwanted.IsChecked = false;

            /*
            if (selectedGames.Count == 0)
            {
                MessageBox.Show("No item Selected");
                return;
            }



            wd1 = new raoul();

            var copy = new ObservableCollection<Aff_Game>(GamesToOrganize);

            Task t = Task.Run(() => AddGamesToSave(machine, selectedGames, GamesModified));
            t.ContinueWith((antecedant) => GamesToOrganize = Asynctest2(copy, selectedGames));

            wd1.ShowDialog();*/
        }

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
        }

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
        }

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


        #region Sauvegarder dans la table Games
        private void SaveGames(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDb<Aff_Game> sDb = new SaveDb<Aff_Game>();
            sDb.GamesTable(GamesModified);
            GamesModified.Clear();
            Init();
        }

        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }
        #endregion

        #region Datagrid Gauche
        //  public static readonly RoutedUICommand RemoveLeftCmd = new RoutedUICommand("Remove Game", "RemoveLeftCmd", typeof(pWorkPage));

        #endregion

        #region Datagrid Droite
        //  public static readonly RoutedUICommand RemoveRightCmd = new RoutedUICommand("Remove Game", "RemoveRightCmd", typeof(pWorkPage));
        private void Can_RemoveRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }
        private void Ex_RemoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            IList items = dgRight.SelectedItems;
            List<Aff_Game> toDel = new List<Aff_Game>();
            for (int i = 0; i < items.Count; i++)
            {
                Aff_Game g = (Aff_Game)items[i];
                toDel.Add(g);
            }

            foreach (Aff_Game g in toDel)
            {
                GamesToOrganize.Add(g);
                GamesModified.Remove(g);
            }
        }

        //   public static readonly RoutedUICommand ResetRightCmd = new RoutedUICommand("Reset", "ResetRightCmd", typeof(pWorkPage));
        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (Aff_Game g in GamesModified)
            {
                GamesToOrganize.Add(g);
            }

            GamesModified.Clear();
        }


        #endregion

        private void AddToRight(object sender, ExecutedRoutedEventArgs e)
        {
            bool ignored = false;

            if (dg2Organize.SelectedItems.Count == 0)
                MessageBox.Show("Select Game(s)");

            foreach (Aff_Game game2add in dg2Organize.SelectedItems)
            {
                if (GamesModified.FirstOrDefault(x => x.Game_Name.Equals(game2add.Game_Name)) != null)
                    continue;
                /*
                if (dbGames.FirstOrDefault(x => x.Game_Name.Equals(game2add.Game_Name)) != null)
                {
                    ignored = true;
                    continue;
                }*/

                GamesModified.Add(game2add);
            }

            if (ignored)
                MessageBox.Show("Some games removed from list to save because they are already present in database");

        }

        private void Can_Add(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToOrganize.Count > 0;
        }

        private void Can_RemoveMachine(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dg2Organize.SelectedItem != null;

        }

        private void Ex_RemoveMachine(object sender, ExecutedRoutedEventArgs e)
        {
            Aff_Game game = dg2Organize.SelectedItem as Aff_Game;
            game.Machine = null;
            game.Aff_Machine = null;
        }

        #region remove game
        private void Can_RemoveGame(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_RemoveGame(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<Aff_Game> selected = dgRight.SelectedItems.Cast<Aff_Game>();

            GamesModified.RemoveSilentRange(selected);
            GamesModified.SignalChange();
        }
        #endregion

        private void Ex_Refresh(object sender, ExecutedRoutedEventArgs e)
        {
            Init();
        }

        #region
        private void Can_LaunchGame(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_LaunchGame(object sender, ExecutedRoutedEventArgs e)
        {
            string game = ((CT_Game)dgRight.SelectedItem).Game_Name;

            if (Directory.Exists(PProp.Default.MameFolder))
            {

                //dgRight.ContextMenu.Visibility = Visibility.Collapsed;
                Task.Run(() => MameLauncher(game));


             //   Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                MessageBox.Show($"Problem with Mame Path: '{PProp.Default.MameFolder}'");
            }
        }

        private void MameLauncher(string game = null)
        {
            try
            {
            Directory.SetCurrentDirectory(PProp.Default.MameFolder);

            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "mame64.exe";
            if (game != null)
                ExternalProcess.StartInfo.Arguments = game;
            Trace.WriteLine($"Launching: {game}");
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ExternalProcess.Start();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            ExternalProcess.WaitForExit();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        #endregion

        private void Ex_LaunchMame(object sender, ExecutedRoutedEventArgs e)
        {            
            if (Directory.Exists(PProp.Default.MameFolder))
            {

                //dgRight.ContextMenu.Visibility = Visibility.Collapsed;
                Task.Run(() => MameLauncher());
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                MessageBox.Show($"Problem with Mame Path: '{PProp.Default.MameFolder}'");
            }
        }
    }
}
