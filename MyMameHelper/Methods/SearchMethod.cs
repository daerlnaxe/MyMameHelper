using MyMameHelper.ContTable;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Methods
{

    public class SearchMethod
    {


        public ObservableCollection<CT_Game> CTGames(ObservableCollection<CT_Game>  games)
        {
            ObservableCollection<CT_Game> feed = new ObservableCollection<CT_Game>();
            Search searchBox = new Search();
            searchBox.Games = games;

            searchBox.ShowDialog();
            if (searchBox.DialogResult == true)
            {
                foreach (CT_Game g in searchBox.GamesFound)
                {
                    feed.Add(g);
                }
            }

            return feed;
        }
    }
}
