using MyMameHelper.ContTable;
using MyMameHelper.Parsers;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pCompare_Games.xaml
    /// </summary>
    public partial class pCompare_Games : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MyObservableCollection<CT_Game> _LeftGames = new MyObservableCollection<CT_Game>();
        public MyObservableCollection<CT_Game> Left_Games
        {
            get { return _LeftGames; }
            set
            {
                if (value != _LeftGames)
                {
                    _LeftGames = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private MyObservableCollection<CT_Game> _RightGames = new MyObservableCollection<CT_Game>();
        public MyObservableCollection<CT_Game> Right_Games
        {
            get { return _RightGames; }
            set
            {
                if (value != _RightGames)
                {
                    _RightGames = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<CT_Game> _DiffGames = new ObservableCollection<CT_Game>();
        public ObservableCollection<CT_Game> Diff_Games
        {
            get { return _DiffGames; }
            set
            {
                if (value != _DiffGames)
                {
                    _DiffGames = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #region commandes
        public static readonly RoutedCommand Left_LoadFileCmd = new RoutedCommand("Load File...", typeof(pCompare_Games));

        public static readonly RoutedCommand Right_LoadFileCmd = new RoutedCommand("CustomCommand", typeof(pCompare_Games));
        
        public static readonly RoutedCommand Missing_LeftCmd = new RoutedCommand("Missing Left..", typeof(pCompare_Games));
        
        public static readonly RoutedCommand Missing_RightCmd = new RoutedCommand("Missing Right...", typeof(pCompare_Games));

        public static readonly RoutedCommand Clear_LeftCmd = new RoutedCommand("Clear", typeof(pCompare_Games));

        public static readonly RoutedCommand Clear_RightCmd = new RoutedCommand("Clear", typeof(pCompare_Games));
        
        public static readonly RoutedCommand Clear_DiffCmd = new RoutedCommand("Clear", typeof(pCompare_Games));        

        public static readonly RoutedCommand Diff_WorkCmd = new RoutedCommand("Work", typeof(pCompare_Games));
        

        private void CEx_Both(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = Left_Games.Count > 0 && Right_Games.Count > 0;
        }
        private void CEx_Left(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Left_Games.Count > 0;
        }
        private void CEx_Right(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Right_Games.Count > 0;
        }

        /// <summary>
        /// Active les possibilités quand la collection diff est supérieure à 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CEx_Diff(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Diff_Games.Count > 0;
        }
        #endregion


        public pCompare_Games()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Left_LoadFile(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = String.IsNullOrEmpty(Properties.Settings.Default.MameFolder) ? string.Empty : Properties.Settings.Default.MameFolder;
            if (openFileDialog.ShowDialog() == true)
            {
                Left_Games.ChangeContent = MameExportParser.Try_TxtParse(openFileDialog.FileName);
                //DBGames = MameExportParser.Txt_Parse(openFileDialog.FileName);
                //btSearch.IsEnabled = true;
                //btSave.IsEnabled = true;
                //  Properties.Settings.Default.ExportFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                //  Properties.Settings.Default.Save();
            }

        }

        private void Right_LoadFile(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = String.IsNullOrEmpty(Properties.Settings.Default.MameFolder) ? string.Empty : Properties.Settings.Default.MameFolder;
            if (openFileDialog.ShowDialog() == true)
            {
                Right_Games.ChangeContent = MameExportParser.Try_TxtParse(openFileDialog.FileName);
                //DBGames = MameExportParser.Txt_Parse(openFileDialog.FileName);
                //btSearch.IsEnabled = true;
                //btSave.IsEnabled = true;
                //  Properties.Settings.Default.ExportFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                //  Properties.Settings.Default.Save();
            }

        }
        /*
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            ListSortDirection direction;

            if (headerClicked != null)
            {
                
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        */
        private void Sort(string sortBy, ListSortDirection direction)
        {
            /*  ICollectionView dataView =
                CollectionViewSource.GetDefaultView(lv.ItemsSource);

              dataView.SortDescriptions.Clear();
              SortDescription sd = new SortDescription(sortBy, direction);
              dataView.SortDescriptions.Add(sd);
              dataView.Refresh();*/

        }

        #region comparaisons
        /*
        private void Left_Compare(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Run Left Comparison ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(Left_Work);

                progressW.Total = 100;
                progressW.ShowDialog();
            }
        }
        private void Left_Work(ProgressWindow sender, DoWorkEventArgs e)
        {
            //get the provided argument as usual
            object myArgument = e.Argument;
            ObservableCollection<CT_Game> diff = new ObservableCollection<CT_Game>();

            List<CT_Game> tempGames = new List<CT_Game>(Right_Games);

            for (int i = 0; i < Left_Games.Count; i++)
            {
                CT_Game g = Left_Games[i];

                CT_Game found = null;
                foreach (CT_Game y in tempGames)
                {
                    if (g.Equals(y))
                    {
                        found = y;
                        break;
                    }
                }

                if (found == null)
                    diff.Add(g);
                else
                {
                    tempGames.Remove(found);
                }

                sender.SetProgress(i * 100 / Left_Games.Count);
            }
           // return diff;
        }
        */

        raoul wd1;
        BackgroundWorker worker;
        bool cancel = false;
        private void Left_Compare(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Run Left Comparison ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                wd1 = new raoul();
                wd1.Progress_Max = 100;
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(Left_Work);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();


                wd1.ShowDialog();
                if (worker.IsBusy)
                    worker.CancelAsync();
            }
        }

        private void Right_Compare(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Run Right Comparison ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                wd1 = new raoul();
                wd1.Progress_Max = 100;
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(Right_Work);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();

                wd1.ShowDialog();
                if (worker.IsBusy)
                    worker.CancelAsync();
            }
        }
        private void Left_Work(object sender, DoWorkEventArgs e)
        {
            //get the provided argument as usual
            object myArgument = e.Argument;

            List<CT_Game> tempGames = new List<CT_Game>(Right_Games);

            for (int i = 0; i < Left_Games.Count; i++)
            {
                CT_Game g = Left_Games[i];

                CT_Game found = null;
                foreach (CT_Game y in tempGames)
                {
                    if (g.Equals(y))
                    {
                        found = y;
                        break;
                    }
                }

                if (found == null)
                    worker.ReportProgress(i * 100 / Left_Games.Count, g);
                else
                {
                    tempGames.Remove(found);
                }

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

            }
            // return diff;
        }
        private void Right_Work(object sender, DoWorkEventArgs e)
        {
            //get the provided argument as usual
            object myArgument = e.Argument;

            List<CT_Game> tempGames = new List<CT_Game>(Left_Games);

            for (int i = 0; i < Right_Games.Count; i++)
            {
                CT_Game d = Right_Games[i];

                CT_Game found = null;

                foreach (CT_Game y in tempGames)
                {
                    if (d.Equals(y))
                    {
                        found = y;
                        break;
                    }
                }

                // todo: bidouillage faire quelque chose de plus propre
                if (found == null)
                    worker.ReportProgress(i * 100 / Right_Games.Count, d);

                else
                {
                    worker.ReportProgress(i * 100 / Right_Games.Count);
                    tempGames.Remove(found);
                }

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }

            //  return diff;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            wd1.Progress_Value = e.ProgressPercentage;
            if (e.UserState != null)
                Diff_Games.Add((CT_Game)e.UserState);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled == true)
            {
                Debug.WriteLine("Canceled !");
            }
            else if (e.Error != null)
            {
                Debug.WriteLine("Error: " + e.Error.Message);
            }
            else
            {
                Debug.WriteLine("Done !");
            }
            wd1.Close();
        }



        #endregion

        #region Clear
        private void Clear_Left(object sender, ExecutedRoutedEventArgs e)
        {
            Left_Games.Clear();
        }

        private void Clear_Right(object sender, ExecutedRoutedEventArgs e)
        {
            Right_Games.Clear();
        }
        private void Clear_Diff(object sender, ExecutedRoutedEventArgs e)
        {
            Diff_Games.Clear();
        }
        #endregion

        /// <summary>
        /// Envoie à la page de données des traitement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work_WithData(object sender, ExecutedRoutedEventArgs e)
        {
            //pWorkPage pWork = new pWorkPage();
          //  pWork.SetGamesToOrganize(Diff_Games);
           // NavigationService.Navigate(pWork);
        }
    }
}
