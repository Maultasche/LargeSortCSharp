using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    /// <summary>
    /// The interface for chunk file creation functionality
    /// </summary>
    public interface IChunkFileCreator
    {
        /// <summary>
        /// Creates one or more chunk files where each file contains a chunk of sorted integers
        /// </summary>
        /// <remarks>
        /// This method assumes that sortedIntegerChunks != null.
        /// If sortedIntegerChunks is empty, no chunk files will be created.
        /// </remarks>
        /// <param name="sortedIntegerChunks">A collection of integer chunks, where each chunk has its integers
        /// sorted in ascending order</param>
        /// <param name="chunkFileTemplate">The file template of the chunk files to be generated. The file template
        /// must contain a "{0}" where an integer describing which chunk the file is associated with will be inserted</param>
        /// <param name="outputPath">The directory to which the chunk files will be written</param>
        /// <param name="updateProgress">A method that will be called to update chunk file creation progress. The 
        /// number of files that have been created so far will be passed to this method whenever a chunk file
        /// is written.</param>
        /// <returns>A list of the names of the chunk files that were created</returns>
        List<string> CreateChunkFiles(IEnumerable<List<int>> sortedIntegerChunks, string chunkFileTemplate, string outputPath,
            Action<int> updateProgress = null);
    }
}
