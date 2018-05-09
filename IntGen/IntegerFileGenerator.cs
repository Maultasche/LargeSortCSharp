using System;
using System.Collections.Generic;
using System.Text;

namespace IntGen
{
    /// <summary>
    /// Implements the functionality for generating an integer file
    /// </summary>
    public class IntegerFileGenerator : IIntegerFileGenerator
    {
        IRandomIntegerGenerator randomIntegerGenerator = null;

        public IntegerFileGenerator(IRandomIntegerGenerator randomIntegerGenerator)
        {
            this.randomIntegerGenerator = randomIntegerGenerator;
        }
    }
}
