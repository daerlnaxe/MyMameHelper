using MyMameHelper.ContTable;
using MyMameHelper.Methods;
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
    public partial class pBuildRoms : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly RoutedCommand Select_AllCmd = new RoutedCommand("Select All", typeof(pBuildRoms));

        //  public MyObservableCollection<Aff_Game> DbGames { get; set; } = new MyObservableCollection<Aff_Game>();

        #region pas en fonction pour le moment
        /// <summary>
        /// Active la recherche de parent quand on ajoute aux roms à sauvegarder
        /// </summary>
        public Boolean ParentChecked { get; set; } = true;

        /// <summary>
        /// Active la recherche de frêres quand on ajoute aux roms à sauvegarder
        /// </summary>
        public Boolean BrothersChecked { get; set; } = true;
        #endregion

        /// <summary>
        /// Manufacturers
        /// </summary>
        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();

        // A Lever ? 2025
        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();

        // A lever ? 2025
        public MyObservableCollection<CT_Constructeur> Developers { get; set; } = new MyObservableCollection<CT_Constructeur>();

        public MyObservableCollection<CT_Rom> RomsToSave { get; set; } = new MyObservableCollection<CT_Rom>();

        public MyObservableCollection<RawMameRom> RawRomsCollec { get; set; } = new MyObservableCollection<RawMameRom>();

        public CT_Constructeur CbDeveloper_Selected { get; set; }


        /// <summary>
        /// Liste des roms en base
        /// </summary>
        /// <remarks>
        /// Utilisé pour faire le différentiel avec les rawroms.
        /// </remarks>
        private List<CT_Rom> _RomsInDb;

        /// <summary>
        /// 2025, utilisé à la sauvegarde
        /// </summary>
        private List<CT_Game> _GamesInDB;

        private List<RawMameRom> rawRomsDeleted = new List<RawMameRom>();

        //private List<RawMameRom> rawRomsSelected;
        //      private List<Aff_Rom> romsList;
        private List<CT_Rom> romsSelected;

        private List<RawMameRom> ListRoms { get; set; }



        public pBuildRoms()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Load Roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Chargement de la table des développeurs
                using (SQLite_Op sqReq = new SQLite_Op())
                {
                    //2025 levé Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Developers, all: true));
                    Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));
                    _RomsInDb = sqReq.AffRoms_List();
                    _GamesInDB = sqReq.GetListOf(CT_Game.Result2Class, new Obj_Select(table: PProp.Default.T_Games, colonnes: new[] { "ID", "Game_Name" }, all: true));
                }

                // Chargement asynchrone des roms
                AsyncWindowProgress aLoad = new AsyncWindowProgress();
                aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoadTempRoms);
                aLoad.ShowDialog();
                RawRomsCollec.ChangeContent = ListRoms;
            }
        }

        /// <summary>
        /// Chargement des roms temporaires
        /// </summary>
        /// <param name="aLoad"></param>
        private void AsyncLoadTempRoms(AsyncWindowProgress aLoad)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            aLoad.AsyncMessage("Loading Roms...");
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Obj_Select objSel = new Obj_Select(table: PProp.Default.T_TempRoms, all: true);

                /* SqlCond[] condBios = new SqlCond[] { new SqlCond("Is_Bios", eWhere.Equal, "True") };
                 objSel.Conditions = condBios;*/
                ListRoms = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, objSel);
            }

            for (int i = 0; i < ListRoms.Count; i++)
            {
                RawMameRom rawRom = ListRoms[i];

                if (_RomsInDb.FirstOrDefault<CT_Rom>(x => x.Archive_Name.Equals(rawRom.Name)) != null)
                {
                    ListRoms.Remove(rawRom);
                    i--;
                }
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
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
        }

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





        #region Datagrid Gauche
        private void Can_Left2Right(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RawRomsCollec.Count > 0;
        }

        private void Ex_Left2Right(object sender, ExecutedRoutedEventArgs e)
        {
            List<RawMameRom> rawRomsSelected = dg2Organize.SelectedItems.Cast<RawMameRom>().ToList();
            TransRaw2Rom(rawRomsSelected);
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Ne va pas sélectionner à gauche comme avec Select All
        /// </remarks>
        private void Add_All(object sender, ExecutedRoutedEventArgs e)
        {
            List<RawMameRom> rawRomsSelected = dg2Organize.SelectedItems.Cast<RawMameRom>().ToList();
            TransRaw2Rom(RawRomsCollec.ToList());
        }

        /// <summary>
        /// Transforme une rom temporaire en rom (avec les jonctions)
        /// </summary>
        /// <param name="rawRomsSelected"></param>
        private void TransRaw2Rom(List<RawMameRom> rawRomsSelected)
        {

            //Contenu de LinkRoms

            /*

                // Recherche des roms en relation
                foreach (RawMameRom rom in RawRomsCollec)
            {
                foreach (RawMameRom selRom in rawRomsSelected)
                {
                    /*
                    if (selRom == rom)
                        continue;
                        */
            /*
        if (rawRomsSelected.FirstOrDefault(x => x.ID == rom.ID) != null)
            continue;

        if (selRom.Clone_Of.Equals(rom.Name))
            tmp.Add(rom);


        if (string.IsNullOrEmpty(rom.Clone_Of))
            continue;


        /*  if (rom.Clone_Of.Equals(selRom.Name))
              tmp.Add(rom);


          if (!string.IsNullOrEmpty(selRom.Clone_Of) && selRom.Clone_Of.Equals(rom.Clone_Of))
              tmp.Add(rom);*/

            /*
                    if (selRom.Clone_Of.Equals(rom.Name))
                        tmp.Add(rom);

                    if (rom.Clone_Of.Equals(selRom.Clone_Of))
                        tmp.Add(rom);
                }
            }*/
            //rawRomsSelected.AddRange(tmp);

            /* foreach (var rom in tmp)
                 Console.WriteLine($"{rom.ID} | {rom.Name}");

             rawRomsSelected = tmp;*/

            AsyncWindowProgress window;



            #region 2025/11/06 split pour async


            window = new AsyncWindowProgress();

            window.Total = rawRomsSelected.Count;
            window.Arguments = new List<object>() { rawRomsSelected };
            window.Message_Value = "Linking Roms";



            window.go += new AsyncWindowProgress.AsyncAction(Link2Roms);
            window.ShowDialog();

            rawRomsSelected = (List<RawMameRom>)window.Arguments[0];
            #endregion

            /*
            #region Ajout des manufacturers non présents
            //IEnumerable<string> manus = rawRomsSelected.Select(x => x.Manufacturer);
            MyObservableCollection<CT_Constructeur> manuToAdd = new MyObservableCollection<CT_Constructeur>();
            foreach(var rawrom in rawRomsSelected)
            {
                if (Constructeurs.FirstOrDefault(x => x.Nom == rawrom.Manufacturer) == null && manuToAdd.FirstOrDefault(x=> x.Nom == rawrom.Manufacturer)== null)
                    // Ajout à la liste des constructeurs à sauvegarder
                    manuToAdd.Add(new CT_Constructeur()
                    {
                        Nom = rawrom.Manufacturer,
                    });                    
            }

            SaveInDB.Insert_Manus(manuToAdd);

            // Mise à jour de la liste des constructeurs
            using (SQLite_Req sqReq = new SQLite_Req())
            {                            
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));
            
            #endregion
            }*/


            window = new AsyncWindowProgress();
            window.Arguments = new List<object>() { rawRomsSelected };
            window.Message_Value = "Moving Left to Right";
            window.go += new AsyncWindowProgress.AsyncAction(AsyncLeft2Right);
            //          
            window.ShowDialog();

            RomsToSave.SignalChange();
            RawRomsCollec.SignalChange();
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        private void Link2Roms(AsyncWindowProgress window)
        {
            List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.Arguments[0]; // ajouté en splittant vers de l'async

            // Make a list with only the clone of
            List<RawMameRom> cloneof_list = new List<RawMameRom>(RawRomsCollec.Where(x => !string.IsNullOrEmpty(x.Clone_Of)));
            List<RawMameRom> parent_list = new List<RawMameRom>(RawRomsCollec.Where(x => string.IsNullOrEmpty(x.Clone_Of)));

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


                // Cas d'un parent
                if (string.IsNullOrEmpty(selRom.Clone_Of))
                {
                    if (ParentChecked)
                    {
                        // on récupère tous les enfants
                        for (int k = 0; k < cloneof_list.Count; k++)
                        {
                            if (cloneof_list[k].Clone_Of == selRom.Name)
                            {
                                tmp.Add(cloneof_list[k]);
                                cloneof_list.RemoveAt(k);
                                k--;
                            }
                        }
                    }
                    Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (if),  temps: {sw1.ElapsedMilliseconds} ms");
                }
                else
                {
                    if (ParentChecked)
                    {
                        RawMameRom parent = null;
                        // on récupère tous les parents
                        for (int k = 0; k < parent_list.Count; k++)
                        {
                            if (parent_list[k].Name == selRom.Clone_Of)
                            {
                                parent = parent_list[k];

                                tmp.Add(parent_list[k]);

                                // si on lève, les autres roms enfants n'auront plus la possibilité de se lier. Mais nous n'avons pas de liaison ici à faire.
                                parent_list.RemoveAt(k);
                                //k--;
                                // normalement un seul parent.
                                break;
                            }
                        }

                        if (parent == null)
                        {
                            Console.WriteLine("la rom parent a probablement déjà été levée");
                        }
                    }


                    Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (else),  temps: {sw1.ElapsedMilliseconds} ms");
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


        /// <summary>
        /// Transformation Raw en CT
        /// </summary>
        /// <param name="window"></param>
        private void AsyncLeft2Right(AsyncWindowProgress window)
        {
            List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.Arguments[0];

            for (int i = 0; i < rawRomsSelected.Count; i++)
            {
                RawMameRom rawRom = rawRomsSelected[i];

                CT_Rom aRom = rawRom;
                /*new CT_Rom();
                    aRom.Archive_Name = rawRom.Name;
                    aRom.Description = rawRom.Description;
                    aRom.Aff_Clone_Of = rawRom.Clone_Of;
                    aRom.SourceFile = rawRom.Source_File;
                */
                aRom.Unwanted = false;
                if (string.IsNullOrEmpty(rawRom.Clone_Of))
                    aRom.IsParent = true;



                #region 
                // Transformation du Constructeur
                CT_Constructeur dev = Constructeurs.FirstOrDefault(x => x.Nom.Equals(rawRom.Manufacturer));

                // Le constructeur existe dans la table et correspond à celui affiché par la rawrom
                if (dev != null)
                {
                    //aRom.Manufacturer = dev.ID;
                    //aRom.Aff_Manufacturer = dev.Nom;
                    aRom.Manufacturer = dev;
                }
                // Le constructeur n'a jamais été entré
                else
                {
                    // rawRom.Manufacturer;
                    aRom.Manufacturer = new CT_Constructeur()
                    {
                        Nom = rawRom.Manufacturer,
                    };
                }
                #endregion


                RomsToSave.AddSilent(aRom);
                rawRomsDeleted.Add(rawRom);
                RawRomsCollec.RemoveSilent(rawRom);

                window.AsyncUpProgressPercent(i);
            }
        }


        #region Filtre de gauche

        private string LeftRomMode;

        private string _LeftFilter;
        public string LeftFilter
        {
            get { return _LeftFilter; }
            set
            {
                if (!value.Equals(_LeftFilter))
                {
                    _LeftFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private RawMameRom _S4L;
        public RawMameRom LeftSelected
        {
            get { return _S4L; }
            set
            {
                if (value != _S4L)
                {
                    _S4L = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private void LeftMode_Changed(object sender, RoutedEventArgs e)
        {
            LeftRomMode = ((RadioButton)sender).Content.ToString();
            if (LeftFilter != null)
                Select_Left();
        }

        private void Select_Left()
        {
            if (LeftRomMode == "Mode Game")
                LeftSelected = RawRomsCollec.FirstOrDefault(x => x.Description.StartsWith(LeftFilter, StringComparison.OrdinalIgnoreCase));
            else if (LeftRomMode == "Mode Archive")
                LeftSelected = RawRomsCollec.FirstOrDefault(x => x.Name.StartsWith(LeftFilter, StringComparison.OrdinalIgnoreCase));

            if (LeftSelected != null)
            {
                dg2Organize.ScrollIntoView(dg2Organize.SelectedItem);
            }
        }

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
            if (LeftFilter is null)
                LeftFilter = string.Empty;

            //
            if (e.Key == Key.Back)
                LeftFilter = LeftFilter.Length > 0 ? LeftFilter.Substring(0, LeftFilter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                LeftFilter = string.Empty;

            else
                LeftFilter += k;


            Select_Left();
        }
        #endregion

        #region Datagrid Droite

        private void Can_Right2Left(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsToSave.Count > 0;
        }

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
                        */
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

        private void Can_ResetRight(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsToSave.Count > 0;
        }

        private void Ex_ResetRight(object sender, ExecutedRoutedEventArgs e)
        {
            romsSelected = dgRight.SelectedItems.Cast<CT_Rom>().ToList();
            AsyncWindowProgress window = new AsyncWindowProgress();
            window.go += new AsyncWindowProgress.AsyncAction(AsyncResetRight);
            window.ShowDialog();

            RawRomsCollec.SignalChange();
            rawRomsDeleted.Clear();
            RomsToSave.Clear();
        }

        private void AsyncResetRight(AsyncWindowProgress window)
        {
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                CT_Rom sel = RomsToSave[i];
                for (int j = 0; j < rawRomsDeleted.Count; j++)
                {
                    RawMameRom deleted = rawRomsDeleted[j];
                    if (deleted.Name.Equals(sel.Archive_Name))
                    {
                        RawRomsCollec.AddSilent(deleted);

                        break;
                    }
                }

                window.AsyncUpProgressPercent(i);
            }
        }
        #endregion

        #region Filtre de Droite
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


        private void RightMode_Changed(object sender, RoutedEventArgs e)
        {
            LeftRomMode = ((RadioButton)sender).Content.ToString();
            if (LeftFilter != null)
                Select_Right();
        }


        private void Select_Right()
        {
            if (RightRomMode == "Mode Game")
                RightSelected = RomsToSave.FirstOrDefault(x => x.Description.StartsWith(RightFilter, StringComparison.OrdinalIgnoreCase));
            else if (LeftRomMode == "Mode Archive")
                RightSelected = RomsToSave.FirstOrDefault(x => x.Archive_Name.StartsWith(RightFilter, StringComparison.OrdinalIgnoreCase));

            if (RightSelected != null)
            {
                dgRight.ScrollIntoView(dgRight.SelectedItem);
            }
        }

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
            if (RightFilter is null)
                RightFilter = string.Empty;

            //
            if (e.Key == Key.Back)
                RightFilter = RightFilter.Length > 0 ? RightFilter.Substring(0, RightFilter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                RightFilter = string.Empty;

            else
                RightFilter += k;


            //Select_Right();
        }
        #endregion


        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException("Vérifier la compatibilité après le spit de transraw2rom");
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
        }

        #region Change
        private void Can_Change(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsToSave.Count > 0;
        }

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

        private void CbConstructeur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int idConstruct = Convert.ToInt32(cb.SelectedValue);

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                var objSel = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                objSel.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                objSel.AddOrders(new SqlOrder("Nom"));

                Machines.ChangeContent = sqReq.GetListOf(CT_Machine.Result2Class, objSel);
            }
        }




        #region Sauver/Updater dans la table Games
        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsToSave.Count > 0;
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
        private void SaveRoms(object sender, ExecutedRoutedEventArgs e)
        {
            #region Etat des lieux

            // Sauvegarde des manufactureurs manquant
            List<CT_Constructeur> manuToAdd = new List<CT_Constructeur>();

            // Sauvegarde des jeux manquants
            List<CT_Game> gameToAdd = new List<CT_Game>();

            //
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                var rom = RomsToSave[i];

                // Constructeurs
                //if (Constructeurs.FirstOrDefault(x => x.Nom == rom.Aff_Manufacturer) == null && manuToAdd.FirstOrDefault(x => x.Nom == rom.Aff_Manufacturer) == null)
                if (Constructeurs.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null && manuToAdd.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null)
                    // Ajout à la liste des constructeurs à sauvegarder
                    manuToAdd.Add(new CT_Constructeur()
                    {
                        //Nom = rom.Aff_Manufacturer,
                        Nom = rom.Manufacturer.Nom,
                    });

                // Games
                var posPar = rom.Description.IndexOf('(');
                var gameName = posPar > 0 ? rom.Description.Substring(0, posPar).Trim() : rom.Description;
                Debug.WriteLine(gameName);

                // Vérification pour éviter les doublons dans les gamesToAdd  + jeux en base (_GamesInDB). 
                if (gameToAdd.FirstOrDefault(x => x.Game_Name.Equals(gameName)) == null && _GamesInDB.FirstOrDefault(x => x.Game_Name.Equals(gameName)) == null)
                {
                    gameToAdd.Add(
                        new CT_Game
                        {
                            Game_Name = gameName,
                        });
                }
            }
            #endregion Etat des lieux



            #region Sauvegarde des constructeurs
            Debug.WriteLine("Ajout des constructeurs");
            if (manuToAdd.Count > 0)
            {
                if (MessageBox.Show("Would you want to save missing manufacturers. Refusing it, will stop all the process.", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Sauvegarde dans la base
                    SaveInDB.Insert_Manus(manuToAdd);


                    // Mise à jour de la liste des constructeurs
                    using (SQLite_Op sqReq = new SQLite_Op())
                    {
                        Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));
                    }
                }
                else
                {
                    return;
                }

            }
            #endregion Sauvegarde des constructeurs




            #region Games
            // Ajout de la collection.
            using (SQLite_Op sqOp = new SQLite_Op())
            {
                sqOp.Insert_CollecInGames(gameToAdd);
                _GamesInDB = sqOp.GetListOf(CT_Game.Result2Class, new Obj_Select(table: PProp.Default.T_Games, colonnes: new[] { "ID", "Game_Name" }, all: true));
            }
            #endregion



            Debug.WriteLine("Construction des liaisons");
            /* Construction des liaisons 
             *      On le fait aussi pour les games, puisqu'une fois sauvé on ne verra plus dans la liste ensuite */
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                // Liaison manufacturers
                CT_Rom rom = RomsToSave[i];
                if (rom.Manufacturer.ID == 0)
                {
                    /*
                    var tmp = Constructeurs.FirstOrDefault(x => x.Nom == rom.Aff_Manufacturer);
                    rom.Manufacturer = tmp.ID;
                    rom.Aff_Manufacturer = tmp.Nom;
                    */
                    var tmp = Constructeurs.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom);
                    rom.Manufacturer = tmp;

                }

                // Liaison des games                
                if (rom.Game.ID == 0)
                {
                    var posPar = rom.Description.IndexOf('(');
                    var gameName = posPar > 0 ? rom.Description.Substring(0, posPar).Trim() : rom.Description;

                    var tmp = _GamesInDB.FirstOrDefault(x => x.Game_Name.Equals(gameName));
                    rom.Game = tmp;
                }


            }


            // Sauvegarde des roms
            if (MessageBox.Show("Would you want to save this roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Sauvegarde des roms
                AsyncWindowProgress window = new AsyncWindowProgress();
                window.Arguments.Add(RomsToSave.ToList());
                window.go += new AsyncWindowProgress.AsyncAction(AsyncSaveRoms);
                window.ShowDialog();
            }
            RomsToSave.Clear();
        }







        /// <summary>
        /// Sauvegarde des roms dans la base de données
        /// </summary>
        /// <param name="window"></param>
        private void AsyncSaveRoms(AsyncWindowProgress window)
        {
            List<CT_Rom> parentsToSave = new List<CT_Rom>();
            List<CT_Rom> childrenToSave = new List<CT_Rom>();
            List<CT_Rom> romsTS = (List<CT_Rom>)window.Arguments[0];

            #region Ajout des manufacturers non présents
            //IEnumerable<string> manus = rawRomsSelected.Select(x => x.Manufacturer);
            // MyObservableCollection<CT_Constructeur> manuToAdd = new MyObservableCollection<CT_Constructeur>();

            /*foreach (var rom in romsTS)
            {
                if (Constructeurs.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null && manuToAdd.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null)
                    // Ajout à la liste des constructeurs à sauvegarder
                    manuToAdd.Add(new CT_Constructeur()
                    {
                        Nom = rom.Manufacturer.Nom,
                    });
            }*/

            #endregion


            foreach (CT_Rom rom in romsTS)
            {
                var id = Constructeurs.FirstOrDefault(x => x.Nom.Equals(rom.Manufacturer));

                // HAndler pour spliter les roms parents et les roms enfants
                if (rom.IsParent == true)
                    parentsToSave.Add(rom);
                else
                    childrenToSave.Add(rom);
            }

            // Sauvegarde des roms parents
            List<CT_Rom> sParentsRoms = null;
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));

                window.AsyncMessage("Insertion of Parent Roms");
                sqReq.Insert_Roms(parentsToSave, true);

                Obj_Select oSel = new Obj_Select(PProp.Default.T_Roms, all: true);
                oSel.AddConds(new SqlCond("IsParent", eWhere.Is, 1));

                sParentsRoms = sqReq.GetListOf<CT_Rom>(CT_Rom.Result2Class, oSel);
            }

            // Assignation de la rom parent
            foreach (CT_Rom child in childrenToSave)
            {
                CT_Rom parRom = sParentsRoms.First(x => x.Archive_Name.Equals(child.Aff_Clone_Of));
                child.Clone_Of = parRom.ID;
            }

            // Sauvegarde des roms enfants
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));
                window.AsyncMessage("Insertion of Children Roms");
                sqReq.Insert_Roms(childrenToSave, true);
            }
        }
        #endregion
    }
}
