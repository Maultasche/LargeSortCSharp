using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using IntSort;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the ChunkStreamCreator class
    /// </summary>
    public class IntegerFileCreatorTests
    {
        /// <summary>
        /// Defines tests for the CreateIntegerChunkGenerator method
        /// </summary>
        [TestFixture]
        public class CreateIntegerChunkGeneratorTests
        {
            /// <summary>
            /// Tests the creation of an integer chunk generator for two-integer chunks
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 2
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorTwoIntegerChunks()
            {
                const int chunkSize = 2;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int>() { 4, 21 },
                    new List<int>() { 45, 1 },
                    new List<int>() { -4, -43 },
                    new List<int>() { 3, 0 },
                    new List<int>() { 2, 8 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for one-integer chunks
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 1
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorOneIntegerChunks()
            {
                const int chunkSize = 1;

                //Create the test data
                List<int> integers = new List<int> { 8, 1, -5, 6, -1, 0, 13, 10, 22, 7 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int>() { 8 },
                    new List<int>() { 1 },
                    new List<int>() { -5 },
                    new List<int>() { 6 },
                    new List<int>() { -1 },
                    new List<int>() { 0 },
                    new List<int>() { 13 },
                    new List<int>() { 10 },
                    new List<int>() { 22 },
                    new List<int>() { 7 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for where the number
            /// of integers is not evenly divisible by the chunk size
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 3
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorNonAlignedChunkSize()
            {
                const int chunkSize = 3;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int>() { 4, 21, 45 },
                    new List<int>() { 1, -4, -43 },
                    new List<int>() { 3, 0, 2 },
                    new List<int>() { 8 },
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for where the chunk
            /// size is equal to the number of integers
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorEqualChunkAndIntegerSize()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for where the 
            /// chunk size is larger than the number of integers
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 12
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorChunkSizeLargerThanIntegers()
            {
                const int chunkSize = 12;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for where there is
            /// only 1 integer with a larger chunk size
            /// </summary>
            /// <remarks>
            /// 1 integer with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorOneIntegerLargerChunkSize()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int> { 4 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int> { 4 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for where there is
            /// only 1 integer with a chunk size of one
            /// </summary>
            /// <remarks>
            /// 1 integer with a chunk size of 1
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorOneIntegerOneChunkSize()
            {
                const int chunkSize = 1;

                //Create the test data
                List<int> integers = new List<int> { 4 };

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>()
                {
                    new List<int> { 4 }
                };

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator for an empty
            /// stream
            /// </summary>
            /// <remarks>
            /// 0 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorNoIntegers()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int>();

                //Calculate the expected chunks
                List<List<int>> expectedChunks = new List<List<int>>();

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator with a small number
            /// of integers and a small chunk size
            /// </summary>
            /// <remarks>
            /// 100 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorSmallIntegersSmallChunk()
            {
                const int chunkSize = 10;
                const int numOfIntegers = 100;

                //Generate the test data
                List<int> integers = GenerateTestData(numOfIntegers);

                //Calculate the expected chunks
                List<List<int>> expectedChunks = CalculateExpectedChunks(integers, chunkSize);

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator with a large number
            /// of integers and a small chunk size
            /// </summary>
            /// <remarks>
            /// 1000000 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorLargeIntegersSmallChunk()
            {
                const int chunkSize = 10;
                const int numOfIntegers = 580;

                //Generate the test data
                List<int> integers = GenerateTestData(numOfIntegers);

                //Calculate the expected chunks
                List<List<int>> expectedChunks = CalculateExpectedChunks(integers, chunkSize);

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator with a small number
            /// of integers and a large chunk size
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 1000000
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorSmallIntegersLargeChunk()
            {
                const int chunkSize = 1000000;
                const int numOfIntegers = 10;

                //Generate the test data
                List<int> integers = GenerateTestData(numOfIntegers);

                //Calculate the expected chunks
                List<List<int>> expectedChunks = CalculateExpectedChunks(integers, chunkSize);

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Tests the creation of an integer chunk generator with a large number
            /// of integers and a large chunk size
            /// </summary>
            /// <remarks>
            /// 1000000 integers with a chunk size of 10000
            /// </remarks>
            [Test]
            public void TestIntegerChunkGeneratorLargeIntegersLargeChunk()
            {
                const int chunkSize = 10000;
                const int numOfIntegers = 1000000;

                //Generate the test data
                List<int> integers = GenerateTestData(numOfIntegers);

                //Calculate the expected chunks
                List<List<int>> expectedChunks = CalculateExpectedChunks(integers, chunkSize);

                //Run the test
                RunIntegerChunkGeneratorTest(integers, expectedChunks, chunkSize);
            }

            /// <summary>
            /// Runs an integer chunk generator test
            /// </summary>
            /// <remarks>
            /// This test verifies that the correct chunks are generated containing the correct
            /// integers in the correct order.
            /// </remarks>
            /// <param name="testIntegers">The integers to be used as test data. These will be put into
            /// a stream, which will be passed to the CreateIntegerChunkGenerator method</param>
            /// <param name="expectedChunks">The expected chunks that the results are to be compared against</param>
            /// <param name="chunkSize">The size of the chunks to be created</param>
            private void RunIntegerChunkGeneratorTest(List<int> testIntegers, List<List<int>> expectedChunks, 
                int chunkSize)
            {
                Assert.That(testIntegers, Is.Not.Null);

                //Create memory stream and stream reader from the test integers
                using (Stream integerStream = CreateIntegerStream(testIntegers))
                using (StreamReader integerStreamReader = new StreamReader(integerStream))
                {
                    //Create the chunk generator
                    IChunkStreamCreator chunkStreamCreator = new ChunkStreamCreator();

                    IEnumerable<List<int>> actualChunks = chunkStreamCreator.CreateIntegerChunkGenerator(
                        integerStreamReader, chunkSize);

                    //Compare the chunks generated to the expected chunks
                    int chunksGenerated = 0;

                    //Get the enumerators for the actual chunks and expected chunks
                    IEnumerator<List<int>> actualChunkEnumerator = actualChunks.GetEnumerator();
                    IEnumerator<List<int>> expectedChunkEnumerator = expectedChunks.GetEnumerator();

                    bool actualChunksAvailable = actualChunkEnumerator.MoveNext();
                    bool expectedChunksAvailable = expectedChunkEnumerator.MoveNext();

                    //Enumerate until there are either the actual chunks or expected chunks run out
                    while (actualChunksAvailable && expectedChunksAvailable)
                    {
                        //Get the current chunks and compare
                        List<int> actualChunk = actualChunkEnumerator.Current;
                        List<int> expectedChunk = expectedChunkEnumerator.Current;

                        //Verify that the two chunks contain the same integers in the same order
                        Assert.That(actualChunk.Count, Is.EqualTo(expectedChunk.Count),
                            "The actual and expected chunks are not the same size");

                        for (int chunkIndex = 0; chunkIndex < actualChunk.Count; chunkIndex++)
                        {
                            Assert.That(actualChunk[chunkIndex], Is.EqualTo(expectedChunk[chunkIndex]),
                                string.Format("The actual chunk differs from the expected chunk at index {0}", chunkIndex));
                        }

                        //Increment the number of chunks generated
                        chunksGenerated++;

                        //Move the enumerators ahead
                        actualChunksAvailable = actualChunkEnumerator.MoveNext();
                        expectedChunksAvailable = expectedChunkEnumerator.MoveNext();
                    }

                    integerStreamReader.Close();
                }
            }

            /// <summary>
            /// Calculates the chunks that will be expected from a set of integers
            /// </summary>
            /// <param name="testIntegers">The integers to be formed into chunks</param>
            /// <param name="chunkSize">The size of those chunks</param>
            /// <returns></returns>
            private List<List<int>> CalculateExpectedChunks(List<int> testIntegers, int chunkSize)
            {
                List<List<int>> chunkedIntegers = new List<List<int>>();
                List<int> currentChunk = new List<int>(); 

                for(int index = 0; index < testIntegers.Count; index++)
                {
                    //Add the current integer to the current chunk
                    currentChunk.Add(testIntegers[index]);

                    //If this was the last integer in the chunk or if it was the last integer in the collection,
                    //then add the current chunk to the chunked integers and create a new chunk
                    if((index + 1) % chunkSize == 0 || (index + 1) == testIntegers.Count)
                    {
                        chunkedIntegers.Add(currentChunk);
                        currentChunk = new List<int>();
                    }
                }

                return chunkedIntegers;
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

            ///// <summary>
            ///// Tests the creation of an integer file with a single integer
            ///// </summary>
            //[Test]
            //public void TestIntegerFileCreationOneInteger()
            //{
            //    List<int> integers = new List<int>() { 3 };

            //    RunIntegerFileCreationTest(integers, TestFile);
            //}

            ///// <summary>
            ///// Tests the creation of an integer file with multiple integers
            ///// </summary>
            //[Test]
            //public void TestIntegerFileCreationMultipleIntegers()
            //{
            //    List<int> integers = new List<int>() { 3, 5, -1, 334, 2, -23, 14 };

            //    RunIntegerFileCreationTest(integers, TestFile);
            //}

            ///// <summary>
            ///// Tests the creation of an integer file with a random integer generator producing
            ///// large numbers of random integers
            ///// </summary>
            //[Test]
            //public void TestIntegerFileCreationRandomIntegers()
            //{
            //    IRandomIntegerGenerator integerGenerator = new RandomIntegerGenerator();

            //    IEnumerable<int> integers = integerGenerator.CreateIntegerGenerator(-100, 100, 1000);

            //    RunIntegerFileCreationTest(integers, TestFile);
            //}

            ///// <summary>
            ///// Runs an integer file creation test
            ///// </summary>
            ///// <param name="integers">The integers to write to the test file</param>
            ///// <param name="filePath">The test file path</param>
            ///// <param name="integersExpected">True if integers are expected to come from the integers enumerable,
            ///// otherwise false.</param>
            //private void RunIntegerFileCreationTest(IEnumerable<int> integers, string filePath, bool integersExpected = true)
            //{
            //    //Create the stream that will represent a file stream. We'll use a memory stream instead.
            //    Stream testStream = new MemoryStream();

            //    //Mock the File I/O module
            //    Mock<IFileIO> mockFileIO = new Mock<IFileIO>();

            //    //Mock the method to create a file
            //    mockFileIO.Setup(mock => mock.CreateFile(filePath)).Returns(testStream);

            //    //Keep track of the integers that are written
            //    List<int> writtenIntegers = new List<int>();

            //    //Mock the method to write an integer to the stream
            //    mockFileIO.Setup(mock => mock.WriteIntegerToStream(It.IsAny<StreamWriter>(), It.IsAny<int>()))
            //        .Callback((StreamWriter streamWriter, int integer) => {
            //            //Verify that the stream writer was created from the correct stream
            //            Assert.That(streamWriter.BaseStream, Is.EqualTo(testStream));

            //            //Add the integer to the collection of written integers
            //            writtenIntegers.Add(integer);
            //        });

            //    //Keep track of the integers that are generated
            //    List<int> generatedIntegers = new List<int>();

            //    //When an integer comes out of the enumerable, make a note of it so that we can compare it
            //    //to the integers that were written. We may not be able to reenumerate over the source.
            //    IEnumerable<int> integersToWrite = integers.Select(integer =>
            //    {
            //        generatedIntegers.Add(integer);

            //        return integer;
            //    });

            //    //Create the integer file creator
            //    IIntegerFileCreator fileCreator = new IntegerFileCreator(mockFileIO.Object);

            //    //Run the method to create the integer file
            //    fileCreator.CreateIntegerTextFile(integersToWrite, filePath);

            //    //If the integersExpected flag was set, verify that a non-zero number of integers were generated and written
            //    if (integersExpected)
            //    {
            //        Assert.That(generatedIntegers.Count, Is.AtLeast(1));
            //        Assert.That(writtenIntegers.Count, Is.AtLeast(1));
            //    }

            //    //Verify that the expected number of integers were written
            //    Assert.That(writtenIntegers.Count, Is.EqualTo(generatedIntegers.Count));

            //    //Verify that the generated and written integers are equivalent
            //    Assert.That(writtenIntegers, Is.EquivalentTo(generatedIntegers));
            //}
        }
    }
}
