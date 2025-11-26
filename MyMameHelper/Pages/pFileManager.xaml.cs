using MyMameHelper.ContTable;
using MyMameHelper.Parsers;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using Path = System.IO.Path;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour MoveATXTFile.xaml
    /// </summary>
    public partial class pFileManager : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        //public MyObservableCollection<Aff_Game> DbGames = new MyObservableCollection<Aff_Game>();

        public List<CT_Game> IncompleteGames = new List<CT_Game>();
        public List<CT_Rom> MissingRoms = new List<CT_Rom>();


        private string[] _DirFiles;
        private Dictionary<uint, Aff_Machine> _DicMachines;

        private string _RomFolder;
        public string Rom_Folder
        {
            get { return _RomFolder; }
            set
            {
                if (value != _RomFolder)
                {
                    _RomFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private string _Destination_Folder;
        public string Destination_Folder
        {
            get { return _Destination_Folder; }
            set
            {
                if (value != _Destination_Folder)
                {
                    _Destination_Folder = value;
                    NotifyPropertyChanged();
                }
            }
        }


        #region Checkboxes
        public Boolean MoveFiles { get; set; }

        public bool OverWriteFiles { get; set; }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public pFileManager()
        {
            InitializeComponent();

            Rom_Folder = Properties.Settings.Default.RomSource;
            Destination_Folder = Properties.Settings.Default.RomDestination;

            DataContext = this;

        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Récupération des machines
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                List<Aff_Machine> truite = sqReq.List_MachinesJoin();
                _DicMachines = truite.ToDictionary(x => x.ID, x => x);
            }
        }



        /*
        private void Create_HelpFiles_Click(object sender, RoutedEventArgs e)
        {
            if (DbGames.Count == 0)
            {
                System.Windows.MessageBox.Show("File list empty, use \"Load...\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Destination_Folder))
            {
                System.Windows.MessageBox.Show("You forget to indicate which destination folder you would", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (CT_Game g in DbGames)
            {
                throw new Exception("A revoir");

                /*
                string helpFile = System.IO.Path.Combine(Destination_Folder, $"{g.Parent_Name}.txt");

                using (StreamWriter file = new StreamWriter(helpFile, append: false))
                {
                    file.WriteLine(g.Game_Name);
                }*/
        /*
            }

            System.Windows.MessageBox.Show("All Help Files created", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }*/



        private void RB_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {


                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Rom_Folder = fbd.SelectedPath;
                    Properties.Settings.Default.RomSource = Rom_Folder;
                    Properties.Settings.Default.Save();
                }
            }

        }


        private void DF_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.SelectedPath = Properties.Settings.Default.RomDestination;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Destination_Folder = fbd.SelectedPath;
                    Properties.Settings.Default.RomDestination = Destination_Folder;
                    Properties.Settings.Default.Save();

                }
            }
        }


        [Obsolete]
        /*
        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            if (DbGames.Count == 0)
            {
                System.Windows.MessageBox.Show("File list empty, use \"Load...\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Rom_Folder))
            {
                System.Windows.MessageBox.Show("You forget to indicate which rom folder you would to use", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Destination_Folder))
            {
                System.Windows.MessageBox.Show("You forget to indicate which destination folder you would", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int i = 0;
            foreach (CT_Game g in DbGames)
            {
                throw new Exception("A revoir");
                /*
                string archiveFile = System.IO.Path.Combine(Rom_Folder, $"{g.Parent_Name}.zip");

                if (!File.Exists(archiveFile)) continue;


                string helpFile = System.IO.Path.Combine(Destination_Folder, $"{g.Parent_Name}.txt");

                if (useFiles.IsChecked == true)
                {
                    using (StreamWriter file = new StreamWriter(helpFile, append: false))
                    {
                        file.WriteLine(g.Game_Name);
                    }
                }


                //string destFile = System.IO.Path.Combine(Destination_Folder, $"{g.Parent_Name}.zip");
                //File.Move(archiveFile, destFile);*/
        /*
                i++;
            }
            System.Windows.MessageBox.Show($"{i} File(s) Moved", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }*/

        private void Proceed_Roms(object sender, ExecutedRoutedEventArgs e)
        {

            if (MessageBox.Show("Are you sure ?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;


            if (!Directory.Exists(PProp.Default.RomSource))
            {            
                MessageBox.Show("Enter a valid Rom Source ", "Error - Directory", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (string.IsNullOrEmpty(Destination_Folder))
            {
                MessageBox.Show("Enter a valid Destination Folder", "Error - Directory", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                


            // Récupérer les fichiers dans le répertoire
            _DirFiles = Directory.GetFiles(PProp.Default.RomSource);

            // Récupérer les jeux et les roms associées
            Get_RomMapped();
            Debug.WriteLine($"Nombre de roms trouvées: {_RomsMapped.Count}");


            List<CT_Rom_Mapped> FilteredGamesMapped = new List<CT_Rom_Mapped>(_RomsMapped);

            // Méthode
            // Construction du chemin de destination
            //string destPath = "0-Miscellaneous";
            string arboChoosen = (string)cbArboType.SelectionBoxItem;
            Debug.WriteLine($"Méthode choisie: '{arboChoosen}'");


            // Pour chaque jeu
            foreach (CT_Rom_Mapped romMapped in FilteredGamesMapped)
            {
                Debug.WriteLine($"Travail sur : '{romMapped.Archive_Name}'");

                string dest = Destination_Folder;

                var dbG = romMapped.Game;


                // According to the arborescence type chosen
                switch (arboChoosen)
                {
                    case "Machine":
                        dest = Get_Path4Machine(romMapped);
                        break;

                    default:
                        dest = PProp.Default.RomSource;
                        break;
                }



                //dest = Path.Combine(dest, $"{dbG.}.zip");


                //Console.WriteLine($"{dicMachines[Convert.ToUInt32(dbG.Machine)].Constructeur} | {dbG.Machine.Nom} | {dbG.Game_Name}");

                string romFile = Path.Combine(PProp.Default.RomSource, $"{romMapped.Archive_Name}.zip");


                // Vérifie que le fichier existe
                Debug.Write($"Test présence '{romFile}': ");
                if (!File.Exists(romFile))
                {
                    Debug.WriteLine("Absent");

                    MissingRoms.Add(romMapped);

                    if (IncompleteGames.FirstOrDefault(x => x == dbG) == null)
                    {
                        IncompleteGames.Add(dbG);
                    }

                    continue;
                }

                Debug.WriteLine("Présent");

                // Déplacement + option d'écrasement.
                string destFile = Path.Combine(dest, $"{romMapped.Archive_Name}.zip");

                try
                {
                    bool overW = false;

                    if (OverWrite.IsChecked == true)
                        overW = true;

                    //var tesDir = Path.GetDF(dest);

                    if (!Directory.Exists(dest))
                        Directory.CreateDirectory(dest);

                    if (dbG.Unwanted == true && useUnwanted.IsChecked == true)
                        File.Create(destFile);


                    // Déplacement des fichiers
                    if (MoveFiles)
                    {
                        if (!OverWriteFiles)
                        {
                            System.Windows.MessageBox.Show("File exists, unable to move file if you don't allow to overwrite", "File exists", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else if (OverWriteFiles && File.Exists(destFile))
                        {
                            File.Delete(destFile);
                        }
                        File.Move(romFile, destFile);
                    }
                    else
                        File.Copy(romFile, destFile, overW);
                }
                catch (IOException ioExc)
                {
                    Debug.WriteLine(ioExc.Message);

                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }



            }

            System.Windows.MessageBox.Show("File operation finished", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);

            return;
            // throw new Exception("A revoir");
            /* foreach (Aff_Game dbG in DbGames)
             {

                 /*


                 // Construction du chemin de destination
                 string dest = "0-Miscellaneous";
                 switch (methodChoosen)
                 {
                     case "Machine":
                         dest = Get_Path4Machine(dbG);
                         break;
                 }

                 dest = Path.Combine(dest, $"{ dbG.Parent_Name}.zip");

                 // Déplacement + option d'écrasement.

                 try
                 {
                     bool overW = false;

                     if (OverWrite.IsChecked == true)
                         overW = true;

                     if (!Directory.Exists(Path.GetDirectoryName(dest)))
                         Directory.CreateDirectory(Path.GetDirectoryName(dest));

                     if (dbG.Unwanted == true && useUnwanted.IsChecked == true)
                         File.Create(dest);
                     else
                         File.Copy(dbgFile, dest, overW);
                 }
                 catch (Exception exc)
                 {
                     Console.WriteLine(exc.Message);
                 }*/
            /*}*/
        }




        /// <summary>
        /// Formate un path en combinant le répertoire de destination, le nom du constructeur, la machine
        /// </summary>
        /// <param name="dbG"></param>
        /// <returns></returns>
        /// <remarks>
        /// Limite les erreurs en renvoyant dans un dossier spécifique
        /// </remarks>
        private string Get_Path4Machine(CT_Rom_Mapped romMapped)
        {

            if (romMapped.Machine == null || romMapped.Machine_Id == null)
                return  Path.Combine(PProp.Default.RomDestination, "_No_Machine");

            string dest = null;



            dest = Path.Combine(PProp.Default.RomDestination, romMapped.Machine.Nom);

            if (romMapped.Game.Unwanted == true && useUnwanted.IsChecked == true)
            {
                dest = Path.Combine(dest, "Unwanted");
            }

            return dest;
        }





        private List<CT_Rom_Mapped> _RomsMapped;


        /// <summary>
        /// Construit et lance l'asyncloadmapGames
        /// </summary>
        private void Get_RomMapped()
        {
            // Chargement asynchrone des Jeux et des roms associées
            AsyncWindowProgress aLoad = new AsyncWindowProgress();
            aLoad.go += new AsyncWindowProgress.AsyncAction(AsyncLoad_RomMapped);
            aLoad.ShowDialog();
        }

        /// <summary>
        /// Récupère en base les valeurs avec liaison des deux tables
        /// </summary>
        /// <param name="aLoad"></param>
        private void AsyncLoad_RomMapped(AsyncWindowProgress aLoad)
        {
            aLoad.AsyncMessage("Loading enhanced Roms...");
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                _RomsMapped = sqReq.List_Roms4Move();


            }
        }



    }
}
