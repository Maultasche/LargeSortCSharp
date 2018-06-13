using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace IntGen
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
                yield return new Example("Generating a file of random integers", new CommandLineOptions()
                {
                    Count = 100,
                    LowerBound = -100,
                    UpperBound = 100,
                    FilePath = "output/randomIntegers.txt"
                });
            }
        }

        /// <summary>
        /// Get or sets the number of integers to be generated
        /// </summary>
        [Option('c', "count", Required = true, HelpText = "The number of integers to generate")]        
        public uint Count { get; set; }

        /// <summary>
        /// Gets or sets the lower bound (inclusive) of the range of integers to be generated
        /// </summary>
        [Option('l', "lowerBound", Required = true, HelpText = "The lower bound of the integer range")]
        public int LowerBound { get; set; }

        [Value(0, MetaName = "File Path", Required = true, HelpText = "The file to which the generated integers will be written")]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the upper bound (inclusive) of the range of integers to be generated
        /// </summary>
        [Option('u', "upperBound", Required = true, HelpText = "The upper bound of the integer range")]
        public int UpperBound { get; set; }
    }
}
