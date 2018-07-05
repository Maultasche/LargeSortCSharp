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

        /// <see cref="IFileIO.RenameFile(string, string)"/>
        public void RenameFile(string filePath, string newFileName)
        {
            //Calculate the renamed file path
            string fileDirectory = Path.GetDirectoryName(filePath);
            string renamedFilePath = Path.Combine(fileDirectory, newFileName);

            //Rename it using a move operations
            File.Move(filePath, renamedFilePath);
        }

        public void WriteIntegerToStream(StreamWriter integerStreamWriter, int integer)
        {
            integerStreamWriter.WriteLine(integer);
        }
    }
}
