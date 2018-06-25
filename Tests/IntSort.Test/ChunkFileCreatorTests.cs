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
                //Create the test data
                var testChunks = CreateIntegerChunks(numOfChunks: 10, chunkSize: 10);

                //Run the test
                RunCreateChunkFilesTest(testChunks);
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
                //Create the test data
                var testChunks = CreateIntegerChunks(numOfChunks: 10, chunkSize: 10000);

                //Run the test
                RunCreateChunkFilesTest(testChunks);
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are a large number of chunks
            /// with a small number of integers
            /// </summary>
            /// <remarks>
            /// 1000 chunks with 10 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesLargeChunksSmallIntegers()
            {
                //Create the test data
                var testChunks = CreateIntegerChunks(numOfChunks: 1000, chunkSize: 10);

                //Run the test
                RunCreateChunkFilesTest(testChunks);
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are a large number of chunks
            /// with a large number of integers
            /// </summary>
            /// <remarks>
            /// 1000 chunks with 1000 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesLargeChunksLargeIntegers()
            {
                //Create the test data
                var testChunks = CreateIntegerChunks(numOfChunks: 1000, chunkSize: 1000);

                //Run the test
                RunCreateChunkFilesTest(testChunks);
            }

            /// <summary>
            /// Tests the creation of an integer chunk files where there are chunks
            /// with no integers
            /// </summary>
            /// <remarks>
            /// 10 chunks with 0 integers each
            /// </remarks>
            [Test]
            public void TestCreateChunkFilesChunksWithNoIntegers()
            {
                //Create the test data
                var testChunks = CreateIntegerChunks(numOfChunks: 10, chunkSize: 0);

                //Run the test
                RunCreateChunkFilesTest(testChunks);
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
                //Create the test data
                var testChunks = new List<List<int>>();

                //Run the test
                RunCreateChunkFilesTest(testChunks);
            }

            /// <summary>
            /// Runs a test of the CreateChunkFiles method
            /// </summary>
            /// <param name="integerChunks">The integer chunks that are to be written to chunk files</param>
            private void RunCreateChunkFilesTest(List<List<int>> integerChunks)
            {
                const string ChunkFileTemplate = "chunkFile{0}.txt";
                const string OutputDirectory = "output/chunkFiles";

                //Create a dictionary to store the chunks and file names that were added
                var chunkFileContents = new Dictionary<string, IEnumerable<int>>();

                //Mock the integer file creator to keep track of the chunks that were written
                Mock<IIntegerFileCreator> mockIntegerFileCreator = new Mock<IIntegerFileCreator>();

                mockIntegerFileCreator.Setup(mock => mock.CreateIntegerTextFile(It.IsAny<IEnumerable<int>>(),
                    It.IsAny<string>()))
                    .Callback((IEnumerable<int> chunk, string fileName) => chunkFileContents.Add(fileName, chunk));

                //Construct the chunk file creator
                IChunkFileCreator chunkFileCreator = new ChunkFileCreator(mockIntegerFileCreator.Object);

                //Create the integer chunk files
                List<string> createdChunkFiles = chunkFileCreator.CreateChunkFiles(integerChunks, ChunkFileTemplate, 
                    OutputDirectory);

                //Verify that the method returned the expected chunk files
                VerifyCreatedChunkFileNames(createdChunkFiles, ChunkFileTemplate, integerChunks.Count());

                //Verify that the expected chunk files were actually written
                VerifyChunkFiles(chunkFileContents, integerChunks, ChunkFileTemplate, OutputDirectory);
            }

            /// <summary>
            /// Creates an ordered set of integer chunks to be used in testing
            /// </summary>
            /// <param name="numOfChunks">The number of chunks to be created</param>
            /// <param name="chunkSize">The size of each chunk</param>
            /// <returns></returns>
            private List<List<int>> CreateIntegerChunks(int numOfChunks, int chunkSize)
            {
                var chunkData = Enumerable.Range(0, numOfChunks)
                    .Select(chunkNum => GenerateIntegerChunk(chunkSize).OrderBy(integer => integer).ToList())                    
                    .ToList();

                return chunkData;
            }

            /// <summary>
            /// Generates a chunk of random integers to be used as test data
            /// </summary>
            /// <param name="numOfIntegers">The number of integers to generate</param>
            /// <returns></returns>
            private List<int> GenerateIntegerChunk(int numOfIntegers)
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
            /// Verifies that the chunk files written have the correct file paths and the correct file contents
            /// </summary>
            /// <param name="chunkFiles">The files that were written, where the key is the file path and the
            /// value is the chunk of integers that was written</param>
            /// <param name="integerChunks">The integer chunks that were written to files</param>
            /// <param name="chunkFileTemplate">The file template that was used to create the chunk file names</param>
            /// <param name="outputDirectory">The directory where the chunk files were written</param>
            private void VerifyChunkFiles(Dictionary<string, IEnumerable<int>> chunkFiles, List<List<int>> integerChunks,
                string chunkFileTemplate, string outputDirectory)
            {
                //Iterate over each chunk, verifying that its contents were written correctly to the correct file path
                for(int chunkNum = 1; chunkNum <= integerChunks.Count; chunkNum++)
                {
                    //Calculate the expected file path from the current chunk
                    string chunkFilePath = Path.Combine(outputDirectory, string.Format(chunkFileTemplate, chunkNum));

                    //Verify that the expected file path was written to
                    Assert.That(chunkFiles, Contains.Key(chunkFilePath));

                    IEnumerable<int> actualChunk = chunkFiles[chunkFilePath];
                    List<int> expectedChunk = integerChunks[chunkNum - 1];

                    //Verify that the chunk was written to the file
                    Assert.That(actualChunk, Is.Not.Null);

                    //Verify that the chunk that was written contains the expected number of integers
                    Assert.That(actualChunk.Count(), Is.EqualTo(expectedChunk.Count));

                    //Verify that the integers that were written match the chunk that was written
                    actualChunk.Zip(expectedChunk, Tuple.Create)
                        .ToList()
                        .ForEach(integerPair => Assert.That(integerPair.Item1, Is.EqualTo(integerPair.Item2)));
                }
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
        }
    }
}
