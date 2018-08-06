using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using LargeSort.Shared;

namespace IntSort
{
    /// <summary>
    /// Implements functionality for collecting integer file information
    /// </summary>
    public class IntegerFileInfoCollector : IIntegerFileInfoCollector
    {
        private IFileIO fileIO = null;

        /// <summary>
        /// Instantiates an instance of IntegerFileInfoCollector
        /// </summary>
        /// <param name="fileIO">File I/O functionality</param>
        public IntegerFileInfoCollector(IFileIO fileIO)
        {
            this.fileIO = fileIO;
        }

        /// <see cref="IIntegerFileInfoCollector.GetIntegerFileInfo(string, int)"/>
        public IntegerFileInfo GetIntegerFileInfo(string filePath, int chunkSize)
        {
            Debug.Assert(filePath != null);
            Debug.Assert(chunkSize > 0);

            IntegerFileInfo fileInfo = new IntegerFileInfo()
            {
                NumOfChunks = 0,
                NumOfIntegers = 0
            };

            //Open a text reader to the file
            using (StreamReader fileReader = fileIO.CreateFileStreamReader(filePath))
            {
                while(!fileReader.EndOfStream)
                {
                    //Read each line to count the number of lines in the file
                    fileReader.ReadLine();

                    fileInfo.NumOfIntegers++;
                }
            }

            //Now that we have the number of integers, calculate the number of chunks
            fileInfo.NumOfChunks = (int)Math.Ceiling(Decimal.Divide(fileInfo.NumOfIntegers, chunkSize));

            return fileInfo;
        }
    }
}
