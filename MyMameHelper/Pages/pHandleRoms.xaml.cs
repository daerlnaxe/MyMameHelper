using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logique d'interaction pour pHandleRoms.xaml
    /// </summary>
    public partial class pHandleRoms : Page
    {
        public MyObservableCollection<CT_Bios> BiosCollec { get; set; } = new MyObservableCollection<CT_Bios>();
        public MyObservableCollection<Aff_Game> GamesCollec { get; set; } = new MyObservableCollection<Aff_Game>();
        public MyObservableCollection<CT_Mechanical> MecanicsCollec { get; set; } = new MyObservableCollection<CT_Mechanical>();
        public MyObservableCollection<RawMameRom> RawRomsCollec { get; set; } = new MyObservableCollection<RawMameRom>();



        public pHandleRoms()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            AsyncWindowProgress aLoad = new AsyncWindowProgress();
            aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncProceed);
            aLoad.ShowDialog();
        }

        private void AsyncProceed(AsyncWindowProgress aLoad)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<RawMameRom> rawRoms = null;
            List<CT_Rom> savedRoms;
            List<CT_Bios> BiosList;
            List<Aff_Game> GamesList;                               // Est utilisée pour les jeux déjà sauvés
            List<Aff_Game> GamesToSave = new List<Aff_Game>();       // Est utilisée pour les jeux à sauver
            List<CT_Mechanical> MecasList;


            using (SQLite_Req sqReq = new SQLite_Req())
            {
                // Chargement des roms de temp
                aLoad.AsyncMessage("Loading Temp Roms...");
                Obj_Select objSel = new Obj_Select(table: PProp.Default.T_TempRoms, all: true);
                rawRoms = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, objSel);

                // Chargement des roms déjà sauvegardées (on ne travaille que sur elles)
                aLoad.AsyncMessage("Loading Saved Roms...");
                savedRoms = sqReq.AffRoms_List();
                //savedRoms = sqReq.GetListOf<Aff_Rom>(Aff_Rom.Result2Class, new Obj_Select(PProp.Default.T_Roms, all: true));

                // Chargement des bios sauvegardées
                aLoad.AsyncMessage("Loading Saved bios...");
                BiosList = sqReq.GetListOf(CT_Bios.Result2Class, new Obj_Select(PProp.Default.T_Bios, all: true));

                // Chargement des jeux sauvegardées
                aLoad.AsyncMessage("Loading Saved Games...");
                GamesList = sqReq.GetListOf<Aff_Game>(Aff_Game.Result2Class, new Obj_Select(PProp.Default.T_Games, all: true));

                // Chargement des mecanics sauvegardées
                aLoad.AsyncMessage("Loading Saved Mecanics...");
                MecasList = sqReq.GetListOf(CT_Mechanical.Result2Class, new Obj_Select(PProp.Default.T_Mechanics, all: true));

            }

            Console.WriteLine(sw.ElapsedMilliseconds);

            // Filtre les rawroms en ne gardant que celles déjà sauvées
            aLoad.AsyncMessage("Filtering raw by saved roms");
            for (int i = 0; i < rawRoms.Count; i++)
            {
                RawMameRom rom = rawRoms[i];

                // Vérification de la présence dans les roms sauvegardées
                var sRom = savedRoms.FirstOrDefault(x => x.Archive_Name == rom.Name);

                if (sRom != null)
                    continue;

                rawRoms.RemoveAt(i);
                i--;
            }

            aLoad.AsyncMessage("Handle Parents roms");


            // Gestion des parents
            for (int i = 0; i < rawRoms.Count; i++)
            {
                RawMameRom rawRom = rawRoms[i];

                // On enlève ceux qui ne sont pas parents
                if (!string.IsNullOrEmpty(rawRom.Clone_Of))
                    continue;

                // On récupère la rom parent
                CT_Rom romParent = savedRoms.FirstOrDefault(x => x.Archive_Name == rawRom.Name);
                romParent.SourceFile = rawRom.Source_File;
                romParent.Aff_Manufacturer = rawRom.Manufacturer;


                // Travail sur les bios
                if (rawRom.Is_Bios)
                {
                    CT_Bios bios = BiosList.FirstOrDefault(x => x.Bios_Name.Equals(rawRom.Name));
                    if (bios == null)
                    {
                        bios = Make_Bios(rawRom);
                        BiosList.Add(bios);
                    }

                    // Si la rom n'est pas déjà ajoutée on ajoute
                    if (bios.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romParent.Archive_Name)) == null)
                        bios.Roms.Add(romParent);
                }
                // Travail sur les mécanichals
                else if (rawRom.Is_Mechanical)
                {
                    CT_Mechanical meca = MecasList.FirstOrDefault(x => x.Meca_Name.Equals(rawRom.Name));
                    if (meca == null)
                    {
                        meca = Make_Mecanical(rawRom);
                        MecasList.Add(meca);
                    }

                    // Si la rom n'est pas déjà ajoutée on ajoute
                    if (meca.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romParent.Archive_Name)) == null)
                        meca.Roms.Add(romParent);
                }
                // Travail sur les jeux
                else
                {
                    Aff_Game game = GamesList.FirstOrDefault(x => x.Game_Name.Equals(rawRom.Name));
                    if (game == null)
                    {
                        game = Make_Games(rawRom);
                        GamesToSave.Add(game);
                    }

                    // Si la rom n'est pas déjà ajoutée on ajoute
                    if (game.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romParent.Archive_Name)) == null)
                        game.Roms.Add(romParent);
                }
            }

            aLoad.AsyncMessage("Handle Children roms");

            RawMameRom cause = null;
            string problem = string.Empty;
            try
            {
                // Gestion des enfants ? 
                for (int i = 0; i < rawRoms.Count; i++)
                {

                    RawMameRom rawRom = rawRoms[i];
                    cause = rawRom;


                    // On enlève ceux sont parents ?
                    if (string.IsNullOrEmpty(rawRom.Clone_Of))
                        continue;

                    // On récupère la rom enfant qui correspond
                    CT_Rom romChild = savedRoms.FirstOrDefault(x => x.Archive_Name == rawRom.Name);
                    romChild.SourceFile = rawRom.Source_File;
                    romChild.Aff_Manufacturer = rawRom.Manufacturer;

                    // Travail sur les bios
                    if (rawRom.Is_Bios)
                    {
                        CT_Bios bios = BiosList.FirstOrDefault(x => x.Roms[0].ID == romChild.Clone_Of);

                        // Si la rom n'est pas déjà ajoutée on ajoute
                        if (bios.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romChild.Archive_Name)) == null)
                            bios.Roms.Add(romChild);
                    }
                    // Travail sur les mécanichals
                    else if (rawRom.Is_Mechanical)
                    {
                        CT_Mechanical meca = MecasList.FirstOrDefault(x => x.Roms[0].ID == romChild.Clone_Of);

                        // Si la rom n'est pas déjà ajoutée on ajoute
                        if (meca.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romChild.Archive_Name)) == null)
                            meca.Roms.Add(romChild);
                    }
                    // Travail sur les jeux
                    else
                    {
                        CT_Game game = null;

                        // Test si le parent est  déjà dans les jeux sauvés
                        if (game == null)
                            game = FindInGames.GameByRoms(GamesList, x => x.ID == romChild.Clone_Of );
                           // GamesList.FirstOrDefault(x => x.Roms[0].ID == romChild.Clone_Of);

                        // Test si le parent est dans la liste des jeux à sauver
                        if (game==null)
                            game = GamesToSave.FirstOrDefault(x => x.Roms[0].ID == romChild.Clone_Of);

                        // Dans le cas ou le jeu n'aurait pas été ajouté par la boucle des parents
                        if (game == null)
                        {
                            problem += romChild.Archive_Name + Environment.NewLine;
                            continue;
                        }

                        // Si la rom n'est pas déjà ajoutée on ajoute
                        if (game.Roms.FirstOrDefault(x => x.Archive_Name.Equals(romChild.Archive_Name)) == null)
                            game.Roms.Add(romChild);


                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                Console.WriteLine($"Cause = {cause.ID} , {cause.Name} ");
            }

            if (!string.IsNullOrEmpty(problem))
                MessageBox.Show("Problem on following roms:" + Environment.NewLine + problem);

            aLoad.AsyncMessage("Handling  Roms...");

            Console.WriteLine(sw.ElapsedMilliseconds);
            aLoad.AsyncMessage("Load Bios...");
            BiosCollec.ChangeContent = BiosList;
            aLoad.AsyncMessage("Load Games...");
            GamesCollec.ChangeContent = GamesToSave;
            aLoad.AsyncMessage("Load Mecas...");
            MecanicsCollec.ChangeContent = MecasList;

            Console.WriteLine(sw.ElapsedMilliseconds);
            aLoad.AsyncMessage("Empty roms...");
            RawRomsCollec.Clear();

            //   GamesCollec.ChangeContent = games;
            Console.WriteLine(sw.ElapsedMilliseconds);
            // Récupération des roms de type bios


        }

        private CT_Bios Make_Bios(RawMameRom rom)
        {
            CT_Bios ctB = new CT_Bios();
            ctB.Bios_Name = rom.Name;
            ctB.Description = rom.Description;

            return ctB;
        }

        private Aff_Game Make_Games(RawMameRom rom)
        {
            Aff_Game ctG = new Aff_Game();
            ctG.Game_Name = rom.Name;
            ctG.Description = rom.Description;

            return ctG;
        }

        private CT_Mechanical Make_Mecanical(RawMameRom rom)
        {
            CT_Mechanical ctM = new CT_Mechanical();
            ctM.Meca_Name = rom.Name;
            ctM.Description = rom.Description;

            return ctM;
        }

        /*
        private CT_Rom Get_RomInfos(RawMameRom rom)
        {
            CT_Rom romRet = new CT_Rom();
            romRet.Archive_Name = rom.Name;
            romRet.Description = rom.Description;
            // romRet.Aff_Manufacturer = rom.Manufacturer;

            return romRet;
        }*/

        #region Bios
        private void Can_BiosPreview(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = BiosCollec.Count > 0;
        }

        private void Ex_BiosPreview(object sender, ExecutedRoutedEventArgs e)
        {
            wPrevBios windowBios = new wPrevBios();
            windowBios.Bios = BiosCollec;
            windowBios.Show();
        }

        #endregion

        #region Games
        private void Can_GamesPreview(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesCollec.Count > 0;
        }
        private void Ex_GamesPreview(object sender, ExecutedRoutedEventArgs e)
        {
            wPrevGames windowGames = new wPrevGames();
            windowGames.Games = GamesCollec;
            windowGames.Show();
        }
        #endregion

        #region Meca
        private void Can_MecaPreview(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MecanicsCollec.Count > 0;

        }

        private void Ex_MecaPreview(object sender, ExecutedRoutedEventArgs e)
        {
            wPrevMeca windowMeca = new wPrevMeca();
            windowMeca.Mecas = MecanicsCollec;
            windowMeca.Show();
        }
        #endregion

        #region save
        public static readonly RoutedUICommand Save_All = new RoutedUICommand("Save All Categories", "Save_All", typeof(pHandleRoms));
        public static readonly RoutedUICommand PreSave_Bios = new RoutedUICommand("PreSave Bios", "PreSave_Bios", typeof(pHandleRoms));
        public static readonly RoutedUICommand PreSave_Games = new RoutedUICommand("PreSave Games", "PreSave_Games", typeof(pHandleRoms));
        public static readonly RoutedUICommand PreSave_Mechanics = new RoutedUICommand("PreSave Mechanics", "PreSave_Mechanics", typeof(pHandleRoms));
        //public static readonly RoutedUICommand Fill_Manufacts = new RoutedUICommand("Fill Manufacturers", "Fill_Manufacts", typeof(pHandleRoms));

        private void Can_SaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesCollec.Count > 0 && BiosCollec.Count > 0 && MecanicsCollec.Count > 0;
        }

        private void Ex_SaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Save everything to Temp Db ?", "Save to Db", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                //     progressW.DoWork += new ProgressWindow.DoWorkEventHandler(SaveAllTemp_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();
            }
        }

        private void SaveBios_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            object myArgument = e.Argument;

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));

                sqReq.Insert_BiosInTemp(BiosCollec);
            }
        }
        /*
                private void SaveBios_DoWork(ProgressWindow sender, DoWorkEventArgs e)
                {
                    object myArgument = e.Argument;

                    using (SQLite_Req sqReq = new SQLite_Req())
                    {
                        sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));

                        sqReq.Insert_GamesInTemp(GamesCollec);
                    }
                }



                private void SaveBios_DoWork(ProgressWindow sender, DoWorkEventArgs e)
                {
                    object myArgument = e.Argument;

                    using (SQLite_Req sqReq = new SQLite_Req())
                    {
                        sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));

                        sqReq.Insert_MecasInTemp(GamesCollec);

                    }
                }
                */
        private void Can_SaveBios(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = BiosCollec.Count > 0;
        }

        private void Ex_SaveBios(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Can_SaveGames(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesCollec.Count > 0;
        }

        private void Ex_SaveGames(object sender, ExecutedRoutedEventArgs e)
        {
            pBuildGames pbGames = new pBuildGames();
            pbGames.GamesToOrganize = GamesCollec;
            NavigationService.Navigate(pbGames);
        }

        private void Can_SaveMecas(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MecanicsCollec.Count > 0;
        }

        private void Ex_SaveMecas(object sender, ExecutedRoutedEventArgs e)
        {

        }

        /*
        private void Can_Manufacts(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GamesCollec.Count > 0;
        }

        private void Ex_Manufacts(object sender, ExecutedRoutedEventArgs e)
        {
            List<CT_Constructeur> previousM = null;
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                Obj_Select selConst = new Obj_Select(PProp.Default.T_Companies, all: true, orders: new SqlOrder("Nom"));
                previousM = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, selConst);

                foreach (CT_Game game in GamesCollec)
                {
                    foreach (CT_Rom rom in game.Roms)
                    {
                        if (rom.Manufacturer == null)
                            continue;

                        CT_Constructeur present = previousM.FirstOrDefault(x => x.Nom.Equals(rom.Manufacturer));
                        if (present != null)
                            continue;


                        // Recherche en base
                        SqlCond condManu = new SqlCond(colonne: "Nom", eWhere.Like, rom.Manufacturer);
                        CT_Constructeur manu = sqReq.Get_Companie(new SqlCond[] { condManu });
                        if (manu != null)
                            continue;

                        // Ajout à la base
                   //     bool res = sqReq.Insert_Companie(new CT_Constructeur(rom.Manufacturer));

                        if (!res)
                            return;

                    }
                }
            }
        }*/
        #endregion

        private void Ex_FlushTemp(object sender, ExecutedRoutedEventArgs e)
        {
            bool res = false;
            if (System.Windows.MessageBox.Show("Reset The Table Containing Temporary Roms ?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (SQLite_Req sqReq = new SQLite_Req())
                    res = sqReq.Flush_TempRoms();
            }

            if (res)
                System.Windows.MessageBox.Show("Table Flushed");
        }


    }
}
