using System;
using System.IO;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;

using LargeSort.Shared.Interfaces;
using LargeSort.Shared.Implementations;

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
                    .WithParsed(options => GenerateIntegers(options))
                    .WithNotParsed(errors => Environment.ExitCode = -1);
            }
            catch(Exception exception)
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
            serviceCollection.AddSingleton<IRandomIntegerGenerator, RandomIntegerGenerator>();
            serviceCollection.AddSingleton<IIntegerFileCreator, IntegerFileCreator>();

            return serviceCollection.BuildServiceProvider();
        }

        static void GenerateIntegers(CommandLineOptions options)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
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
