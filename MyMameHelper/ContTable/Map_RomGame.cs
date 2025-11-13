using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class Map_RomGame : CT_Rom
    {
        private string _Game_Name;
        public string Game_Name
        {
            get => _Game_Name;
            set
            {
                if (value != _Game_Name)
                {
                    _Game_Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
