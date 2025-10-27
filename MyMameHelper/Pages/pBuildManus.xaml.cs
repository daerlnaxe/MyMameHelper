﻿using MyMameHelper.ContTable;
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
    public partial class pBuildManus : Page
    {
        public MyObservableCollection<RawMameRom> GameManufacturers { get; set; } = new MyObservableCollection<RawMameRom>();
        public MyObservableCollection<CT_Constructeur> Manufacturers { get; set; } = new MyObservableCollection<CT_Constructeur>();



        public pBuildManus()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


            if (MessageBox.Show("Begin Build Manufacturers processing ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<RawMameRom> gManu = new List<RawMameRom>();
                List<CT_Constructeur> companies;

                using (SQLite_Req sqReq = new SQLite_Req())
                {
                    Obj_Select obsDev = new Obj_Select(PProp.Default.T_Constructeurs, all:true);
                    companies = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, obsDev);

                    Obj_Select obsJ = new Obj_Select(table: PProp.Default.T_TempRoms, colonnes: new string[] { "Manufacturer" }, groups: new string[] { "Manufacturer" });
                    obsJ.Conditions = new SqlCond[] { new SqlCond(colonne: "Is_Bios", eWhere.Equal, "False"), new SqlCond(link: Linker.And, colonne: "Is_Mechanical", eWhere.Equal, "True"), new SqlCond(Linker.And, "Manufacturer", eWhere.Is_Not, "null") };

                    gManu = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, obsJ);
                }

                for(int i=0; i< gManu.Count; i++)
                {
                    RawMameRom rom = gManu[i];
                    if(companies.FirstOrDefault(x => x.Nom.Equals(rom.Manufacturer))!= null)
                    {
                        gManu.RemoveAt(i);
                        i--;
                    }
                }

                GameManufacturers.ChangeContent = gManu;
            }
            else
            {

            }


        }

        #region Swap Right To Left
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
                Manufacturers.Add(constructor);
            }

            GameManufacturers.RemoveSilentRange(selectedManus);
        }
        #endregion

        #region Swap Right To Left
        private void Can_SR2L(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Manufacturers.Count > 0;

        }
        private void Ex_SR2L(object sender, ExecutedRoutedEventArgs e)
        {
            List<CT_Constructeur> selectedManus = dgDevs.SelectedItems.Cast<CT_Constructeur>().ToList();
            foreach (CT_Constructeur constructor in selectedManus)
            {
                RawMameRom rom = new RawMameRom();
                rom.Manufacturer = constructor.Nom;
                GameManufacturers.Add(rom);
            }

            Manufacturers.RemoveSilentRange(selectedManus);
        }
        #endregion

        #region Reset Right
        private void Can_RR(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Manufacturers.Count > 0;
        }


        private void Ex_RR(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (CT_Constructeur constructor in Manufacturers)
            {
                RawMameRom rom = new RawMameRom();
                rom.Manufacturer = constructor.Nom;
                GameManufacturers.Add(rom);
            }
            Manufacturers.Clear();
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
            e.CanExecute = Manufacturers.Count > 0;

        }
        private void Ex_Save(object sender, ExecutedRoutedEventArgs e)
        {
            AsyncWindowProgress awP = new AsyncWindowProgress();
            awP.go += new AsyncWindowProgress.AsyncAction(Save_DoWork);
            awP.ShowDialog();

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                MainWindow.NumberOf_Manus = sqReq.Count(PProp.Default.T_Constructeurs);
            }

        }

        private void Save_DoWork(AsyncWindowProgress window)
        {
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));
                sqReq.Insert_Manus(Manufacturers);
            }
        }


        #endregion


    }
}
