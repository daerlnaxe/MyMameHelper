using MyMameHelper.ContTable;
using MyMameHelper.Parsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using MessageBox = System.Windows.MessageBox;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour MoveATXTFile.xaml
    /// </summary>
    public partial class pMoveATXFiles : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MyObservableCollection<CT_Game> _Games = new MyObservableCollection<CT_Game>();
        public MyObservableCollection<CT_Game> Games
        {
            get { return _Games; }
            set
            {
                if (value != _Games)
                {
                    _Games = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _RomFolder;
        private string _Destination_Folder;



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
        public pMoveATXFiles()
        {
            InitializeComponent();
            Rom_Folder = Properties.Settings.Default.RomSource;
            Destination_Folder = Properties.Settings.Default.RomDestination;

            DataContext = this;
        }

        /// <summary>
        /// Load files list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadList_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = String.IsNullOrEmpty(Properties.Settings.Default.ExportFolder) ? string.Empty : Properties.Settings.Default.ExportFolder;
            if (openFileDialog.ShowDialog() == true)
            {
                Games.ChangeContent = MameExportParser.Try_TxtParse(openFileDialog.FileName);
                Properties.Settings.Default.ExportFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.Save();
            }
        }

        private void Create_HelpFiles_Click(object sender, RoutedEventArgs e)
        {
            if (Games.Count == 0)
            {
                MessageBox.Show("File list empty, use \"Load...\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Destination_Folder))
            {
                MessageBox.Show("You forget to indicate which destination folder you would", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (CT_Game g in Games)
            {
                string helpFile = System.IO.Path.Combine(Destination_Folder, $"{g.Parent_Name}.txt");

                using (StreamWriter file = new StreamWriter(helpFile, append: false))
                {
                    file.WriteLine(g.Game_Name);
                }
            }

            MessageBox.Show("All Help Files created", "", MessageBoxButton.OK, MessageBoxImage.Information);
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



        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            if (Games.Count == 0)
            {
                MessageBox.Show("File list empty, use \"Load...\"", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Rom_Folder))
            {
                MessageBox.Show("You forget to indicate which rom folder you would to use", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Destination_Folder))
            {
                MessageBox.Show("You forget to indicate which destination folder you would", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int i = 0;
            foreach (CT_Game g in Games)
            {
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


                string destFile = System.IO.Path.Combine(Destination_Folder, $"{g.Parent_Name}.zip");
                File.Move(archiveFile, destFile);
                i++;
            }
            MessageBox.Show($"{i} File(s) Moved", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }



    }
}
