using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Methods
{
    public static class FindInGames
    {
        internal static Aff_Game GameByRoms(List<Aff_Game> gamesList, Func<CT_Rom, bool> test)
        {
            foreach (Aff_Game game in gamesList)
            {
                foreach(CT_Rom rom in game.Roms)
                {
                    if (test(rom))
                        return game;
                }
            }
            return null;
        }
    }
}
