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
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class raoul : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Progress_Max { get; set; }

        private int _Progress_Value;
        public int Progress_Value
        {
            get { return _Progress_Value; }
            set
            {


                if (value != _Progress_Value)
                {
                    _Progress_Value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void CloseByAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Close();
            }));
        }

        public raoul()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
