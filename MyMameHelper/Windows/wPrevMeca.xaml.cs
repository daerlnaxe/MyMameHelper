using MyMameHelper.ContTable;
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
    /// Logique d'interaction pour wPrevList.xaml
    /// </summary>
    public partial class wPrevMeca : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MyObservableCollection<CT_Mechanical> Mecas { get; set; } = new MyObservableCollection<CT_Mechanical>();


        public wPrevMeca()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region filtre
        private string RomModes;
        private string _Filter;

        public string Filter
        {
            get { return _Filter; }
            set
            {
                if (!value.Equals(_Filter))
                {
                    _Filter = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private CT_Mechanical _S4L;
        public CT_Mechanical Selected4List
        {
            get { return _S4L; }
            set
            {
                if (value != _S4L)
                {
                    _S4L = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void Mode_Changed(object sender, RoutedEventArgs e)
        {
            RomModes = ((RadioButton)sender).Content.ToString();
            if (Filter != null)
                Select_LeftGame();
        }

        private void Select_LeftGame()
        {
            if (RomModes == "Mode Game")
                Selected4List = Mecas.FirstOrDefault(x => x.Description.StartsWith(Filter, StringComparison.OrdinalIgnoreCase));
            else if (RomModes == "Mode Archive")
                Selected4List = Mecas.FirstOrDefault(x => x.Meca_Name.StartsWith(Filter, StringComparison.OrdinalIgnoreCase));

            if (Selected4List != null)
            {
                dgMecas.ScrollIntoView(dgMecas.SelectedItem);
            }
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
             if (!((e.Key >= Key.A && e.Key <= Key.Z)
                || (e.Key >= Key.D0 && e.Key <= Key.D9)
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                || e.Key == Key.Back
                || e.Key == Key.Space
                || e.Key == Key.Delete
                || e.Key == Key.Decimal
                || e.Key == Key.OemPeriod
                || e.Key == Key.Subtract
                || e.Key == Key.Add
             ))
                return;

            char k = Methods.Keyboard.GetCharFromKey(e.Key);
            //
            if (Filter is null)
                Filter = string.Empty;

            //
            if (e.Key == Key.Back)
                Filter = Filter.Length > 0 ? Filter.Substring(0, Filter.Length - 1) : string.Empty;

            else if (e.Key == Key.Delete)
                Filter = string.Empty;

            else
                Filter += k;


            Select_LeftGame();
        }
        #endregion
    }
}
