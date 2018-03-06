﻿using System;
using System.IO;

using LargeSort.Shared.Interfaces;

namespace LargeSort.Shared.Implementations
{
    /// <summary>
    /// Implements file I/O functionality
    /// </summary>
    public class FileIO : IFileIO
    {
        /// <see cref="IFileIO.CreateDirectory(string)"/>
        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        /// <see cref="IFileIO.GetDirectoryFromFilePath(string)"/>
        public string GetDirectoryFromFilePath(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
    }
}
