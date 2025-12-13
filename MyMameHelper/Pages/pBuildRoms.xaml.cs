using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.Models;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
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


namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pWorkPage.xaml
    /// </summary>
    public partial class pBuildRoms : Page
    {

        public static readonly RoutedCommand Select_AllCmd = new RoutedCommand("Select All", typeof(pBuildRoms));

        //  public MyObservableCollection<Aff_Game> DbGames { get; set; } = new MyObservableCollection<Aff_Game>();


        private MBuildRoms _MContext;




        // A lever ? 2025
        public MyObservableCollection<CT_MameManufacturer> Developers { get; set; } = new MyObservableCollection<CT_MameManufacturer>();



        public CT_MameManufacturer CbDeveloper_Selected { get; set; }



        //private List<RawMameRom> rawRomsSelected;
        //      private List<Aff_Rom> romsList;
        private List<CT_Rom> romsSelected;





        public pBuildRoms()
        {
            InitializeComponent();

            //DataContext = this;
            DataContext = _MContext = new MBuildRoms();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Load Roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Cursor = Cursors.Wait;
                _MContext.LoadCollecs();
                this.Cursor = Cursors.Arrow;
            }
        }



        private void Select_All(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            dg2Organize.SelectAll(); // utile ?
            Mouse.OverrideCursor = Cursors.Arrow;
        }



        private void AllwaysTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /*
        private void CbDeveloper_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }*/

        #region Simulation
        raoul wd1;

        //public static readonly RoutedUICommand AddCmd = new RoutedUICommand("Simulate", "AddCmd", typeof(pWorkPage));

        /*
    private void Can_Simulate(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = DbGames.Count > 0;
    }
    */
        /*
        private void Ex_Add(object sender, ExecutedRoutedEventArgs e)
        {
            //CT_Machine machine = (CT_Machine)cbMachines.SelectedItem;

            List<Aff_Game> toModify = null;
            if (dg2Organize.SelectedItems.Count == 0)
            {
                toModify = DbGames.ToList();
            }
            else
            {
                toModify = dg2Organize.SelectedItems.Cast<Aff_Game>().ToList();
            }
            dg2Organize.SelectedIndex = -1;

            RomsToSave.AddRange(toModify);
            DbGames.RemoveRange(toModify);
        }
        */

        /*
        /// <summary>
        /// obsolète ?
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
            throw new NotImplementedException("A identifier");
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





        #region Datagrid Gauche
        private void Can_Left2Right(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MContext.RawRomsFiltered.Count > 0;
        }

        /// <summary>
        /// Ajoutes les roms sélectionnées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_Left2Right(object sender, ExecutedRoutedEventArgs e)
        {
            List<RawMameRom> rawRomsSelected = dg2Organize.SelectedItems.Cast<RawMameRom>().ToList();
            _MContext.TransRaw2Rom(rawRomsSelected);
        }
        #endregion


        /// <summary>
        /// Ajoute toutes les rawroms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Ne va pas sélectionner à gauche comme avec Select All
        /// </remarks>
        private void Add_All(object sender, ExecutedRoutedEventArgs e)
        {
            // List<RawMameRom> rawRomsSelected = dg2Organize.SelectedItems.Cast<RawMameRom>().ToList();
            _MContext.TransRaw2Rom(_MContext.RawRomsFiltered.ToList());
        }

        /*
         /// <summary>
         /// 
         /// </summary>
         /// <param name="window"></param>
         /// <remarks>
         /// ~ 5 minutes
         /// </remarks>
         private void LinkRoms(AsyncWindowProgress window)
         {
             List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.Arguments[0]; // ajouté en splittant vers de l'async

             Stopwatch swTotal = new Stopwatch();
             swTotal.Start();
             //rah
             List<RawMameRom> tmp = new List<RawMameRom>();
             tmp.AddRange(rawRomsSelected);

             int i = 0;
             foreach (RawMameRom selRom in rawRomsSelected)
             {
                 Stopwatch sw1 = new Stopwatch();
                 sw1.Start();

                 if (string.IsNullOrEmpty(selRom.Clone_Of))
                 {
                     // on récupère tous les enfants
                     IEnumerable<RawMameRom> children = RawRomsCollec.Where(x => x.Clone_Of.Equals(selRom.Name));


                     // Ajoute ceux qui ne sont pas présents
                     foreach (var child in children)
                     {
                         if (tmp.FirstOrDefault(x => x.ID == child.ID) == null)
                             tmp.Add(child);
                     }
                     Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (if),  temps: {sw1.ElapsedMilliseconds} ms");
                 }
                 else
                 {

                     // on récupère tous les parents
                     RawMameRom parent = RawRomsCollec.FirstOrDefault(x => x.Name.Equals(selRom.Clone_Of));


                     if (parent == null)
                     {
                         Console.WriteLine("la");
                         continue;
                     }


                     if (tmp.FirstOrDefault(x => x.ID == parent.ID) == null)
                         tmp.Add(parent);

                     //  Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des parents (else),  temps: {sw1.ElapsedMilliseconds} ms");

                     // on récupère tous les enfants
                     IEnumerable<RawMameRom> children = RawRomsCollec.Where(x => x.Clone_Of.Equals(selRom.Clone_Of));

                     // Ajoute ceux qqui ne sont pas présents
                     foreach (var child in children)
                     {
                         if (tmp.FirstOrDefault(x => x.ID == child.ID) == null)
                             tmp.Add(child);
                     }
                     //  Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (else),  temps: {sw1.ElapsedMilliseconds} ms");
                 }
                 Debug.WriteLine($"Fin pour {selRom.Name},  temps: {sw1.ElapsedMilliseconds} ms");
                 sw1.Stop();

                 // lié au passage asynchrone
                 window.AsyncUpProgressPercent(i);
                 i++;
             }

             window.Arguments[0] = tmp;

             Debug.WriteLine($"Fin Total,  temps: {swTotal.ElapsedMilliseconds} ms");
             swTotal.Stop();
         }

        */





        /*
        private void LeftMode_Changed(object sender, RoutedEventArgs e)
        {
            _MContext.LeftRomMode = ((RadioButton)sender).Content.ToString();
            if (_MContext.LeftFilter != null)
                Select_Left();
        }
        */


        private void Select_Left()
        {

            if (_MContext.LeftRomMode == "Mode Game")
                _MContext.LeftSelected = _MContext.RawRomsFiltered.FirstOrDefault(x => x.Description.StartsWith(_MContext.LeftFilter, StringComparison.OrdinalIgnoreCase));
            else if (_MContext.LeftRomMode == "Archive Select")
                _MContext.LeftSelected = _MContext.RawRomsFiltered.FirstOrDefault(x => x.Name.StartsWith(_MContext.LeftFilter, StringComparison.OrdinalIgnoreCase));

            if (_MContext.LeftSelected != null)
            {
                dg2Organize.ScrollIntoView(dg2Organize.SelectedItem);
            }
        }

        #region
        /// <summary>
        /// Permet de filtrer la datagrid de gauche en pressant une touche du clavier.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.A && e.Key <= Key.Z)
               || (e.Key >= Key.D0 && e.Key <= Key.D9)
               || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
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

            var LeftFilter = _MContext.LeftFilter;

            if (LeftFilter is null)
                LeftFilter = string.Empty;

            //
            if (e.Key == Key.Back)
                _MContext.LeftFilter = LeftFilter.Length > 0 ? LeftFilter.Substring(0, LeftFilter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                _MContext.LeftFilter = string.Empty;

            else
                _MContext.LeftFilter += k;


            Select_Left();
        }
        #endregion



        #region Datagrid Droite
        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MContext.RomsToSave.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            _MContext.ResetFromRight();
        }



        private void Can_Right2Left(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MContext.RomsToSave.Count > 0;
        }


        /// <summary>
        /// Enlève de la datagrid de droite et déplace à gauche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_Right2Left(object sender, ExecutedRoutedEventArgs e)
        {
            romsSelected = dgRight.SelectedItems.Cast<CT_Rom>().ToList();
            _MContext.RemoveFromRight(romsSelected);
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_Right2Left(object sender, ExecutedRoutedEventArgs e)
        {
            romsSelected = dgRight.SelectedItems.Cast<CT_Rom>().ToList();

            List<CT_Rom> tmp = new List<CT_Rom>();
            tmp.AddRange(romsSelected);

            // Recherche des roms en relation

            foreach (CT_Rom selRom in romsSelected)
            {
                foreach (CT_Rom rom in RomsToSave)
                {
                    /*
                    if (selRom == rom)
                        continue;
                        *//*
                    if (romsSelected.Contains(rom))
                        continue;

                    if (rom.Aff_Clone_Of.Equals(selRom.Archive_Name))
                        tmp.Add(rom);

                    if (string.IsNullOrEmpty(rom.Aff_Clone_Of))
                        continue;

                    if (selRom.Aff_Clone_Of.Equals(rom.Archive_Name))
                        tmp.Add(rom);

                    if (rom.Aff_Clone_Of.Equals(selRom.Aff_Clone_Of))
                        tmp.Add(rom);
                }
            }
            romsSelected.AddRange(tmp);
            tmp.Clear();

            AsyncWindowProgress window = new AsyncWindowProgress();
            window.go += new AsyncWindowProgress.AsyncAction(AsyncRight2Left);
            //window.Arguments = new List<object>() { RomsToSave.ToList() };
            window.ShowDialog();
            RomsToSave.SignalChange();
            RawRomsCollec.SignalChange();
        }
        */


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        private void AsyncRight2Left(AsyncWindowProgress window)
        {
            //romsList = new List<Aff_Rom>();
            // Sélectionnés
            for (int i = 0; i < romsSelected.Count; i++)
            {
                CT_Rom sel = romsSelected[i];
                for (int j = 0; j < rawRomsDeleted.Count; j++)
                {
                    RawMameRom deleted = rawRomsDeleted[j];
                    if (deleted.Name.Equals(sel.Archive_Name))
                    {
                        RawRomsCollec.AddSilent(deleted);
                        rawRomsDeleted.Remove(deleted);
                        RomsToSave.RemoveSilent(sel);
                        break;
                    }
                }

                window.AsyncUpProgressPercent(i);
            }
        }

        */



        #endregion

        #region Filtre de Droite
        /*
        private string RightRomMode;
        private string _RightFilter;
        public string RightFilter
        {
            get { return _RightFilter; }
            set
            {
                if (!value.Equals(_RightFilter))
                {
                    _RightFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private CT_Rom _S4R;
        public CT_Rom RightSelected
        {
            get { return _S4R; }
            set
            {
                if (value != _S4R)
                {
                    _S4R = value;
                    NotifyPropertyChanged();
                }
            }
        }
        */


        private void RightMode_Changed(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                _MContext.LeftRomMode = ((RadioButton)sender).Content.ToString();

            /*if (LeftFilter != null)
                Select_Right();*/
        }


        /*
        private void Select_Right()
        {
            if (RightRomMode == "Mode Game")
                RightSelected = RomsToSave.FirstOrDefault(x => x.SourceFile.StartsWith(RightFilter, StringComparison.OrdinalIgnoreCase));
            else if (LeftRomMode == "Mode Archive")
                RightSelected = RomsToSave.FirstOrDefault(x => x.Archive_Name.StartsWith(RightFilter, StringComparison.OrdinalIgnoreCase));

            if (RightSelected != null)
            {
                dgRight.ScrollIntoView(dgRight.SelectedItem);
            }
        }*/

        /// <summary>
        /// Obsolète
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.A && e.Key <= Key.Z)
               || (e.Key >= Key.D0 && e.Key <= Key.D9)
               || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
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

            /*
            if (RightFilter is null)
                RightFilter = string.Empty;

            //
            if (e.Key == Key.Back)
                RightFilter = RightFilter.Length > 0 ? RightFilter.Substring(0, RightFilter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                RightFilter = string.Empty;

            else
                RightFilter += k;


            //Select_Right();*/
        }

        #endregion

        /*
        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            //throw new NotImplementedException("Vérifier la compatibilité après le spit de transraw2rom");
            RawRomSearch sp = new RawRomSearch();
            if (sp.ShowDialog() == true)
            {
                var foundRRoms = sp.RomsFound.ToList();

                for (int i = 0; i < foundRRoms.Count; i++)
                {
                    RawMameRom rawRom = foundRRoms[i];

                    if (_RomsInDb.FirstOrDefault<CT_Rom>(x => x.Archive_Name.Equals(rawRom.Name)) != null)
                    {
                        foundRRoms.Remove(rawRom);
                        i--;
                    }
                }

                TransRaw2Rom(foundRRoms);
                // RomsToSave.ChangeContent = sp.RomsFound.ToList();
            }
        }*/



        #region Change
        /*
        private void Can_Change(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsToSave.Count > 0;
        }*/



        /*
        private void Ex_Change(object sender, ExecutedRoutedEventArgs e)
        {
            // A refaire car les manufacturers ne sont plus des uint mais des objets

            if (dgRight.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Game(s)");
                return;
            }

            for (int i = 0; i < dgRight.SelectedItems.Count; i++)
            {
                CT_Rom rom = dgRight.SelectedItems[i] as CT_Rom;
                if (rom is null)
                    continue;

                Trace.WriteLine(rom.Archive_Name);

                if (CbDeveloper_Selected != null)
                {
                    rom.Manufacturer = CbDeveloper_Selected.ID;
                    rom.Aff_Manufacturer = CbDeveloper_Selected.Nom;
                }

                // Unwanted

                if (cbUnwanted.IsChecked == true)
                    rom.Unwanted = true;
                else
                    rom.Unwanted = false;

                // Genre
                /*
                //Developpeurs
                if (cboxDevs.SelectedItem != null)
                {
                    CT_Constructeur dev = cboxDevs.SelectedItem as CT_Constructeur;
                    game.Developer = dev.ID;
                    game.Aff_Developer = dev.Nom;
                }
                *//*
            }
            CbDeveloper_Selected = null;
        }
        */

        #endregion

        #region Delete
        /*
        private void Can_Delete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DbGames.Count > 0;
        }
        */
        /*
        private void DeleteGames(object sender, ExecutedRoutedEventArgs e)
        {
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

        }
        */
        #endregion

        /// <summary>
        /// Obsolète ?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbConstructeur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { /*
            ComboBox cb = (ComboBox)sender;
            int idConstruct = Convert.ToInt32(cb.SelectedValue);

            using (SQLite_OP sqReq = new SQLite_OP())
            {
                var objSel = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                objSel.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                objSel.AddOrders(new SqlOrder("Nom"));

                Machines.ChangeContent = sqReq.GetListOf(CT_Machine.Result2Class, objSel);
            }*/
        }




        #region Sauver/Updater dans les tables
        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _MContext?.RomsToSave.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Sémantiquement parlant on part du principe d'amener les questions une par une mais d'interrompre le processus si la chaine n'est pas validée.
        /// Les manufacturers et les jeux ont deux logiques différentes.
        ///     Le manufacturer existe en liaison avec la rawrom, on s'appuie dessus
        ///     Le Jeu existe peut être en base mais directement avec la rawrom, par contre une fois la rom entrée on ne la verra plus par rapport au différentiel au chargement.
        /// </remarks>
        private void EX_SaveToDB(object sender, ExecutedRoutedEventArgs e)
        {

            if (this._MContext.SaveToDB())
            {
                MessageBox.Show("Task Finished", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

            }

        }



        #endregion


    }
}
