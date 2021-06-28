using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LargeSort.Shared
{
    /// <summary>
    /// Stores information about the current console
    /// </summary>
    public class ConsoleInfo
    {
        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the cursor size
        /// </summary>
        public int CursorSize { get; set; }

        /// <summary>
        /// Gets or sets whether the cursor is visible
        /// </summary>
        public bool CursorVisible { get; set; }

        /// <summary>
        /// Gets or sets the foreground color
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Extracts the current console settings and saves them in a ConsoleInfo object
        /// </summary>
        /// <returns>The current console settings</returns>
        public static ConsoleInfo CurrentInfo()
        {
            var color = Console.BackgroundColor;

            ConsoleInfo currentSettings = new ConsoleInfo()
            {
                BackgroundColor = Console.BackgroundColor,                
                ForegroundColor = Console.ForegroundColor
            };

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                currentSettings.CursorVisible = Console.CursorVisible;
                currentSettings.CursorSize = Console.CursorSize;
            }

            return currentSettings;
        }

        /// <summary>
        /// Restores a set of console settings to the global console
        /// </summary>
        /// <param name="consoleSettings">The console settings to be restored</param>
        public static void RestoreSettings(ConsoleInfo consoleSettings)
        {
            Console.BackgroundColor = consoleSettings.BackgroundColor;            
            Console.ForegroundColor = consoleSettings.ForegroundColor;

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.CursorSize = consoleSettings.CursorSize;
                Console.CursorVisible = consoleSettings.CursorVisible;
            }
            else
            {
                Process.Start("tput", "cnorm -- normal");
            }
        }
    }
}
