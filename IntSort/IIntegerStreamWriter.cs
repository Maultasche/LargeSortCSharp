using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Defines functionality for writing to an integer stream
    /// </summary>
    public interface IIntegerStreamWriter
    {
        /// <summary>
        /// Writes an integer to an integer stream
        /// </summary>
        /// <remarks>
        /// This method will write each integer to the text stream followed by a newline character, so
        /// that the text stream has a single integer on each line of text.
        /// This method assumes that textStreamWriter != null.
        /// </remarks>
        /// <param name="textStreamWriter">The stream writer pointing to a text stream</param>
        /// <param name="integer">The integer to be written to the text stream</param>
        void WriteInteger(StreamWriter textStreamWriter, int integer);
    }
}
