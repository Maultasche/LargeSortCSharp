using System;
using System.Collections.Generic;
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
        const string ChunkInProgressMessage = "Creating Chunk Files...";
        const string ChunkCompletedMessage = "Chunk File Creation Complete";

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
                        //Sort the integers
                        List<string> intermediateFiles = SortIntegers(options);

                        Console.WriteLine();

                        //If the option to keep the intermediate files was not specified, delete them
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
        /// <param name="updateMergeProgress"></param>
        private static List<string> SortIntegers(CommandLineOptions options)
        {
            IFileIO fileIO = serviceProvider.GetService<IFileIO>();
            IIntegerFileInfoCollector integerFileInfoCollector = serviceProvider.GetService<IIntegerFileInfoCollector>();

            //Collect information on the input file and display it
            IntegerFileInfo inputFileInfo = integerFileInfoCollector.GetIntegerFileInfo(options.InputFile, (int)options.ChunkSize);

            DisplayInputFileInfo(inputFileInfo);

            //Store the intermediate files so that they can be deleted later on
            List<string> intermediateFiles = new List<string>();

            //Find the output directory where the output file will be written
            string outputDirectory = fileIO.GetDirectoryFromFilePath(options.OutputFilePath);

            //Create the progress bar for the chunk file creation
            var chunkProgressBar = CreateProgressBar(inputFileInfo.NumOfChunks, ChunkInProgressMessage);

            //Create the progress callback that will be passed to the chunk file creation code
            Action<int> updateChunkProgress = (int chunkNum) => UpdateProgressBar(chunkProgressBar, chunkNum,
                ChunkCompletedMessage);

            //Create the sorted chunk files
            string chunkFileTemplate = string.Format(GenFileTemplate, "1", "{0}");

            List<string> chunkFiles = CreateSortedChunkFiles(options.InputFile, (int)options.ChunkSize,
                outputDirectory, chunkFileTemplate, updateChunkProgress);

            intermediateFiles.AddRange(chunkFiles);

            return intermediateFiles;
        }

        /// <summary>
        /// Creates sorted chunks files from an input file
        /// </summary>
        /// <param name="inputFile">The input file from which the integers are to be read</param>
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
        /// Updates a progress bar
        /// </summary>
        /// <param name="progressBar">The progress bar to be updated</param>
        /// <param name="currentSteps">The total number of steps that have been completed so far</param>
        /// <param name="completeMessage">The message to display when all steps have been completed</param>
        static void UpdateProgressBar(ProgressBar progressBar, int currentSteps, string completeMessage)
        {
            //If we update for every step, and there are large numbers of steps being completed in a short amount of time, 
            //the application will spend more time updating the progress bar than it will doing its real work
            //So the solution is to update the progress bar every 1% of Steps.

            int tickSize = Math.Max(progressBar.MaxTicks / 100, 1);

            if (currentSteps % tickSize == 0)
            {
                progressBar.Tick(currentSteps);

                if (currentSteps == progressBar.MaxTicks)
                {
                    progressBar.Message = completeMessage;
                }
            }
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

            return serviceCollection.BuildServiceProvider();
        }
    }
}
