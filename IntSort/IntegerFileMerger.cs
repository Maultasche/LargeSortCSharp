using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using LargeSort.Shared;

namespace IntSort
{
    /// <summary>
    /// Implements integer file merger functionality
    /// </summary>
    public class IntegerFileMerger : IIntegerFileMerger
    {
        IFileIO fileIO = null;
        IIntegerStreamMerger integerStreamMerger = null;

        /// <summary>
        /// Constructs a new instance of IntegerFileMerger
        /// </summary>
        /// <param name="fileIO">An instance of the file I/O functionality</param>
        /// <param name="integerStreamMerger">An instance of the integer stream merger</param>
        public IntegerFileMerger(IFileIO fileIO, IIntegerStreamMerger integerStreamMerger)
        {
            this.fileIO = fileIO;
            this.integerStreamMerger = integerStreamMerger;
        }

        /// <see cref="IIntegerFileMerger.MergeIntegerFiles(List{string}, int, string, string, Action{int})"/>
        public List<string> MergeIntegerFiles(List<string> integerFiles, int mergeCount, string fileTemplate, string outputDirectory,
            Action<int> updateProgress)
        {
            Debug.Assert(integerFiles != null);
            Debug.Assert(mergeCount > 0);
            Debug.Assert(fileTemplate != null);
            Debug.Assert(outputDirectory != null);

            List<string> outputFiles = new List<string>();

            //Create the output directory if it doesn't already exist
            fileIO.CreateDirectory(outputDirectory);

            //Group the integer files into merge groups using mergeCount
            List<List<string>> mergeFileGroups = integerFiles
                .Chunk(mergeCount)
                .Select(fileGroup => fileGroup.ToList())
                .ToList();

            int mergeFileGroupNum = 1;
            int totalIntegersUpdated = 0;

            //If an updateProgress method was provided, create another method that will be passed to the 
            //file group merge process. This will sum up the integers merged over all the merge processes and call
            //the updateProgress method with the total.
            Action<int> updateGroupMergeProgress = updateProgress == null ? updateProgress :
                integersUpdated => 
                {
                    totalIntegersUpdated++;

                    updateProgress.Invoke(totalIntegersUpdated);
                };

            //Merge each file group
            mergeFileGroups.ForEach(fileGroup =>
            {
                string outputFilePath = MergeInputFileGroup(fileGroup, outputDirectory, fileTemplate, mergeFileGroupNum, 
                    updateGroupMergeProgress);

                outputFiles.Add(outputFilePath);

                mergeFileGroupNum++;
            });


            return outputFiles;
        }

        /// <summary>
        /// Merges an input file group into a single output file
        /// </summary>
        /// <param name="fileGroup">The names of the input files to be merged</param>
        /// <param name="outputDirectory">The directory the output file is to be written in</param>
        /// <param name="fileTemplate">The template for the output file</param>
        /// <param name="fileGroupNumber">The file group number</param>
        /// <param name="updateProgress">A method that will be called to update chunk file creation progress. The 
        /// number of integers that have been merged so far in the file group will be passed to this method 
        /// whenever an integer is merged.</param>
        /// <returns>The name of the output file that was written</returns>
        private string MergeInputFileGroup(List<string> fileGroup, string outputDirectory, string fileTemplate, 
            int fileGroupNumber, Action<int> updateProgress = null)
        {
            //Create the input stream readers
            List<StreamReader> inputStreamReaders = fileGroup
                .Select(file => fileIO.CreateFileStreamReader(file))
                .ToList();

            //Create the output file and output file stream writer
            string outputFilePath = Path.Combine(outputDirectory, string.Format(fileTemplate, fileGroupNumber));
            using (Stream outputFileStream = fileIO.CreateFile(outputFilePath))
            using (StreamWriter outputStreamWriter = new StreamWriter(outputFileStream))
            {
                //Merge the input streams into the output stream
                integerStreamMerger.MergeIntegerStreams(inputStreamReaders, outputStreamWriter, updateProgress);
            }

            //Close the input stream readers
            inputStreamReaders.ForEach(inputStreamReader => inputStreamReader.Close());

            return outputFilePath;
        }
    }
}
