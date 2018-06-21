using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace IntSort
{
    /// <summary>
    /// Contains the options that can be specified on the command line
    /// </summary>
    public class CommandLineOptions
    {
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Sorting a file of random integers", new CommandLineOptions()
                {
                    ChunkSize = 10000,
                    InputFile = "data/randomIntegers.txt",
                    KeepIntermediate = true,
                    OutputFilePath = "output/sortedIntegers.txt"
                });
            }
        }

        /// <summary>
        /// Gets or sets the size of the chunks of integers that will be sorted
        /// </summary>
        [Option('c', "chunkSize", Required = true, HelpText = "The number of integers to be loaded in memory and sorted at any given time")]
        public uint ChunkSize { get; set; }

        /// <summary>
        /// Gets or sets the input file to be used
        /// </summary>
        [Option('i', "inputFile", Required = true, HelpText = "The path to the input file containing the integers to be sorted")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets whether the intermediate files are to be kept
        /// </summary>
        [Option(longName: "keepIntermediate", Default = false, HelpText = "Keeps all intermediate files")]
        public bool KeepIntermediate { get; set; }

        /// <summary>
        /// Gets or sets the file path where the output files will be written
        /// </summary>
        [Value(0, MetaName = "Output File Path", Required = true, HelpText = "The file to which the sorted integers will be written")]
        public string OutputFilePath { get; set; }
    }
}
