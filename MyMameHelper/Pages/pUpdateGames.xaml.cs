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

        /// <summary>
        /// Liste de référence prise dans la bdd
        /// </summary>
        public MyObservableCollection<Aff_Game> DbGames { get; set; } = new MyObservableCollection<Aff_Game>();

        /// <summary>
        /// Liste des jeux en cours de modification
        /// </summary>
        public MyObservableCollection<Aff_Game> GamesModified { get; set; } = new MyObservableCollection<Aff_Game>();

        /// <summary>
        /// Stockage des jeux pour permetre de revenir sur les modifications
        /// </summary>
        private List<Aff_Game> _TempGame { get; set; } = new List<Aff_Game>();

        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();

        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();

        public MyObservableCollection<CT_Constructeur> Developers { get; set; } = new MyObservableCollection<CT_Constructeur>();

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



        public pUpdateGames()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /* A revoir
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                DbGames.ChangeContent = sqReq.AffGames_List(order: new SqlOrder("Game_Name"));

                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Constructeurs, all: true));
                Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Companies, all: true));
                Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, new Obj_Select(table: PProp.Default.T_Genres, all: true));
            }
            */
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
            e.CanExecute = GamesModified.Count > 0;
        }

        private void UpdateGames(object sender, ExecutedRoutedEventArgs e)
        {
            /* A revoir
            UpdateDbGames<Aff_Game> sDb = new UpdateDbGames<Aff_Game>();
            sDb.Update_GamesTable(GamesModified);

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                DbGames.ChangeContent = sqReq.AffGames_List();
            }
            GamesModified.Clear();
            */
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

            List<Aff_Game> toModify = null;
            if (dg2Organize.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Game(s)", "", MessageBoxButton.OK);
            }

            //toModify =.Cast<Aff_Game>().ToList();

            foreach (Aff_Game game in dg2Organize.SelectedItems)
            {
                if (GamesModified.FirstOrDefault(x => x.Game_Name.Equals(game.Game_Name)) != null)
                    continue;

                GamesModified.AddSilent(new Aff_Game(game));
            }
            // _TempGame.AddRange(toModify);
            // DbGames.RemoveSilentRange(toModify);

            GamesModified.SignalChange();
            dg2Organize.SelectedIndex = -1;
            // DbGames.SignalChange();
        }
        #endregion

        #region Datagrid Droite

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
                DbGames.Add(g);
                GamesModified.Remove(g);
            }
        }

        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (Aff_Game g in GamesModified)
            {
                DbGames.Add(g);
            }

            GamesModified.Clear();
        }




        #endregion

        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            SearchPlus sp = new SearchPlus();
            if (sp.ShowDialog() == true)
            {
                DbGames.ChangeContent = sp.GamesFound.ToList();
            }
        }

        private void CbMachines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region Change
        private void Can_Change(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
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
                Aff_Game game = dgRight.SelectedItems[i] as Aff_Game;
                if (game is null)
                    continue;

                Trace.WriteLine(game.Game_Name);
                if (cbMachines.SelectedItem != null)
                {
                    CT_Machine machine = cbMachines.SelectedItem as CT_Machine;
                    game.Machine = machine.ID;
                    game.Aff_Machine = machine.Nom;
                }

                // Unwanted
                game.Unwanted = cbUnwanted.IsChecked;

                foreach (CT_Rom rom in game.Roms)
                    rom.Unwanted = cbUnwanted.IsChecked;

                // Genre
                if (cboxGenres.SelectedItem != null)
                {
                    CT_Genre genre = cboxGenres.SelectedItem as CT_Genre;
                    game.Genre = genre.ID;
                    game.Aff_Genre = genre.Nom;
                }

                //Developpeurs
                if (cboxDevs.SelectedItem != null)
                {
                    CT_Constructeur dev = cboxDevs.SelectedItem as CT_Constructeur;
                    game.Developer = dev.ID;
                    game.Aff_Developer = dev.Nom;
                }

                if (cboxRate.SelectedItem != null)
                {
                    DictionaryEntry rate = (DictionaryEntry)cboxRate.SelectedItem;
                    game.Rate = Convert.ToUInt32(rate.Key); ;
                }
            }
            cbMachines.SelectedItem = null;

        }
        #endregion

        #region Edit
        private void Can_EditRom(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesModified.Count > 0;
        }

        private void Ex_EditRom(object sender, ExecutedRoutedEventArgs e)
        {
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
            }
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
