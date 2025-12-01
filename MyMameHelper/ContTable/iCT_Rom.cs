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
        string SourceFile { get; set; }

        string Year { get; set; }

        uint? Game_Id { get; set; }
        uint? Machine_Id { get; set; }

        //uint Manufacturer { get; set; }
        CT_MameManufacturer Manufacturer { get; set; }


        bool IsParent { get; set; }

        bool? Unwanted { get; set; }

        uint Clone_Of { get; set; }

        bool IsPinball { get; set; }  
    }
}
