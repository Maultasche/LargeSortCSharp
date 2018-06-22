using System;
using System.Collections.Generic;

using LargeSort.Shared;

namespace IntSort
{
    /// <summary>
    /// Implements the chunk file creation functionality
    /// </summary>
    public class ChunkFileCreator : IChunkFileCreator
    {
        private IIntegerFileCreator integerFileCreator = null;

        /// <summary>
        /// Instantiates a new instance of ChunkFileCreate
        /// </summary>
        /// <param name="integerFileCreator">An integer file creator instance</param>
        public ChunkFileCreator(IIntegerFileCreator integerFileCreator)
        {
            this.integerFileCreator = integerFileCreator;
        }

        /// <see cref="IChunkFileCreator.CreateChunkFiles(IEnumerable{List{int}}, string, string)"/>
        /// <returns></returns>
        public List<string> CreateChunkFiles(IEnumerable<List<int>> sortedIntegerChunks, string chunkFileTemplate, 
            string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
