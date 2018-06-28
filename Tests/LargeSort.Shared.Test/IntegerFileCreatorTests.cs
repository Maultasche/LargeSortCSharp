using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;
using NUnit.Framework;

using SharedTest;

namespace LargeSort.Shared.Test
{
    /// <summary>
    /// Contains the tests for the IntegerFileCreator class
    /// </summary>
    public class IntegerFileCreatorTests
    {
        /// <summary>
        /// Defines tests for the CreateIntegerGenerator method
        /// </summary>
        [TestFixture]
        public class CreateIntegerTextFileTests
        {
            const string TestFile = "TestFiles/testfile.txt";

            /// <summary>
            /// Tests the creation of an integer file with no integers
            /// </summary>
            [Test]
            public void TestIntegerFileCreationNoIntegers()
            {
                List<int> integers = new List<int>() { };

                RunIntegerFileCreationTest(integers, TestFile, integersExpected: false);
            }

            /// <summary>
            /// Tests the creation of an integer file with a single integer
            /// </summary>
            [Test]
            public void TestIntegerFileCreationOneInteger()
            {
                List<int> integers = new List<int>() { 3 };

                RunIntegerFileCreationTest(integers, TestFile);
            }

            /// <summary>
            /// Tests the creation of an integer file with multiple integers
            /// </summary>
            [Test]
            public void TestIntegerFileCreationMultipleIntegers()
            {
                List<int> integers = new List<int>() { 3, 5, -1, 334, 2, -23, 14 };

                RunIntegerFileCreationTest(integers, TestFile);
            }

            /// <summary>
            /// Tests the creation of an integer file with a random integer generator producing
            /// large numbers of random integers
            /// </summary>
            [Test]
            public void TestIntegerFileCreationRandomIntegers()
            {
                IEnumerable<int> integers = TestData.GenerateTestData(1000);

                RunIntegerFileCreationTest(integers, TestFile);
            }

            /// <summary>
            /// Runs an integer file creation test
            /// </summary>
            /// <param name="integers">The integers to write to the test file</param>
            /// <param name="filePath">The test file path</param>
            /// <param name="integersExpected">True if integers are expected to come from the integers enumerable,
            /// otherwise false.</param>
            private void RunIntegerFileCreationTest(IEnumerable<int> integers, string filePath, bool integersExpected = true)
            {
                //Create the stream that will represent a file stream. We'll use a memory stream instead.
                Stream testStream = new MemoryStream();

                //Mock the File I/O module
                Mock<IFileIO> mockFileIO = new Mock<IFileIO>();

                //Mock the method to create a file
                mockFileIO.Setup(mock => mock.CreateFile(filePath)).Returns(testStream);

                //Keep track of the integers that are written
                List<int> writtenIntegers = new List<int>();

                //Mock the method to write an integer to the stream
                mockFileIO.Setup(mock => mock.WriteIntegerToStream(It.IsAny<StreamWriter>(), It.IsAny<int>()))
                    .Callback((StreamWriter streamWriter, int integer) => {
                        //Verify that the stream writer was created from the correct stream
                        Assert.That(streamWriter.BaseStream, Is.EqualTo(testStream));

                        //Add the integer to the collection of written integers
                        writtenIntegers.Add(integer);
                     });

                //Keep track of the integers that are generated
                List<int> generatedIntegers = new List<int>();

                //When an integer comes out of the enumerable, make a note of it so that we can compare it
                //to the integers that were written. We may not be able to reenumerate over the source.
                IEnumerable<int> integersToWrite = integers.Select(integer =>
                {
                    generatedIntegers.Add(integer);

                    return integer;
                });

                //Create the integer file creator
                IIntegerFileCreator fileCreator = new IntegerFileCreator(mockFileIO.Object);

                //Run the method to create the integer file
                fileCreator.CreateIntegerTextFile(integersToWrite, filePath);

                //If the integersExpected flag was set, verify that a non-zero number of integers were generated and written
                if(integersExpected)
                {
                    Assert.That(generatedIntegers.Count, Is.AtLeast(1));
                    Assert.That(writtenIntegers.Count, Is.AtLeast(1));
                }

                //Verify that the expected number of integers were written
                Assert.That(writtenIntegers.Count, Is.EqualTo(generatedIntegers.Count));

                //Verify that the generated and written integers are equivalent
                Assert.That(writtenIntegers, Is.EquivalentTo(generatedIntegers));
            }
        }
    }
}
