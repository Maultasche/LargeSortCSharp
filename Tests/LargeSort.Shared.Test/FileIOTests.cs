using System;
using System.IO;

using Moq;
using NUnit.Framework;

using LargeSort.Shared.Implementations;
using LargeSort.Shared.Interfaces;

namespace LargeSort.Shared.Test
{
    /// <summary>
    /// Contains the tests for the FileIO class
    /// </summary>
    public class FileIOTests
    {
        /// <summary>
        /// Common functionality for FileIO tests
        /// </summary>
        public class FileIOTestBase
        {

        }

        /// <summary>
        /// Contains tests for the GetDirectoryFromFilePath method
        /// </summary>
        [TestFixture]
        public class GetDirectoryFromFilePathTests : FileIOTestBase
        {
            /// <summary>
            /// Tests retrieving the directory with just a file name
            /// </summary>
            [Test]
            public void TestWithFileName()
            {
                const string TestFilePath = "somefile.txt";
                const string ExpectedDirectory = "";

                TestWithFilePath(TestFilePath, ExpectedDirectory);
            }

            /// <summary>
            /// Tests retrieving the directory with a file in a subdirectory
            /// </summary>
            [Test]
            public void TestWithSubDirectory()
            {
                const string TestFilePath = "files/somefile.txt";
                const string ExpectedDirectory = "files";

                TestWithFilePath(TestFilePath, ExpectedDirectory);
            }

            /// <summary>
            /// Tests getting the directory of a file nested in multiple levels of subdirectories
            /// </summary>
            [Test]
            public void TestWithNestedDirectories()
            {
                const string TestFilePath = "bananas/files/somefile.txt";
                const string ExpectedDirectory = "bananas\\files";

                TestWithFilePath(TestFilePath, ExpectedDirectory);
            }

            /// <summary>
            /// Tests getting the directory of a file that is in a sibling directory
            /// </summary>
            [Test]
            public void TestWithRelativeDirectory()
            {
                const string TestFilePath = "../apples/somefile.txt";
                const string ExpectedDirectory = "..\\apples";

                TestWithFilePath(TestFilePath, ExpectedDirectory);
            }

            /// <summary>
            /// Tests getting the directory of a file that is in a parent directory
            /// </summary>
            [Test]
            public void TestWithParentDirectory()
            {
                const string TestFilePath = "../";
                const string ExpectedDirectory = "..";

                TestWithFilePath(TestFilePath, ExpectedDirectory);
            }

            /// <summary>
            /// Tests getting the directory of an empty file path
            /// </summary>
            [Test]
            public void TestWithEmptyFilePath()
            {
                const string TestFilePath = "";

                //This should throw an ArgumentException
                IFileIO fileIO = new FileIO();

                Assert.That(() => fileIO.GetDirectoryFromFilePath(TestFilePath), Throws.ArgumentException);
            }

            /// <summary>
            /// Tests the GetDirectoryFromFilePath method
            /// </summary>
            /// <param name="testFilePath">The file path to use for testing</param>
            /// <param name="expectedDirectory">The expected result</param>
            private void TestWithFilePath(string testFilePath, string expectedDirectory)
            {
                IFileIO fileIO = new FileIO();

                string directory = fileIO.GetDirectoryFromFilePath(testFilePath);

                Assert.That(directory, Is.EqualTo(expectedDirectory));
            }
        }

        /// <summary>
        /// Contains tests for the CreateDirectory method
        /// </summary>
        [TestFixture]
        public class CreateDirectoryTests : FileIOTestBase
        {
            /// <summary>
            /// Tests creating a subdirectory
            /// </summary>
            [Test]
            public void TestCreateSubDirectory()
            {
                const string TestDirectory = "files";

                TestWithDirectory(TestDirectory);
            }

            /// <summary>
            /// Tests creating a subdirectory in an existing subdirectory
            /// </summary>
            [Test]
            public void TestCreateSubDirectoryOfExistingSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestDirectory = "sub/tree";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Run the tests
                TestWithDirectory(TestDirectory);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);
            }

            /// <summary>
            /// Tests creating a subdirectory in a non-existent subdirectory
            /// </summary>
            [Test]
            public void TestCreateSubDirectoryOfNonExistentSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestDirectory = "sub/tree";

                TestWithDirectory(TestDirectory);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);
            }

            /// <summary>
            /// Tests the CreateDirectory method
            /// </summary>
            /// <param name="testDirectory">The directory to be created</param>
            private void TestWithDirectory(string testDirectory)
            {
                IFileIO fileIO = new FileIO();

                //Create the directory
                fileIO.CreateDirectory(testDirectory);

                //Verify that the directory exists
                Assert.That(Directory.Exists(testDirectory), Is.True);

                //Delete the directory
                Directory.Delete(testDirectory, true);
            }
        }

        /// <summary>
        /// Contains tests for the CreateFile method
        /// </summary>
        [TestFixture]
        public class CreateFileTests : FileIOTestBase
        {
            /// <summary>
            /// Tests creating a file in the same directory
            /// </summary>
            [Test]
            public void TestCreateFileInSameDirectory()
            {                
                const string TestFile = "testFile.txt";

                //Run the tests
                TestWithFile(TestFile);
            }

            /// <summary>
            /// Tests creating a file in a subdirectory
            /// </summary>
            [Test]
            public void TestCreateFileInSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Run the tests
                TestWithFile(TestFile);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests creating a file when that file already exists
            /// </summary>
            [Test]
            public void TestCreateFileAlreadyExists()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Create the file in the subdirectory
                IFileIO fileIO = new FileIO();
                FileStream fileStream = fileIO.CreateFile(TestFile);
                fileStream.Close();

                //Verify that the file was created
                Assert.That(File.Exists(TestFile), Is.True);

                //Run the tests
                TestWithFile(TestFile);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests the CreateFile method
            /// </summary>
            /// <param name="testFile">The file to be created</param>
            private void TestWithFile(string testFile)
            {
                IFileIO fileIO = new FileIO();

                //Create the file
                FileStream fileStream = fileIO.CreateFile(testFile);

                //Close the file stream
                fileStream.Close();

                //Verify that the file exists
                Assert.That(File.Exists(testFile), Is.True);

                //Delete the file
                File.Delete(testFile);

                //Verify that the file has been deleted
                Assert.That(File.Exists(testFile), Is.False);
            }
        }

        /// <summary>
        /// Contains tests for the WriteIntegerToStream method
        /// </summary>
        [TestFixture]
        public class WriteIntegerToStreamTests : FileIOTestBase
        {
            /// <summary>
            /// Tests writing an integer to a stream
            /// </summary>
            [Test]
            public void TestWriteIntegerToStream()
            {
                const int TestInteger = 24;

                //Create a mock stream writer
                MemoryStream memoryStream = new MemoryStream();
                Mock<StreamWriter> mockStreamWriter = new Mock<StreamWriter>(memoryStream);

                //Write an integer to the stream writer
                IFileIO fileIO = new FileIO();

                fileIO.WriteIntegerToStream(mockStreamWriter.Object, TestInteger);

                //Verify that the correct line was written
                mockStreamWriter.Verify(mock => mock.WriteLine(It.Is<int>(integer => integer == TestInteger)),
                    Times.Once);
            }
        }
    }
}
