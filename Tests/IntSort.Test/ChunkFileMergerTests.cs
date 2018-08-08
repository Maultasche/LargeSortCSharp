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
    /// Contains the tests for the ChunkFileMerger class
    /// </summary>
    public class ChunkFileMergerTests
    {
        /// <summary>
        /// Defines tests for the MergeChunkFilesIntoSingleFile method
        /// </summary>
        [TestFixture]
        public class MergeChunkFilesIntoSingleFileTests
        {
            /// <summary>
            /// Tests with the merging of a single chunk file
            /// </summary>
            [Test]
            public void TestMergeChunkFilesSingleFile()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 1;

                List<string> inputFiles = CreateChunkFileNames(NumOfFiles);

                //Run the test
                RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with a single generation of merges
            /// </summary>
            [Test]
            public void TestMergeChunkFilesSingleGeneration()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 10;

                List<string> inputFiles = CreateChunkFileNames(NumOfFiles);

                //Run the test
                RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with a multiple generations of merges
            /// </summary>
            [Test]
            public void TestMergeChunkFilesMultipleGenerations()
            {
                //This scenario should result in 4 merge generations
                //The first generation results in 103 output files
                //The second generation results in 11 output files
                //The third generation results in 2 output files
                //The fourth generation results in 1 output file
                const int MergeCount = 10;
                const int NumOfFiles = 1023;

                List<string> inputFiles = CreateChunkFileNames(NumOfFiles);

                //Run the test
                RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount);
            }

            /// <summary>
            /// Tests with a multiple ranges of chunk files from 1 to 100,
            /// testing each number in this range
            /// </summary>
            [Test]
            public void TestMergeChunkFilesChunkFileRanges()
            {
                const int MergeCount = 10;
                const int StartOfRange = 1;
                const int EndOfRange = 100;

                for (int numOfFiles = StartOfRange; numOfFiles <= EndOfRange; numOfFiles++)
                {
                    List<string> inputFiles = CreateChunkFileNames(numOfFiles);

                    //Run the test
                    RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount);
                }
            }

            /// <summary>
            /// Tests with a variety of different starting generation numbers
            /// </summary>
            [Test]
            public void TestMergeChunkFilesStartingGenerations()
            {
                const int MergeCount = 10;
                const int NumOfFiles = 1023;
                const int MinStartingGeneration = 1;
                const int MaxStartingGeneration = 12;

                for(int startingGeneration = MinStartingGeneration; startingGeneration <= MaxStartingGeneration; startingGeneration++)
                {
                    List<string> inputFiles = CreateChunkFileNames(NumOfFiles);

                    //Run the test
                    RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount, startingGeneration);
                }
            }

            /// <summary>
            /// Tests the merging of chunk files with the update progress method callback
            /// </summary>
            [Test]
            public void TestMergeIntegerStreamUpdateProgress()
            {
                //This scenario should result in 4 merge generations
                //The first generation results in 103 output files
                //The second generation results in 11 output files
                //The third generation results in 2 output files
                //The fourth generation results in 1 output file
                const int MergeCount = 10;
                const int NumOfFiles = 1023;
                const int IntegersPerFile = 10;
                const int ExpectedMerges = NumOfFiles * IntegersPerFile;
                const int ExpectedGenerations = 4;
                const int StartingGeneration = 2;

                //We need to track how many times updateProgress is called and what is passed in when it
                //is called.
                //This dictionary tracks the calls and what was passed. The key is the the generation number
                //and the value is the values that were passed for that generation during updateProgress calls
                Dictionary<int, List<int>> updateProgressCalls = new Dictionary<int, List<int>>();

                Action<int, int> updateProgress = (generation, integerCount) =>
                {
                    if (updateProgressCalls.ContainsKey(generation))
                    {
                        //If the dictionary already contains an entry for this generation, add the integer count
                        updateProgressCalls[generation].Add(integerCount);
                    }
                    else
                    {
                        //If the dictionary does not contain an entry for this generation, create that entry
                        updateProgressCalls.Add(generation, new List<int>() { integerCount });
                    }
                };

                List<string> inputFiles = CreateChunkFileNames(NumOfFiles);

                //Run the test
                RunMergeChunkFilesIntoSingleFileTest(inputFiles, MergeCount, StartingGeneration, updateProgress);

                //Verify that updateProgressCalls has entries for each generation
                Assert.That(updateProgressCalls.Count, Is.EqualTo(ExpectedGenerations));

                //Verify that updateProgress was called the expected number of times for each generation
                //and that the values passed ranged from 1 to ExpectedMerges with no repeats
                for (int generation = StartingGeneration; generation < StartingGeneration + ExpectedGenerations; generation++)
                {
                    List<int> generationCalls = updateProgressCalls[generation];

                    Assert.That(generationCalls.Count, Is.EqualTo(ExpectedMerges));

                    Assert.That(Enumerable.Range(1, ExpectedMerges).SequenceEqual(generationCalls), Is.True);
                }
            }

            /// <summary>
            /// Runs a test of the MergeChunkFilesIntoSingleFile method
            /// </summary>
            /// <remarks>
            /// This method does not explicitly test the updateProgress callback. It passes along the callback,
            /// but how it is called must be verified separately.
            /// </remarks>
            /// <param name="chunkFiles">The file names of the chunk files to be used for this test. These
            /// files do not actually need to exist.</param>
            /// <param name="mergeCount">The number of files to merge at a time when testing</param>
            /// <param name="updateProgress">A method that will be called to update integer merging progress. The 
            /// generation number and number of integers that have been merged so far will be passed to this 
            /// method whenever an integer is written an output file.</param>
            private void RunMergeChunkFilesIntoSingleFileTest(List<string> chunkFiles, int mergeCount, 
                int startingGeneration = 1, Action<int, int> updateProgress = null)
            {
                const string IntermediateFileTemplate = "gen{0}-{1}.txt";
                const string OutputDirectory = "output/";
                const string OutputFile = "sortedIntegers.txt";

                //Calculate the number of merge generations
                int mergeGenerations = CalculateMergeGenerations(chunkFiles.Count, mergeCount);

                //Calculate the expected intermediate files for each merge generation. 
                List<List<string>> expectedIntermediateFiles = CalculateExpectedIntermediateFiles(chunkFiles.Count,
                    mergeCount, mergeGenerations, IntermediateFileTemplate, OutputDirectory, startingGeneration);

                //Mock the file I/O functionality
                Mock<IFileIO> mockFileIO = CreateMockFileIO();

                //Mock the integer file merger functionality
                Mock<IIntegerFileMerger> mockIntegerFileMerger = CreateMockIntegerFileMerger(chunkFiles.Count);

                //Create the chunk file merger
                IChunkFileMerger chunkFileMerger = new ChunkFileMerger(mockFileIO.Object, mockIntegerFileMerger.Object);

                //Merge the chunk files
                List<string> intermediateFiles = chunkFileMerger.MergeChunkFilesIntoSingleFile(chunkFiles,
                    mergeCount, IntermediateFileTemplate, OutputFile, OutputDirectory, startingGeneration,
                    updateProgress);

                //Verify that the expected intermediate files were created
                Assert.That(intermediateFiles, Is.EquivalentTo(expectedIntermediateFiles.SelectMany(fileGroup => fileGroup)));

                //Verify that the integer file merger was called correctly for each merge generation
                VerifyIntegerFileMerges(mockIntegerFileMerger, chunkFiles, expectedIntermediateFiles, mergeCount, 
                    IntermediateFileTemplate, OutputDirectory, startingGeneration);

                //Verify that the output directory was created correctly
                mockFileIO.Verify(mock => mock.CreateDirectory(
                    It.Is<string>(directory => directory == OutputDirectory)), Times.Once);

                string outputFilePath = Path.Combine(OutputDirectory, OutputFile);

                //Verify that any previously-existing output file was deleted
                mockFileIO.Verify(mock => mock.DeleteFile(It.Is<string>(file => file == outputFilePath)), Times.Once);

                //Verify that the final merge file was renamed to the output file
                string finalMergeFile = Path.Combine(OutputDirectory, 
                    string.Format(IntermediateFileTemplate, startingGeneration + mergeGenerations - 1, "1"));

                mockFileIO.Verify(mock => mock.RenameFile(
                    It.Is<string>(originalFile => originalFile == finalMergeFile),
                    It.Is<string>(renamedFile => renamedFile == outputFilePath)), Times.Once);

                mockFileIO.VerifyNoOtherCalls();
                mockIntegerFileMerger.VerifyNoOtherCalls();
            }

            /// <summary>
            /// Calculates the expected intermediate files
            /// </summary>
            /// <param name="chunkFilesCount">The numebr of chink files being merged</param>
            /// <param name="mergeCount">The merge count</param>
            /// <param name="mergeGenerations">The number of expected merge generations</param>
            /// <param name="intermediateFileTemplate">The intermediate file tempalte</param>
            /// <param name="outputDirectory">The output directory</param>
            /// <param name="startingGeneration">The number of the first merge generation</param>
            /// <returns></returns>
            private List<List<string>> CalculateExpectedIntermediateFiles(int chunkFilesCount, int mergeCount, 
                int mergeGenerations, string intermediateFileTemplate, string outputDirectory, int startingGeneration)
            {
                List<List<string>> expectedIntermediateFiles = Enumerable.Range(startingGeneration, mergeGenerations)
                    .Select(generation =>
                    {
                        int generationOutputFiles = CalculateNumberOfGenerationOutputFiles(chunkFilesCount,
                            mergeCount, generation - startingGeneration + 1);

                        return Enumerable.Range(1, generationOutputFiles)
                            .Select(fileNumber => Path.Combine(outputDirectory, 
                                string.Format(intermediateFileTemplate, generation, fileNumber)))
                            .ToList();
                    })                        
                    .ToList();

                return expectedIntermediateFiles;
            }

            /// <summary>
            /// Calculates the number of merge generations from the chunk files count and the merge count
            /// </summary>
            /// <param name="chunkFilesCount">The number of chunk files that are being merged</param>
            /// <param name="mergeCount">The number of files to be merged together in a single generation</param>
            /// <returns>The number of merge generations it will take to finish the merges</returns>
            private int CalculateMergeGenerations(int chunkFilesCount, int mergeCount)
            {
                //Since the merge operation is logarithmic, the number of generations is the 
                //log (base mergeCount) of the number of chunk files rounded up to the next
                //whole number
                int mergeGenerations = (int)Math.Ceiling(Math.Log(chunkFilesCount, mergeCount));

                //If there is one file, the log operation will result in a 0. Make sure that the number
                //of generations is always at least 1
                mergeGenerations = Math.Max(mergeGenerations, 1);

                return mergeGenerations;
            }

            /// <summary>
            /// Calculates the number of output files that will result from a particular merge generation
            /// </summary>
            /// <param name="chunkFilesCount">The number of chunk files that are being merged</param>
            /// <param name="mergeCount">The number of files to be merged together in a single generation</param>
            /// <param name="generation">The generation number (starts at 1)</param>
            /// <returns>The number of output files that will result from this merge generation</returns>
            private int CalculateNumberOfGenerationOutputFiles(int chunkFilesCount, int mergeCount, int generation)
            {
                //We can find the number of output files for a generation by dividing the number of chunk files 
                //by mergeCount to the Nth power, where N is the generation number. We then round that number up.
                int numOfOutputFiles = (int)Math.Ceiling(chunkFilesCount / (Math.Pow(mergeCount, generation)));

                return numOfOutputFiles;
            }

            /// <summary>
            /// Generates the names of chunk files for use in testing
            /// </summary>
            /// <param name="numOfFiles">The number of file names to generate</param>
            /// <returns>The collection of generated file names</returns>
            private List<string> CreateChunkFileNames(int numOfFiles)
            {
                const string fileTemplate = "ChunkFile{0}.txt";

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

                //Mock the DeleteFile method to do nothing
                mockFileIO.Setup(mock => mock.DeleteFile(It.IsAny<string>()));

                //Mock the RenameFile method to do nothing
                mockFileIO.Setup(mock => mock.DeleteFile(It.IsAny<string>()));

                return mockFileIO;
            }

            /// <summary>
            /// Creates and returns a mock integer file merger
            /// </summary>
            /// <param name="numOfChunkFiles">The number of chunk files being merged</param>
            /// <returns>The mock integer file merger</returns>
            private Mock<IIntegerFileMerger> CreateMockIntegerFileMerger(int numOfChunkFiles)
            {
                Mock<IIntegerFileMerger> mockIntegerFileMerger = new Mock<IIntegerFileMerger>();

                Func<List<string>, int, string, string, Action<int>, List<string>> createIntermediateFileNames = 
                    (integerFiles, mergeCount, fileTemplate, outputDirectory, updateProgress) =>
                    {
                        //Calculate the number of output files and generate the output file names
                        int numOfOutputFiles = (int)Math.Ceiling(Decimal.Divide(integerFiles.Count, mergeCount));

                        List<string> outputFiles = Enumerable
                            .Range(1, numOfOutputFiles)
                            .Select(fileNum => Path.Combine(outputDirectory, string.Format(fileTemplate, fileNum)))
                            .ToList();

                        return outputFiles;
                    };

                //Mock MergeIntegerFiles so that it just returns the output files when the updateProgress parameter is null
                mockIntegerFileMerger.Setup(mock => mock.MergeIntegerFiles(It.IsAny<List<string>>(), It.IsAny<int>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.Is<Action<int>>(updateProgress => updateProgress == null)))
                        .Returns(createIntermediateFileNames);

                //Mock MergeIntegerFiles the updateProgress method is called 10 times per integer file being merged 
                //(mimicking integer files with 10 integers each) when updateProgress != null. It will also
                //return output files based on the template that was passed in
                var mergeWithProgressSetup = mockIntegerFileMerger.Setup(mock => mock.MergeIntegerFiles(
                        It.IsAny<List<string>>(), It.IsAny<int>(),  It.IsAny<string>(), It.IsAny<string>(), 
                        It.Is<Action<int>>(updateProgress => updateProgress != null)))
                    .Returns(createIntermediateFileNames)
                    .Callback((List<string> integerFiles, int mergeCount, string fileTemplate, string outputDirectory, 
                        Action<int> updateProgress) =>
                    {
                        //Call the updateProgress method 10 times for every input file being merged
                        Enumerable.Range(1, 10 * numOfChunkFiles).ToList().ForEach(number => updateProgress.Invoke(number));
                    });


                return mockIntegerFileMerger;
            }

            /// <summary>
            /// Verifies that the integer files were merged correctly
            /// </summary>
            /// <param name="mockIntegerFileMerger">A mock integer file merger</param>
            /// <param name="chunkFiles">The chunk files that were merged</param>
            /// <param name="expectedIntermediateFiles">The expected intermediate files</param>
            /// <param name="mergeCount">The merge count</param>
            /// <param name="intermediateFileTemplate">The template for the intermediate files</param>
            /// <param name="outputDirectory">The output directory</param>
            /// <param name="startingGeneration">The number to use as the starting generation</param>
            private void VerifyIntegerFileMerges(Mock<IIntegerFileMerger> mockIntegerFileMerger, List<string> chunkFiles, 
                List<List<string>> expectedIntermediateFiles, int mergeCount, string intermediateFileTemplate, 
                string outputDirectory, int startingGeneration)
            {
                //Verify that the integer files were merged correctly for each merge generation
                //The first generation merge will involve the chunk files and each subsequent generation
                //merge will involve the output files from the previous generation
                string expectedFileTemplate = string.Format(intermediateFileTemplate, startingGeneration, "{0}");

                mockIntegerFileMerger.Verify(mock => mock.MergeIntegerFiles(
                    It.Is<List<string>>(integerFiles => integerFiles.OrderBy(file => file).SequenceEqual(chunkFiles.OrderBy(file => file))),
                    It.Is<int>(mergeCountParam => mergeCountParam == mergeCount),
                    It.Is<string>(fileTemplate => fileTemplate == expectedFileTemplate),
                    It.Is<string>(directory => directory == outputDirectory),
                    It.IsAny<Action<int>>()), Times.Once);

                //Verify for the subsequent generations (output from gen 1 to output of gen N - 1)
                for (int generation = startingGeneration; generation < startingGeneration + expectedIntermediateFiles.Count - 1; generation++)
                {
                    List<string> expectedFiles = expectedIntermediateFiles[generation - startingGeneration]
                        .OrderBy(file => file)
                        .ToList();

                    expectedFileTemplate = string.Format(intermediateFileTemplate, generation + 1, "{0}");

                    mockIntegerFileMerger.Verify(mock => mock.MergeIntegerFiles(
                        It.Is<List<string>>(integerFiles => integerFiles.OrderBy(file => file).SequenceEqual(expectedFiles)),
                        It.Is<int>(mergeCountParam => mergeCountParam == mergeCount),
                        It.Is<string>(fileTemplate => fileTemplate == expectedFileTemplate),
                        It.Is<string>(directory => directory == outputDirectory),
                        It.IsAny<Action<int>>()), Times.Once);
                }
            }
        }
    }
}
