using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    /// <summary>
    /// Defines functionality for collecting integer file information
    /// </summary>
    public interface IIntegerFileInfoCollector
    {
        /// <summary>
        /// Retrieves integer file information for a particular file
        /// </summary>
        /// <remarks>
        /// This method assumes that the integer file exists and that it
        /// is an integer file, with a single integer on each line.
        /// This method also assumes that chunkSize > 0 and that filePath != null.
        /// </remarks>
        /// <param name="filePath">The path to an integer file</param>
        /// <param name="chunkSize">The chunk size being used</param>
        /// <returns>Information regarding the integer file</returns>
        IntegerFileInfo GetIntegerFileInfo(string filePath, int chunkSize);
    }
}
