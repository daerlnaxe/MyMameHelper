using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour wLinkMachine.xaml
    /// </summary>
    public partial class wLinkMachine : Window
    {
        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();

        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();


        public CT_Machine Machine { get; set; } = new CT_Machine();
        public CT_Constructeur Constructeur { get; set; } = new CT_Constructeur();


        public wLinkMachine()
        {
            InitializeComponent();
            DataContext = this;

            using(SQLite_Req sqlReq = new SQLite_Req())
            {
                Constructeurs.ChangeContent = sqlReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(PProp.Default.T_Constructeurs, all:true)); ;
                Machines.ChangeContent = sqlReq.GetListOf<CT_Machine>(CT_Machine.Result2Class, new Obj_Select(PProp.Default.T_Machines, all:true)); ;

            }

        }

        private void Can_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine.ID != 0 & Constructeur.ID != 0;
        }

        private void Ex_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Machine.IDConstructeur = Constructeur.ID;
            
            DialogResult = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CboxConst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
