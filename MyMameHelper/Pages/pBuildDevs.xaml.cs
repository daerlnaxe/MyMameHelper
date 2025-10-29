using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pBuildDevs.xaml
    /// </summary>
    public partial class pBuildDevs : Page
    {
        public MyObservableCollection<RawMameRom> GameManufacturers { get; set; } = new MyObservableCollection<RawMameRom>();
        public MyObservableCollection<CT_Constructeur> Developers { get; set; } = new MyObservableCollection<CT_Constructeur>();



        public pBuildDevs()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Begin Build Developers processing ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<RawMameRom> gDev = new List<RawMameRom>();
                List<CT_Constructeur> companies;

                using (SQLite_Req sqReq = new SQLite_Req())
                {
                    #region Collection of Devs
                    Obj_Select obsDev = new Obj_Select(PProp.Default.T_Companies, all: true);
                    companies = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, obsDev);
                    #endregion

                    #region Collection of Temporary Roms
                    Obj_Select obsJ = new Obj_Select(table: PProp.Default.T_TempRoms, colonnes: new string[] { "Manufacturer" }, groups: new string[] { "Manufacturer" });
                    obsJ.Conditions = new SqlCond[] { new SqlCond(colonne: "Is_Bios", eWhere.Equal, "False"), new SqlCond(link: Linker.And, colonne: "Is_Mechanical", eWhere.Equal, "False"), new SqlCond(Linker.And, "Manufacturer", eWhere.Not_Equal, "null") };
                    gDev = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, obsJ);
                    #endregion
                }

                for (int i = 0; i < gDev.Count; i++)
                {
                    RawMameRom rom = gDev[i];
                    if (companies.FirstOrDefault(x => x.Nom.Equals(rom.Manufacturer)) != null)
                    {
                        gDev.RemoveAt(i);
                        i--;
                    }
                }

                GameManufacturers.ChangeContent = gDev;
            }
            else
            {

            }


        }

        #region Swap Left To Right
        private void Can_SL2R(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GameManufacturers.Count > 0;

        }
        private void Ex_SL2R(object sender, ExecutedRoutedEventArgs e)
        {
            List<RawMameRom> selectedManus = dgManus.SelectedItems.Cast<RawMameRom>().ToList();
            foreach (RawMameRom rom in selectedManus)
            {
                CT_Constructeur constructor = new CT_Constructeur(rom.Manufacturer);
                Developers.Add(constructor);
            }

            GameManufacturers.RemoveSilentRange(selectedManus);
        }
        #endregion

        #region Swap Right To Left
        private void Can_SR2L(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Developers.Count > 0;

        }
        private void Ex_SR2L(object sender, ExecutedRoutedEventArgs e)
        {
            List<CT_Constructeur> selectedDevs = dgDevs.SelectedItems.Cast<CT_Constructeur>().ToList();
            foreach (CT_Constructeur constructor in selectedDevs)
            {
                RawMameRom rom = new RawMameRom();
                rom.Manufacturer = constructor.Nom;
                GameManufacturers.Add(rom);
            }

            Developers.RemoveSilentRange(selectedDevs);
        }
        #endregion

        #region Reset Right
        private void Can_RR(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Developers.Count > 0;
        }


        private void Ex_RR(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (CT_Constructeur constructor in Developers)
            {
                RawMameRom rom = new RawMameRom();
                rom.Manufacturer = constructor.Nom;
                GameManufacturers.Add(rom);
            }
            Developers.Clear();
        }

        #endregion

        #region Remove From Raw
        private void Remove_Bootlegs(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameManufacturers.Count; i++)
            {
                RawMameRom manufact = GameManufacturers[i];
                if (manufact.Manufacturer.Contains("bootleg"))
                {
                    GameManufacturers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Remove_Null(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameManufacturers.Count; i++)
            {
                RawMameRom manufact = GameManufacturers[i];
                if (manufact.Manufacturer == null)
                {
                    GameManufacturers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Remove_Hack(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameManufacturers.Count; i++)
            {
                RawMameRom manufact = GameManufacturers[i];
                if (manufact.Manufacturer.StartsWith("hack"))
                {
                    GameManufacturers.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        #region Save
        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Developers.Count > 0;

        }
        private void Ex_Save(object sender, ExecutedRoutedEventArgs e)
        {
            AsyncWindowProgress awP = new AsyncWindowProgress();
            awP.go += new AsyncWindowProgress.AsyncAction(Save_DoWork);
            awP.ShowDialog();

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                MainWindow.NumberOf_Dev = sqReq.Count(PProp.Default.T_Companies);
            }
        }

        private void Save_DoWork(AsyncWindowProgress windows)
        {
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => windows.AsyncUpProgressPercent(y));
                sqReq.Insert_Devs(Developers);
            }
        }



        #endregion

        #region Select All
        private void Can_SelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GameManufacturers.Count > 0;
        }

        private void Ex_SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            dgManus.SelectAll();
        }
        #endregion
    }
}
