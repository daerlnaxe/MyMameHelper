using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyMameHelper.Pages
{
    /// <summary>
    /// Logique d'interaction pour pMapGames.xaml
    /// </summary>
    public partial class pMapGames : Page
    {
        public MyObservableCollection<CT_Rom> RomsToMap { get; set; } = new MyObservableCollection<CT_Rom>();


        public pMapGames()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Chargement 
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                /*Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Developers, all: true));
                /*Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));*/
                RomsToMap.ChangeContent = sqReq.AffRoms_List();
            }

        }
    }
}
