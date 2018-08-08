using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    /// <summary>
    /// Contains functionality for merging chunk files
    /// </summary>
    public interface IChunkFileMerger
    {
        /// <summary>
        /// Merges a set of integer chunk files into a single output file
        /// </summary>
        /// <remarks>
        /// This method runs all generations of merges, starting with the initial chunk files and calling 
        /// IIntegerFileMerger.MergeIntegerFile() for every merge generation until all the integers have 
        /// been merged into a single file.
        /// </remarks>
        /// <param name="chunkFiles">The chunk files to be merged</param>
        /// <param name="mergeCount">The maximum number of files to be merged at a time</param>
        /// <param name="intermediateFileTemplate">The template for intermediate files, which has a {0} placeholder
        /// for the generation number and a {1} placeholder for the file number</param>
        /// <param name="outputFile">The output file that the final merged results are to be written</param>
        /// <param name="outputDirectory">The output directory where the intermediate files and final output
        /// file are to be written</param>
        /// <param name="startingGeneration">The generation number to start with when merging</param>
        /// <param name="updateProgress">A method that will be called to update file merge progress. The 
        /// first parameter is the merge generation number and the second number is the number of integers that 
        /// have been merged as part of that generation.</param>
        /// <returns>The intermediate files that were written</returns>
        List<string> MergeChunkFilesIntoSingleFile(List<string> chunkFiles, int mergeCount, string intermediateFileTemplate,
            string outputFile, string outputDirectory, int startingGeneration, Action<int, int> updateProgress);
    }
}
