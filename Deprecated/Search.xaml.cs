using MyMameHelper.ContTable;
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

namespace MyMameHelper.Windows
{
    /// <summary>
    /// Logique d'interaction pour Search.xaml
    /// </summary>
    public partial class Search : Window, INotifyPropertyChanged
    {
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<CT_Game> Games { get; set; }
        private ObservableCollection<CT_Game> _GamesFound { get; set; } = new ObservableCollection<CT_Game>();
        public ObservableCollection<CT_Game> GamesFound
        {
            get { return _GamesFound; }
            set
            {
                if (value != _GamesFound)
                {
                    _GamesFound = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _LastOrder;

        public string TypeRecherche { get; set; }

        public Search()
        {


            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Games == null)
                throw new Exception("Games is empty");
        }


        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {

            GamesFound.Clear();

            switch (TypeRecherche)
            {
                case "Archive Name":
                    Search_InArchive();
                    if (lvFound.Visibility == Visibility.Hidden)
                        lvFound.Visibility = Visibility.Visible;
                    break;
                case "Game Name":
                    Search_InGame();
                    if (lvFound.Visibility == Visibility.Hidden)
                        lvFound.Visibility = Visibility.Visible;
                    break;

                default:
                    lvFound.Visibility = Visibility.Hidden;
                    return;
            }


        }

        private void Search_InArchive()
        {
            string toSearch = tbSearch.Text;
            for (int i = 0; i < Games.Count; i++)
            {
                CT_Game game = Games[i];

                if (cbIgnoreCase.IsChecked == true && game.Parent_Name.IndexOf(toSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    GamesFound.Add(game);

                }
                else if (game.Parent_Name.IndexOf(toSearch) >= 0)
                {
                    GamesFound.Add(game);
                    //Games.RemoveAt(i);
                }
            }
            _LastOrder = "AN_Asc";
        }


        private void Search_InGame()
        {
            string toSearch = tbSearch.Text;
            for (int i = 0; i < Games.Count; i++)
            {
                CT_Game game = Games[i];

                if (cbIgnoreCase.IsChecked == true && game.Game_Name.IndexOf(toSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    GamesFound.Add(game);

                }
                else if (game.Game_Name.IndexOf(toSearch) >= 0)
                {
                    GamesFound.Add(game);
                    //Games.RemoveAt(i);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            TypeRecherche = rb.Content.ToString();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var colClicked = e.OriginalSource as GridViewColumnHeader;

            if (colClicked.Content.ToString() == "Game Name")
            {
                if (_LastOrder == "GN_Asc")
                {
                    GamesFound = new ObservableCollection<CT_Game>(GamesFound.OrderByDescending(x => x.Game_Name));
                    _LastOrder = "GN_Desc";
                }
                else
                {
                    GamesFound = new ObservableCollection<CT_Game>(GamesFound.OrderBy(x => x.Game_Name));
                    _LastOrder = "GN_Asc";
                }
            }

            if (colClicked.Content.ToString() == "Archive Name")
            {
                if (_LastOrder == "AN_Asc")
                {
                    GamesFound = new ObservableCollection<CT_Game>(GamesFound.OrderByDescending(x => x.Parent_Name));
                    _LastOrder = "AN_Desc";
                }
                else
                {
                    GamesFound = new ObservableCollection<CT_Game>(GamesFound.OrderBy(x => x.Parent_Name));
                    _LastOrder = "AN_Asc";

                }

            }
        }

        #region menu
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            IList items = lvFound.SelectedItems;
            List<CT_Game> toDel = new List<CT_Game>();
            for (int i = 0; i < items.Count; i++)
            {
                CT_Game g = (CT_Game)items[i];
                toDel.Add(g);
            }

            foreach(CT_Game g in toDel)            
                GamesFound.Remove(g);
            
        }

        #endregion

        private void btFeed_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
