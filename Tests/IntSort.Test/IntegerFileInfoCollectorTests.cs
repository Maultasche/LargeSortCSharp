using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;
using NUnit.Framework;

using LargeSort.Shared;
using SharedTest;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the IntegerFileInfoCollector class
    /// </summary>
    public class IntegerFileInfoCollectorTests
    {
        /// <summary>
        /// Defines tests for the GetIntegerFileInfo method
        /// </summary>
        [TestFixture]
        public class GetIntegerFileInfoTests
        {
            const string TestFile = "TestIntegerFile.txt";

            /// <summary>
            /// Tests the collection of file info for two-integer chunks
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 2
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionTwoIntegerChunks()
            {
                const int chunkSize = 2;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 5,
                    NumOfIntegers = 10
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info for one-integer chunks
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 1
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionOneIntegerChunks()
            {
                const int chunkSize = 1;

                //Create the test data
                List<int> integers = new List<int> { 8, 1, -5, 6, -1, 0, 13, 10, 22, 7 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 10,
                    NumOfIntegers = 10
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info  where the number
            /// of integers is not evenly divisible by the chunk size
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 3
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionNonAlignedChunkSize()
            {
                const int chunkSize = 3;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 4,
                    NumOfIntegers = 10
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info where the chunk
            /// size is equal to the number of integers
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionEqualChunkAndIntegerSize()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 1,
                    NumOfIntegers = 10
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info where the 
            /// chunk size is larger than the number of integers
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 12
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionChunkSizeLargerThanIntegers()
            {
                const int chunkSize = 12;

                //Create the test data
                List<int> integers = new List<int> { 4, 21, 45, 1, -4, -43, 3, 0, 2, 8 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 1,
                    NumOfIntegers = 10
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info where there is
            /// only 1 integer with a larger chunk size
            /// </summary>
            /// <remarks>
            /// 1 integer with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionOneIntegerLargerChunkSize()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int> { 4 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 1,
                    NumOfIntegers = 1
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info where there is
            /// only 1 integer with a chunk size of one
            /// </summary>
            /// <remarks>
            /// 1 integer with a chunk size of 1
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionOneIntegerOneChunkSize()
            {
                const int chunkSize = 1;

                //Create the test data
                List<int> integers = new List<int> { 4 };

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 1,
                    NumOfIntegers = 1
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info for an empty
            /// stream
            /// </summary>
            /// <remarks>
            /// 0 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionNoIntegers()
            {
                const int chunkSize = 10;

                //Create the test data
                List<int> integers = new List<int>();

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 0,
                    NumOfIntegers = 0
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info with a small number
            /// of integers and a small chunk size
            /// </summary>
            /// <remarks>
            /// 100 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionSmallIntegersSmallChunk()
            {
                const int chunkSize = 10;
                const int numOfIntegers = 100;

                //Generate the test data
                List<int> integers = TestData.GenerateTestData(numOfIntegers);

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 10,
                    NumOfIntegers = 100
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info with a large number
            /// of integers and a small chunk size
            /// </summary>
            /// <remarks>
            /// 1000000 integers with a chunk size of 10
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionLargeIntegersSmallChunk()
            {
                const int chunkSize = 10;
                const int numOfIntegers = 580;

                //Generate the test data
                List<int> integers = TestData.GenerateTestData(numOfIntegers);

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 58,
                    NumOfIntegers = numOfIntegers
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file info with a small number
            /// of integers and a large chunk size
            /// </summary>
            /// <remarks>
            /// 10 integers with a chunk size of 1000000
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionSmallIntegersLargeChunk()
            {
                const int chunkSize = 1000000;
                const int numOfIntegers = 10;

                //Generate the test data
                List<int> integers = TestData.GenerateTestData(numOfIntegers);

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 1,
                    NumOfIntegers = numOfIntegers
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Tests the collection of file fino with a large number
            /// of integers and a large chunk size
            /// </summary>
            /// <remarks>
            /// 1000000 integers with a chunk size of 10000
            /// </remarks>
            [Test]
            public void TestIntegerFileInfoCollectionLargeIntegersLargeChunk()
            {
                const int chunkSize = 10000;
                const int numOfIntegers = 1000000;

                //Generate the test data
                List<int> integers = TestData.GenerateTestData(numOfIntegers);

                //Create the expected file info
                IntegerFileInfo expectedFileInfo = new IntegerFileInfo()
                {
                    NumOfChunks = 100,
                    NumOfIntegers = numOfIntegers
                };

                //Run the test
                RunIntegerFileInfoCollectionTest(integers, expectedFileInfo, chunkSize);
            }

            /// <summary>
            /// Runs an integer file info test
            /// </summary>
            /// <remarks>
            /// This test verifies that the information is read from the integer file correctly.
            /// </remarks>
            /// <param name="testIntegers">The integers to be used as test data. These will be put into
            /// a stream, which will be passed to the GetIntegerFileInfo method</param>
            /// <param name="expectedFileInfo">The expected information that will be returned</param>
            /// <param name="chunkSize">The size of the chunks to be created</param>
            private void RunIntegerFileInfoCollectionTest(List<int> testIntegers, IntegerFileInfo expectedFileInfo, 
                int chunkSize)
            {
                Assert.That(testIntegers, Is.Not.Null);

                //Create memory stream and stream reader from the test integers
                using (Stream integerStream = TestStream.CreateIntegerStream(testIntegers))
                using (StreamReader integerStreamReader = new StreamReader(integerStream))
                {
                    //Construct the file info collector. The mock file I/O module will pretend to
                    //read from a non-existent test file and return a stream containing the text integers
                    Mock<IFileIO> mockFileIO = CreateMockFileIO(integerStreamReader);

                    IIntegerFileInfoCollector fileInfoCollector = new IntegerFileInfoCollector(mockFileIO.Object);

                    //Collect the information on the test file
                    IntegerFileInfo actualFileInfo = fileInfoCollector.GetIntegerFileInfo(TestFile, chunkSize);

                    //Verify that the information was correct
                    VerifyIntegerFileInfo(expectedFileInfo, actualFileInfo);

                    //Verify that the file I/O module was called correctly
                    mockFileIO.Verify(mock => mock.CreateFileStreamReader(It.Is<string>(filePath => filePath == TestFile)),
                        Times.Once);

                    mockFileIO.VerifyNoOtherCalls();
                }
            }

            /// <summary>
            /// Asserts that the expected file information is equal to the actual file information and that neither
            /// one of the parameters is null/
            /// </summary>
            /// <param name="expectedFileInfo">The expected file information</param>
            /// <param name="actualFileInfo">The actual file information</param>
            private void VerifyIntegerFileInfo(IntegerFileInfo expectedFileInfo, IntegerFileInfo actualFileInfo)
            {
                Assert.That(expectedFileInfo, Is.Not.Null);
                Assert.That(actualFileInfo, Is.Not.Null);

                Assert.That(actualFileInfo.NumOfChunks, Is.EqualTo(expectedFileInfo.NumOfChunks));
                Assert.That(actualFileInfo.NumOfIntegers, Is.EqualTo(expectedFileInfo.NumOfIntegers));
            }

            /// <summary>
            /// Creates a mock file I/O module for this suite of tests
            /// </summary>
            /// <param name="integerStreamReader">A stream reader from which integers
            /// are to be read</param>
            /// <returns>A mock file I/O module</returns>
            Mock<IFileIO> CreateMockFileIO(StreamReader integerStreamReader)
            {
                Mock<IFileIO> mockFileIO = new Mock<IFileIO>();

                mockFileIO.Setup(mock => mock.CreateFileStreamReader(It.IsAny<string>()))
                    .Returns(integerStreamReader);

                return mockFileIO;
            }
        }
    }
}
