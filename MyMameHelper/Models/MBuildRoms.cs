using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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


        #region Radio Buttons
        public string Archive_Mode => "Archive Mode";
        public string Description_Mode => "Description Mode";
        public string Source_Mode => "SourceFile Mode";


        /// <summary>
        /// RadioBouton de gauche pour Archive
        /// </summary>
        private bool _LeftRbArchive { get; set; }
        public bool LeftRbArchive
        {
            get => _LeftRbArchive;
            set
            {
                _LeftRbArchive = value;
                NotifyPropertyChanged();

                if (value == true)
                {
                    LeftRbDescription = false;
                    LeftRbSrcFile = false;


                    LeftRomMode = Archive_Mode;
                }
            }
        }

        /// <summary>
        /// RadioBouton de gauche pour Description
        /// </summary>
        private bool _LeftRbDescription { get; set; }
        public bool LeftRbDescription
        {
            get => _LeftRbDescription;
            set
            {
                _LeftRbDescription = value;
                NotifyPropertyChanged();

                if (value == true)
                {
                    LeftRomMode = Description_Mode;

                    LeftRbArchive = false;
                    LeftRbSrcFile = false;
                }
            }
        }

        /// <summary>
        /// RadioBouton de gauche pour Sourcefile
        /// </summary>
        private bool _LeftRbSrcFile { get; set; }
        public bool LeftRbSrcFile
        {
            get => _LeftRbSrcFile;
            set
            {
                _LeftRbSrcFile = value;
                NotifyPropertyChanged();


                if (value == true)
                {
                    LeftRomMode = Source_Mode;

                    LeftRbArchive = false;
                    LeftRbDescription = false;
                }
            }
        }
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
        private string LeftRomMode
        {
            get => _LeftRomMode;
            set
            {
                if (_LeftRomMode != value)
                {
                    _LeftRomMode = value;
                    //NotifyPropertyChanged();
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


        /*
        internal void getFilteredLeft()
        {
            if (LeftRomMode == "Mode Game")
                LeftSelected = RawRomsFiltered.FirstOrDefault(x => x.Description.StartsWith(LeftFilter, StringComparison.OrdinalIgnoreCase));
            else if (LeftRomMode == "Archive Select")
                LeftSelected = RawRomsFiltered.FirstOrDefault(x => x.Name.StartsWith(LeftFilter, StringComparison.OrdinalIgnoreCase));

        }*/
        #endregion Filtre de gauche






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
        public MyList<RawMameRom> RawRomsFiltered
        {

            get
            {
                var filteredRomsCollec = new MyList<RawMameRom>();


                // Sans filtre on renvoie cash
                if (string.IsNullOrEmpty(LeftFilter) || string.IsNullOrEmpty(LeftRomMode))
                {
                    //filteredRomsCollec.AddSilentRange(_RawRomsCollec);
                    //filteredRomsCollec.AddRange(_RawRomsCollec);
                    return _RawRomsCollec;
                }

                string leftFilter = LeftFilter.ToUpper();


                if (LeftRomMode.Equals(Archive_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Description.ToUpper().StartsWith(leftFilter))
                            filteredRomsCollec.Add(rom);
                }
                else if (LeftRomMode.Equals(Description_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Description.ToUpper().Contains(leftFilter))
                            filteredRomsCollec.Add(rom);
                }
                else if (LeftRomMode.Equals(Source_Mode))
                {
                    foreach (RawMameRom rom in _RawRomsCollec)
                        if (rom.Source_File.ToUpper().Contains(leftFilter))
                            filteredRomsCollec.Add(rom);
                }

                return filteredRomsCollec;

            }

        }



        /// <summary>
        /// Roms à sauvegarder en base
        /// </summary>
        private MyList<CT_Rom> _RomsToSave = new MyList<CT_Rom>();
        public MyList<CT_Rom> RomsToSave
        {
            get => _RomsToSave;
            set
            {
                if (_RomsToSave != value)
                {
                    _RomsToSave = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Machines
        /// </summary>
        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();


        /// <summary>
        /// Manufacturers
        /// </summary>
        public MyObservableCollection<CT_MameManufacturer> MameManufacturers { get; set; } = new MyObservableCollection<CT_MameManufacturer>();


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




        #region Chargement
        //private List<RawMameRom> ListRoms { get; set; }
        private MyList<RawMameRom> _RawRomsDeleted = new MyList<RawMameRom>();

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
                MameManufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true));

                // Roms déjà construites
                _RomsInDb = sqReq.AffRoms_List();

                // Jeux dans la base
                _GamesInDB = sqReq.GetListOf(CT_Game.Result2Class, new Obj_Select(table: PProp.Default.T_Games, fields: new[] { "ID", "Game_Name" }, all: true));
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


            // Link des roms
            AsyncWorkList<RawMameRom> mProgress;

            mProgress = new AsyncWorkList<RawMameRom>();
            mProgress.Arguments = new List<object>() { rawRomsSelected };
            //mProgress.go += new AsyncWorkBool.AsyncBoolAction(Link2Roms);
            mProgress.go += new AsyncWorkList<RawMameRom>.AsyncListAction(Async_Link2Roms);

            AsyncWindowProgressG windowG = new AsyncWindowProgressG();
            windowG.Total = rawRomsSelected.Count;
            windowG.Message_Value = "Linking Roms";
            windowG.ProgressContext = mProgress;
            windowG.ShowDialog();

            //test = (bool)windowG.Resultat;

            rawRomsSelected = (List<RawMameRom>)windowG.Resultat;
            mProgress = null;
            windowG = null;

            // 9s pour la totalité + barre de progression (réduit à 5s en utilisant l'update sur %), réduction à 2.173s par optimisation du code.

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

            // Déplacement
            AsyncWorkList<CT_Rom> mProgress2 = new AsyncWorkList<CT_Rom>();
            mProgress2.Arguments = new List<object>() { rawRomsSelected };

            mProgress2.go += new AsyncWorkList<CT_Rom>.AsyncListAction(AsyncLeft2Right);
            //          
            windowG = new AsyncWindowProgressG();
            windowG.Total = rawRomsSelected.Count;
            windowG.Message_Value = "Moving Left to Right";
            windowG.ProgressContext = mProgress2;
            windowG.ShowDialog();  // <=  4.7s  avec barre de progression, 1.4s avec réduction UI update au % 

            List<CT_Rom> aRoms = mProgress2.Resultats;

            // On ajoute dans un stockage temporaire
            _RawRomsDeleted.AddRange(rawRomsSelected);  // <= 3ms

            #region suppression à gauche <= 34 ms au lieu de 13s
            // Utilisation d'un hashset pour gagner en rapidité
            var archives = new HashSet<string>(
                aRoms.Select(r => r.Archive_Name)
            );

            RawRomsCollec.RemoveAll(r => archives.Contains(r.Name));

            RawRomsCollec = new MyList<RawMameRom>(RawRomsCollec);
            #endregion


            // Assignation à droite
            RomsToSave.AddRange(aRoms);
            RomsToSave.Sort((a, b) => string.Compare(a.Archive_Name, b.Archive_Name, StringComparison.CurrentCulture));

            //
            RomsToSave = new MyList<CT_Rom>(RomsToSave);
            //RomsToSave.SignalChange();
            //RawRomsFiltered.SignalChange();

        }




        /// <summary>
        /// Lie les roms avec divers éléments
        /// </summary>
        /// <param name="window"></param>
        private List<RawMameRom> Async_Link2Roms(AsyncWindowProgressG window)
        {

            List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.ProgressContext.Arguments[0]; // ajouté en splittant vers de l'async

            // Split des roms splittées entre parents et clones
            List<RawMameRom> parentSlctd = rawRomsSelected.Where(x => string.IsNullOrEmpty(x.Clone_Of)).ToList();
            List<RawMameRom> cloneSlctd = rawRomsSelected.Where(x => !string.IsNullOrEmpty(x.Clone_Of)).ToList();


            //rah
            List<RawMameRom> tmp = new List<RawMameRom>();

            // Vérification
            if (parentSlctd.Count() != 0)
            {

                // Liste de tous les clones existants
                List<RawMameRom> cloneofAll = new List<RawMameRom>(RawRomsCollec.Where(x => !string.IsNullOrEmpty(x.Clone_Of)));

                //
                window.Message_Value = "Link des roms: Ajout des parents et enfants";
                window.Total = parentSlctd.Count;
                window.Progress_Value = 0;

                // Parcours des roms parents sélectionnées
                for (int i = 0; i < parentSlctd.Count; i++)
                {
                    RawMameRom selRom = parentSlctd[i];

                    // Ajout de la rom parent
                    tmp.Add(selRom);

                    // pas encore actif
                    if (ParentChecked = true)
                    {
                        // on récupère tous les enfants manquants
                        for (int k = 0; k < cloneofAll.Count; k++)
                        {
                            if (cloneofAll[k].Clone_Of == selRom.Name)
                            {
                                tmp.Add(cloneofAll[k]);

                                // On elimite des roms sélectionnées
                                cloneSlctd.Remove(cloneofAll[k]);

                                // Réduction du temps de traitement
                                cloneofAll.RemoveAt(k);
                                k--;
                            }
                        }
                    }
                    window.AsyncUpProgressPercent(i);
                }
                window.AsyncUpProgressPercent(parentSlctd.Count);
            }


            // Récupération des parents si nécessaire
            if (cloneSlctd.Count() != 0)
            {
                window.Message_Value = "Link des roms: Ajout des enfants et du parent lié";
                window.Total = cloneSlctd.Count;
                window.Progress_Value = 0;


                List<RawMameRom> parentAll = new List<RawMameRom>(RawRomsCollec.Where(x => string.IsNullOrEmpty(x.Clone_Of)));

                for (int i = 0; i < cloneSlctd.Count; i++)
                {
                    RawMameRom selRom = cloneSlctd[i];

                    tmp.Add(selRom);

                    // Vérification que le parent n'est pas déjà présent
                    if (tmp.FirstOrDefault(x => selRom.Clone_Of.Equals(x.Name)) != null)
                        continue;

                    // Récupération du parent
                    var parent = parentAll.FirstOrDefault(x => selRom.Clone_Of.Equals(x.Name));

                    //
                    if (parent == null)
                        continue;

                    // Ajout du parent à tmp
                    tmp.Add(parent);

                    // On lève le parent de la liste
                    parentAll.Remove(parent);

                    window.AsyncUpProgressPercent(i);

                }
            }


            Stopwatch swTotal = new Stopwatch();
            swTotal.Start();



            // On récupère les clones pour les parents qu'on ajoute à la clone list si non existant

            /*
            foreach (RawMameRom selRom in parent_list)
            {
                // pas encore actif
                if (ParentChecked = true)
                {
                    // on récupère tous les enfants manquants
                    for (int k = 0; k < cloneof_list.Count; k++)
                    {
                        if (cloneof_list[k].Clone_Of != selRom.Name)
                        {
                            //tmp.Add(cloneof_list[k]);
                            cloneof_list.Add(k);
                            //k--;
                        }
                    }
                }
                //Debug.WriteLine($"Ajouts pour {selRom.Name} après récupérations des enfants (if),  temps: {sw1.ElapsedMilliseconds} ms");
            }
            */
            /*
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
                            // Console.WriteLine("la rom parent a probablement déjà été levée"); Consommateur
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
            */

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
        private MyList<CT_Rom> AsyncLeft2Right(AsyncWindowProgressG window)
        {
            List<RawMameRom> rawRomsSelected = (List<RawMameRom>)window.ProgressContext.Arguments[0];
            MyList<CT_Rom> result = new MyList<CT_Rom>();

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
                CT_MameManufacturer dev = null;
                // Transformation du Constructeur
                foreach (var mManuF in MameManufacturers)
                {
                    if (string.IsNullOrEmpty(mManuF.Nom))
                    {
                        Debug.WriteLine($">>>>> Manufacturer n'ayant pas de nom: {mManuF.ID}");
                        continue;
                    }

                    if (mManuF.Nom.Equals(rawRom.Manufacturer))
                    {
                        dev = mManuF;
                        break;
                    }
                }


                //CT_MameManufacturer dev = MameManufacturers.FirstOrDefault(x => x.Nom.Equals(rawRom.Manufacturer));

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



                result.Add(aRom);



                window.AsyncUpProgressPercent(i);
            }

            return result;
        }

        /// <summary>
        /// Enlève tout à droite, et rajoute à gauche
        /// </summary>
        /// <returns></returns>
        internal bool ResetFromRight()
        {
            //
            RawRomsCollec.AddRange(_RawRomsDeleted);
            RawRomsCollec.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));
            RawRomsCollec = new MyList<RawMameRom>(RawRomsCollec);


            _RawRomsDeleted.Clear();
            RomsToSave = new MyList<CT_Rom>();

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool RemoveFromRight(List<CT_Rom> romsSelected)
        {
            AsyncWorkList<RawMameRom> aWList = new AsyncWorkList<RawMameRom>();
            aWList.Arguments = new List<object>() { romsSelected, _RawRomsDeleted };
            aWList.go += new AsyncWorkList<RawMameRom>.AsyncListAction(AsyncRemoveRight);

            AsyncWindowProgressG window = new AsyncWindowProgressG();
            window.ProgressContext = aWList;

            window.ShowDialog(); // <= 9s passé à 77ms + barre progression


            // Ajoute les résultats aux RawRoms affichées
            RawRomsCollec.AddRange(aWList.Resultats);
            RawRomsCollec.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));

            RawRomsCollec = new MyList<RawMameRom>(RawRomsCollec);

            // On va lever des roms à sauver
            // On va lever du datagrid de droite
            RomsToSave.RemoveRange(romsSelected);
            // RomsToSave.Sort((a, b) => string.Compare(a.Archive_Name, b.Archive_Name, StringComparison.CurrentCulture));
            RomsToSave = new MyList<CT_Rom>(RomsToSave);



            return true;
        }



        /// <summary>
        /// Permet de faire repasser les rawromsdeleted de nouveau dans la liste de celles affichées
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        /// <remarks>
        /// Va lever des rawroms deleted pour faire gagner en rapidité.
        /// </remarks>
        /// 
        private List<RawMameRom> AsyncRemoveRight(AsyncWindowProgressG window)
        {
            List<CT_Rom> romsSelected = (List<CT_Rom>)window.ProgressContext.Arguments[0];
            List<RawMameRom> rawRomsDeleted = (MyList<RawMameRom>)window.ProgressContext.Arguments[1];

            var romCollec = new List<RawMameRom>();
            window.Total = romsSelected.Count;


            // Le Hashset bombardera
            var archiveNames = new HashSet<string>(romsSelected.Select(r => r.Archive_Name));


            //RawRomsCollec.RemoveAll(r => archives.Contains(r.Name));
            int i = 0;
            foreach (var r in rawRomsDeleted)
            {
                if (archiveNames.Contains(r.Name))
                    romCollec.Add(r);

                i++;
                window.AsyncUpProgressPercent(i);
            }

            /*

            for (int i = 0; i < romsSelected.Count; i++)
            {
                CT_Rom sel = romsSelected[i];


                for (int j = 0; j < rawRomsDeleted.Count; j++)
                {
                    RawMameRom deleted = _RawRomsDeleted[j];
                    if (deleted.Name.Equals(sel.Archive_Name))
                    {
                        romCollec.Add(deleted);
                        //rawRomsDeleted.Remove(deleted);

                        break;
                    }
                }

                // on lève pour faire gagner en rapidité

                window.AsyncUpProgressPercent(i);
            }
            */

            return romCollec;
        }


        #region Sauvegarde
        internal bool SaveToDB()
        {

            if (MessageBox.Show("Would you want to save missing manufacturers. Refusing it, will stop all the process.", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //Cursor = Cursors.Wait;
                SaveMameManufacturers();
                //Cursor = Cursors.Arrow;
            }
            else { return false; }


            if (!SaveGames())
            {
                return false;
            }

            #region Liaisons
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
                    var tmp = MameManufacturers.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom);
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
            #endregion Liaisons

            //2.7s la constructin des liaisons

            // Sauvegarde des roms
            if (MessageBox.Show("Would you want to save this roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //Cursor = Cursors.Wait;
                SaveRoms();
                //Cursor = Cursors.Arrow;
            }
            else { return false; }


            return true;


        }



        /// <summary>
        /// Sauvegarde des jeux manquants en base
        /// </summary>
        /// <returns></returns>
        private bool SaveGames()
        {
            Debug.WriteLine("Sauvegarde des Jeux");
            try
            {
                #region Etat des lieux > Détermine les jeux à ajouter

                // Sauvegarde des jeux manquants
                List<CT_Game> gameToAdd = new List<CT_Game>();

                //
                for (int i = 0; i < RomsToSave.Count; i++)
                {
                    var rom = RomsToSave[i];

                    // Games
                    var posPar = rom.SourceFile.IndexOf('(');
                    var gameName = posPar > 0 ? rom.SourceFile.Substring(0, posPar).Trim() : rom.SourceFile;
                    //Debug.WriteLine(gameName); Consommateur

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
                // Ajout de la collection. 2s pour le filtre au dessus
                SaveInDB.Insert_Games(gameToAdd);

                using (SQLite_OP sqOp = new SQLite_OP())
                {
                    _GamesInDB = sqOp.GetListOf(CT_Game.Result2Class, new Obj_Select(table: PProp.Default.T_Games, fields: new[] { "ID", "Game_Name" }, all: true));
                    // 51ms
                }

                #endregion

                // 2s pour l'ajout de jeu au lieu de plus de 8s mais pas de progress
                return true;
            }
            catch
            {
                return false;
            }

        }



        private bool SaveRoms()
        {
            // Sauvegarde des roms
            /*if (MessageBox.Show("Would you want to save this roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {*/
            // Sauvegarde des roms
            AsyncWindowProgress window = new AsyncWindowProgress();
            window.Arguments.Add(RomsToSave.ToList());
            window.go += new AsyncWindowProgress.AsyncAction(AsyncSaveRoms);
            window.ShowDialog();
            //}
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
                var id = MameManufacturers.FirstOrDefault(x => x.Nom.Equals(rom.Manufacturer));

                // HAndler pour spliter les roms parents et les roms enfants
                if (rom.IsParent == true)
                    parentsToSave.Add(rom);
                else
                    childrenToSave.Add(rom);
            }

            // 3s mais aucune barre de progression
            // Sauvegarde des roms parents
            List<CT_Rom> sParentsRoms = null;
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));

                window.AsyncMessage("Insertion of Parent Roms");

                // Insertion en base
                sqReq.InsertMassive_Roms(parentsToSave, true);

                // 2minutes avec barre de progression pour la totalité
                // Select ??
                Obj_Select oSel = new Obj_Select(PProp.Default.T_Roms, all: true);
                oSel.AddConds(new SqlCond("IsParent", eWhere.Is, 1));

                sParentsRoms = sqReq.GetListOf<CT_Rom>(CT_Rom.Result2Class, oSel);// 230ms avec gestion de l'erreur au niveau de trans
            }

            // Assignation de la rom parent
            foreach (CT_Rom child in childrenToSave)
            {
                CT_Rom parRom = sParentsRoms.First(x => x.Archive_Name.Equals(child.Aff_Clone_Of));
                child.Clone_Of = parRom.ID;
            }

            // 7 secondes d'assignation (réduit à 4s avec gestion des erreurs de trans
            // Sauvegarde des roms enfants
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.UpdateProgress += ((x, y) => window.AsyncUpProgressPercent(y));
                window.AsyncMessage("Insertion of Children Roms");
                sqReq.InsertMassive_Roms(childrenToSave, true);
            }
        }
        //275 s ! 12s en faisant un commit différent, or du while

        #region Sauvegarde des constructeurs
        internal void SaveMameManufacturers()
        {
            Debug.WriteLine("Vérification des constructeurs manquants");

            // Sauvegarde des manufactureurs manquant
            List<CT_MameManufacturer> manuToAdd = new List<CT_MameManufacturer>();
            for (int i = 0; i < RomsToSave.Count; i++)
            {
                var rom = RomsToSave[i];

                if (rom.Manufacturer == null || string.IsNullOrEmpty(rom.Manufacturer.Nom))
                    continue;

                if (MameManufacturers.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null && manuToAdd.FirstOrDefault(x => x.Nom == rom.Manufacturer.Nom) == null)
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
                    MameManufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true));
                }
            }

        }


    }
    #endregion Sauvegarde des constructeurs
    #endregion  Sauvegarde

}

