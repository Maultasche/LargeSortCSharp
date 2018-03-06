using System;
using System.IO;

using CommandLine;

namespace IntGen
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parseResult = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(options => GenerateIntegers(options))
                    .WithNotParsed(errors => Environment.ExitCode = -1);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
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
