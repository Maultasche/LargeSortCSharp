using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    /// <summary>
    /// Contains information regarding an integer file
    /// </summary>
    public class IntegerFileInfo
    {
        /// <summary>
        /// Gets or sets the number of chunks found in the integer file
        /// </summary>
        public int NumOfChunks { get; set; }

        /// <summary>
        /// Gets or sets the number of integers found in the integer file
        /// </summary>
        public int NumOfIntegers { get; set; }
    }
}
