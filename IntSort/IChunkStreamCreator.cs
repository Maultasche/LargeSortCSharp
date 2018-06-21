using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort
{
    interface IChunkStreamCreator
    {
        /// <summary>
        /// Creates a generator that produces chunks of integers based on text in a stream reader
        /// </summary>
        /// <remarks>
        /// This method assumes that the text in the stream reader will consist of only integers, where
        /// each integer is separated by a newline, so each integer would be on its own line.
        /// The stream reader will be read from as the IEnumerable is iterated over, which will happen
        /// after this method has been called. So don't close the stream reader until the resulting
        /// chunk iterable has been iterated over. 
        /// As a result of this behavior, only one chunk of integers should be in memory at once.
        /// </remarks>
        /// <param name="textStreamReader">The stream reader to read from</param>
        /// <param name="chunkSize">The number of integers that will comprise each chunk</param>
        /// <returns>A iterable that will read from the stream reader and create integer chunks as
        /// it is being iterated over.</returns>
        IEnumerable<List<int>> CreateIntegerFileStreamGenerator(StreamReader textStreamReader, int chunkSize);
    }
}
