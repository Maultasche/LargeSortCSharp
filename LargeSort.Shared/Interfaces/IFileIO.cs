using System;
using System.IO;

namespace LargeSort.Shared.Interfaces
{
    /// <summary>
    /// Provides the interface to file I/O functionality
    /// </summary>
    public interface IFileIO
    {
        /// <summary>
        /// Creates a directory
        /// </summary>
        /// <remarks>
        /// If the directory already exists, nothing happens
        /// </remarks>
        /// <param name="directoryPath">The path to the directory to be created</param>
        void CreateDirectory(string directoryPath);

        /// <summary>
        /// Creates a file
        /// </summary>
        /// <remarks>
        /// If the file already exists, it is overwritten
        /// </remarks>
        /// <param name="filePath">The path to the file to be created</param>
        FileStream CreateFile(string filePath);

        /// <summary>
        /// Returns the directory portion of a file path
        /// </summary>
        /// <remarks>
        /// The file path does not have to be currently existing.
        /// </remarks>
        /// <param name="filePath">A path to a file</param>
        /// <returns>The directory portion of the file path</returns>
        string GetDirectoryFromFilePath(string filePath);
    }
}
