using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Defines functionality for reading an integer file
    /// </summary>
    public interface IIntegerStreamReader
    {
        /// <summary>
        /// Reads integers one at a time from a stream reader
        /// </summary>
        /// <remarks>
        /// This method assumes that the text stream has a single integer on each line of text.
        /// This method assumes that streamReader != null.
        /// </remarks>
        /// <param name="textStreamReader">The stream reader pointing to a text stream</param>
        /// <returns>An enumerable that will pull integers from the stream one by one</returns>
        IEnumerable<int> CreateIntegerReaderGenerator(StreamReader testStreamReader);
    }
}
