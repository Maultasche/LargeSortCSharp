using System;
using System.Collections.Generic;
using System.Text;

namespace IntSort
{
    /// <summary>
    /// Implements integer file merger functionality
    /// </summary>
    public class IntegerFileMerger : IIntegerFileMerger
    {
        /// <see cref="IIntegerFileMerger.MergeIntegerFiles(List{string}, int, string, string, Action{int})"/>
        public List<string> MergeIntegerFiles(List<string> integerFiles, int mergeCount, string fileTemplate, string outputDirectory,
            Action<int> updateProgress)
        {
            throw new NotImplementedException();
        }
    }
}
