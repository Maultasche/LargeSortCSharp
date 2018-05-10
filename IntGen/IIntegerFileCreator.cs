using System;
using System.Collections.Generic;
using System.Text;

namespace IntGen
{
    /// <summary>
    /// Defines the functionality for generating an integer file
    /// </summary>
    public interface IIntegerFileCreator
    {
        /// <summary>
        /// Creates an integer text file with each integer on its own line
        /// </summary>
        /// <remarks>
        /// If any file already exists at the specified file path, it will be overwritten.
        /// If any directory in the file path does not exist, it will be created
        /// </remarks>
        /// <param name="integers">The integers to write to the file</param>
        /// <param name="filePath">The path to the file where the integers will be written</param>
        void CreateIntegerTextFile(IEnumerable<int> integers, string filePath);
    }
}
