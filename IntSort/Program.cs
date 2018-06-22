using System;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;

using LargeSort.Shared;

namespace IntSort
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
                        Console.WriteLine("Input File: " + options.InputFile);
                        Console.WriteLine("Chunk Size: " + options.ChunkSize);
                        Console.WriteLine("Keep Intermediate: " + options.KeepIntermediate);
                        Console.WriteLine("Output File: " + options.OutputFilePath);
                    })
                    .WithNotParsed(errors => Environment.ExitCode = -1);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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
            serviceCollection.AddSingleton<IIntegerFileCreator, IntegerFileCreator>();
            serviceCollection.AddSingleton<IChunkStreamCreator, ChunkStreamCreator>();
            serviceCollection.AddSingleton<IChunkFileCreator, ChunkFileCreator>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
