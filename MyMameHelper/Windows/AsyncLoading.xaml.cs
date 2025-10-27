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
    /// Logique d'interaction pour Loading.xaml
    /// </summary>
    public partial class AsyncLoading : Window, INotifyPropertyChanged
    {
        public delegate void AsyncAction(AsyncLoading windows);
        public AsyncAction go;



        // maj interface
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void AsyncClose()
        {
            this.Dispatcher?.Invoke(new Action(() => this.Close()));
        }

        public void AsyncInform(string value)
        {
            this.Dispatcher?.Invoke(new Action(() => Inform = value));
        }


        private string _Inform;
        public string Inform
        {
            get { return _Inform; }
            set
            {
                if (!value.Equals(_Inform))
                {
                    _Inform = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public AsyncLoading()
        {
            InitializeComponent();
            DataContext = this;

            Inform = "0";
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
            Console.WriteLine("Fini");
            this.AsyncClose();
        }


    }
}
