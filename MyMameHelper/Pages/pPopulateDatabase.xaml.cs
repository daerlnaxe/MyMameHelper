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
    public partial class pPopulateDatabase : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MyObservableCollection<RawMameRom> RawRomsCollec { get; set; } = new MyObservableCollection<RawMameRom>();

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
                if (value != _IndexRom)
                {
                    _IndexRom = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public pPopulateDatabase()
        {
            InitializeComponent();
            DataContext = this;
        }


        /// <summary>
        /// Chargement d'un fichier M.A.M.E xml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_LoadXML(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fod = new OpenFileDialog();
            fod.InitialDirectory = PProp.Default.MameFolder;

            if (fod.ShowDialog() == DialogResult.OK)
            {
                MameXMLRaw mRaw = new MameXMLRaw();

                RawRomsCollec.ChangeContent = mRaw.TryToParse(fod.FileName);
            }
        }

        #region Populate DB
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Can_Populate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RawRomsCollec.Count > 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ex_Populate(object sender, ExecutedRoutedEventArgs e)
        {
            // bool poursuivre = false;
            if (System.Windows.MessageBox.Show("Save everything to Temp Db ?", "Save to Db", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(SaveAllTemp_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();

                using (SQLite_OP sqReq = new SQLite_OP())
                {
                    MainWindow.NumberOf_TempRoms = sqReq.Count(PProp.Default.T_TempRoms);
                }


            }

            if (MainWindow.NumberOf_TempRoms != RawRomsCollec.Count)
                return;

            if (System.Windows.MessageBox.Show("Populate Machines in DB ?\nUse machine mapper to rebuild links with roms", "Populate Machines", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(PopulateMachine_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();

            }
        }


        /// <summary>
        /// Populate Temp Roms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAllTemp_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            object myArgument = e.Argument;

            using (SQLite_OP sqOP = new SQLite_OP())
            {
                //sqOP.Stopit=sender.Stopit;
                sqOP.UpdateProgress += ((x, y) => sender.SetProgress(y));
                sender.Closing += sqOP.Sender_Closing;
                // Insertion des rawroms
//                sqOP.Insert_RawRomsInTemp(RawRomsCollec);
                sqOP.InsertMassive_RawRomsInTemp(RawRomsCollec);

            }
        }



        /// <summary>
        /// Populate Machines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopulateMachine_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            object myArgument = e.Argument;

            using (SQLite_OP sqOP = new SQLite_OP())
            {
                sqOP.Drop_TMachine();
                sqOP.Create_TMachine();

                List<CT_Occurence<RawMameRom>> rawroms = sqOP.Get_RRomGroupedSFile(); // 62ms

                sqOP.UpdateProgress += ((x, y) => sender.SetProgress(y));
                // Insertion des rawroms
                Dictionary<string, List<CT_Machine>> machines = TableFeeder.Machine(rawroms); //<= 310ms, pas d'UI

                sqOP.Insert_Machines(machines["identified"], false, false); 
                sqOP.Insert_Machines(machines["money"], false, false);
                sqOP.Insert_Machines(machines["SystemRoms"], ignore: false, preservePK: true);
                sqOP.Insert_Machines(machines["Constructeurs"], ignore: false, preservePK: true);
            }

        }
        #endregion

        #region remove rom

        private void Can_Remove(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RawRomsCollec.Count > 0;
        }
        private void Ex_Remove(object sender, ExecutedRoutedEventArgs e)
        {
            RawRomsCollec.RemoveAt(IndexRom);
        }

        #endregion
    }
}
