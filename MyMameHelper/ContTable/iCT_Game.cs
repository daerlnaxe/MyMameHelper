using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public interface iCT_Games
    {
        uint ID { get; set; }


        string Game_Name { get; set; }
        string Description { get; set; }
 
        uint? Machine_Id { get; set; }

        bool? Unwanted { get; set; }

        uint? Developer_Id { get; set; }

        uint? Genre_Id { get; set; }

        bool? IsMahjong { get; set; }
        bool? IsQuizz { get; set; }

        uint? Rate { get; set; }
    }
}
