using System;
using System.Collections.Generic;
using System.Linq;

namespace IntSort
{
    /// <summary>
    /// Extends the functionality of IEnumerable
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Divides a collection into multiple collections (chunks) of a particular size
        /// </summary>
        /// <typeparam name="T">The collection element type</typeparam>
        /// <param name="source">The collection to be divided</param>
        /// <param name="count">The number of elements in each chunk</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int count)
        {
            List<List<T>> chunks = new List<List<T>>();

            using (var enumerator = source.GetEnumerator())
            {
                bool moreElements = enumerator.MoveNext();

                while(moreElements)
                {
                    var chunkTuple = GetChunk(enumerator, count);

                    chunks.Add(chunkTuple.Item1.ToList());

                    moreElements = chunkTuple.Item2;
                }
                
            }

            return chunks;
        }

        /// <summary>
        /// Retrieves a chunk of elements from an enumerator
        /// </summary>
        /// <remarks>
        /// This method assumes that the enumerator has not come to the end of the collection.
        /// </remarks>
        /// <typeparam name="T">The enumerator element type</typeparam>
        /// <param name="source">The enumerator to use for iterating over the elements</param>
        /// <param name="chunkSize">The size of the chunks to be created</param>
        /// <returns>A tuple where the first item contains the chunk and the second item is a
        /// boolean that indicates whether there are more elements to be iterated over</returns>
        private static Tuple<IEnumerable<T>, bool> GetChunk<T>(IEnumerator<T> source, int chunkSize)
        {
            List<T> chunk = new List<T>();

            bool moreElements = true;

            for(int elementCount = 0; elementCount < chunkSize && moreElements; elementCount++)
            {
                chunk.Add(source.Current);

                moreElements = source.MoveNext();
            }

            return Tuple.Create(chunk as IEnumerable<T>, moreElements);
        }
    }
}
