using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyMameHelper.Commands
{
    public static class FileCommands
    {
        public static readonly RoutedUICommand Load_File = new RoutedUICommand("Load_File", "Load_File", typeof(FileCommands));


    }
}
