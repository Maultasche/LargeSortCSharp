﻿using System;
using System.Collections.Generic;
using System.IO;

namespace LargeSort.Shared
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
        /// <returns>A stream pointing to the file</returns>
        Stream CreateFile(string filePath);

        /// <summary>
        /// Creates a stream reader that reads from a particular file
        /// </summary>
        /// <remarks>
        /// This method assumes that the file exists.
        /// </remarks>
        /// <param name="filePath">The path to the file to be read</param>
        /// <returns>A stream reader that can read from the file</returns>
        StreamReader CreateFileStreamReader(string filePath);

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <remarks>
        /// If the file does not exist, nothing happens
        /// </remarks>
        /// <param name="filePath">The path to the file to be deleted</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Indicates if a file exists
        /// </summary>
        /// <param name="filePath">The path to a file</param>
        /// <returns>true if the file exists, otherwise false</returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Returns the directory portion of a file path
        /// </summary>
        /// <remarks>
        /// The file path does not have to be currently existing.
        /// </remarks>
        /// <param name="filePath">A path to a file</param>
        /// <returns>The directory portion of the file path</returns>
        string GetDirectoryFromFilePath(string filePath);

        /// <summary>
        /// Renames a file
        /// </summary>
        /// <remarks>
        /// A rename will fail if another file with that name already exists
        /// </remarks>
        /// <param name="filePath">The file to be renamed</param>
        /// <param name="newFileName">The new name to be given to the file</param>
        void RenameFile(string filePath, string newFileName);

        /// <summary>
        /// Writes an integer to a stream using a specific StreamWriter
        /// </summary>
        /// <remarks>
        /// The integer will be converted to its text form and a newline character will be written after the
        /// integer.
        /// This method assumes that integerStreamWriter != null.
        /// </remarks>
        /// <param name="integerStreamWriter">The stream writer to use to write an integer</param>
        /// <param name="integer">The integer to be written</param>
        void WriteIntegerToStream(StreamWriter integerStreamWriter, int integer);
    }
}
