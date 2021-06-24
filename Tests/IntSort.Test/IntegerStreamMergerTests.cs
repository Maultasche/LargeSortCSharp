using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;
using NUnit.Framework;

using IntSort;
using LargeSort.Shared;
using SharedTest;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the IntegerStreamMerger class
    /// </summary>
    public class IntegerStreamMergerTests
    {
        /// <summary>
        /// Defines tests for the MergeIntegerStreams method
        /// </summary>
        [TestFixture]
        public class MergeIntegerStreamsTests
        {
            /// <summary>
            /// Tests the merging of integer streams where the integer streams are all of the same length
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsSameLengthStreams()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams that all have the same length, except for one stream, which is
            /// shorter than the rest
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsSameLengthOneShorterStream()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber - 1)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Add a shorter set of integers to the end of the test data
                testIntegers.Add(TestData.GenerateTestData(StandardStreamLength / 2)
                    .OrderBy(integer => integer)
                    .ToList());

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams that all have the same length, except for one stream, which is
            /// longer than the rest
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsSameLengthOneLongerStream()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber - 1)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Add a longer set of integers to the end of the test data
                testIntegers.Add(TestData.GenerateTestData(StandardStreamLength * 2)
                    .OrderBy(integer => integer)
                    .ToList());

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams with a variety of lengths
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsWithLengthVariety()
            {
                //Create the test data
                List<List<int>> unorderedTestData = new List<List<int>>();

                unorderedTestData.Add(TestData.GenerateTestData(10));
                unorderedTestData.Add(TestData.GenerateTestData(2));
                unorderedTestData.Add(TestData.GenerateTestData(7));
                unorderedTestData.Add(TestData.GenerateTestData(15));
                unorderedTestData.Add(TestData.GenerateTestData(5));
                unorderedTestData.Add(TestData.GenerateTestData(20));
                unorderedTestData.Add(TestData.GenerateTestData(5));
                unorderedTestData.Add(TestData.GenerateTestData(8));
                unorderedTestData.Add(TestData.GenerateTestData(17));
                unorderedTestData.Add(TestData.GenerateTestData(9));

                //Convert the unordered test data into ordered test data with a single statement
                List<List<int>> testIntegers = unorderedTestData
                    .Select(integers => integers.OrderBy(integer => integer).ToList())
                    .ToList();

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams where one of the streams has no integers
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsStreamWithNoIntegers()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber - 1)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Add an empty set of integers to the end of the test data
                testIntegers.Add(new List<int>());

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams where one of the streams has one integer
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsStreamWithOneInteger()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber - 1)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Add a single integer to the end of the test data
                testIntegers.Add(TestData.GenerateTestData(1));

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams where there is only one input stream
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsSingleInputStream()
            {
                const int StandardStreamLength = 10;

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, 1)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams where there are no input streams
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamsNoInputStreams()
            {
                //Create the test data
                List<List<int>> testIntegers = new List<List<int>>();

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers);
            }

            /// <summary>
            /// Tests the merging of streams with the update progress method callback
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamUpdateProgress()
            {
                const int StandardStreamLength = 10;
                const int StreamNumber = 10;

                //We need to track how many times updateProgress is called and what is passed in when it
                //is called.
                //This dictionary tracks the calls and what was passed. The key is the the call instance and 
                //the value is the number of file passed
                Dictionary<int, int> updateProgressCalls = new Dictionary<int, int>();

                //Keep track of how many times updateProgress has been called
                int updateProgressCounter = 0;

                Action<int> updateProgress = integerCount =>
                {
                    updateProgressCounter++;

                    updateProgressCalls.Add(updateProgressCounter, integerCount);
                };

                //Create the test data
                List<List<int>> testIntegers = Enumerable.Range(0, StreamNumber)
                    .Select(streamNumber => TestData.GenerateTestData(StandardStreamLength)
                        .OrderBy(integer => integer)
                        .ToList())
                    .ToList();

                //Run the test
                RunMergeIntegerStreamsTest(testIntegers, updateProgress);

                //Verify that update progress was called once for every integer and that the integer count
                //passed as a parameter incremented by one each time
                Assert.That(updateProgressCounter, Is.EqualTo(testIntegers.Sum(integerSet => integerSet.Count)));

                updateProgressCalls.ToList().ForEach(updateProgressCall =>
                    Assert.That(updateProgressCall.Key, Is.EqualTo(updateProgressCall.Value)));
            }

            /// <summary>
            /// Runs a test of the MergeIntegerStreams method
            /// </summary>
            /// <remarks>
            /// This method does not explicitly test the updateProgress callback. It passes along the callback,
            /// but how it is called must be verified separately.
            /// </remarks>
            /// <param name="testIntegers">The integers that will be used for the test. Each List{int} represents
            /// an input stream that will be created.</param>
            /// <param name="updateProgress">A method that will be called to update integer merging progress. The 
            /// number of integers that have been merged so far will be passed to this method whenever an integer
            /// is written to the output stream.</param>
            private void RunMergeIntegerStreamsTest(List<List<int>> testIntegers, Action<int> updateProgress = null)
            {
                List<StreamReader> inputStreamReaders = null;
                StreamWriter outputStreamWriter = null;

                try
                {
                    //Create the integer stream readers for the input streams
                    inputStreamReaders = testIntegers
                        .Select(integerList => CreateIntegerStream(integerList))
                        .Select(integerStream => new StreamReader(integerStream))
                        .ToList();

                    //Create the output stream writer
                    MemoryStream outputStream = new MemoryStream();
                    outputStreamWriter = new StreamWriter(outputStream);

                    //Merge the input streams into the output stream
                    IIntegerStreamMerger integerStreamMerger = new IntegerStreamMerger(new MockIntegerStreamReader(),
                        CreateMockIntegerStreamWriter().Object);

                    integerStreamMerger.MergeIntegerStreams(inputStreamReaders, outputStreamWriter, updateProgress);

                    //Verify that the streams were merged correctly
                    //Flush the output stream writer and reset the output stream position
                    outputStreamWriter.Flush();
                    outputStream.Position = 0;

                    //Extract the actual integers written to the output stream
                    List<int> actualMergedIntegers = null;

                    using (StreamReader outputStreamReader = new StreamReader(outputStream))
                    {
                        actualMergedIntegers = outputStreamReader.ReadToEnd().Split("\n")
                            .Where(line => line != string.Empty)
                            .Select(line => Convert.ToInt32(line))
                            .ToList();
                    }

                    //Create the expected merged integers
                    List<int> expectedMergedIntegers = testIntegers
                        .SelectMany(integerList => integerList)
                        .OrderBy(integer => integer)
                        .ToList();

                    //Verify that the actual integers match the expected integers
                    actualMergedIntegers.Zip(expectedMergedIntegers, Tuple.Create)
                        .ToList()
                        .ForEach(integerPair => Assert.That(integerPair.Item1, Is.EqualTo(integerPair.Item2)));
                }
                finally
                {
                    //Close the integer input streams
                    if(inputStreamReaders != null)
                    {
                        inputStreamReaders.ForEach(inputStream => inputStream.Close());
                    }

                    //Close the output stream
                    if(outputStreamWriter != null)
                    {
                        outputStreamWriter.Close();
                    }
                }
            }

            /// <summary>
            /// Creates a mock integer stream writer
            /// </summary>
            /// <remarks>
            /// This mock integer stream writer actually does pretty much the same as the real thing,
            /// but we don't want to couple this test to that implementation
            /// </remarks>
            /// <returns>A mock stream writer</returns>
            private Mock<IIntegerStreamWriter> CreateMockIntegerStreamWriter()
            {
                Mock<IIntegerStreamWriter> mockIntegerStreamWriter = new Mock<IIntegerStreamWriter>();

                mockIntegerStreamWriter.Setup(mock => mock.WriteInteger(It.IsAny<StreamWriter>(), It.IsAny<int>()))
                    .Callback<StreamWriter, int>((streamWriter, integer) => streamWriter.WriteLine(integer));

                return mockIntegerStreamWriter;
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
            /// Defines a mock stream reader
            /// </summary>
            /// <remarks>
            /// We have to do it this way rather than the usual way using Moq because generators
            /// don't work in lambda expressions.
            /// We're pretty much doing this the same way as the real stream reader because it's
            /// the easiest way to test and we don't want to couple this test to that implementation.
            /// </remarks>
            private class MockIntegerStreamReader : IIntegerStreamReader
            {
                ///<see cref="IIntegerStreamReader.CreateIntegerReaderGenerator(StreamReader)"/>
                public IEnumerable<int> CreateIntegerReaderGenerator(StreamReader textStreamReader)
                {
                    //Keep reading the stream until we hit the end
                    while (!textStreamReader.EndOfStream)
                    {
                        string line = textStreamReader.ReadLine();

                        //Attempt to parse the line as an integer
                        int integer = Convert.ToInt32(line);

                        //Yield the integer
                        yield return integer;
                    }
                }
            }
        }
    }
}
