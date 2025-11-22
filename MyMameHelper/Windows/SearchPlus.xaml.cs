using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.ListView;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour Search.xaml
    /// </summary>
    public partial class SearchPlus : Window, INotifyPropertyChanged
    {
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MyObservableCollection<Aff_Game> GamesFound { get; set; } = new MyObservableCollection<Aff_Game>();

        private string _LastOrder;

        public string TypeRecherche { get; set; }

        public string Archive_Param { get; set; }
        public string Game_Param { get; set; }
        public MyObservableCollection<CT_Constructeur> Constructeurs { get; set; } = new MyObservableCollection<CT_Constructeur>();
        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();




        public SearchPlus()
        {
            InitializeComponent();
            DataContext = this;

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Manufacturers, all: true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        #region menu


        #endregion


        private void cbConstruct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int idConstruct = Convert.ToInt32(cb.SelectedValue);

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Obj_Select obj_Select = new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" });
                obj_Select.AddConds(new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()));
                obj_Select.AddOrders(new SqlOrder("Nom"));


                Machines.ChangeContent = sqReq.GetListOf(CT_Machine.Result2Class, obj_Select);
            }
        }

        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            List<SqlCond> conditions = new List<SqlCond>();

            if (!string.IsNullOrEmpty(Archive_Param))
            {
                SqlCond archCond = new SqlCond("Archive_Name", eWhere.Like, Archive_Param);
                conditions.Add(archCond);
            }

            if (!string.IsNullOrEmpty(Game_Param))
            {
                SqlCond gameCond = new SqlCond("Game_Name", eWhere.Like, Game_Param);
                conditions.Add(gameCond);
            }

            if (cbMachine.SelectedItem != null)
            {
                CT_Machine selMachine = (CT_Machine)cbMachine.SelectedItem;
                SqlCond machineCond = new SqlCond("Machine", eWhere.Equal, selMachine.ID);
                conditions.Add(machineCond);
            }

            int i = 0;
            foreach (SqlCond cond in conditions)
            {
                if (i > 0)
                    cond.Link = Linker.And;
                i++;
            }

            SqlCond[] sqlConds = conditions.Count == 0 ? null : conditions.ToArray();

            throw new NotImplementedException("A revoir");
            /* 
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                GamesFound.ChangeContent = sqReq.AffGames_List(sqlConds, new SqlOrder(new string[] { "Game_Name" }));
            }*/
        }

        private void btFeed_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }



    }
}
