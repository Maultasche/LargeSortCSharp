using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using IntSort;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the IntegerStreamReader class
    /// </summary>
    public class IntegerStreamReaderTests
    {
        /// <summary>
        /// Defines tests for the CreateIntegerReaderGenerator method
        /// </summary>
        [TestFixture]
        public class CreateIntegerReaderGeneratorTests
        {
            /// <summary>
            /// Tests the creation of an integer reader generator for a small number of integers
            /// </summary>
            [Test]
            public void TestIntegerReaderGeneratorSmallNumber()
            {
                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Run the test
                RunIntegerReaderGeneratorTest(integers);
            }

            /// <summary>
            /// Tests the creation of an integer reader generator for a large number of integers
            /// </summary>
            [Test]
            public void TestIntegerReaderGeneratorLargeNumber()
            {
                //Create the test data
                List<int> integers = GenerateTestData(1000);

                //Run the test
                RunIntegerReaderGeneratorTest(integers);
            }

            /// <summary>
            /// Tests the creation of an integer reader generator for a single integer
            /// </summary>
            [Test]
            public void TestIntegerReaderGeneratorSingleInteger()
            {
                //Create the test data
                List<int> integers = new List<int> { 4 };

                //Run the test
                RunIntegerReaderGeneratorTest(integers);
            }

            /// <summary>
            /// Tests the creation of an integer reader generator where the stream has no integers
            /// </summary>
            [Test]
            public void TestIntegerReaderGeneratorNoIntegers()
            {
                //Create the test data
                List<int> integers = new List<int>();

                //Run the test
                RunIntegerReaderGeneratorTest(integers);
            }

            /// <summary>
            /// Runs an integer reader generator test
            /// </summary>
            /// <remarks>
            /// This test verifies that the integers are read in the correct order
            /// </remarks>
            /// <param name="testIntegers">The integers to be used as test data. These will be put into
            /// a stream, which will be used by the CreateIntegerReaderGenerator method</param>
            private void RunIntegerReaderGeneratorTest(List<int> testIntegers)
            {
                Assert.That(testIntegers, Is.Not.Null);

                //Create memory stream and stream reader from the test integers
                using (Stream integerStream = CreateIntegerStream(testIntegers))
                using (StreamReader streamReader = new StreamReader(integerStream))
                {
                    //Create the integer stream reader
                    IIntegerStreamReader integerStreamReader = new IntegerStreamReader();

                    IEnumerable<int> streamIntegers = integerStreamReader.CreateIntegerReaderGenerator(
                        streamReader);

                    int integersGenerated = 0;

                    //Get the enumerators for the actual integers and expected integers
                    IEnumerator<int> actualIntegerEnumerator = streamIntegers.GetEnumerator();
                    IEnumerator<int> expectedIntegerEnumerator = testIntegers.GetEnumerator();

                    bool actualIntegersAvailable = actualIntegerEnumerator.MoveNext();
                    bool expectedIntegersAvailable = expectedIntegerEnumerator.MoveNext();

                    //Enumerate until there are either the actual integers or expected integers run out
                    while (actualIntegersAvailable && expectedIntegersAvailable)
                    {
                        //Get the current integers and compare
                        int actualInteger = actualIntegerEnumerator.Current;
                        int expectedInteger = expectedIntegerEnumerator.Current;

                        //Verify that the two integers are the same
                        Assert.That(actualInteger, Is.EqualTo(expectedInteger),
                            "The actual and expected integers are not the same");

                        //Increment the number of integers generated
                        integersGenerated++;

                        //Move the enumerators ahead
                        actualIntegersAvailable = actualIntegerEnumerator.MoveNext();
                        expectedIntegersAvailable = expectedIntegerEnumerator.MoveNext();
                    }

                    streamReader.Close();
                }
            }

            /// <summary>
            /// Creates a stream of text that contains a set of integers, with each
            /// integer on its own line
            /// </summary>
            /// <param name="testIntegers">The set of integers to be put into the stream</param>
            /// <returns>The stream containing the integers</returns>
            private Stream CreateIntegerStream(List<int> testIntegers)
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

            /// <summary>
            /// Generates a set of random integers to be used as test data
            /// </summary>
            /// <param name="numOfIntegers">The number of integers to generate</param>
            /// <returns></returns>
            private List<int> GenerateTestData(int numOfIntegers)
            {
                const int lowerBound = 1;
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
}
