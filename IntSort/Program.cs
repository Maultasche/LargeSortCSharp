using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using ShellProgressBar;

using LargeSort.Shared;

namespace IntSort
{
    class Program
    {
        const string GenFileTemplate = "gen{0}-{1}.txt";
        const string ChunkProgressMessage = "Creating chunk files";
        const string MergeProgressMessage = "Merging Gen {0} intermediate files";

        //This is the number of the first merge generation. We consider the creation of the 
        //chunk files to be the first generation and the results of the first merge to 
        //be the second generation
        const int FirstMergeGeneration = 2;

        //The number of files to merge at a time
        const int MergeCount = 10;

        private static IServiceProvider serviceProvider = null;

        static void Main(string[] args)
        {
            ConsoleInfo consoleSettings = ConsoleInfo.CurrentInfo();

            try
            {
                //Create the dependency injection service provider
                serviceProvider = CreateServiceProvider();

                //Parse the arguments from the command line and act in accordance to whether they were parsed
                //successfully or not
                var parseResult = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        //Keep track of when the sort operation begins
                        //TODO

                        //Sort the integers
                        List<string> intermediateFiles = SortIntegers(options);

                        //If the option to keep the intermediate files was not specified, delete them
                        //TODO

                        //Display the amount of time it took to perform the entire sort operation
                        //TODO
                    })
                    .WithNotParsed(errors => Environment.ExitCode = -1);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                ConsoleInfo.RestoreSettings(consoleSettings);
            }
        }

        /// <summary>
        /// Sorts the integers found in an input file and writes the sorted integers to an output file
        /// </summary>
        /// <param name="options">The command line options</param>
        private static List<string> SortIntegers(CommandLineOptions options)
        {
            IIntegerFileInfoCollector integerFileInfoCollector = serviceProvider.GetService<IIntegerFileInfoCollector>();

            //Collect information on the input file
            IntegerFileInfo inputFileInfo = integerFileInfoCollector.GetIntegerFileInfo(options.InputFile, (int)options.ChunkSize);

            //Store the intermediate files so that they can be deleted later on
            List<string> intermediateFiles = new List<string>();

            List<string> chunkFiles = CreateChunkFiles(options, inputFileInfo);

            intermediateFiles.AddRange(chunkFiles);

            List<string> mergeIntermediateFiles = MergeChunkFiles(chunkFiles, options, inputFileInfo);

            intermediateFiles.AddRange(mergeIntermediateFiles);

            return intermediateFiles;
        }

        /// <summary>
        /// Creates the chunk files
        /// </summary>
        /// <param name="options">The command line options</param>
        /// <param name="inputFileInfo">Information regarding the input file</param>
        /// <returns>The chunk files that were created</returns>
        private static List<string> CreateChunkFiles(CommandLineOptions options, IntegerFileInfo inputFileInfo)
        {
            IFileIO fileIO = serviceProvider.GetService<IFileIO>();

            DisplayInputFileInfo(inputFileInfo);

            //Find the output directory where the output file will be written
            string outputDirectory = fileIO.GetDirectoryFromFilePath(options.OutputFilePath);

            //Create the progress bar for the chunk file creation
            var chunkProgressBar = CreateProgressBar(inputFileInfo.NumOfChunks, ChunkProgressMessage);

            //Create the progress callback that will be passed to the chunk file creation code
            Action<int> updateChunkProgress = (int chunkNum) => UpdateProgressBar(chunkProgressBar, chunkNum);

            //Create the sorted chunk files
            string chunkFileTemplate = string.Format(GenFileTemplate, "1", "{0}");

            List<string> chunkFiles = CreateSortedChunkFiles(options.InputFile, (int)options.ChunkSize,
                outputDirectory, chunkFileTemplate, updateChunkProgress);

            OnProgressBarCompleted();

            DisplayChunkFileCreationResults(chunkFiles.Count);

            return chunkFiles;
        }

        /// <summary>
        /// Creates sorted chunks files from an input file
        /// </summary>
        /// <param name="inputFile">The input file from which the integers are to be read</param>
        /// <param name="chunkSize">The chunk size to use when chunking the input</param>
        /// <param name="outputDirectory">The output directory where the intermediate files are to be written</param>
        /// 
        /// <returns>The names of the chunk files that were created</returns>
        private static List<string> CreateSortedChunkFiles(string inputFile, int chunkSize, string outputDirectory, 
            string chunkFileTemplate, Action<int> updateChunkProgress)
        {
            IFileIO fileIO = serviceProvider.GetService<IFileIO>();

            //Create the chunk stream generator from the input file
            IChunkStreamCreator chunkStreamCreator = serviceProvider.GetService<IChunkStreamCreator>();

            StreamReader inputStreamReader = fileIO.CreateFileStreamReader(inputFile);

            //Create the chunk generator and add a transformation that sorts each chunk as it is emitted
            IEnumerable<List<int>> sortedChunkGenerator = chunkStreamCreator
                .CreateIntegerChunkGenerator(inputStreamReader, chunkSize)
                .Select(chunk => chunk.OrderBy(integer => integer).ToList());

            //Create the chunk files
            IChunkFileCreator chunkFileCreator = serviceProvider.GetService<IChunkFileCreator>();

            List<string> chunkFiles = chunkFileCreator.CreateChunkFiles(sortedChunkGenerator, chunkFileTemplate, outputDirectory, 
                updateChunkProgress);

            return chunkFiles;
        }

        /// <summary>
        /// Merges a set of chunk files into a single output file
        /// </summary>
        /// <param name="chunkFiles">The chunk files to be merged</param>
        /// <param name="options">The command line options</param>
        /// <param name="inputFileInfo">Information regarding the input file</param>
        /// <returns>The intermediate files that were created while merging</returns>
        private static List<string> MergeChunkFiles(List<string> chunkFiles, CommandLineOptions options,
            IntegerFileInfo inputFileInfo)
        {
            List<string> intermediateFiles = new List<string>();
           
            IFileIO fileIO = serviceProvider.GetService<IFileIO>();
            IChunkFileMerger chunkFileMerger = serviceProvider.GetService<IChunkFileMerger>();

            //Find the output directory where the output file will be written
            string outputDirectory = fileIO.GetDirectoryFromFilePath(options.OutputFilePath);

            //Convert the chunk file names into chunk file paths
            List<string> chunkFilePaths = chunkFiles
                .Select(chunkFileName => Path.Combine(outputDirectory, chunkFileName))
                .ToList();

            //Keep a dictionary of progress bars where the key is the merge generation number, and
            //the value is the progress bar for that merge generation
            Dictionary<int, ProgressBar> mergeProgressBars = new Dictionary<int, ProgressBar>();

            Action<int, int> updateMergeProgress = (int gen, int integersProcessed) =>
            {
                if (mergeProgressBars.ContainsKey(gen))
                {
                    bool stepsComplete = UpdateProgressBar(mergeProgressBars[gen], integersProcessed);

                    if(stepsComplete)
                    {
                        OnProgressBarCompleted();

                        //Display the results of the merge operation. We can calculate the number of output
                        //files that will result from each merge generation
                        int mergedFilesCount = CalculateNumberOfGenerationOutputFiles(chunkFiles.Count, MergeCount, 
                            gen - FirstMergeGeneration + 1);

                        DisplayGenMergeResults(gen, mergedFilesCount);
                    }
                }
                else
                {
                    //Create a progress bar for the current generation
                    mergeProgressBars[gen] = CreateProgressBar(inputFileInfo.NumOfIntegers, 
                        string.Format(MergeProgressMessage, gen - 1));
                }
            };

            
            chunkFileMerger.MergeChunkFilesIntoSingleFile(chunkFilePaths, MergeCount, GenFileTemplate, 
                Path.GetFileName(options.OutputFilePath), outputDirectory, FirstMergeGeneration, updateMergeProgress);

            return intermediateFiles;
        }

        /// <summary>
        /// Calculates the number of output files that will result from a particular merge generation
        /// </summary>
        /// <param name="chunkFilesCount">The number of chunk files that are being merged</param>
        /// <param name="mergeCount">The number of files to be merged together in a single generation</param>
        /// <param name="generation">The generation number (starts at 1)</param>
        /// <returns>The number of output files that will result from this merge generation</returns>
        private static int CalculateNumberOfGenerationOutputFiles(int chunkFilesCount, int mergeCount, int generation)
        {
            //We can find the number of output files for a generation by dividing the number of chunk files 
            //by mergeCount to the Nth power, where N is the generation number. We then round that number up.
            int numOfOutputFiles = (int)Math.Ceiling(chunkFilesCount / (Math.Pow(mergeCount, generation)));

            return numOfOutputFiles;
        }

        #region UI Methods

        /// <summary>
        /// Creates a progress bar
        /// </summary>
        /// <param name="totalSteps">The total number of steps the progress bar will represent</param>
        /// <param name="message">The message to display next to the progress bar</param>
        /// <returns>The constructed progress bar</returns>
        static ProgressBar CreateProgressBar(int totalSteps, string message)
        {
            var options = new ProgressBarOptions()
            {
                BackgroundCharacter = '\u2593',
                ProgressBarOnBottom = true,
                DisplayTimeInRealTime = false,
                ForegroundColor = ConsoleColor.Gray,
                ForegroundColorDone = ConsoleColor.Gray,
                BackgroundColor = ConsoleColor.DarkGray
            };

            var progressBar = new ProgressBar(totalSteps, message, options);

            return progressBar;
        }

        /// <summary>
        /// Displays the integer file information for the input file
        /// </summary>
        /// <param name="inputFileInfo">The input file information</param>
        private static void DisplayInputFileInfo(IntegerFileInfo inputFileInfo)
        {
            string integerOutput = "Number of Integers: {0}";
            string chunkOutput = "Number of Chunks: {0}";

            Console.WriteLine(string.Format(integerOutput, inputFileInfo.NumOfIntegers));
            Console.WriteLine(string.Format(chunkOutput, inputFileInfo.NumOfChunks));
        }

        /// <summary>
        /// Displays information regarding the results of chunk file creation
        /// </summary>
        /// <param name="chunkFilesCreated">The number of chunk files that were created</param>
        private static void DisplayChunkFileCreationResults(int chunkFilesCreated)
        {
            string output = "{0} Gen 1 intermediate files were generated";

            Console.WriteLine(string.Format(output, chunkFilesCreated));
        }

        /// <summary>
        /// Displays a message indicating the result of a single generation merge operation
        /// </summary>
        /// <remarks>
        /// This method assumes that gen > 0 and that mergedFilesCount > 0
        /// </remarks>
        /// <param name="gen">The merge generation</param>
        /// <param name="mergedFilesCount">The number of intermediate files that were a product
        /// of the merge operation</param>
        private static void DisplayGenMergeResults(int gen, int mergedFilesCount)
        {
            Debug.Assert(gen > 0);
            Debug.Assert(mergedFilesCount > 0);

            string output = "Gen {0} files were merged into {1}";
            string multipleFileDescriptor = "{0} Gen {1} intermediate files";
            string singleFileDescriptor = "a single output file";
            string fileDescriptor = string.Empty;
            
            //Configure the file descriptor depending on how many files resulted from the merge operation
            if(mergedFilesCount == 1)
            {
                fileDescriptor = singleFileDescriptor;
            }
            else
            {
                fileDescriptor = string.Format(multipleFileDescriptor, mergedFilesCount, gen);
            }

            Console.WriteLine(string.Format(output, gen - 1, fileDescriptor));
        }

        /// <summary>
        /// Takes care of the cleanup work that is necessary after the progress bar completes
        /// </summary>
        /// <remarks>
        /// When the progress bar completes, the cursor remains on the message above the progress bar.
        /// So as a result, any subsequent output will be overlaid on the progress bar. This method
        /// corrects the cursor position so that any subsequent output will be shown below the progress
        /// bar.
        /// </remarks>
        static void OnProgressBarCompleted()
        {
            Console.CursorTop += 2;
        }

        /// <summary>
        /// Updates a progress bar
        /// </summary>
        /// <param name="progressBar">The progress bar to be updated</param>
        /// <param name="currentSteps">The total number of steps that have been completed so far</param>
        /// <param name="completeMessage">The message to display when all steps have been completed</param>
        /// <returns>true if all steps in the progress bar have been completed, otherwise false</returns>
        static bool UpdateProgressBar(ProgressBar progressBar, int currentSteps)
        {
            bool stepsCompleted = false;

            //If we update for every step, and there are large numbers of steps being completed in a short amount of time, 
            //the application will spend more time updating the progress bar than it will doing its real work
            //So the solution is to update the progress bar every 1% of Steps.

            int tickSize = Math.Max(progressBar.MaxTicks / 100, 1);

            if (currentSteps % tickSize == 0 || currentSteps == progressBar.MaxTicks)
            {
                progressBar.Tick(currentSteps);

                if (currentSteps == progressBar.MaxTicks)
                {
                    stepsCompleted = true;
                }
            }

            return stepsCompleted;
        }

        #endregion

        /// <summary>
        /// Creates a service provider used for dependency injection
        /// </summary>
        /// <returns>The service provider for this application</returns>
        static IServiceProvider CreateServiceProvider()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IFileIO, FileIO>();
            serviceCollection.AddSingleton<IIntegerFileCreator, IntegerFileCreator>();
            serviceCollection.AddSingleton<IChunkStreamCreator, ChunkStreamCreator>();
            serviceCollection.AddSingleton<IChunkFileCreator, ChunkFileCreator>();
            serviceCollection.AddSingleton<IChunkFileMerger, ChunkFileMerger>();
            serviceCollection.AddSingleton<IIntegerFileInfoCollector, IntegerFileInfoCollector>();
            serviceCollection.AddSingleton<IIntegerFileMerger, IntegerFileMerger>();
            serviceCollection.AddSingleton<IIntegerStreamMerger, IntegerStreamMerger>();
            serviceCollection.AddSingleton<IIntegerStreamReader, IntegerStreamReader>();
            serviceCollection.AddSingleton<IIntegerStreamWriter, IntegerStreamWriter>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
