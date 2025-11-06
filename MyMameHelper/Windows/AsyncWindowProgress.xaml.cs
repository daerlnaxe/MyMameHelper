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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour AsyncWindowProgress.xaml
    /// </summary>
    public partial class AsyncWindowProgress : Window, INotifyPropertyChanged
    {
        public delegate void AsyncAction(AsyncWindowProgress window);
        public AsyncAction go;        

        // maj interface
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public List<object> Arguments { get; set; } = new List<object>();

        //
        public int Total { get; set; } = 100;

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

        private string _Message_Value;
        public string Message_Value
        {
            get { return _Message_Value; }
            set
            {
                if (value != null & !value.Equals(_Message_Value))
                {
                    _Message_Value = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public void AsyncUpProgressValue(int value)
        {
            this.Dispatcher?.Invoke(new Action(() => Progress_Value = value));
        }


        public void AsyncUpProgressPercent(int percent)
        {
            this.Dispatcher?.Invoke(new Action(() => Progress_Value = percent));
        }

        public void AsyncMessage(string value)
        {
            this.Dispatcher?.Invoke(new Action(() => Message_Value = value));
        }

        public void AsyncClose()
        {
            this.Dispatcher?.Invoke(new Action(() => this.Close()));
        }

        public AsyncWindowProgress()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AsyncWork();
        }

        private void AsyncWork()
        {
            AsyncCallback ra = new AsyncCallback(Finished);
            
            go?.BeginInvoke(this, ra, null);

            return;
            Task t = Task.Run(() => go);
            Task.WaitAll(t);
            Console.WriteLine("Fini");
        }

        private void Finished(IAsyncResult a)
        {
            Progress_Value = 100;
            Console.WriteLine("Fini");
            this.AsyncClose();
        }
    }
}
