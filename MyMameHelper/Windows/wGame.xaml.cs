using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour EditGame.xaml
    /// </summary>
    public partial class wGame : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Aff_Game _Game;
        public Aff_Game Game
        {
            get { return _Game; }
            set
            {
                if (value != _Game)
                {
                    _Game = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Aff_Game _OldGame;

        public uint? SelIndexGenre { get; set; }

        public uint? SelIndexMachine { get; set;}
        
        public MyObservableCollection<CT_Genre> Genres { get; set; } = new MyObservableCollection<CT_Genre>();

        public wGame()
        {
            using (SQLite_Op sqReq = new SQLite_Op())
            {
                Genres.ChangeContent = sqReq.GetListOf<CT_Genre>(CT_Genre.Result2Class, new Obj_Select(table: PProp.Default.T_Genres, all: true));
                /*
                Constructeurs.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Constructeurs, all: true));
                Developers.ChangeContent = sqReq.GetListOf<CT_Constructeur>(CT_Constructeur.Result2Class, new Obj_Select(table: PProp.Default.T_Companies, all: true));
                */

            }

            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }





    }
}
