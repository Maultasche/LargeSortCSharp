using System;
using System.IO;

namespace LargeSort.Shared
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

        /// <see cref="IFileIO.CreateFile(string)"/>
        public Stream CreateFile(string filePath)
        {
            return File.Create(filePath);
        }

        /// <see cref="IFileIO.CreateFileStreamReader(string)"/>
        public StreamReader CreateFileStreamReader(string filePath)
        {
            StreamReader fileStreamReader = new StreamReader(filePath);

            return fileStreamReader;
        }

        /// <see cref="IFileIO.FileExists(string)"/>
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <see cref="IFileIO.GetDirectoryFromFilePath(string)"/>
        public string GetDirectoryFromFilePath(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public void WriteIntegerToStream(StreamWriter integerStreamWriter, int integer)
        {
            integerStreamWriter.WriteLine(integer);
        }
    }
}
