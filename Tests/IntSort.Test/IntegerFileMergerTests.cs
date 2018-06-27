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
    /// Contains the tests for the IntegerFileMerger class
    /// </summary>
    public class IntegerFileMergerTests
    {
        /// <summary>
        /// Defines tests for the MergeIntegerFiles method
        /// </summary>
        [TestFixture]
        public class MergeIntegerFilesTests
        {
            ///// <summary>
            ///// Tests the creation of an integer chunk files where there are a small number of chunks
            ///// with a small number of integers
            ///// </summary>
            ///// <remarks>
            ///// 10 chunks with 10 integers each
            ///// </remarks>
            //[Test]
            //public void TestCreateChunkFilesSmallChunksSmallIntegers()
            //{
            //    //Create the test data
            //    var testChunks = CreateIntegerChunks(numOfChunks: 10, chunkSize: 10);

            //    //Run the test
            //    RunCreateChunkFilesTest(testChunks);
            //}



            ///// <summary>
            ///// Tests the update progress method callback when creating chunk files
            ///// </summary>
            ///// <remarks>
            ///// 100 chunks with 10 integers each
            ///// </remarks>
            //[Test]
            //public void TestUpdateProgress()
            //{
            //    //We need to track how many times updateProgress is called and what is passed in when it
            //    //is called.
            //    //This dictionary tracks the calls and what was passed. The key is the the call instance and 
            //    //the value is the number of file passed
            //    Dictionary<int, int> updateProgressCalls = new Dictionary<int, int>();

            //    //Keep track of how many times updateProgress has been called
            //    int updateProgressCounter = 0;

            //    Action<int> updateProgress = fileCount =>
            //    {
            //        updateProgressCounter++;

            //        updateProgressCalls.Add(updateProgressCounter, fileCount);
            //    };

            //    //Create the test data
            //    var testChunks = CreateIntegerChunks(numOfChunks: 10, chunkSize: 0);

            //    //Run the test
            //    RunCreateChunkFilesTest(testChunks, updateProgress);

            //    //Verify that update progress was called once for every chunk and that the file count
            //    //passed as a parameter incremented by one each time
            //    Assert.That(updateProgressCounter, Is.EqualTo(testChunks.Count));

            //    updateProgressCalls.ToList().ForEach(updateProgressCall =>
            //        Assert.That(updateProgressCall.Key, Is.EqualTo(updateProgressCall.Value)));                
            //}

            ///// <summary>
            ///// Runs a test of the CreateChunkFiles method
            ///// </summary>
            ///// <param name="integerChunks">The integer chunks that are to be written to chunk files</param>
            //private void RunCreateChunkFilesTest(List<List<int>> integerChunks, Action<int> updateProgress = null)
            //{
            //    const string ChunkFileTemplate = "chunkFile{0}.txt";
            //    const string OutputDirectory = "output/chunkFiles";

            //    //Create a dictionary to store the chunks and file names that were added
            //    var chunkFileContents = new Dictionary<string, IEnumerable<int>>();

            //    //Mock the integer file creator to keep track of the chunks that were written
            //    Mock<IIntegerFileCreator> mockIntegerFileCreator = new Mock<IIntegerFileCreator>();

            //    mockIntegerFileCreator.Setup(mock => mock.CreateIntegerTextFile(It.IsAny<IEnumerable<int>>(),
            //        It.IsAny<string>()))
            //        .Callback((IEnumerable<int> chunk, string fileName) => chunkFileContents.Add(fileName, chunk));

            //    //Construct the chunk file creator
            //    IChunkFileCreator chunkFileCreator = new ChunkFileCreator(mockIntegerFileCreator.Object);

            //    //Create the integer chunk files
            //    List<string> createdChunkFiles = chunkFileCreator.CreateChunkFiles(integerChunks, ChunkFileTemplate, 
            //        OutputDirectory, updateProgress);

            //    //Verify that the method returned the expected chunk files
            //    VerifyCreatedChunkFileNames(createdChunkFiles, ChunkFileTemplate, integerChunks.Count());

            //    //Verify that the expected chunk files were actually written
            //    VerifyChunkFiles(chunkFileContents, integerChunks, ChunkFileTemplate, OutputDirectory);
            //}

        }
    }
}
