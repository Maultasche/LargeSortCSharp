using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Implements the functionality for reading from an integer stream
    /// </summary>
    public class IntegerStreamReader : IIntegerStreamReader
    {
        ///<see cref="IIntegerStreamReader.CreateIntegerReaderGenerator(StreamReader)"/>
        public IEnumerable<int> CreateIntegerReaderGenerator(StreamReader textStreamReader)
        {
            //Assert the preconditions
            Debug.Assert(textStreamReader != null);

            //Keep reading the stream until we hit the end
            while (!textStreamReader.EndOfStream)
            {
                string line = textStreamReader.ReadLine();

                //Attempt to parse the line as an integer
                int integer = Convert.ToInt32(line);

                //Yield the integer
                yield return integer;
            }
        }
    }
}
