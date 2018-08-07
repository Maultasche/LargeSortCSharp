using System;
using System.Collections.Generic;
using System.Text;

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
            ConsoleInfo currentSettings = new ConsoleInfo()
            {
                BackgroundColor = Console.BackgroundColor,
                CursorSize = Console.CursorSize,
                CursorVisible = Console.CursorVisible,
                ForegroundColor = Console.ForegroundColor
            };

            return currentSettings;
        }

        /// <summary>
        /// Restores a set of console settings to the global console
        /// </summary>
        /// <param name="consoleSettings">The console settings to be restored</param>
        public static void RestoreSettings(ConsoleInfo consoleSettings)
        {
            Console.BackgroundColor = consoleSettings.BackgroundColor;
            Console.CursorSize = consoleSettings.CursorSize;
            Console.CursorVisible = consoleSettings.CursorVisible;
            Console.ForegroundColor = consoleSettings.ForegroundColor;
        }
    }
}
