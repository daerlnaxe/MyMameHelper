using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    public class M_GameType: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private List<CT_Rom> _RomList = new List<CT_Rom>();
        /// <summary>
        /// Clones
        /// </summary>
        public List<CT_Rom> Roms 
        {
            get => _RomList;
            set 
            {
                if (_RomList != value) 
                { 
                    _RomList = value;
                    NotifyPropertyChanged();    
                    
                }
            
            }
        }

        /// <summary>
        /// Description corresponding to parent rom description
        /// </summary>
        public string Description { get; set; }
    }
}
