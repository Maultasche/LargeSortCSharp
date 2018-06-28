using System;
using System.Collections.Generic;

namespace SharedTest
{
    /// <summary>
    /// Implements functionality related to test data
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// Generates a set of random integers to be used as test data
        /// </summary>
        /// <param name="numOfIntegers">The number of integers to generate</param>
        /// <returns></returns>
        public static List<int> GenerateTestData(int numOfIntegers)
        {
            const int lowerBound = -1000;
            const int upperBound = 1000;

            List<int> randomIntegers = new List<int>();

            Random rng = new Random();

            //Precompute the exclusive upper bound. The value passed in is inclusive.
            int exclusiveUpperBound = upperBound + 1;

            for (int i = 0; i < numOfIntegers; i++)
            {
                randomIntegers.Add(rng.Next(lowerBound, exclusiveUpperBound));
            }

            return randomIntegers;
        }
    }
}
