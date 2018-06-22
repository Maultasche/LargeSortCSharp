using System;
using System.Collections.Generic;
using System.Linq;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using ShellProgressBar;

using LargeSort.Shared;

namespace IntGen
{
    class Program
    {
        private static IServiceProvider serviceProvider = null;

        static void Main(string[] args)
        {
            try
            {
                //Create the dependency injection service provider
                serviceProvider = CreateServiceProvider();

                //Parse the arguments from the command line and act in accordance to whether they were parsed
                //successfully or not
                var parseResult = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        //Create the progress bar
                        var progressBar = CreateProgressBar(options.Count);

                        //Generate the integers and pass a method that will update the progress bar
                        GenerateIntegers(options, generatedIntegers => UpdateProgressBar(progressBar, generatedIntegers));

                        GenerateIntegers(options, generatedIntegers => { });

                        Console.WriteLine();
                    })
                    .WithNotParsed(errors => Environment.ExitCode = -1);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        static ProgressBar CreateProgressBar(uint totalIntegers)
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

            var progressBar = new ProgressBar(Convert.ToInt32(totalIntegers), "Generating Integers...", options);

            return progressBar;
        }

        static void UpdateProgressBar(ProgressBar progressBar, int generatedIntegers)
        {
            //If we update for every integer, and large numbers of integers are being generated, 
            //the application will spend more time updating the progress bar than it will doing its real work
            //So the solution is to update the progress bar every 1% of generated integers.
            //For a million integers, this change reduced the runtime from over 9 minutes 
            //to under a second

            int tickSize = progressBar.MaxTicks / 100;

            if(generatedIntegers % tickSize == 0)
            {
                progressBar.Tick(generatedIntegers);

                if (generatedIntegers == progressBar.MaxTicks)
                {
                    progressBar.Message = "Integer Generation Complete";
                }          
            }
        }

        /// <summary>
        /// Creates a service provider used for dependency injection
        /// </summary>
        /// <returns>The service provider for this application</returns>
        static IServiceProvider CreateServiceProvider()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IFileIO, FileIO>();
            serviceCollection.AddSingleton<IRandomIntegerGenerator, RandomIntegerGenerator>();
            serviceCollection.AddSingleton<IIntegerFileCreator, IntegerFileCreator>();

            return serviceCollection.BuildServiceProvider();
        }


        static void GenerateIntegers(CommandLineOptions options, Action<int> updateIntegerGenerationStatus)
        {
            //Get an instance of the integer file creator
            IIntegerFileCreator integerFileCreator = serviceProvider.GetService<IIntegerFileCreator>();

            //Get an instance of the random integer generator
            IRandomIntegerGenerator randomIntegerGenerator = serviceProvider.GetService<IRandomIntegerGenerator>();

            //Create the random integers. The IEnumerable is actually a generator, so it generates them one at a time as
            //it is enumerated rather than a whole bunch at once
            IEnumerable<int> randomIntegers = randomIntegerGenerator.CreateIntegerGenerator(options.LowerBound, 
                options.UpperBound, (int)options.Count);

            int integersGenerated = 0;


            //When an integer is enumerated, update the integer generation status
            randomIntegers = randomIntegers.Select(integer =>
            {
                integersGenerated++;

                updateIntegerGenerationStatus(integersGenerated);              

                return integer;
            });

            //Create the integer file with the randomly-generated integers
            integerFileCreator.CreateIntegerTextFile(randomIntegers, options.FilePath);
        }

        static void OutputParsedOptions(CommandLineOptions options)
        {
            Console.WriteLine("Count: " + options.Count);
            Console.WriteLine("Lower Bound: " + options.LowerBound);
            Console.WriteLine("Upper Bound: " + options.UpperBound);
            Console.WriteLine("Output File: " + options.FilePath);
        }
    }
}
