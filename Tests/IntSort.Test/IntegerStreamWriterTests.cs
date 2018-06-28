using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using SharedTest;

namespace IntSort.Test
{
    /// <summary>
    /// Contains the tests for the IntegerStreamWriter class
    /// </summary>
    public class IntegerStreamWriterTests
    {
        /// <summary>
        /// Defines tests for the WriteInteger method
        /// </summary>
        [TestFixture]
        public class WriteIntegerTests
        {
            /// <summary>
            /// Tests writing a small number of integers
            /// </summary>
            [Test]
            public void TestWriteIntegerSmallNumber()
            {
                //Create the test data
                List<int> integers = TestData.GenerateTestData(10);

                //Run the test
                RunWriteIntegerTest(integers);
            }

            /// <summary>
            /// Tests writing a large number of integers
            /// </summary>
            [Test]
            public void TestWriteIntegerLargeNumber()
            {
                //Create the test data
                List<int> integers = TestData.GenerateTestData(1000);

                //Run the test
                RunWriteIntegerTest(integers);
            }

            /// <summary>
            /// Tests writing a single integer
            /// </summary>
            [Test]
            public void TestWriteIntegerSingle()
            {
                //Create the test data
                List<int> integers = TestData.GenerateTestData(1);

                //Run the test
                RunWriteIntegerTest(integers);
            }

            /// <summary>
            /// Runs a test of the WriteInteger method
            /// </summary>
            /// <remarks>
            /// This test verifies that the integers are written in the correct order
            /// </remarks>
            /// <param name="testIntegers">The integers to be used as test data. These integers 
            /// will be written to a stream</param>
            private void RunWriteIntegerTest(List<int> testIntegers)
            {
                Assert.That(testIntegers, Is.Not.Null);

                //Create memory stream and stream writer
                using (Stream integerStream = new MemoryStream())
                using (StreamWriter streamWriter = new StreamWriter(integerStream))
                {
                    //Create the integer stream writer
                    IIntegerStreamWriter integerStreamWriter = new IntegerStreamWriter();

                    //Write all the test integers to the stream
                    testIntegers.ForEach(integer => integerStreamWriter.WriteInteger(streamWriter, integer));

                    //Flush the stream writer
                    streamWriter.Flush();

                    //Reset the stream's position to the beginning
                    integerStream.Position = 0;

                    //Verify that the integers were written correctly by reading them out one by one
                    using (StreamReader streamReader = new StreamReader(integerStream))
                    {
                        testIntegers.ForEach(expectedInteger =>
                        {
                            string line = streamReader.ReadLine();

                            int actualInteger = Convert.ToInt32(line.TrimEnd());

                            Assert.That(actualInteger, Is.EqualTo(expectedInteger));
                        });
                    }
                }
            }
        }
    }
}
