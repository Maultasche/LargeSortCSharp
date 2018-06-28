using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace IntSort
{
    /// <summary>
    /// Implements integer stream merge functionality
    /// </summary>
    public class IntegerStreamMerger : IIntegerStreamMerger
    {
        IIntegerStreamReader integerStreamReader = null;
        IIntegerStreamWriter integerStreamWriter = null;

        /// <summary>
        /// Initializes an instance of IntegerStreamMerger
        /// </summary>
        /// <param name="integerStreamReader">An instance of an integer stream reader</param>
        /// <param name="integerStreamWriter">An instance of an integer stream writer</param>
        public IntegerStreamMerger(IIntegerStreamReader integerStreamReader, IIntegerStreamWriter integerStreamWriter)
        {
            this.integerStreamReader = integerStreamReader;
            this.integerStreamWriter = integerStreamWriter;
        }

        /// <see cref="IIntegerStreamMerger.MergeIntegerStreams(List{StreamReader}, StreamWriter, Action{int})"
        public void MergeIntegerStreams(List<StreamReader> inputStreams, StreamWriter outputStream,
            Action<int> updateProgress = null)
        {
            //Convert the stream readers to enumerator tuples and then filter out any streams that
            //are initially empty
            var inputEnumerators = GetInputEnumerators(inputStreams)
                .Where(enumerator => enumerator.EndOfStream == false)
                .ToList();

            int integersWritten = 0;

            //Loop until there are no more enumerators left
            while (inputEnumerators.Count > 0)
            {
                //Write the smallest integer among the enumerators to the output stream
                var minValueEnumerator = WriteSmallestInteger(inputEnumerators, outputStream);

                //Update progress
                integersWritten++;

                updateProgress?.Invoke(integersWritten);

                //Move the min value enumerator to the next position in the stream
                if (!minValueEnumerator.IncrementEnumerator())
                {
                    //If the min value enumerator hit the end of the stream, remove it from
                    //the list of input enumerators
                    inputEnumerators.Remove(minValueEnumerator);
                }
            }
        }

        /// <summary>
        /// Creates a collection of integer stream enumerators that were created from the input streams
        /// </summary>
        /// <param name="inputStreams">The input streams</param>
        /// <returns>A collection of integer stream enumerators</returns>
        private List<IntegerStreamEnumerator> GetInputEnumerators(List<StreamReader> inputStreams)
        {
            List<IntegerStreamEnumerator> inputEnumerators = inputStreams
                .Select(inputStream => integerStreamReader.CreateIntegerReaderGenerator(inputStream))
                .Select(integerGenerator => integerGenerator.GetEnumerator())
                .Select(enumerator => new IntegerStreamEnumerator(enumerator))
                .ToList();

            return inputEnumerators;
        }

        /// <summary>
        /// Extracts the enumerator that is pointing to the smallest possible integer
        /// </summary>
        /// <remarks>
        /// This method assumes that enumerators != null.
        /// </remarks>
        /// <param name="enumerators">The enumerators whose values are to be examined</param>
        /// <returns>The enumerator with the minimum value, or null if no enumerators were present</returns>
        private IntegerStreamEnumerator GetMinValueEnumerator(List<IntegerStreamEnumerator> enumerators)
        {
            Debug.Assert(enumerators != null);

            //Iterate through the enumerators, finding the one with the min value
            var minValueEnumerator = enumerators.Aggregate((minEnumerator, currentEnumerator) =>
            {
                return minEnumerator.CurrentValue <= currentEnumerator.CurrentValue ? 
                    minEnumerator : currentEnumerator;
            });

            return minValueEnumerator;
        }

        /// <summary>
        /// Writes the smallest integer found amongst the input enumerators to the output stream
        /// </summary>
        /// <remarks>
        /// This method assumes that inputEnumerators != null and outputStream != null.
        /// </remarks>
        /// <param name="inputEnumerators">A collection of input enumerators</param>
        /// <param name="outputStream">The output stream to be writen to</param>
        /// <param name="updateProgress">A method that will be called to update stream merging progress. The 
        /// number of integers that have been merged so far will be passed to this method whenever an integer
        /// is written to the output stream</param>
        /// <returns>The enumerator that had been pointing to the smallest integer</returns>
        private IntegerStreamEnumerator WriteSmallestInteger(List<IntegerStreamEnumerator> inputEnumerators,
            StreamWriter outputStream)
        {
            //Get the enumerator the smallest integer value
            var minValueEnumerator = GetMinValueEnumerator(inputEnumerators);

            int minValueInteger = minValueEnumerator.CurrentValue;

            //Write the min value to the output stream
            integerStreamWriter.WriteInteger(outputStream, minValueInteger);

            return minValueEnumerator;
        }

        /// <summary>
        /// Represents the current state of an integer stream enumerator
        /// </summary>
        private class IntegerStreamEnumerator
        {
            /// <summary>
            /// Initializes an IntegerStreamEnumerator object with an IEnumerator
            /// </summary>
            /// <remarks>
            /// This method assumes that MoveNext() has not yet been called on the IEnumerator.
            /// This constructor will call MoveNext() to point the enumerator at the first position
            /// </remarks>
            /// <param name="enumerator">The enumerator to encapsulate in this object</param>
            public IntegerStreamEnumerator(IEnumerator<int> enumerator)
            {
                IntegerEnumerator = enumerator;

                //Move the iterator to the initial position
                IncrementEnumerator();
            }

            /// <summary>
            /// Gets the current enumerator value
            /// </summary>
            /// <remarks>
            /// This property assumes that EndOfStream == false.
            /// </remarks>
            public int CurrentValue
            {
                get
                {
                    Debug.Assert(EndOfStream == false);

                    return IntegerEnumerator.Current;
                }
            }

            /// <summary>
            /// Gets or sets the integer stream enumerator
            /// </summary>
            public IEnumerator<int> IntegerEnumerator { get; set; }

            /// <summary>
            /// Gets or sets whether the enumerator has reached the end of the stream
            /// </summary>
            public bool EndOfStream { get; set; }

            /// <summary>
            /// Increments the enumerator
            /// </summary>
            /// <remarks>
            /// This method also sets the EndOfStream property
            /// </remarks>
            /// <returns>true if the enumerator was incremented to the next value in the stream,
            /// false if it hit the end of the stream</returns>
            public bool IncrementEnumerator()
            {
                bool moveNext = IntegerEnumerator.MoveNext();

                EndOfStream = !moveNext;

                return moveNext;
            }
        }
    }
}
