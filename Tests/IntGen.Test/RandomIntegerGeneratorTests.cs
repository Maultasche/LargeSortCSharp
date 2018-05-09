using System;
using System.Collections.Generic;

using NUnit.Framework;

using IntGen;

namespace IntGen.Test
{
    /// <summary>
    /// Contains the tests for the RandomIntegerGenerator class
    /// </summary>
    public class RandomIntegerGeneratorTests
    {
        /// <summary>
        /// Defines tests for the CreateIntegerGenerator method
        /// </summary>
        [TestFixture]
        public class CreateIntegerGeneratorTests
        {
            //private static uint[] integerCounts = new uint[] { 0, 1, 10, 100, 1000, 10000, 100000 };

            /// <summary>
            /// Tests the random integer count
            /// </summary>
            [Test]
            public void TestIntegerCount()
            {
                //Test with an increasing number of integers
                TestRandomIntegerGenerator(1, 10, 0);
                TestRandomIntegerGenerator(1, 10, 1);
                TestRandomIntegerGenerator(1, 10, 10);
                TestRandomIntegerGenerator(1, 10, 100);
                TestRandomIntegerGenerator(1, 10, 1000);
                TestRandomIntegerGenerator(1, 10, 10000);
                TestRandomIntegerGenerator(1, 10, 100000);
            }

            /// <summary>
            /// Tests the random integer bounds with positive bounds
            /// </summary>
            [Test]
            public void TestPositiveIntegerBounds()
            {
                //Test with positive bounds
                TestRandomIntegerGenerator(5, 27, 1000);
                TestRandomIntegerGenerator(10, 100, 1000);
                TestRandomIntegerGenerator(443, 3423453, 1000);
            }

            /// <summary>
            /// Tests the random integer bounds with negative bounds
            /// </summary>
            [Test]
            public void TestNegativeIntegerBounds()
            {
                //Test with positive bounds
                TestRandomIntegerGenerator(-10, -1, 1000);
                TestRandomIntegerGenerator(-50, -8, 1000);
                TestRandomIntegerGenerator(-100, -90, 1000);
                TestRandomIntegerGenerator(-3000, -1223, 1000);
                TestRandomIntegerGenerator(-32423, -22, 1000);
            }

            /// <summary>
            /// Tests the random integer bounds with bounds containing positive and negative numbers
            /// </summary>
            [Test]
            public void TestPositiveNegativeIntegerBounds()
            {
                //Test with positive bounds
                TestRandomIntegerGenerator(-2, -2, 1000);
                TestRandomIntegerGenerator(-1, 0, 1000);
                TestRandomIntegerGenerator(-100, -10, 1000);
                TestRandomIntegerGenerator(-32423, -8564, 1000);
            }

            /// <summary>
            /// Tests the random integer bounds where the lower bound equals the upper bound
            /// </summary>
            [Test]
            public void TestEqualBounds()
            {
                //Test with positive bounds
                TestRandomIntegerGenerator(1, 1, 1000);
                TestRandomIntegerGenerator(0, 0, 1000);
                TestRandomIntegerGenerator(20, 20, 1000);
                TestRandomIntegerGenerator(-34, -34, 1000);
            }

            /// <summary>
            /// Tests creating a random integer generator with a particular set of lower bounds
            /// and an integer count
            /// </summary>
            /// <param name="lowerBound">The lower bounds (inclusive) of the integers to be generated</param>
            /// <param name="upperBound">The upper bounds (inclusive) of the integers to be generated</param>
            /// <param name="count">The number of integers to be generated</param>
            private void TestRandomIntegerGenerator(int lowerBound, int upperBound, uint count)
            {
                IRandomIntegerGenerator randomIntGenerator = new RandomIntegerGenerator();

                //Create the random integer generator
                IEnumerable<int> randomIntegers = randomIntGenerator.CreateIntegerGenerator(lowerBound,
                    upperBound, count);

                //Iterate over the random integers, verifying that there are the correct number of integers
                //and that they all fall within the specified bounds
                int integerCount = 0;

                foreach(int randomInt in randomIntegers)
                {
                    //Verify that the random integer is within the specified bounds
                    Assert.That(randomInt, Is.AtLeast(lowerBound));
                    Assert.That(randomInt, Is.AtMost(upperBound));

                    integerCount++;
                }

                //Verify that the correct number of integers were generated
                Assert.That(integerCount, Is.EqualTo(count));
            }
        }
    }
}
