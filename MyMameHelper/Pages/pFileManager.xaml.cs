using MyMameHelper.ContTable;
using MyMameHelper.Parsers;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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

        public MyObservableCollection<Aff_Game> DbGames = new MyObservableCollection<Aff_Game>();
        public List<Aff_Game> MissingGames = new List<Aff_Game>();


        private string[] _DirFiles;
        private Dictionary<uint, Aff_Machine> dicMachines;

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

        public Boolean MoveFiles { get; set; }

        public pFileManager()
        {
            InitializeComponent();

            Rom_Folder = Properties.Settings.Default.RomSource;
            Destination_Folder = Properties.Settings.Default.RomDestination;

            DataContext = this;

        }



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
            }

            System.Windows.MessageBox.Show("All Help Files created", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void RB_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
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
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Destination_Folder = fbd.SelectedPath;
                    Properties.Settings.Default.RomDestination = Destination_Folder;
                    Properties.Settings.Default.Save();

                }
            }
        }


        [Obsolete]
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
                i++;
            }
            System.Windows.MessageBox.Show($"{i} File(s) Moved", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void Proceed_Roms(object sender, ExecutedRoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Are you sure ?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Récupérer les fichiers dans le répertoire
            _DirFiles = Directory.GetFiles(PProp.Default.RomSource);

            // Récupérer les données de la base, table roms
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                DbGames.ChangeContent = sqReq.AffGames_List();

            }

            // Méthode
            string methodChoosen = (string)cbMethod.SelectionBoxItem;


            foreach (Aff_Game dbG in DbGames)
            {
                string dbgFile = Path.Combine(PProp.Default.RomSource, $"{dbG.Parent_Name}.zip");
                string destPath = null;

                if (MoveFiles == true)
                {

                }
                else
                {

                }
            }

            throw new Exception("A revoir");
            foreach (Aff_Game dbG in DbGames)
            {

                /*
                string dbgFile = Path.Combine(PProp.Default.RomSource, $"{dbG.Parent_Name}.zip");
                string destPath = null;

                Console.WriteLine($"{dicMachines[Convert.ToUInt32(dbG.Machine)].Constructeur} | {dbG.Aff_Machine} | {dbG.Game_Name}");

                //Debug.WriteLine("Test présence "+dbgFile);
                if (!File.Exists(dbgFile))
                {
                    MissingGames.Add(dbG);
                    continue;
                }

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
            }
        }



        private string Get_Path4Machine(Aff_Game dbG)
        {
            string dest = null;

            dest = Path.Combine(PProp.Default.RomDestination, dicMachines[Convert.ToUInt32(dbG.Machine)].Constructeur, dbG.Aff_Machine);

            if (dbG.Unwanted == true && useUnwanted.IsChecked == true)
            {
                dest = Path.Combine(dest, "Unwanted");
            }

            return dest;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Récupération des machines
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                List<Aff_Machine> truite = sqReq.List_MachinesJoin();
                dicMachines = truite.ToDictionary(x => x.ID, x => x);
            }
        }
    }
}
