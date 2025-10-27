using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMameHelper.Windows
{

    /// <summary>
    /// Logique d'interaction pour Progress.xaml
    /// </summary>
    public partial class ProgressWindow : Window, INotifyPropertyChanged
    {
        public delegate void DoWorkEventHandler(ProgressWindow sender, DoWorkEventArgs e);
        public event DoWorkEventHandler DoWork;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public int Total { get; set; }

        public int _Progress_Value = 0;
        public int Progress_Value
        {
            get { return _Progress_Value; }
            set
            {
                if (value != _Progress_Value)
                {
                    _Progress_Value = value;
                   // OnPropertyChanged("Progress_Value"); 
                    NotifyPropertyChanged();
                }
            }
        }

        BackgroundWorker worker;

        public ProgressWindow()
        {
            InitializeComponent();
            DataContext = this;
           
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //the background worker started
            //let's call the user's event handler
            if (DoWork != null)
                DoWork(this, e);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
         //   progressB.Value = e.ProgressPercentage;
            Progress_Value = e.ProgressPercentage;

        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                Debug.WriteLine("Error");
            //DialogResult = DialogResult.Abort;
            else if (e.Cancelled)
                Debug.WriteLine("Cancelled");
            //DialogResult = DialogResult.Cancel;
            else
                Debug.WriteLine("Everything cool");
            //DialogResult = DialogResult.OK;

            //
            Close();
        }

        public void SetProgress(int percent)
        {
            //do not update the progress bar if the value didn't change

                worker.ReportProgress(percent);
            
        }
    }
}
