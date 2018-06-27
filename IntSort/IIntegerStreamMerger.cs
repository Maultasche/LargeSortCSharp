using System;
using System.Collections.Generic;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Defines integer stream merge functionality
    /// </summary>
    interface IIntegerStreamMerger
    {
        /// <summary>
        /// Merges the sorted integers coming from one or more integers streams into a single sorted output integer stream
        /// </summary>
        /// <remarks>
        /// This method will take multiple streams of sorted integers and merge them so that output stream contains
        /// all the integers from the input streams in a sorted order.
        /// This method assumes that inputStreams != null and outputStream != null.
        /// This method assumes that the integers coming from the input streams are already sorted
        /// </remarks>
        /// <param name="inputStreams">The streams whose integers are to be merged.</param>
        /// <param name="outputStream">The output stream into which the merged integers are to be written in sorted
        /// order</param>
        /// <param name="updateProgress">A method that will be called to update chunk file creation progress. The 
        /// number of integers that have been merged so far will be passed to this method whenever an integer
        /// is merged</param>
        void MergeIntegerStreams(List<IIntegerStreamReader> inputStreams, IIntegerStreamWriter outputStream, 
            Action<int> updateProgress = null);
    }
}
