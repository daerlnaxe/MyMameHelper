using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour Configuration.xaml
    /// </summary>
    public partial class Configuration : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _MameFolder;
        public string Mame_Folder
        {
            get { return _MameFolder; }
            set
            {
                if (value != _MameFolder)
                {
                    _MameFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Configuration()
        {
            InitializeComponent();
            Mame_Folder = Properties.Settings.Default.MameFolder;
            DataContext = this;

        }

        #region Paths
        private void Choose_MameFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.SelectedPath = Properties.Settings.Default.MameFolder;
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Rom_Folder = fbd.SelectedPath;
                    Mame_Folder = fbd.SelectedPath;
                    Properties.Settings.Default.LastPath = fbd.SelectedPath;
                }
            }
        }

        private void TbMameFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(tbMameFolder.Text))
                Mame_Folder = tbMameFolder.Text;
        }
        #endregion



        /// <summary>
        /// Sauver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MameFolder = Mame_Folder;
            
            Properties.Settings.Default.Save();
            DialogResult = true;
        }


    }
}
