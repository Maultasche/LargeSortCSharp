using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort.Test
{
    /// <summary>
    /// Contains functionality related to testing with streams
    /// </summary>
    static class TestStream
    {
        /// <summary>
        /// Creates a stream of text that contains a set of integers, with each
        /// integer on its own line
        /// </summary>
        /// <param name="testIntegers">The set of integers to be put into the stream</param>
        /// <returns>The stream containing the integers</returns>
        public static Stream CreateIntegerStream(List<int> testIntegers)
        {
            MemoryStream integerStream = new MemoryStream();
            StreamWriter integerStreamWriter = new StreamWriter(integerStream);

            testIntegers.ForEach(integer => integerStreamWriter.WriteLine(integer));

            integerStreamWriter.Flush();

            //Make sure that the stream position is reset to the beginning of the stream
            //so that any reads will happen from the beginning
            integerStream.Position = 0;

            return integerStream;
        }
    }
}
