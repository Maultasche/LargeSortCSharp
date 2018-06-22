using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntGen
{
    /// <summary>
    /// Implements the functionality for generating random integers
    /// </summary>
    public class RandomIntegerGenerator : IRandomIntegerGenerator
    {
        /// <see cref="IRandomIntegerGenerator.CreateIntegerGenerator(int, int, int)"/>
        public IEnumerable<int> CreateIntegerGenerator(int lowerBound, int upperBound, int count)
        {
            Debug.Assert(lowerBound <= upperBound);
            Debug.Assert(count >= 0);

            //We don't need crypto-strength randomness, so the pseudo-random number generator is just fine
            Random rng = new Random();

            //Precompute the exclusive upper bound. The value passed in is inclusive.
            int exclusiveUpperBound = upperBound + 1;

            for(int i = 0; i < count; i++)
            {
                yield return rng.Next(lowerBound, exclusiveUpperBound);
            }
        }
    }
}
