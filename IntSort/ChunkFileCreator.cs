using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            List<string> chunkFiles = new List<string>();

            int chunkNum = 1;

            sortedIntegerChunks.ToList().ForEach(chunk =>
            {
                //Create the file name for this chunk
                string chunkFileName = string.Format(chunkFileTemplate, chunkNum);

                //Create the chunk file containing the chunk
                integerFileCreator.CreateIntegerTextFile(chunk, Path.Combine(outputPath, chunkFileName));

                chunkFiles.Add(chunkFileName);

                chunkNum++;
            });

            return chunkFiles;
        }
    }
}
