using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    interface IChunkFileCreator
    {
        /// <summary>
        /// Creates one or more chunk files where each file contains a chunk of sorted integers
        /// </summary>
        /// <param name="sortedIntegerChunks">A collection of integer chunks, where each chunk has its integers
        /// sorted in ascending order</param>
        /// <param name="chunkFileTemplate">The file template of the chunk files to be generated. The file template
        /// must contain a "{0}" where an integer describing which chunk the file is associated with will be inserted</param>
        /// <param name="outputPath">The directory to which the chunk files will be written</param>
        /// <returns>A list of the names of the chunk files that were created</returns>
        List<string> CreateChunkFiles(IEnumerable<List<int>> sortedIntegerChunks, string chunkFileTemplate, string outputPath);
    }
}
