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
    public partial class pWorkPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ObservableCollection<Aff_Game> _GamesToOrganize = new ObservableCollection<Aff_Game>();
        public ObservableCollection<Aff_Game> GamesToOrganize
        {
            get { return _GamesToOrganize; }
            set
            {
                if (value != _GamesToOrganize)
                {
                    _GamesToOrganize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        


        private ObservableCollection<Aff_Game> _GamesModified = new ObservableCollection<Aff_Game>();
        public ObservableCollection<Aff_Game> GamesModified
        {
            get { return _GamesModified; }
            set
            {
                if (value != _GamesModified)
                {
                    _GamesModified = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();
        /*public ObservableCollection<CT_Constructeur> Constructeurs
        {
            get { return _Constructeurs; }
            set
            {
                if (value != _Constructeurs)
                {
                    _Constructeurs = value;
                    NotifyPropertyChanged();
                }
            }
        }*/

        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();
        /*
        public ObservableCollection<CT_Machine> Machines
        {
            get { return _Machines; }
            set
            {
                if (value != _Machines)
                {
                    _Machines = value;
                    NotifyPropertyChanged();
                }
            }
        }*/

            /*
        delegate void AsyncGame(Aff_Game game);
        private void RemoveGame(Aff_Game game)
        {
            if (!Dispatcher.CheckAccess())
            {
                AsyncGame loglog = new AsyncGame(RemoveGame);
                Dispatcher.Invoke(loglog, new object[] { game });
            }
            else
            {
                GamesToOrganize.Remove(game);

            }

        }*/


        public pWorkPage()
        {

            InitializeComponent();
            
            DataContext = this;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Aff_Game ag in GamesToOrganize)
                GamesModified.Add(new Aff_Game(ag));

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Constructeurs, all: true));
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

        public static readonly RoutedUICommand SimulateCmd = new RoutedUICommand("Simulate", "SimulateCmd", typeof(pWorkPage));

        private void Can_Simulate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesToOrganize.Count > 0;
        }

        private void Ex_Simulate(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Machine machine = (CT_Machine)cbMachines.SelectedItem;            
            IList selectedGames = dg2Organize.SelectedItems;

            List<Aff_Game> toModify = null;
            if (selectedGames.Count == 0)
            {
                toModify = GamesToOrganize.ToList();
            }
            else
            {
                toModify = dg2Organize.SelectedItems.Cast<Aff_Game>().ToList();
            }

            foreach (Aff_Game selected in toModify)
            {
                foreach (Aff_Game destination in GamesModified)
                {
                    if(selected.Equals(destination))
                    {
                        if (machine != null)
                        {
                            destination.Aff_Machine = machine.Nom;
                            destination.Machine = Convert.ToUInt32(machine.ID);
                        }

                    }
                }
            }
           
            
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
            NavigationService.Navigate(new pAccueil());
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


    }
}
