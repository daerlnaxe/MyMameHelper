using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class M_TypeRom
    {
        /// <summary>
        /// Clones
        /// </summary>
        public List<CT_Rom> Roms { get; set; } = new List<CT_Rom>();

        /// <summary>
        /// Description corresponding to parent rom description
        /// </summary>
        public string Description { get; set; }
    }
}
