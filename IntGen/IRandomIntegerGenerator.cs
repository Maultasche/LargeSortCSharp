using System;
using System.Collections.Generic;
using System.Text;

namespace IntGen
{
    /// <summary>
    /// Defines the functionality for generating random integers
    /// </summary>
    public interface IRandomIntegerGenerator
    {
        /// <summary>
        /// Creates a generator that generates random integers
        /// </summary>
        /// <remarks>
        /// This method assumes that lowerBound &lt;= upperBound
        /// </remarks>
        /// <param name="lowerBound">The lower bound (inclusive) of the range that the generated
        /// integers will fall under</param>
        /// <param name="upperBound">The upper bound (inclusive) of the range that the generated
        /// integers will fall under</param>
        /// <param name="count">The number of integers to be generated</param>
        /// <returns>An enumerable that will generate random integers as it is iterated over</returns>
        IEnumerable<int> CreateIntegerGenerator(int lowerBound, int upperBound, uint count);
    }
}
