using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Implements the chunk stream creation functionality
    /// </summary>
    public class ChunkStreamCreator : IChunkStreamCreator
    {
        /// <see cref="IChunkStreamCreator.CreateIntegerChunkGenerator(StreamReader, int)"/>
        public IEnumerable<List<int>> CreateIntegerChunkGenerator(StreamReader textStreamReader, int chunkSize)
        {
            //Assert the preconditions
            Debug.Assert(textStreamReader != null);
            Debug.Assert(chunkSize > 0);

            List<int> currentChunk = new List<int>();

            //Keep reading the stream until we hit the end
            while(!textStreamReader.EndOfStream)
            {
                string line = textStreamReader.ReadLine();

                //Attempt to parse the line as an integer
                int integer = Convert.ToInt32(line);

                //Add the integer to the current chunk
                currentChunk.Add(integer);

                //If the current chunk has reached the chunk size, yield it and start a new chunk
                if(currentChunk.Count == chunkSize)
                {
                    yield return currentChunk;

                    currentChunk = new List<int>();
                }
            }
        }
    }
}
