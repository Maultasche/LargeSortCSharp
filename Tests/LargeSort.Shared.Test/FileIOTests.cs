using System;
using System.IO;

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
    }
}
