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
    /// Logique d'interaction pour wAddMachine.xaml
    /// </summary>
    public partial class wMachine : Window
    { 
        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; }

        /*
                public uint IdConstructeur { get; set; }
                public string MachineName { get; set; }      

                public uint Year { get; set; }


                public bool AllowCPath { get; set; }
                */
        public object SelectedConstructeur { get; set; }

        public CT_Machine Machine { get; set; }


        public wMachine()
        {
            InitializeComponent();
            DataContext = this;
            //cboxConst.SelectedIndex = 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
