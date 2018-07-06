using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Defines integer file merger functionality
    /// </summary>
    public interface IIntegerFileMerger
    {
        /// <summary>
        /// Merges a set of integer files into one or more output files
        /// </summary>
        /// <remarks>
        /// The number of output files = ceiling(number of integer files / mergeCount).
        /// This method assumes that integerFiles != null, mergeCount > 0, fileTemplate != null,
        /// and outputDirectory != null.
        /// This method also assumes that the integer files exist, the output directory can be created,
        /// and that files can be written to the output directory.
        /// </remarks>
        /// <param name="integerFiles">Paths to the integer files to be merged</param>
        /// <param name="mergeCount">The number of integer files to merge at a time</param>
        /// <param name="fileTemplate">The file template of the output files. The file template
        /// must contain a "{0}" where an integer will be inserted</param>
        /// <param name="outputDirectory">The directory into which the output files will be written</param>
        /// <param name="updateProgress">A method that will be called to update chunk file creation progress. The 
        /// number of integers that have been merged so far will be passed to this method whenever an integer
        /// is merged.</param>
        /// <returns>The paths of the output files that were created</returns>
        List<string> MergeIntegerFiles(List<string> integerFiles, int mergeCount, string fileTemplate, string outputDirectory,
            Action<int> updateProgress = null);
    }
}
