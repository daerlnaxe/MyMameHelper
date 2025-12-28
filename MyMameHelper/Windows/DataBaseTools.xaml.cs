using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.Models;
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

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour DataBaseTools.xaml
    /// </summary>
    public partial class DataBaseTools : Window
    {

        private MDBTools _MDBTools;

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





        public MyObservableCollection<Aff_Machine> Machines { get; private set; } = new MyObservableCollection<Aff_Machine>();



        public DataBaseTools()
        {
            InitializeComponent();
            DataContext = _MDBTools = new MDBTools();



            _MDBTools.Init();
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
                    _MDBTools.DataBase_Path = fbd.SelectedPath;
                    Properties.Settings.Default.LastPath = fbd.SelectedPath;
                }
            }
        }

        private void tbDataBase_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(tbDataBase_Path.Text))
                _MDBTools.DataBase_Path = tbDataBase_Path.Text;
        }

        private void Create_DataBase_Click(object sender, RoutedEventArgs e)
        {
            SQLiteNewDb.Create(_MDBTools.DataBase_Path);
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
            _MDBTools.AddGenre();
        }

        private void Can_EditGenre(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Genres.Count > 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_EditGenre(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Genre ctGenre = (CT_Genre)dgGenres.SelectedItem;

            LambdaValue lval = new LambdaValue();
            lval.Valeur = ctGenre.Nom;
            ctGenre.Nom = lval.Valeur;

            if (lval.ShowDialog() == true)
            {
                _MDBTools.EditGenre(ctGenre);
            }
        }


        private void Can_RemoveGenre(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Genres.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_RemoveGenre(object sender, ExecutedRoutedEventArgs e)
        {
            CT_Genre ctGenre = (CT_Genre)dgGenres.SelectedItem;
            if (System.Windows.MessageBox.Show($"Remove {ctGenre.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _MDBTools.RemoveGenre(ctGenre);
            }
        }

        #endregion Genre


        #region Hardware
        private void Can_AddConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Ajouter un constructeur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_AddConstructor(object sender, ExecutedRoutedEventArgs e)
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Insert_Constructor(new CT_Constructor() { Nom = lval.Valeur });
                    _MDBTools.Constructors.ChangeContent = sqReq.GetListOf<CT_Constructor>(CT_Constructor.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }



        private void Can_EditConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Constructors.Count > 0;
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
                    _MDBTools.Constructors.ChangeContent = sqReq.GetListOf<CT_Constructor>(CT_Constructor.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Can_RemoveConstructor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Constructors.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_RemoveConstructor(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctConst = (CT_MameManufacturer)dgConstructors.SelectedItem;
            if (System.Windows.MessageBox.Show($"Remove {ctConst.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    SqlCond cond = new SqlCond("ID", eWhere.Equal, ctConst.ID);
                    sqReq.Delete_Constructor(new SqlCond[] { cond });
                    _MDBTools.Constructors.ChangeContent = sqReq.GetListOf<CT_Constructor>(CT_Constructor.Result2Class, new Obj_Select(table: "Constructeurs", all: true));
                }
            }
        }
        #endregion



        #region Machines
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            CT_Constructor ctConst = (CT_Constructor)dgConstructors.SelectedItem;
            CT_Machine ctMachine = new CT_Machine((Aff_Machine)dgMachines.SelectedItem);

            wMachine wMach = new wMachine();
            wMach.Constructeurs = _MDBTools.Constructors;
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

                    SqlCond condition = new SqlCond("Constructeurs.ID", eWhere.Like, linkMachine.Machine.Constructeur_Id);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_AddManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                _MDBTools.AddManufacturer(lval.Valeur);


            }
        }

        private void Can_EditManufacturer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Manufacturers.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_EditManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctManu = (CT_MameManufacturer)dgManufacturers.SelectedItem;

            LambdaValue lval = new LambdaValue();
            lval.Valeur = ctManu.Nom;
            ctManu.Nom = lval.Valeur;


            if (lval.ShowDialog() == true)
            {
                _MDBTools.EditManufacturer(ctManu);
            }
        }

        private void Can_RemoveManufacturer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MDBTools.Manufacturers.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_RemoveManufacturer(object sender, ExecutedRoutedEventArgs e)
        {
            CT_MameManufacturer ctManu = (CT_MameManufacturer)dgManufacturers.SelectedItem;

            if (System.Windows.MessageBox.Show($"Remove {ctManu.Nom} ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _MDBTools.RemoveManufacturer(ctManu);
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


        #region Machine
        /// <summary>
        /// Construction des machines en se servant des roms temporaires de MAME
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Détruit l'ancienne
        /// </remarks>
        private void Build_Machines(object sender, RoutedEventArgs e)
        {
            try
            {
                _MDBTools.BuildMachines();

                System.Windows.MessageBox.Show("Machines created", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Problem with machine builder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.WriteLine($"Erreur à la construction des machines: {ex}");
            }

        }


        /// <summary>
        /// Update de la table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Machines(object sender, RoutedEventArgs e)
        {
            try
            {
                _MDBTools.Update_Machine();

                System.Windows.MessageBox.Show("Table Machines updated", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Problem with table machine updater", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.WriteLine($"Erreur à l'update de la tale machine: {ex}");
            }

        }
        #endregion Machine


        #region Remap
        /// <summary>
        /// Map Rom's Machine_ID to Machine table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remap_RomMachine(object sender, RoutedEventArgs e)
        {
            _MDBTools.RemapRomMachine();
            System.Windows.MessageBox.Show("Remap finished", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }


        private void Remap_ManuMachine(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion Remap

        private void Remap_RomManu(object sender, RoutedEventArgs e)
        {
            _MDBTools.Remap_RomManu();
            System.Windows.MessageBox.Show("Remap finished", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
