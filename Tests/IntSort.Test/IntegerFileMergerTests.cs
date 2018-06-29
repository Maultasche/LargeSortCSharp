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
    /// Contains the tests for the IntegerFileMerger class
    /// </summary>
    public class IntegerFileMergerTests
    {
        /// <summary>
        /// Defines tests for the MergeIntegerFiles method
        /// </summary>
        [TestFixture]
        public class MergeIntegerFiles
        {
            /// <summary>
            /// Tests with input files that exactly match the merge count
            /// </summary>
            [Test]
            public void TestMergeIntegerFilesSameFileAndMergeCount()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 10;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with input files that are a multiple of the merge count
            /// </summary>
            [Test]
            public void TestMergeIntegerFileFileCountMultipleOfMergeCount()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 20;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with input files that are not an exact multiple of the merge count
            /// </summary>
            [Test]
            public void TestMergeIntegerFileFileCountNotMultipleOfMergeCount()
            {
                const int MergeCount = 5;
                const int NumOfFiles = 12;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with fewer input files than the merge count
            /// </summary>
            [Test]
            public void TestMergeIntegerFileFileCountLessThanMergeCount()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 7;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with a merge count of 1
            /// </summary>
            [Test]
            public void TestMergeIntegerFileMergeCountOne()
            {
                const int MergeCount = 1;
                const int NumOfFiles = 5;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with a single input file
            /// </summary>
            [Test]
            public void TestMergeIntegerFileSingleInputFile()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 1;

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests the merging of files with the update progress method callback
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamUpdateProgress()
            {
                const int MergeCount = 3;
                const int NumOfFiles = 10;
                const int IntegersPerFile = 10;

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

                List<string> inputFiles = CreateInputFileNames(NumOfFiles);

                //Run the test
                RunMergeIntegerFilesTest(inputFiles, MergeCount, updateProgress);

                //Verify that update progress was called IntegersPerFile times per file (since the files aren't real,
                //we're simulating IntegersPerFile integers per file) and that the integer count passed as a parameter is 
                //incremented by one each time

                Assert.That(updateProgressCounter, Is.EqualTo(inputFiles.Count * IntegersPerFile));

                updateProgressCalls.ToList().ForEach(updateProgressCall =>
                    Assert.That(updateProgressCall.Key, Is.EqualTo(updateProgressCall.Value)));
            }

            /// <summary>
            /// Runs a test of the MergeIntegerFiles method
            /// </summary>
            /// <remarks>
            /// This method does not explicitly test the updateProgress callback. It passes along the callback,
            /// but how it is called must be verified separately.
            /// </remarks>
            /// <param name="testInputFiles">The file names of the input files to be used for this test. These
            /// files do not actually need to exist.</param>
            /// <param name="mergeCount">The number of files to merge at a time when testing</param>
            /// <param name="updateProgress">A method that will be called to update integer merging progress. The 
            /// number of integers that have been merged so far will be passed to this method whenever an integer
            /// is written an output file.</param>
            private void RunMergeIntegerFilesTest(List<string> testInputFiles, int mergeCount,
                Action<int> updateProgress = null)
            {
                const string OutputFileTemplate = "outputFile{0}.txt";
                const string OutputDirectory = "output/";

                //Create collection of file merges that were made. This will allow us to verify that the 
                //files were merged correctly. The first item in the tuple is the input files that were merged,
                //and the second item of the tuple is the output file they were merged into.
                var fileMerges = new List<Tuple<List<string>, string>>();

                //Mock the file I/O functionality
                Mock<IFileIO> mockFileIO = CreateMockFileIO();

                //Mock the integer stream merger functionality
                Mock<IIntegerStreamMerger> mockIntegerStreamMerger = CreateMockIntegerStreamMerger(fileMerges);

                //Create the integer file merger
                IIntegerFileMerger integerFileMerger = new IntegerFileMerger(mockFileIO.Object, mockIntegerStreamMerger.Object);

                //Merge the integer files
                List<string> outputFiles = integerFileMerger.MergeIntegerFiles(testInputFiles, mergeCount, 
                    OutputFileTemplate, OutputDirectory, updateProgress);

                //Verify that the expected output files were created
                VerifyOutputFiles(outputFiles, testInputFiles.Count, mergeCount, OutputDirectory, OutputFileTemplate);

                //Verify that the output directory was created correctly
                mockFileIO.Verify(mock => mock.CreateDirectory(
                    It.Is<string>(directory => directory == OutputDirectory)), Times.Once);

                //Verify that the output files were created correctly
                outputFiles.ForEach(outputFile => mockFileIO.Verify(mock => mock.CreateFile(
                    It.Is<string>(fileName => fileName == outputFile)), Times.Once));

                //Verify that stream readers were created for all the input files
                testInputFiles.ForEach(inputFile => mockFileIO.Verify(mock => mock.CreateFileStreamReader(
                    It.Is<string>(fileName => inputFile == fileName)), Times.Once));

                //Verify that there weren't any other calls being made to file I/O
                mockFileIO.VerifyNoOtherCalls();

                //Verify that the streams were merged correctly
                VerifyFileStreamMerge(fileMerges, outputFiles, testInputFiles, mergeCount, OutputDirectory);
            }

            /// <summary>
            /// Generates the names of input files for use in testing
            /// </summary>
            /// <param name="numOfFiles">The number of file names to generate</param>
            /// <returns>The collection of generated file names</returns>
            private List<string> CreateInputFileNames(int numOfFiles)
            {
                const string fileTemplate = "InputFile{0}.txt";

                List<string> fileNames = Enumerable
                    .Range(1, numOfFiles)
                    .Select(fileNumber => string.Format(fileTemplate, fileNumber))
                    .ToList();

                return fileNames;
            }

            /// <summary>
            /// Creates and returns a mock file I/O
            /// </summary>
            /// <returns>The mock file I/O</returns>
            private Mock<IFileIO> CreateMockFileIO()
            {
                Mock<IFileIO> mockFileIO = new Mock<IFileIO>();

                //Mock the CreateDirectory method to do nothing
                mockFileIO.Setup(mock => mock.CreateDirectory(It.IsAny<string>()));

                //Mock the CreateFile method return a mock file stream
                mockFileIO.Setup(mock => mock.CreateFile(It.IsAny<string>()))
                    .Returns<string>(filePath => new MockFileStream(filePath));

                //Mock the CreateFileStreamReader to return a stream reader to a mock file stream
                mockFileIO.Setup(mock => mock.CreateFileStreamReader(It.IsAny<string>()))
                    .Returns<string>(filePath => new StreamReader(new MockFileStream(filePath)));

                return mockFileIO;
            }

            /// <summary>
            /// Creates and returns a mock integer stream merger
            /// </summary>
            /// <param name="fileMerges">The data indicating how the files were merged</param>
            /// <returns>The mock file I/O</returns>
            private Mock<IIntegerStreamMerger> CreateMockIntegerStreamMerger(List<Tuple<List<string>, string>> fileMerges)
            {
                Mock<IIntegerStreamMerger> mockIntegerStreamMerger = new Mock<IIntegerStreamMerger>();

                Action<List<StreamReader>, StreamWriter, Action<int>> recordFileMerge =
                    (List<StreamReader> inputStreamReaders, StreamWriter outputStreamWriter, Action<int> updateProgress) =>
                    {
                        var inputFileNames = inputStreamReaders
                            .Select(streamReader => ((MockFileStream)streamReader.BaseStream).FileName)
                            .ToList();
                        var outputFileName = ((MockFileStream)outputStreamWriter.BaseStream).FileName;

                        fileMerges.Add(Tuple.Create(inputFileNames, outputFileName));
                    };

                //Mock MergeIntegerStreams so that it just records the file merge when the updateProgress parameter is null
                mockIntegerStreamMerger.Setup(mock => mock.MergeIntegerStreams(It.IsAny<List<StreamReader>>(),
                        It.IsAny<StreamWriter>(), It.Is<Action<int>>(updateProgress => updateProgress == null)))
                    .Callback(recordFileMerge);

                //Mock MergeIntegerStreams so that it records the file merge and the updateProgress method is called 10 
                //times per input stream (mimicking streams with 10 integers each) when updateProgress != null
                var mergeWithProgressSetup = mockIntegerStreamMerger.Setup(mock => mock.MergeIntegerStreams(It.IsAny<List<StreamReader>>(),
                        It.IsAny<StreamWriter>(), It.Is<Action<int>>(updateProgress => updateProgress != null)))
                    .Callback((List<StreamReader> inputStreamReaders, StreamWriter inputStreamWriter, Action<int> updateProgress) =>
                    {
                        //Record the file merge
                        recordFileMerge(inputStreamReaders, inputStreamWriter, updateProgress);

                        //Call the updateProgress method 10 times for every input stream
                        Enumerable.Range(1, 10 * inputStreamReaders.Count).ToList().ForEach(number => updateProgress.Invoke(number));
                    });
                        

                return mockIntegerStreamMerger;
            }

            /// <summary>
            /// Verifies that the correct output files were returned
            /// </summary>
            /// <param name="outputFiles">The output files returned</param>
            /// <param name="inputFileCount">The number of input files that were merged</param>
            /// <param name="mergeCount">How many input files were merged at a time</param>
            /// <param name="outputFileDirectory">The directory the output files were written to</param>
            /// <param name="outputFileTemplate">The output file template</param>
            private void VerifyOutputFiles(List<string> outputFiles, int inputFileCount, int mergeCount, 
                string outputFileDirectory, string outputFileTemplate)
            {
                int expectedOutputFileCount = (int)Math.Ceiling((decimal)inputFileCount / mergeCount);

                List<string> expectedOutputFiles = Enumerable
                    .Range(1, expectedOutputFileCount)
                    .Select(fileNumber => string.Format(outputFileTemplate, fileNumber))
                    .Select(fileName => Path.Combine(outputFileDirectory, fileName))
                    .ToList();

                Assert.That(outputFiles.Count, Is.EqualTo(expectedOutputFileCount));

                Assert.That(outputFiles, Is.EquivalentTo(expectedOutputFiles));
            }


            /// <summary>
            /// Verifies that the input streams were correctly merged into output streams
            /// </summary>
            /// <param name="fileMerges">The data indicating how the files were merged</param>
            /// <param name="outputFiles">The output files that were created</param>
            /// <param name="inputFiles">The input files that were merged</param>
            /// <param name="mergeCount">How many input files were merged at a time</param>
            /// <param name="OutputDirectory">The directory the output files were written to</param>
            private void VerifyFileStreamMerge(List<Tuple<List<string>, string>> fileMerges, 
                List<string> outputFiles, List<string> inputFiles, int mergeCount, string OutputDirectory)
            {
                //Create a copy of the output files and input files. We'll be removing files from these
                //collections as we encounter them when examining how the files were merged. If there
                //are any left in these collections at the end, then we know that some files weren't
                //merged
                List<string> unMergedOutputFiles = outputFiles.Select(fileName => Path.Combine(OutputDirectory, fileName)).ToList();
                List<string> unMergedInputFiles = inputFiles.Select(fileName => fileName).ToList();

                //If the number of input files is not evenly divisible by mergeCount, there will be a remainder
                //that are merged once. We need to keep track of this remainder to verify that it is merged
                //just once.
                int fileRemainderCount = inputFiles.Count % mergeCount;
                int remainderMergeCount = 0;

                //Iterate over each of the file merges
                fileMerges.ForEach(fileMerge =>
                {
                    //Verify that the number of input files either matched mergeCount or it was a remainder
                    //We also need to verify that if this is a remainder, that there have been no other
                    //remainders
                    Assert.That(fileMerge.Item1.Count == mergeCount || fileMerge.Item1.Count == fileRemainderCount,
                        Is.True);

                    if(fileMerge.Item1.Count == remainderMergeCount)
                    {
                        remainderMergeCount++;
                    }

                    Assert.That(remainderMergeCount <= 1);

                    //Remove the input and output file names from the unmerged input and output files
                    unMergedInputFiles.RemoveAll(inputFile => fileMerge.Item1.Contains(inputFile));
                    unMergedOutputFiles.RemoveAll(outputFile => fileMerge.Item2.Contains(outputFile));
                });

                //Verify that all the input and output files were involved in merges. If so, the unmerged input
                //and output file collections will be empty
                Assert.That(unMergedInputFiles, Is.Empty);
                Assert.That(unMergedInputFiles, Is.Empty);
            }

            /// <summary>
            /// Serves as a mock file stream for use in testing
            /// </summary>
            /// <remarks>
            /// This is a memory stream with an extra property for keeping track
            /// of the file it is associated with
            /// </remarks>
            private class MockFileStream : MemoryStream
            {
                /// <summary>
                /// Initializes an instance of MockFileStream
                /// </summary>
                /// <param name="fileName">The file name this mock file stream is associated with</param>
                public MockFileStream(string fileName)
                {
                    FileName = fileName;
                }

                /// <summary>
                /// Gets or set the file name associated with this mock file stream
                /// </summary>
                public string FileName { get; set; }
            }

        }
    }
}
