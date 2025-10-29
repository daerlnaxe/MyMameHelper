
using Microsoft.Win32;
using MyMameHelper.ContTable;
using MyMameHelper.Pages;
using MyMameHelper.Properties;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PProp = MyMameHelper.Properties.Settings;

using System.Reflection;

namespace MyMameHelper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static int NumberOf_TempRoms { get; set; }
        internal static int NumberOf_Dev { get; set; }
        internal static int NumberOf_Manus { get; set; }

        internal static int NumberOf_Roms { get; set; }

        private Page _Active_Page;
        public Page Active_Page
        {
            get { return _Active_Page; }
            set
            {
                /*if (value != _Active_Page)
                {*/
                    _Active_Page = value;
                    NotifyPropertyChanged();
                //}
            }
        }

        private bool DatabaseOk;

        private ObservableCollection<Aff_Game> _GamesCollection = new ObservableCollection<Aff_Game>();
        public ObservableCollection<Aff_Game> GamesCollection
        {
            get { return _GamesCollection; }
            set
            {
                if (value != _GamesCollection)
                {
                    _GamesCollection = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private Dictionary<string, Page> Pages = new Dictionary<string, Page>();

        /* public Dictionary<string, string> Games
         {
             get { return _Games; }
             set
             {
                 if (value != this._Games)
                 {
                     this._Games = value;
                     NotifyPropertyChanged();
                 }
             }
         }*/



        public MainWindow()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }


            InitializeComponent();

            Active_Page = new pAccueil();
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += v;

            // Pages.Add("PrepareGames", new PrepareGames());
            // Pages.Add("MoveATXFile", new MoveATXTFile());
            // Pages.Add("pCompare_Games.xaml", new pCompare_Games());

            // Test présence de la table Games
            if (string.IsNullOrEmpty(PProp.Default.DataBase_Path))
            {
                PProp.Default.DataBase_Path = "./MyMameHelper.sqlite";
                PProp.Default.Save();
            }

            DataBaseVerification();

            if(!DatabaseOk)
            {
                return;
            }

            if (DatabaseOk && !TestTable())
            {
                System.Windows.MessageBox.Show("Table Missing, Build or Update Database (you can use DbTools)", "Alarm", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                DatabaseOk = false;
                return;
            }

            DataContext = this;

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                NumberOf_TempRoms = sqReq.Count(PProp.Default.T_TempRoms);
                NumberOf_Dev = sqReq.Count(PProp.Default.T_Companies);
                NumberOf_Manus = sqReq.Count(PProp.Default.T_Constructeurs);
                NumberOf_Roms = sqReq.Count(PProp.Default.T_Roms);
            }

        }

        private void DataBaseVerification()
        {
            // Vérification de la base de donnée
            if (File.Exists(PProp.Default.DataBase_Path))
            {
                DatabaseOk = true;
            }
            else
            {
                System.Windows.MessageBox.Show("No Database Linked !", "", MessageBoxButton.OK);
                DatabaseOk = false;
            }

        }

        private bool TestTable()
        {
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                if (sqReq.Check_Table(PProp.Default.T_TempRoms) != true)
                    return false;
                if (sqReq.Check_Table(PProp.Default.T_Roms) != true)
                    return false;
            }
            return true;
        }

        private void BtSettings_Click(object sender, RoutedEventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.ShowDialog();
        }

        private void btDb_Click(object sender, RoutedEventArgs e)
        {
            DataBaseTools dbTools = new DataBaseTools();
            dbTools.ShowDialog();

            DataBaseVerification();
            TestTable();
        }

        public static readonly RoutedCommand AddGames = new RoutedCommand("Add Games", typeof(MainWindow));
        public static readonly RoutedCommand Compare = new RoutedCommand("Compare", typeof(MainWindow));
        // public static readonly RoutedCommand MoveAllGames = new RoutedCommand("Move All Games", typeof(MainWindow));
        public static readonly RoutedCommand FileManager = new RoutedCommand("Move All Games", typeof(MainWindow));
        public static readonly RoutedCommand MoveGames1 = new RoutedCommand("Move Games1", typeof(MainWindow));
        public static readonly RoutedCommand ModifyGames = new RoutedCommand("Modify Games", typeof(MainWindow));

        private void Can_AddGames(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatabaseOk;
        }

        private void Ex_AddGames(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new PrepareGames();// Pages["PrepareGames"];
        }

        private void Can_Compare(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatabaseOk;
        }

        private void Ex_Compare(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pCompare_Games();//Pages["pCompare_Games.xaml"];

        }

        private void Can_MoveAllGames(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatabaseOk;
        }

        private void Ex_MoveAllGames(object sender, ExecutedRoutedEventArgs e)
        {
            //Active_Page = new pCopyFiles();
        }

        private void Can_MoveGames1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatabaseOk;
        }

        private void Ex_MoveGames1(object sender, ExecutedRoutedEventArgs e)
        {
            // Active_Page = new pMoveATXFiles();//Pages["MoveATXFile"];
        }

        private void Can_ModifyGames(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatabaseOk;
        }

        private void Ex_ModifyGames(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pUpdateGames();
        }

        #region Modify Roms
        public static readonly RoutedUICommand ModifyRoms = new RoutedUICommand("Modify Roms", "ModifyRoms", typeof(MainWindow));

        private void Can_ModifyRoms(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumberOf_Roms > 0;
        }

        private void Ex_ModifyRoms(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pUpdateRoms(); 
        }
        #endregion 

        #region Add XML
        public static readonly RoutedUICommand AddByXML = new RoutedUICommand("Add By XML", "AddByXML", typeof(MainWindow));

        private void Ex_AddByXML(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pPopulateTemp();
        }
        #endregion



        #region Build Developer
        public static readonly RoutedUICommand Build_Developers = new RoutedUICommand("Build Developers", "Build_Developers", typeof(MainWindow));

        private void Can_BuildDeveloper(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumberOf_TempRoms > 0;
        }

        private void Ex_BuildDeveloper(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pBuildDevs();
        }

        #endregion

        #region Build Developers
        public static readonly RoutedUICommand Build_Manufacturers = new RoutedUICommand("Build Manufacturers", "Build_Manus", typeof(MainWindow));

        private void Can_BuildManus(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumberOf_TempRoms > 0;
        }

        private void Ex_BuildManus(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pBuildManus();
        }
        #endregion

        #region Build Roms
        public static readonly RoutedUICommand Build_Roms = new RoutedUICommand("Build Roms", "Build_Roms", typeof(MainWindow));

        private void Can_BuildRoms(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumberOf_TempRoms > 0 && NumberOf_Dev > 0;
        }

        private void Ex_BuildRoms(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pBuildRoms();
        }
        #endregion


        #region Rom Manager
        public static readonly RoutedUICommand RomManager = new RoutedUICommand("Rom Manager", "RomManager", typeof(MainWindow));

        private void Can_Manage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumberOf_TempRoms > 0 && NumberOf_Dev > 0 && NumberOf_Manus > 0;
        }

        private void Ex_RomManager(object sender, ExecutedRoutedEventArgs e)
        {
            Active_Page = new pHandleRoms();
        }
        #endregion


        /// <summary>
        /// Bloque la navigation via les raccourcis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNavigating(Object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward)
            {
                e.Cancel = true;
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            }
        }


    }
}
