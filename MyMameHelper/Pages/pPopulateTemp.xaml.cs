using MyMameHelper.ContTable;
using MyMameHelper.Methods;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pPopulateTemp.xaml
    /// </summary>
    public partial class pPopulateTemp : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MyObservableCollection<RawMameRom> RomsCollec { get; set; } = new MyObservableCollection<RawMameRom>();

        public void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }


        private int _IndexRom;
        public int IndexRom
        {
            get { return _IndexRom; }
            set
            {
                if(value != _IndexRom)
                {
                    _IndexRom = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public pPopulateTemp()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Ex_LoadXML(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fod = new OpenFileDialog();
            fod.InitialDirectory = PProp.Default.MameFolder;

            if (fod.ShowDialog() == DialogResult.OK)
            {
                MameXMLRaw mRaw = new MameXMLRaw();

                RomsCollec.ChangeContent =  mRaw.TryToParse(fod.FileName);                
            }
        }

        #region SaveToDb
        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsCollec.Count > 0;
        }
        private void Ex_SaveDb(object sender, ExecutedRoutedEventArgs e)
        {
            if(System.Windows.MessageBox.Show("Save everything to Temp Db ?","Save to Db", MessageBoxButton.YesNo, MessageBoxImage.Question )  == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(SaveAllTemp_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();

                using (SQLite_Op sqReq = new SQLite_Op())
                {
                    MainWindow.NumberOf_TempRoms = sqReq.Count(PProp.Default.T_TempRoms);
                }
            }
        }

        private void SaveAllTemp_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            object myArgument = e.Argument;

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));
                sqReq.Insert_RomsInTemp(RomsCollec);
            }
        }


        #endregion

        #region remove rom

        private void Can_Remove(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsCollec.Count > 0;
        }
        private void Ex_Remove(object sender, ExecutedRoutedEventArgs e)
        {
            RomsCollec.RemoveAt(IndexRom);
        }

        #endregion
    }
}
