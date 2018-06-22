using System;
using System.Collections.Generic;
using System.IO;

namespace LargeSort.Shared
{
    /// <summary>
    /// Implements the functionality for generating an integer file
    /// </summary>
    public class IntegerFileCreator : IIntegerFileCreator
    {
        IFileIO fileIO = null;

        /// <summary>
        /// Initializes an instances of IntegerFileCreator
        /// </summary>
        /// <param name="fileIO">The file I/O functionality</param>
        public IntegerFileCreator(IFileIO fileIO)
        {
            this.fileIO = fileIO;
        }

        /// <see cref="IIntegerFileCreator.CreateIntegerTextFile(IEnumerable{int}, string)"/>
        public void CreateIntegerTextFile(IEnumerable<int> integers, string filePath)
        {
            //Create the directory the file will be living in, if it does not already exist
            fileIO.CreateDirectory(fileIO.GetDirectoryFromFilePath(filePath));

            //Create the file
            using (Stream fileStream = fileIO.CreateFile(filePath))
            {
                //Create a stream writer from the file stream
                using (StreamWriter fileStreamWriter = new StreamWriter(fileStream))
                {
                    //Iterate over each integer and write it to the file
                    foreach(int integer in integers)
                    {
                        fileIO.WriteIntegerToStream(fileStreamWriter, integer);
                    }

                    //Flush the stream writer
                    fileStreamWriter.Flush();
                }
            }
        }
    }
}
