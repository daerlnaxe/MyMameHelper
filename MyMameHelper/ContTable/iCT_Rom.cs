using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public interface iCT_Rom
    {

        uint ID { get; set; }
        string Archive_Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        string Description { get; set; }

        string Year { get; set; }

        uint Manufacturer { get; set; }


        bool IsParent { get; set; }

        bool? Unwanted { get; set; }

        uint Clone_Of { get; set; }
    }
}
