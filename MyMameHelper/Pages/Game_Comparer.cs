using System;
using System.Collections.Generic;
using MyMameHelper.ContTable;

namespace MyMameHelper.Pages
{
    internal class Game_Comparer : IEqualityComparer<CT_Game>
    {
        public bool Equals(CT_Game x, CT_Game y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;
            //Check whether any of the compared objects is null.

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Archive_Name == y.Archive_Name && x.Game_Name == y.Game_Name;
        }

        public int GetHashCode(CT_Game game)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(game, null)) return 0;

            //Get hash code
            int hashArchive_Name = game.Archive_Name == null ? 0 : game.Archive_Name.GetHashCode();
            int hashGame_Name = game.Game_Name == null ? 0 : game.Game_Name.GetHashCode();

            return hashArchive_Name ^ hashGame_Name;
        }
    }
}