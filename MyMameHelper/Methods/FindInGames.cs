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
        internal static CT_Game_Mapped GameByRoms(List<CT_Game_Mapped> gamesList, Func<CT_Rom, bool> test)
        {
            foreach (CT_Game_Mapped game in gamesList)
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
