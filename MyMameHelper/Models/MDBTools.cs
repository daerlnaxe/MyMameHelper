using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using PProp = MyMameHelper.Properties.Settings;
using System.Runtime.Remoting;
using MyMameHelper.Methods;
using MyMameHelper.Windows;
using System.Threading;
using System.Collections.ObjectModel;


namespace MyMameHelper.Models
{
    /// <summary>
    /// DataContext de DataBase Tools.
    /// </summary>
    /// <remarks>
    /// On va travailler à chaque fois sur des données fraiches et donc recharger le contenu de certaines tables, selon les besoins.
    /// </remarks>
    internal class MDBTools : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _DataBase_Path;
        /// <summary>
        /// Chemin d'accès de la base de données
        /// </summary>
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




        #region Collection
        public List<string> lTables { get; private set; }

        public MyObservableCollection<CT_Constructor> Constructors { get; private set; } = new MyObservableCollection<CT_Constructor>();

        public MyObservableCollection<CT_Genre> Genres { get; private set; } = new MyObservableCollection<CT_Genre>();

        public MyObservableCollection<CT_MameManufacturer> Manufacturers { get; private set; } = new MyObservableCollection<CT_MameManufacturer>();
        #endregion Collection


        public MDBTools()
        {
            DataBase_Path = Properties.Settings.Default.DataBase_Path;
            if (String.IsNullOrEmpty(DataBase_Path))
            {
                Properties.Settings.Default.DataBase_Path = DataBase_Path = "./MyMameHelper.sqlite";
                Properties.Settings.Default.Save();
            }
        }


        /// <summary>
        /// Chargement du contenu en base de données
        /// </summary>
        internal void Init()
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
                    Constructors.ChangeContent = sqRead.GetListOf<CT_Constructor>(CT_Constructor.Result2Class, objSelConst);

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


        #region Genres
        /// <summary>
        /// 
        /// </summary>
        internal void AddGenre()
        {
            LambdaValue lval = new LambdaValue();
            if (lval.ShowDialog() == true)
            {
                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    sqReq.Insert_Genre(new CT_Genre()
                    {
                        Nom = lval.Valeur
                    });

                    var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                    objSelGenres.AddOrders(new SqlOrder("Nom"));

                    Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctGenre"></param>
        internal void EditGenre(CT_Genre ctGenre)
        {
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.Update_Genre(ctGenre);
                var objSelGenres = new Obj_Select(table: PProp.Default.T_Genres, all: true);
                objSelGenres.AddOrders(new SqlOrder("Nom"));
                Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, objSelGenres);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctGenre"></param>
        internal void RemoveGenre(CT_Genre ctGenre)
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

        #endregion Genres


        #region Machines
        /// <summary>
        /// Rebuild complet de la table machine
        /// </summary>
        public void BuildMachines()
        {
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                sqOP.Drop_TMachine();
                sqOP.Create_TMachine();

                var srcFiles = sqOP.Get_RRomGroupedSFile();

                Dictionary<string, List<CT_Machine>> machines = TableFeeder.Machine(srcFiles);
                sqOP.InsertMassive_Machines(machines["identified"], false, false);
                sqOP.InsertMassive_Machines(machines["money"], false, false);
                sqOP.InsertMassive_Machines(machines["SystemRoms"], ignore: false, preservePK: true);
                sqOP.InsertMassive_Machines(machines["Constructeurs"], ignore: false, preservePK: true);

            }
        }

        /// <summary>
        /// Update la table machine
        /// </summary>
        internal void Update_Machine()
        {
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                sqOP.SafeAlter_TMachine();

            }
        }
        #endregion


        #region Manufacturers
        /// <summary>
        /// -02-
        /// </summary>
        /// <param name="nom"></param>
        internal void AddManufacturer(string nom)
        {
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.Insert_MameManufacturer(new CT_MameManufacturer()
                {
                    Nom = nom
                });
                Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctManu"></param>
        internal void EditManufacturer(CT_MameManufacturer ctManu)
        {
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                sqReq.Update_MameManufacturer(ctManu);
                Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctManu"></param>
        internal void RemoveManufacturer(CT_MameManufacturer ctManu)
        {
            using (SQLite_OP sqReq = new SQLite_OP())
            {
                SqlCond cond = new SqlCond("ID", eWhere.Equal, ctManu.ID);
                sqReq.Delete_MameManufacturer(new SqlCond[] { cond });

                Manufacturers.ChangeContent = sqReq.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_Constructors, all: true));
            }
        }


        #endregion Manufacturers


        #region Remapping
        internal bool RemapRomMachine()
        {
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                Dictionary<string, object[]> srcFiles = sqOP.Get_RomGroupedSFile();

                // Machines
                var objSel = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                //objSel.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                objSel.AddOrders(new SqlOrder("Nom"));
                var machines = sqOP.GetListOf(CT_Machine.Result2Class, objSel);


                sqOP.Map_RomMachine(machines, srcFiles.Keys.ToList());
                return true;
            }


        }


        /// <summary>
        /// Remap les roms avec les manufacturers
        /// </summary>
        internal void Remap_RomManu()
        {
            IList<CT_Rom> roms = null;

            // Roms temporaires pour faire la correspondance
            List<RawMameRom> tempRoms;

            // Roms à updater
            List<CT_Rom> romsToUpdate = new List<CT_Rom> { };

            using (SQLite_OP sqRead = new SQLite_OP())
            {
                // Récupérer un duo temproms|mamemanufacturer
                Obj_Select objSelect = new Obj_Select(
                    table: PProp.Default.T_TempRoms,
                    fields: new string[] { "Name", "Manufacturer" }

                    );
                objSelect.Orders = new SqlOrder[] { new SqlOrder("Name") };

                //
                tempRoms = (List<RawMameRom>)sqRead.GetCollectionOf<RawMameRom>(RawMameRom.Result2Class, objSelect);

                // Liste des Manufacturers
                Manufacturers.ChangeContent = sqRead.GetListOf<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, new Obj_Select(table: PProp.Default.T_MameManufacturers, all: true));

                // Liste des roms
                roms = sqRead.GetCollectionOf<CT_Rom>(CT_Rom.Result2Class, new Obj_Select
                    (
                    table: PProp.Default.T_Roms,
                    fields: new string[] { "ID", "Archive_Name", "Manufacturer_Id" }
                    ));

                //200 ms ?
            }


            //
            foreach (CT_Rom r in roms)
            {
                RawMameRom rawrom = tempRoms.FirstOrDefault(x => x.Name.Equals(r.Archive_Name));

                // Dans le cas où ça n'existe pas
                if (rawrom == null)
                    continue;

                // Réduction du temps de traitement
                tempRoms.Remove(rawrom);

                // Recherche du manufacturer par rapport à la temprom
                CT_MameManufacturer manufacturer = Manufacturers.FirstOrDefault(x => x.Nom.Equals(rawrom.Manufacturer));

                // Assignation à null pour la rom
                if (manufacturer == null && r.Manufacturer.ID != null)
                {
                    // Ajouter aux roms à updater
                    r.Manufacturer.ID = 0;
                    romsToUpdate.Add(r);
                }
                // Si duo ok, changer le manufacturer_id
                else if (manufacturer != null && r.Manufacturer.ID != manufacturer.ID)
                {
                    // Ajouter aux roms à updater
                    r.Manufacturer.ID = manufacturer.ID;
                    romsToUpdate.Add(r);
                }

            }


            using (SQLite_OP sqWrite = new SQLite_OP())
            {
               // sqWrite.Update_Roms(romsToUpdate);
                sqWrite.Update_MassiveRoms(romsToUpdate);
            }

            //
           /* AsyncWindowProgress window = new AsyncWindowProgress();
            window.Arguments = new List<object>() { romsToUpdate };
            window.Message_Value = "Updating roms";

            window.go += new AsyncWindowProgress.AsyncAction(AsyncUpdateRomManu);

            window.Total = rawRomsSelected.Count;
            window.ShowDialog();
           */
        }
        #endregion Remapping
    }
}