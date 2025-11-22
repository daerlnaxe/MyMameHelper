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
    public partial class RawRomSearch : Window, INotifyPropertyChanged
    {
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MyObservableCollection<RawMameRom> RomsFound { get; set; } = new MyObservableCollection<RawMameRom>();

        private string _LastOrder;

        public string TypeRecherche { get; set; }

        public string Archive_Param { get; set; }
        public string Description { get; set; }


        public MyObservableCollection<CT_Machine> Machines { get; set; } = new MyObservableCollection<CT_Machine>();
        public MyObservableCollection<string> Source_Files { get; set; } = new MyObservableCollection<string>();
        public MyObservableCollection<string> Developers { get; set; } = new MyObservableCollection<string>();


        public RawRomSearch()
        {
            InitializeComponent();
            DataContext = this;

            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Obj_Select objDev = new Obj_Select(PProp.Default.T_TempRoms, colonnes: new string[] { "Manufacturer" }, groups: new string[] { "Manufacturer" });
                objDev.AddConds(new SqlCond("Manufacturer", eWhere.Not_Like, "Null"));

                Developers.ChangeContent = sqReq.GetStringOf(objDev);


                Obj_Select objSourceF = new Obj_Select(PProp.Default.T_TempRoms, colonnes: new string[] { "Source_File" }, groups: new string[] { "Source_File" });
                objSourceF.AddConds(new SqlCond("Source_File", eWhere.Not_Like, "Null"));

                Source_Files.ChangeContent = sqReq.GetStringOf(objSourceF);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        #region menu


        #endregion

        /*
        private void cbConstruct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int idConstruct = Convert.ToInt32(cb.SelectedValue);

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                Machines.ChangeContent = sqReq.GetListOf(
                   CT_Machine.Result2Class,
                   new Obj_Select(table: PProp.Default.T_Machines, colonnes: new string[] { "ID", "Nom" }, conditions: new SqlCond[] { new SqlCond("Constructeur", eWhere.Equal, idConstruct.ToString()) }, orders: new SqlOrder("Nom"))
                   );
            }
        }*/

        private void Ex_Search(object sender, ExecutedRoutedEventArgs e)
        {
            List<SqlCond> conditions = new List<SqlCond>();

            if (!string.IsNullOrEmpty(Archive_Param))
            {
                SqlCond archCond = new SqlCond("Name", eWhere.Like, Archive_Param);
                conditions.Add(archCond);
            }

            if (!string.IsNullOrEmpty(Description))
            {
                SqlCond descCond = new SqlCond("Description", eWhere.Like, Description);
                conditions.Add(descCond);
            }

            if (cboxConstruct.SelectedItem != null)
            {
                string sourceDev = (string)cboxConstruct.SelectedItem;
                SqlCond devCond = new SqlCond("Manufacturer", eWhere.Like, sourceDev);
                conditions.Add(devCond);
            }


            if (cboxSourceF.SelectedItem != null)
            {
                string sourceFSelected = (string)cboxSourceF.SelectedItem;
                SqlCond sourceCond = new SqlCond("Source_File", eWhere.Like, sourceFSelected);
                conditions.Add(sourceCond);
            }

            /*
            if(IdDev != 0)
            {
                SqlCond devCond = new SqlCond("Manufacturer", eWhere.Equal, IdDev);
                conditions.Add(devCond);
            }*/

            int i = 0;
            foreach (SqlCond cond in conditions)
            {
                if (i > 0)
                    cond.Link = Linker.And;
                i++;
            }

            SqlCond[] sqlConds = conditions.Count == 0 ? null : conditions.ToArray();


            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Obj_Select objSel = new Obj_Select(PProp.Default.T_TempRoms, all: true);
                objSel.AddConds(sqlConds);
                objSel.AddOrders(new SqlOrder("Name"));

                RomsFound.ChangeContent = sqReq.GetListOf<RawMameRom>(RawMameRom.Result2Class, objSel);

            }
        }

        private void btFeed_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Ex_Remove(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<RawMameRom> toDel = dgFound.SelectedItems.Cast<RawMameRom>();

            RomsFound.RemoveSilentRange(toDel);
            RomsFound.SignalChange();

        }

        private void Can_Remove(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RomsFound.Count > 0;
        }
    }
}
