using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyMameHelper.Commands
{
    public static class UICommands
    {
        /// <summary>
        /// Load la page de travaille en fournissant la collection sur laquelle travailler
        /// </summary>
        public static readonly RoutedUICommand WorkCmd = new RoutedUICommand("Work","WorkCmd", typeof(UICommands));

        //public static readonly RoutedUICommand AddItem = new RoutedUICommand("Add Item", "AddItem", typeof(UICommands));
        //public static readonly RoutedUICommand RemoveItem = new RoutedUICommand("Remove Item", "RemoveItem", typeof(UICommands));

        public static readonly RoutedUICommand Select_AllCmd = new RoutedUICommand("Select All", "Select_AllCmd", typeof(UICommands));
        public static readonly RoutedUICommand Add_AllCmd = new RoutedUICommand("Add All", "Add", typeof(UICommands));

        public static readonly RoutedUICommand SearchCmd = new RoutedUICommand("Search", "SearchCmd",  typeof(UICommands));
        public static readonly RoutedUICommand ChangeCmd = new RoutedUICommand("Change", "ChangeCmd",  typeof(UICommands));
        public static readonly RoutedUICommand SaveCmd = new RoutedUICommand("Save", "SaveCmd",  typeof(UICommands));
        public static readonly RoutedUICommand SimulateCmd = new RoutedUICommand("Simulate", "SimulateCmd",  typeof(UICommands));

        // Vrac db
        public static readonly RoutedUICommand LoadTemp = new RoutedUICommand("Load From Temp", "LoadTemp", typeof(UICommands));
        public static readonly RoutedUICommand SaveTemp = new RoutedUICommand("Save To Temp", "SaveTemp", typeof(UICommands));
        public static readonly RoutedUICommand Add2Temp = new RoutedUICommand("Add to Temp", "Add2Temp", typeof(UICommands));
        public static readonly RoutedUICommand RemoveFTemp = new RoutedUICommand("Remove From Temp", "RemoveFTemp", typeof(UICommands));
        public static readonly RoutedUICommand FlushTemp = new RoutedUICommand("Flush Temp", "FlushTemp", typeof(UICommands));
        
        // Games db
        public static readonly RoutedUICommand LoadGamesCmd = new RoutedUICommand("Load Games Table", "LoadGamesCmd", typeof(UICommands));
        public static readonly RoutedUICommand SaveGamesCmd = new RoutedUICommand("Save Games Table", "SaveGamesCmd", typeof(UICommands));
        public static readonly RoutedUICommand EditGameCmd = new RoutedUICommand("Edit Game Table", "EditGameCmd", typeof(UICommands));
        
        
        
        //
        public static readonly RoutedUICommand SaveDb = new RoutedUICommand("Save Table", "SaveDb", typeof(UICommands));
        public static readonly RoutedUICommand UpdateDb = new RoutedUICommand("Update Table", "UpdateDb", typeof(UICommands));
        public static readonly RoutedUICommand DeleteFromDb = new RoutedUICommand("Delete From Database", "DeleteFromDb", typeof(UICommands));
        public static readonly RoutedUICommand Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(UICommands));
        
        // Constructor
        public static readonly RoutedUICommand AddConstructor = new RoutedUICommand("Add a Constructor", "AddConstructor", typeof(UICommands));
        public static readonly RoutedUICommand EditConstructor = new RoutedUICommand("Edit a Constructor", "EditConstructor", typeof(UICommands));
        public static readonly RoutedUICommand RemoveConstructor = new RoutedUICommand("Remove a Constructor", "RemoveConstructor", typeof(UICommands));
        // Genre
        public static readonly RoutedUICommand AddGenre = new RoutedUICommand("Add a Genre", "AddGenre", typeof(UICommands));
        public static readonly RoutedUICommand EditGenre = new RoutedUICommand("Edit a Genre", "EditGenre", typeof(UICommands));
        public static readonly RoutedUICommand RemoveGenre = new RoutedUICommand("Remove a Genre", "RemoveGenre", typeof(UICommands));
        // Machine
        public static readonly RoutedUICommand AddMachine = new RoutedUICommand("Add a Machine", "AddMachine", typeof(UICommands));
        public static readonly RoutedUICommand EditMachine = new RoutedUICommand("Edit a Machine", "EditMachine", typeof(UICommands));
        public static readonly RoutedUICommand LinkAMachine = new RoutedUICommand("Link a Machine", "LinkAMachine", typeof(UICommands));
        public static readonly RoutedUICommand RemoveMachine = new RoutedUICommand("Remove a Machine", "RemoveMachine", typeof(UICommands));
        // Manufacturer
        public static readonly RoutedUICommand AddDevelopers = new RoutedUICommand("Add Developers", "AddDevelopers", typeof(UICommands));
        public static readonly RoutedUICommand EditDeveloper = new RoutedUICommand("Edit a Developer", "EditDeveloper", typeof(UICommands));
        public static readonly RoutedUICommand RemoveDeveloper = new RoutedUICommand("Remove a Developer", "RemoveDeveloper", typeof(UICommands));
        #region Roms
        public static readonly RoutedUICommand AddRom = new RoutedUICommand("Add a Rom", "AddRom", typeof(UICommands));
        public static readonly RoutedUICommand AddRoms = new RoutedUICommand("Add Roms", "AddRoms", typeof(UICommands));
        public static readonly RoutedUICommand EditRom = new RoutedUICommand("Edit a Rom", "EditRom", typeof(UICommands));
        public static readonly RoutedUICommand RemoveRom = new RoutedUICommand("Remove a Rom", "RemoveRom", typeof(UICommands));
        #endregion
        #region Games
        public static readonly RoutedUICommand AddGame = new RoutedUICommand("Add a Game", "AddGame", typeof(UICommands));
        public static readonly RoutedUICommand EditGame = new RoutedUICommand("Edit a Game", "EditGame", typeof(UICommands));
        public static readonly RoutedUICommand RemoveGame = new RoutedUICommand("Remove a Game", "RemoveGame", typeof(UICommands));
        public static readonly RoutedUICommand LaunchGame = new RoutedUICommand("Launch Game", "LaunchGame", typeof(UICommands));
        public static readonly RoutedUICommand LaunchMame = new RoutedUICommand("Launch Mame", "LaunchMame", typeof(UICommands));

        #endregion

        // Swaps
        public static readonly RoutedUICommand SwapLeft2Right = new RoutedUICommand("Swap Left To Right", "SwapLeft2Right", typeof(UICommands));
        public static readonly RoutedUICommand SwapRight2Left = new RoutedUICommand("Swap Right To Left", "SwapLeft2Right", typeof(UICommands));

        public static readonly  RoutedUICommand ResetLeft = new RoutedUICommand("Reset Left", "ResetLeft",  typeof(UICommands));
        public static readonly  RoutedUICommand ResetRight = new RoutedUICommand("Reset Right", "ResetRight",  typeof(UICommands));

        // Files
        public static readonly  RoutedUICommand Proceed = new RoutedUICommand("Proceed with Roms", "Proceed",  typeof(UICommands));

        // Type Of roms
        public static readonly RoutedUICommand BiosRoms_Preview = new RoutedUICommand("Preview Bios", "PreviewBios", typeof(UICommands));
        public static readonly RoutedUICommand GamesRoms_Preview = new RoutedUICommand("Preview Games", "GamesRoms_Preview", typeof(UICommands));
        public static readonly RoutedUICommand MecaRoms_Preview = new RoutedUICommand("Preview Mechanical", "MecaRoms_Preview", typeof(UICommands));

    }

}
