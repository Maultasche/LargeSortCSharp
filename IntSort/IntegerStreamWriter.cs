using System;
using System.Diagnostics;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Implements functionality for writing to an integer stream
    /// </summary>
    public class IntegerStreamWriter : IIntegerStreamWriter
    {
        /// <see cref="IIntegerStreamWriter.WriteInteger(StreamWriter, int)"/>
        public void WriteInteger(StreamWriter textStreamWriter, int integer)
        {
            Debug.Assert(textStreamWriter != null);

            textStreamWriter.WriteLine(integer);
        }
    }
}
