using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;
using NUnit.Framework;

using IntSort;
using LargeSort.Shared;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the ChunkStreamCreator class
    /// </summary>
    public class ChunkFileCreatorTests
    {
        /// <summary>
        /// Defines tests for the CreateChunkFiles method
        /// </summary>
        [TestFixture]
        public class CreateChunkFilesTests
        {
            /// <summary>
            /// Tests the creation of an integer chunk files where there are a small number of chunks
            /// with a small number of integers
            /// </summary>
            /// <remarks>
            /// 10 chunks with 10 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesSmallChunksSmallIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are a small number of chunks
            /// with a large number of integers
            /// </summary>
            /// <remarks>
            /// 10 chunks with 10000 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesSmallChunksLargeIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are a large number of chunks
            /// with a small number of integers
            /// </summary>
            /// <remarks>
            /// 10000 chunks with 10 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesLargeChunksSmallIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are a large number of chunks
            /// with a large number of integers
            /// </summary>
            /// <remarks>
            /// 10000 chunks with 10000 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesLargeChunksLargeIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are only chunks
            /// with no integers
            /// </summary>
            /// <remarks>
            /// 10 chunks with 0 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesChunksWithNoIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are no chunks
            /// </summary>
            /// <remarks>
            /// 0 chunks
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesChunksNoIntegers()
            {
                Assert.Fail();
            }

            /// <summary>
            /// Runs a test of the CreateChunkFiles method
            /// </summary>
            /// <param name="integerChunks">The integer chunks that are to be written to chunk files</param>
            private void RunCreateChunkFilesTest(IEnumerable<List<int>> integerChunks)
            {
                const string ChunkFileTemplate = "chunkFile{0}.txt";
                const string OutputDirectory = "output/chunkFiles";

                //Mock the integer file creator
                Mock<IIntegerFileCreator> mockIntegerFileCreator = new Mock<IIntegerFileCreator>();

                mockIntegerFileCreator.Setup(mock => mock.CreateIntegerTextFile(It.IsAny<IEnumerable<int>>(),
                    It.IsAny<string>()));

                //Construct the chunk file creator
                IChunkFileCreator chunkFileCreator = new ChunkFileCreator(mockIntegerFileCreator.Object);

                //Create the integer chunk files
                List<string> createdChunkFiles = chunkFileCreator.CreateChunkFiles(integerChunks, ChunkFileTemplate, 
                    OutputDirectory);

                //Verify that the method returned the expected chunk files
                VerifyCreatedChunkFileNames(createdChunkFiles, ChunkFileTemplate, integerChunks.Count());

                //Verify that the chunk files exist in the correct location
                VerifyCreatedChunkFileNames(createdChunkFiles, OutputDirectory);

                //Verify that the chunk files contain the expected integers

                //Clean up by deleting the chunk files and the directory
            }

            /// <summary>
            /// Creates a set of integer chunks to be used in testing
            /// </summary>
            /// <param name="numOfChunks">The number of chunks to be created</param>
            /// <param name="chunkSize">The size of each chunk</param>
            /// <returns></returns>
            private List<List<int>> CreateIntegerChunks(int numOfChunks, int chunkSize)
            {
                var chunkData = Enumerable.Range(0, numOfChunks)
                    .Select(chunkNum => GenerateTestData(chunkSize))
                    .ToList();

                return chunkData;
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

            /// <summary>
            /// Verifies that the chunk file names were created correctly
            /// </summary>
            /// <param name="chunkFileNames">The chunk file names that were created</param>
            /// <param name="chunkFileTemplate">The chunk file template used to create the files</param>
            /// <param name="numOfChunks">The number of chunks that were used for the test</param>
            private void VerifyCreatedChunkFileNames(List<string> chunkFileNames, string chunkFileTemplate, int numOfChunks)
            {
                //Verify that we have the expected number of chunk file names
                Assert.That(chunkFileNames.Count, Is.EqualTo(numOfChunks),
                    "The number of chunk files that were created does not match the expected number of chunk files");

                //Create the expected chunk files and verify that they match the chunk files
                //that were created
                List<string> expectedChunkFileNames = Enumerable.Range(1, numOfChunks)
                    .Select(chunkNumber => string.Format(chunkFileTemplate, chunkNumber))
                    .ToList();

                Assert.That(chunkFileNames, Is.EquivalentTo(expectedChunkFileNames));
            }

            /// <summary>
            /// Verifies that the chunk file names were created in the correct location
            /// </summary>
            /// <param name="chunkFileNames">The names of the chunk files that were created</param>
            /// <param name="chunkFileTemplate">The chunk file template used to create the files</param>
            /// <param name="numOfChunks">The number of chunks that were used for the test</param>
            private void VerifyCreatedChunkFileNames(List<string> chunkFiles, string outputDirectory)
            {
                chunkFiles.ForEach(fileName => Assert.That(
                    File.Exists(Path.Combine(outputDirectory, fileName)), Is.True));
            }
        }
    }
}
