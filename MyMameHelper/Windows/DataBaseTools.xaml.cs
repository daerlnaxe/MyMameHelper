using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour DataBaseTools.xaml
    /// </summary>
    public partial class DataBaseTools : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /*
        private ObservableCollection<Game> _Constructors = new ObservableCollection<Game>();
        public ObservableCollection<Game> Constructors
        {
            get { return _Constructors; }
            set
            {
                if (value != _Constructors)
                {
                    _Constructors = value;
                    NotifyPropertyChanged();
                }
            }
        }*/

        private string _DataBase_Path;
        public string DataBase_Path
        {
            get { return _DataBase_Path; }
            set
            {
                if (value != _DataBase_Path)
                {
                    _DataBase_Path = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> lTables { get; private set; }

        public MyObservableCollection<CT_MameManufacturer> Constructors { get; private set; } = new MyObservableCollection<CT_MameManufacturer>();
        public MyObservableCollection<CT_Genre> Genres { get; private set; } = new MyObservableCollection<CT_Genre>();
        public MyObservableCollection<Aff_Machine> Machines { get; private set; } = new MyObservableCollection<Aff_Machine>();
        public MyObservableCollection<CT_MameManufacturer> Manufacturers { get; private set; } = new MyObservableCollection<CT_MameManufacturer>();


        public DataBaseTools()
        {
            InitializeComponent();
            DataContext = this;

            DataBase_Path = Properties.Settings.Default.DataBase_Path;
            if (String.IsNullOrEmpty(DataBase_Path))
            {
                Properties.Settings.Default.DataBase_Path = DataBase_Path = "./MyMameHelper.sqlite";
                Properties.Settings.Default.Save();
            }
            Init();
        }

        private void Init()
        {
            try
            {
                using (SQLite_OP sqRead = new SQLite_OP())
                {
                    // Liste des tables
                    lTables = sqRead.GET_TablesName();


                    // Liste des constructeurs 
                    var objSelConst = new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true);
                    objSelConst.AddOrders(new SqlOrder("Nom"));
                    Constructors.ChangeContent = sqRead.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, objSelConst);

                    var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                    objSelGenres.AddOrders(new SqlOrder("Nom"));
                    // Liste des genres
                    Genres.ChangeContent = sqRead.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);


                    // Liste des Manufacturers
                    Manufacturers.ChangeContent = sqRead.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));



                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                // DxMBox.ShowDial($"Problème de connexion à la base de donnée: {exc.Message}", "Alerte", DxTBoxWPF.Common.DxButtons.Ok);
                // Continue = false;
            }

        }

        #region Base de donnée
        private void Choose_DataBase_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                fbd.SelectedPath = Properties.Settings.Default.DataBase_Path;
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    DataBase_Path = fbd.SelectedPath;
                    Properties.Settings.Default.LastPath = fbd.SelectedPath;
                }
            }
        }

        private void tbDataBase_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(tbDataBase_Path.Text))
                DataBase_Path = tbDataBase_Path.Text;
        }

        private void Create_DataBase_Click(object sender, RoutedEventArgs e)
        {
            SQLiteNewDb.Create(DataBase_Path);
        }

        #endregion


        #region
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gch = (GridViewColumnHeader)sender;
            System.Windows.Controls.ListView lv = (System.Windows.Controls.ListView)gch.Parent;
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction, lv);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction, System.Windows.Controls.ListView lv)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
            #endregion
        }

        private void lvConstructors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgConstructors.SelectedItem == null)
                return;

            var selConstructor = (CT_MameManufacturer)dgConstructors.SelectedItem;

            if (selConstructor == null)
                return;

            // Liste des machines
            using (SQLite_OP sqRead = new SQLite_OP())
            {
                SqlCond[] conditions = new SqlCond[] { new SqlCond(colonne: "Constructeurs.ID", eWhere.Equal, selConstructor.ID) };
                Machines.ChangeContent = sqRead.List_MachinesJoin(conditions);
            }
        }

        #region Genre
        private void Can_AddGenre(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Ex_AddGenre(object sender, ExecutedRoutedEventArgs e)
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Insert_Genre(new CT_Genre() { Nom = lval.Valeur });
                    var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                    objSelGenres.AddOrders(new SqlOrder("Nom"));

                    Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);
                }
            }

        }

        private void Can_EditGenre(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Genres.Count > 0;
        }

        private void Ex_EditGenre(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Genre ctGenre = (CT_Genre)dgGenres.SelectedItem;

            LambdaValue lval = new LambdaValue();
            lval.Valeur = ctGenre.Nom;

            if (lval.ShowDialog() == true)
            {
                ctGenre.Nom = lval.Valeur;

                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Update_Genre(ctGenre);
                    var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                    objSelGenres.AddOrders(new SqlOrder("Nom"));
                    Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);
                }
            }
        }

        private void Can_RemoveGenre(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Genres.Count > 0;
        }

        private void Ex_RemoveGenre(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Genre ctGenre = (CT_Genre)dgGenres.SelectedItem;
            if (System.Windows.MessageBox.Show($"Remove {ctGenre.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    SqlCond cond = new SqlCond("ID", eWhere.Equal, ctGenre.ID);
                    sqReq.Delete_Genre(new SqlCond[] { cond });
                    var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                    objSelGenres.AddOrders(new SqlOrder("Nom"));
                    Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);
                }
            }
        }

        #endregion

        #region Hardware
        private void Can_AddConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Ex_AddConstructor(object sender, ExecutedRoutedEventArgs e)
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Insert_MameManufacturer(new CT_MameManufacturer() { Nom = lval.Valeur });
                    Constructors.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }

        private void Can_EditConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Constructors.Count > 0;
        }

        private void Ex_EditConstructor(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctConst = (CT_MameManufacturer)dgConstructors.SelectedItem;

            LambdaValue lval = new LambdaValue();
            lval.Valeur = ctConst.Nom;

            if (lval.ShowDialog() == true)
            {
                ctConst.Nom = lval.Valeur;

                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Update_MameManufacturer(ctConst);
                    Constructors.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }

        private void Can_RemoveConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Constructors.Count > 0;
        }

        private void Ex_RemoveConstructor(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctConst = (CT_MameManufacturer)dgConstructors.SelectedItem;
            if (System.Windows.MessageBox.Show($"Remove {ctConst.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    SqlCond cond = new SqlCond("ID", eWhere.Equal, ctConst.ID);
                    sqReq.Delete_Constructor(new SqlCond[] { cond });
                    Constructors.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }
        #endregion

        #region Machines
        private void Can_AddMachine(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /*
        private void Ex_AddMachine(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Constructeur ctConst = (CT_Constructeur)dgConstructors.SelectedItem;

            wMachine wMach = new wMachine();
            wMach.Constructeurs = Constructors;
            wMach.Machine = new CT_Machine();

            if (ctConst != null)
                wMach.Machine.IDConstructeur = ctConst.ID;

            if (wMach.ShowDialog() == true)
            {
                using (SQLite_Op sqReq = new SQLite_Op())
                {
                    sqReq.Insert_Machine(wMach.Machine);
                    SqlCond condition = new SqlCond("Constructeur", eWhere.Like, wMach.Machine.IDConstructeur);
                    Machines.ChangeContent = sqReq.List_MachinesJoin(new SqlCond[] { condition });
                }
            }
        }*/

        private void Can_EditMachine(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machines.Count > 0;
        }

        private void Ex_EditMachine(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctConst = (CT_MameManufacturer)dgConstructors.SelectedItem;
            CT_Machine ctMachine = new CT_Machine((Aff_Machine)dgMachines.SelectedItem);

            wMachine wMach = new wMachine();
            wMach.Constructeurs = Constructors;
            wMach.Machine = ctMachine;
            //wMach.SelectedConstructeur = ctConst;
            /*
            lval.MachineName = ctMachine.Nom;
            lval.Year = ctMachine.Year;
            lval.AllowCPath = ctMachine.AllowCPath;
            */

            if (wMach.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    /*ctMachine.Nom = lval.MachineName;
                    ctMachine.IDConstructeur = ((CT_Constructeur)lval.SelectedConstructeur).ID;
                    ctMachine.Year = lval.Year;
                    ctMachine.AllowCPath = lval.AllowCPath;*/

                    sqReq.Update_Machine(ctMachine);

                    SqlCond condition = new SqlCond("Constructeur", eWhere.Like, ctConst.ID);
                    Machines.ChangeContent = sqReq.List_MachinesJoin(new SqlCond[] { condition });
                }
            }
        }

        private void Can_RemoveMachine(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machines.Count > 0;
        }

        private void Ex_RemoveMachine(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctConst = (CT_MameManufacturer)dgConstructors.SelectedItem;
            CT_Machine ctMachine = (CT_Machine)dgMachines.SelectedItem;

            if (System.Windows.MessageBox.Show($"Remove {ctMachine.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    SqlCond cond = new SqlCond("ID", eWhere.Equal, ctMachine.ID);
                    sqReq.Delete_Machine(new SqlCond[] { cond });
                    SqlCond condition = new SqlCond("Constructeur", eWhere.Like, ctConst.ID);
                    Machines.ChangeContent = sqReq.List_MachinesJoin(new SqlCond[] { condition });
                }
            }
        }


        private void Ex_LinkAMachine(object sender, ExecutedRoutedEventArgs e)
        {
            wLinkMachine linkMachine = new wLinkMachine();
            if (linkMachine.ShowDialog() == true)
            {
                using (SQLite_OP sqOP = new SQLite_OP())
                {
                    var ctMachine = linkMachine.Machine;
                    sqOP.Update_Machine(ctMachine);

                    SqlCond condition = new SqlCond("Constructeurs.ID", eWhere.Like, linkMachine.Machine.IDConstructeur);
                    Machines.ChangeContent = sqOP.List_MachinesJoin(new SqlCond[] { condition });
                }
            }
        }
        #endregion


        #region Manufacturer
        private void Can_AddManufacturer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Ex_AddManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Insert_Constructor(new CT_MameManufacturer() { Nom = lval.Valeur });
                    Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
                }
            }
        }

        private void Can_EditManufacturer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Manufacturers.Count > 0;
        }

        private void Ex_EditManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctManu = (CT_MameManufacturer)dgManufacturers.SelectedItem;

            LambdaValue lval = new LambdaValue();
            lval.Valeur = ctManu.Nom;

            if (lval.ShowDialog() == true)
            {
                ctManu.Nom = lval.Valeur;

                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Update_MameManufacturer(ctManu);
                    Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
                }
            }
        }

        private void Can_RemoveManufacturer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Manufacturers.Count > 0;
        }

        private void Ex_RemoveManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctManu = (CT_MameManufacturer)dgManufacturers.SelectedItem;

            if (System.Windows.MessageBox.Show($"Remove {ctManu.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    SqlCond cond = new SqlCond("ID", eWhere.Equal, ctManu.ID);
                    sqReq.Delete_MameManufacturer(new SqlCond[] { cond });

                    Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
                }
            }

        }
        #endregion

        /// <summary>
        /// Remise à zero des roms temporaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_TempRoms_Click(object sender, RoutedEventArgs e)
        {
            bool res = false;
            if (System.Windows.MessageBox.Show("Reset The Table Containing Temporary Roms ?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqOp = new SQLite_OP())
                {
                    sqOp.Drop_TTempRom();
                    sqOp.Create_TTempRom();
                }
                    //res = sqReq.Flush_TempRoms();
            }

            if (res)
                System.Windows.MessageBox.Show("Table Flushed");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDB(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofD = new OpenFileDialog();
            ofD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (ofD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SQLiteNewDb.Update_Structure(ofD.FileName);
            }
        }


        /// <summary>
        /// Construction des machines en se servant des roms temporaires de MAME
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build_Machines(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SQLite_OP sqOP = new SQLite_OP())
                {
                    sqOP.Drop_TMachine();
                    sqOP.Create_TMachine();

                    var srcFiles = sqOP.Get_RRGroupedSFile();

                    TableFeeder.Machine(srcFiles);
                }
            }
            catch (Exception ex) 
            { 
                Trace.WriteLine($"Erreur à la construction des machines: {ex}"); 
            }

        }
    }
}
