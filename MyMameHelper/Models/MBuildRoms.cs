using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Models
{
    internal class MBuildRoms : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Archive_Mode => "Archive Mode";
        public string Description_Mode => "Description Mode";
        public string Source_Mode => "SourceFile Mode";

        #region Collections

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


        /// <summary>
        /// Collection des RawRoms
        /// </summary>
        /// <remarks>
        /// Quand elle est mise à jour, on indique que le contenu des raw rom filtrées a été modifié
        /// </remarks>
        private MyList<RawMameRom> _RawRomsCollec /*{ get; set; }*/ = new MyList<RawMameRom>();
        private MyList<RawMameRom> RawRomsCollec
        {
            get => _RawRomsCollec;
            set
            {
                if (value != _RawRomsCollec)
                {
                    _RawRomsCollec = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("RawRomsFiltered");
                }
            }
        }



        /// <summary>
        /// Roms Filtrées
        /// </summary>   
        /// <remarks>
        /// Utilisé par le data grid de gauche, puise dans la RawRomCollec
        /// </remarks>
        public MyObservableCollection<RawMameRom> RawRomsFiltered
        {

            get
            {
                var filteredRomsCollec = new MyObservableCollection<RawMameRom>();


                if (string.IsNullOrEmpty(LeftFilter) || string.IsNullOrEmpty(LeftRomMode))
                {
                    filteredRomsCollec.AddSilentRange(_RawRomsCollec);
                    return filteredRomsCollec;
                }

                string leftFilter= LeftFilter.ToUpper();


                if (LeftRomMode.Equals(Archive_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Description.ToUpper().StartsWith(leftFilter))
                            filteredRomsCollec.AddSilent(rom);
                }
                else if (LeftRomMode.Equals(Description_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Description.ToUpper().Contains(leftFilter))
                            filteredRomsCollec.AddSilent(rom);
                }
                else if (LeftRomMode.Equals(Source_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Source_File.ToUpper().Contains(leftFilter))
                            filteredRomsCollec.AddSilent(rom);
                }

                return filteredRomsCollec;

            }

        }



        /// <summary>
        /// Roms à sauvegarder en base
        /// </summary>
        public MyObservableCollection<CT_Rom> RomsToSave { get; set; } = new MyObservableCollection<CT_Rom>();


        /// <summary>
        /// Machines
        /// </summary>
        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();


        /// <summary>
        /// Manufacturers
        /// </summary>
        public MyObservableCollection<CT_MameManufacturer> Constructeurs { get; set; } = new MyObservableCollection<CT_MameManufacturer>();


        #endregion Collections


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


        #region Filtre de gauche

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

        /// <summary>
        /// Définit quel mode on souhaite parmis ceux spécifiés par les radiobuttons de gauche
        /// </summary>
        private string _LeftRomMode;
        internal string LeftRomMode
        {
            get => _LeftRomMode;
            set
            {
                if (_LeftRomMode != value)
                {
                    _LeftRomMode = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("RawRomsFiltered");

                }
            }
        }


        /// <summary>
        /// Filtre de gauche
        /// </summary>
        /// <remarks>
        /// Notify en temps réel que les roms filtrées peuvent être mise à jour
        /// </remarks>
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
                    NotifyPropertyChanged("RawRomsFiltered");
                }
            }
        }
        #endregion Filtre de gauche




        #region Chargement
        //private List<RawMameRom> ListRoms { get; set; }
        private List<RawMameRom> _RawRomsDeleted = new List<RawMameRom>();

        internal bool LoadCollecs()
        {

            // Chargement de la table des développeurs
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                //2025 levé Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Developers, all: true));
                // Machines
                var objSel = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                //objSel.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                objSel.AddOrders(new SqlOrder("Nom"));
                Machines.ChangeContent = sqReq.GetListOf(CT_Machine.Result2Class, objSel);

                //
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true));

                // Roms déjà construites
                _RomsInDb = sqReq.AffRoms_List();

                // Jeux dans la base
                _GamesInDB = sqReq.GetListOf(CT_Game.Result2Class, new Obj_Select(table: PProp.Default.T_Games, colonnes: new[] { "ID", "Game_Name" }, all: true));
            }

            // Chargement asynchrone des roms
            AsyncWorkList<RawMameRom> awl = new Models.AsyncWorkList<RawMameRom>();
            awl.go += new AsyncWorkList<RawMameRom>.AsyncListAction(AsyncLoadTempRoms);


            AsyncWindowProgressG aLoad = new AsyncWindowProgressG();
            aLoad.ProgressContext = awl;
            aLoad.ShowDialog();

            RawRomsCollec = /*RawRomsCollec.ChangeContent*/ new MyList<RawMameRom>(awl.Resultats);


            return true;
        }


        /// <summary>
        /// Chargement des roms temporaires
        /// </summary>
        /// <param name="aLoad"></param>
        private List<RawMameRom> AsyncLoadTempRoms(AsyncWindowProgressG aLoad)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<RawMameRom> listRMR;

            aLoad.AsyncMessage("Loading Roms...");
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                Obj_Select objSel = new Obj_Select(table: PProp.Default.T_TempRoms, all: true);

                /* SqlCond[] condBios = new SqlCond[] { new SqlCond("Is_Bios", eWhere.Equal, "True") };
                 objSel.Conditions = condBios;*/

                // Récupération de la liste des roms
                listRMR = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, objSel);
            }

            // On enlève ce qui est déjà présent dans les roms en base
            for (int i = 0; i < listRMR.Count; i++)
            {
                RawMameRom rawRom = listRMR[i];

                if (_RomsInDb.FirstOrDefault<CT_Rom>(x => x.Archive_Name.Equals(rawRom.Name)) != null)
                {
                    listRMR.Remove(rawRom);
                    i--;
                }
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
            return listRMR;
        }

        #endregion Chargement


        /// <summary>
        /// Transforme une rom temporaire en rom (avec les jonctions)
        /// </summary>
        /// <param name="rawRomsSelected"></param>
        internal void TransRaw2Rom(IList<RawMameRom> rawRomsSelected)
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





            #region 2025/11/06 split pour async
            AsyncWindowProgressG windowG = new AsyncWindowProgressG();
            windowG.Total = rawRomsSelected.Count;
            windowG.Message_Value = "Linking Roms";


            //test = false;
            AsyncWorkList<RawMameRom> mProgress = new AsyncWorkList<RawMameRom>();
            mProgress.Arguments = new List<object>() { rawRomsSelected };
            //mProgress.go += new AsyncWorkBool.AsyncBoolAction(Link2Roms);
            mProgress.go += new AsyncWorkList<RawMameRom>.AsyncListAction(Async_Link2Roms);

            windowG.ProgressContext = mProgress;
            windowG.ShowDialog();

            //test = (bool)windowG.Resultat;

            rawRomsSelected = (List<RawMameRom>)windowG.Resultat;

            /*
            window = new AsyncWindowProgress();

            window.Total = rawRomsSelected.Count;
            window.Arguments = new List<object>() { rawRomsSelected };
            window.Message_Value = "Linking Roms";



            window.go += new AsyncWindowProgress.AsyncAction(Link2Roms);
            window.ShowDialog();

            rawRomsSelected = (List<RawMameRom>)window.Arguments[0];*/


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
            AsyncWindowProgress window;

            window = new AsyncWindowProgress();
            window.Arguments = new List<object>() { rawRomsSelected };
            window.Message_Value = "Moving Left to Right";
            window.go += new AsyncWindowProgress.AsyncAction(AsyncLeft2Right);
            //          
            window.ShowDialog();

            RomsToSave.SignalChange();
            RawRomsFiltered.SignalChange();
        }




        /// <summary>
        /// Lie les roms avec divers éléments
        /// </summary>
        /// <param name="window"></param>
        private List<RawMameRom> Async_Link2Roms(AsyncWindowProgressG window)
        {


            List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.ProgressContext.Arguments[0]; // ajouté en splittant vers de l'async

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
                //Stopwatch sw1 = new Stopwatch();
                //sw1.Start();


                // Cas d'un parent
                if (string.IsNullOrEmpty(selRom.Clone_Of))
                {
                    if (ParentChecked = true)
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
                    //Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (if),  temps: {sw1.ElapsedMilliseconds} ms");
                }
                else
                {
                    if (ParentChecked = true)
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


                    //Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (else),  temps: {sw1.ElapsedMilliseconds} ms");
                }
                // Debug.WriteLine($"Fin pour {selRom.Name},  temps: {sw1.ElapsedMilliseconds} ms");
                //sw1.Stop();

                // lié au passage asynchrone
                window.AsyncUpProgressPercent(i);
                i++;
            }

            // window.ProgressContext.Arguments[0] = tmp;

            Debug.WriteLine($"Fin Total,  temps: {swTotal.ElapsedMilliseconds} ms");
            swTotal.Stop();

            // 28//11/2025
            return tmp;
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
                CT_MameManufacturer dev = Constructeurs.FirstOrDefault(x => x.Nom.Equals(rawRom.Manufacturer));

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
                    aRom.Manufacturer = new CT_MameManufacturer()
                    {
                        Nom = rawRom.Manufacturer,
                    };
                }
                #endregion

                // Liaison des Machines
                if (aRom.Machine_Id == 0 || aRom.Machine_Id == null)
                {
                    var search = aRom.SourceFile;//.Replace("/", " - ");
                    //search = search.Remove(search.IndexOf('.'));
                    var tmp = Machines.FirstOrDefault(x => x.Nom.Equals(search));
                    if (tmp != null)
                        aRom.Machine_Id = tmp.ID;
                }

                // Détermination si pinball
                if (rawRom.Is_Mechanical && rawRom.Source_File.StartsWith("pinball"))
                    aRom.IsPinball = true;



                RomsToSave.AddSilent(aRom);
                _RawRomsDeleted.Add(rawRom);
                RawRomsCollec.RemoveAll(x => x == rawRom);



                window.AsyncUpProgressPercent(i);
            }
        }


        internal bool ResetAll()
        {
            AsyncWorkList<RawMameRom> aWList = new AsyncWorkList<RawMameRom>();
            aWList.go += new AsyncWorkList<RawMameRom>.AsyncListAction(AsyncResetRight);

            AsyncWindowProgressG window = new AsyncWindowProgressG();
            window.ProgressContext = aWList;

            window.ShowDialog();

            RawRomsCollec.AddRange( aWList.Resultats);
            _RawRomsDeleted.Clear();
            RomsToSave.Clear();

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private List<RawMameRom> AsyncResetRight(AsyncWindowProgressG window)
        {
            var romCollec = new List<RawMameRom>();
            window.Total = RomsToSave.Count;

            for (int i = 0; i < RomsToSave.Count; i++)
            {
                CT_Rom sel = RomsToSave[i];
                for (int j = 0; j < _RawRomsDeleted.Count; j++)
                {
                    RawMameRom deleted = _RawRomsDeleted[j];
                    if (deleted.Name.Equals(sel.Archive_Name))
                    {
                        romCollec.Add(deleted);

                        break;
                    }
                }

                window.AsyncUpProgressPercent(i);
            }

            return romCollec;
        }


        #region Sauvegarde
        internal bool SaveRoms()
        {
            #region Etat des lieux



            // Sauvegarde des jeux manquants
            List<CT_Game> gameToAdd = new List<CT_Game>();

            //
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                var rom = RomsToSave[i];


                // Games
                var posPar = rom.SourceFile.IndexOf('(');
                var gameName = posPar > 0 ? rom.SourceFile.Substring(0, posPar).Trim() : rom.SourceFile;
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





            #region Games
            // Ajout de la collection.
            using (SQLite_OP sqOp = new SQLite_OP())
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
                CT_Rom rom = RomsToSave[i];

                // Liaison manufacturers
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
                if (rom.Game_Id == 0 || rom.Game_Id == null)
                {
                    var posPar = rom.SourceFile.IndexOf('(');
                    var gameName = posPar > 0 ? rom.SourceFile.Substring(0, posPar).Trim() : rom.SourceFile;

                    var tmp = _GamesInDB.FirstOrDefault(x => x.Game_Name.Equals(gameName));
                    rom.Game_Id = tmp.ID;
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

            return true;
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
            using (SQLite_OP sqReq = new SQLite_OP())
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
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));
                window.AsyncMessage("Insertion of Children Roms");
                sqReq.Insert_Roms(childrenToSave, true);
            }
        }


        #region Sauvegarde des constructeurs
        internal void SaveManufacturers()
        {
            Debug.WriteLine("Vérification des constructeurs manquants");

            // Sauvegarde des manufactureurs manquant
            List<CT_MameManufacturer> manuToAdd = new List<CT_MameManufacturer>();
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                var rom = RomsToSave[i];

                if (Constructeurs.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null && manuToAdd.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null)
                    // Ajout à la liste des constructeurs à sauvegarder
                    manuToAdd.Add(new CT_MameManufacturer()
                    {
                        //Nom = rom.Aff_Manufacturer,
                        Nom = rom.Manufacturer.Nom,
                    });
            }

            if (manuToAdd.Count > 0)
            {

                // Sauvegarde dans la base
                SaveInDB.Insert_Manus(manuToAdd);


                // Mise à jour de la liste des constructeurs
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    Constructeurs.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true));
                }
            }

        }

    }
    #endregion Sauvegarde des constructeurs
    #endregion  Sauvegarde

}

