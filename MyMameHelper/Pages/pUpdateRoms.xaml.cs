using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour pRomUpdater.xaml
    /// </summary>
    public partial class pUpdateRoms : Page
    {

        public MyObservableCollection<CT_Rom> DbRoms { get; set; } = new MyObservableCollection<CT_Rom>();
        public MyObservableCollection<CT_Rom> RomsModified { get; set; } = new MyObservableCollection<CT_Rom>();

        private List<CT_Rom> _TempRoms { get; set; } = new List<CT_Rom>();

        public MyObservableCollection<CT_Constructeur> Developers { get; set; } = new MyObservableCollection<CT_Constructeur>();

        public pUpdateRoms()
        {
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                DbRoms.ChangeContent = sqReq.AffRoms_List(order: new SqlOrder("Archive_Name"));
                Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Companies, all: true));
            }

            InitializeComponent();
            DataContext = this;
        }

        #region Add Rom
        private void Can_AddRom(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DbRoms.Count > 0;
        }

        private void Ex_AddRom(object sender, ExecutedRoutedEventArgs e)
        {
            //List<CT_Rom> toModify = null;
            if (dg2Organize.SelectedItems.Count == 0)
                MessageBox.Show("Select Rom(s)", "", MessageBoxButton.OK);

            foreach (CT_Rom rom in dg2Organize.SelectedItems)
            {
                if (RomsModified.FirstOrDefault(x => x.Archive_Name.Equals(rom.Archive_Name)) != null)
                    continue;

                RomsModified.AddSilent(new CT_Rom(rom));
            }

            //  _TempRoms.AddRange(toModify);
            //  DbRoms.RemoveSilentRange(toModify);

            RomsModified.SignalChange();
            dg2Organize.SelectedIndex = -1;
            //  DbRoms.SignalChange();
        }
        #endregion

        #region Remove Rom
        private void Can_RemoveRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsModified.Count > 0;
        }

        private void Ex_RemoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            List<CT_Rom> items = dgRight.SelectedItems.Cast<CT_Rom>().ToList();

            RomsModified.RemoveSilentRange(items);
            RomsModified.SignalChange();
        }
        #endregion

        #region
        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsModified.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            RomsModified.Clear();
        }
        #endregion


        #region Change
        private void Can_Change(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsModified.Count > 0;
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
                CT_Rom game = dgRight.SelectedItems[i] as CT_Rom;
                if (game is null)
                    continue;

                Trace.WriteLine(game.Archive_Name);

                // Unwanted
                game.Unwanted = cbUnwanted.IsChecked;

                //Developpeurs
                if (cboxDevelopers.SelectedItem != null)
                {
                    CT_Constructeur dev = cboxDevelopers.SelectedItem as CT_Constructeur;
                    game.Manufacturer = dev.ID;
                    game.Aff_Manufacturer = dev.Nom;
                }

            }
        }
        #endregion

        #region Update
        private void Can_Update(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsModified.Count > 0;
        }

        private void UpdateRoms(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateDbRoms<CT_Rom> sDb = new UpdateDbRoms<CT_Rom>();
            sDb.Update_GamesTable(RomsModified);

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                DbRoms.ChangeContent = sqReq.AffRoms_List();
            }
            RomsModified.Clear();
        }


        #endregion

        #region Delete from db
        private void Can_DeleteFromDb(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DbRoms.Count > 0;
        }

        private void Ex_DeleteFromDb(object sender, ExecutedRoutedEventArgs e)
        {
            List<CT_Rom> selected = dg2Organize.SelectedItems.Cast<CT_Rom>().ToList();

            if (MessageBox.Show("Delete this roms from database \\n(Note if there is parent, it will delete children roms too) ?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // On choppe les enfants sans quoi il y aura des orphelins
            List<CT_Rom> additionnal = new List<CT_Rom>();
            foreach (CT_Rom rom in selected)
            {
                if (rom.IsParent)
                {
                    // Récupération des roms enfants
                    IEnumerable<CT_Rom> children = DbRoms.Where(x => x.Clone_Of == rom.ID);

                    foreach (CT_Rom child in children)
                        if (selected.FirstOrDefault(x => x.ID == child.ID) == null)
                            additionnal.Add(child);
                }

            }

            selected.AddRange(additionnal);


            using (SQLite_Req sqReq = new SQLite_Req())
            {
                MyObservableCollection<Aff_Game> Games2Update = new MyObservableCollection<Aff_Game>();
                foreach (var rom in selected)
                {

                    SqlCond[] conditions = new SqlCond[] { new SqlCond(colonne: "ID", eWhere.Equal, rom.ID) };
                    sqReq.Delete_Rom(conditions);

                    //
                    SqlCond[] gameCond = new SqlCond[] { new SqlCond(colonne: "Roms", eWhere.Like, $"%{rom.ID}%") };
                    List<Aff_Game> pretends = sqReq.AffGames_List(gameCond);

                    // Recherche du game contenant
                    CT_Game gameCont = null;
                    CT_Rom rom2Rem = null;
                    foreach (Aff_Game game in pretends)
                        foreach (CT_Rom romInGame in game.Roms)
                            if (romInGame.ID == rom.ID)
                            {
                                gameCont = game;
                                rom2Rem = romInGame;
                                
                                break;
                            }

                    // update des jeux
                    if (gameCont != null)
                    {
                        gameCont.Roms.Remove(rom2Rem);
                        sqReq.Update_Game(gameCont);
                    }

                }

                DbRoms.ChangeContent = sqReq.AffRoms_List();

            }

        }
    }
    #endregion
}


