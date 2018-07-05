using System;
using System.IO;

using Moq;
using NUnit.Framework;

using LargeSort.Shared;

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
        /// Contains tests for the CreateFileStreamReader method
        /// </summary>
        [TestFixture]
        public class CreateFileStreamReaderTests : FileIOTestBase
        {
            /// <summary>
            /// Tests creating a read stream in an existing file in the same directory
            /// </summary>
            [Test]
            public void TestReadFileInSameDirectory()
            {
                const string TestFile = "testFile.txt";
                const string FileContents = "This is a test file.";

                //Create the test file
                CreateTestFile(TestFile, FileContents);

                //Run the test
                TestWithFile(TestFile, FileContents, fileExists: true);

                //Delete the test file
                File.Delete(TestFile);
            }

            /// <summary>
            /// Tests creating a read stream for a file in a subdirectory
            /// </summary>
            [Test]
            public void TestReadFileInSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";
                const string FileContents = "This is a test file.";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Create the test file
                CreateTestFile(TestFile, FileContents);

                //Run the test
                TestWithFile(TestFile, FileContents, fileExists: true);

                //Delete the test file
                File.Delete(TestFile);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests creating a read stream for a non-existent file
            /// </summary>
            [Test]
            public void TestReadNonExistentFile()
            {
                const string NonExistentFile = "nonExistentFile.txt";

                //Run the test
                TestWithFile(NonExistentFile, fileExists: false);
            }

            /// <summary>
            /// Creates a test file with the given contents
            /// </summary>
            /// <param name="testFile">A path to the file to be created</param>
            /// <param name="contents">The contents to be written to the file</param>
            private void CreateTestFile(string testFile, string contents)
            {
                IFileIO fileIO = new FileIO();
                
                //Create the file
                using (Stream fileStream = fileIO.CreateFile(testFile))
                {
                    //Create a stream write from the file stream
                    StreamWriter textWriter = new StreamWriter(fileStream);

                    //Write the contents to the file
                    textWriter.Write(contents);

                    //Close the file stream
                    textWriter.Close();
                }
            }

            /// <summary>
            /// Tests the CreateFileStreamReader method
            /// </summary>
            /// <param name="testFile">The file to be read</param>
            private void TestWithFile(string testFile, string expectedFileContents = null, bool fileExists = false)
            {
                IFileIO fileIO = new FileIO();

                bool fileNotFoundExceptionThrown = false;

                try
                {
                    //Create a stream reader for the test file
                    using (StreamReader fileStreamReader = fileIO.CreateFileStreamReader(testFile))
                    {
                        //Read the contents of the file
                        string actualContents = fileStreamReader.ReadToEnd();

                        //Verify that we got the correct data
                        Assert.That(actualContents, Is.EqualTo(expectedFileContents));

                        fileStreamReader.Close();
                    }
                }
                catch(FileNotFoundException)
                {
                    fileNotFoundExceptionThrown = true;

                    //If the file exists, but it was not found, rethrown the exception to fail the test
                    if(fileExists)
                    {
                        throw;
                    }
                }

                //If the file does not exist, assert that a FileNotFoundException was thrown
                if(!fileExists)
                {
                    Assert.That(fileNotFoundExceptionThrown, Is.True);
                }
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
                Stream fileStream = fileIO.CreateFile(TestFile);
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
                Stream fileStream = fileIO.CreateFile(testFile);

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
        /// Contains tests for the FileExists method
        /// </summary>
        [TestFixture]
        public class FileExistsTests : FileIOTestBase
        {
            /// <summary>
            /// Tests if a file exists when that file actually does exist
            /// </summary>
            [Test]
            public void TestFileExistsInSameDirectory()
            {
                const string TestFile = "testFile.txt";
                const string FileContents = "This is a test file.";

                IFileIO fileIO = new FileIO();

                //Create the test file
                CreateTestFile(TestFile, FileContents);

                //Verify that the file exists
                Assert.That(fileIO.FileExists(TestFile), Is.True);

                //Delete the test file
                File.Delete(TestFile);
            }

            /// <summary>
            /// Tests if a file exists when that file actually does exist in a subdirectory
            /// </summary>
            [Test]
            public void TestFileExistsInSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";
                const string FileContents = "This is a test file.";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Create the test file
                IFileIO fileIO = new FileIO();

                //Create the test file
                CreateTestFile(TestFile, FileContents);

                //Verify that the file exists
                Assert.That(fileIO.FileExists(TestFile), Is.True);

                //Delete the test file
                File.Delete(TestFile);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests creating a read stream for a non-existent file
            /// </summary>
            [Test]
            public void TestFileExistsNonExistentFile()
            {
                const string NonExistentFile = "nonExistentFile.txt";

                IFileIO fileIO = new FileIO();

                //Verify that the non-existent file does not exists
                Assert.That(fileIO.FileExists(NonExistentFile), Is.False);
            }

            /// <summary>
            /// Creates a test file with the given contents
            /// </summary>
            /// <param name="testFile">A path to the file to be created</param>
            /// <param name="contents">The contents to be written to the file</param>
            private void CreateTestFile(string testFile, string contents)
            {
                IFileIO fileIO = new FileIO();

                //Create the file
                using (Stream fileStream = fileIO.CreateFile(testFile))
                {
                    //Create a stream write from the file stream
                    StreamWriter textWriter = new StreamWriter(fileStream);

                    //Write the contents to the file
                    textWriter.WriteLine(contents);

                    //Close the file stream
                    textWriter.Close();
                }
            }
        }

        /// <summary>
        /// Contains tests for the RenameFile method
        /// </summary>
        [TestFixture]
        public class RenameFileTests : FileIOTestBase
        {
            /// <summary>
            /// Tests creating renaming a file in the same directory
            /// </summary>
            [Test]
            public void TestRenameFileInSameDirectory()
            {
                const string TestFile = "testFile.txt";
                const string NewFileName = "renamedFile.txt";

                //Run the tests
                TestWithFile(TestFile, NewFileName);
            }

            /// <summary>
            /// Tests renaming a file in a subdirectory
            /// </summary>
            [Test]
            public void TestRenameFileInSubDirectory()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";
                const string NewFileName = "renamedFile.txt";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Run the tests
                TestWithFile(TestFile, NewFileName);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests renaming a file when another file that has the new name already exists
            /// </summary>
            [Test]
            public void TestCreateFileAlreadyExists()
            {
                const string SubDirectory = "sub";
                const string TestFile = "sub/testFile.txt";
                const string NewFileName = "renamedFile.txt";
                const string NewFilePath = "sub/renamedFile.txt";

                //Create the subdirectory
                Directory.CreateDirectory(SubDirectory);

                //Create a file with the new file name in the subdirectory
                Stream fileStream = File.Create(NewFilePath);
                fileStream.Close();

                //Verify that the file was created
                Assert.That(File.Exists(NewFilePath), Is.True);

                //Run the tests, and assert that it throws an I/O exception
                Assert.That(() => TestWithFile(TestFile, NewFileName), Throws.Exception.TypeOf<IOException>());

                //Delete the existing file
                File.Delete(TestFile);

                //Delete the subdirectory
                Directory.Delete(SubDirectory);

                //Verify that the subdirectory was deleted
                Assert.That(Directory.Exists(SubDirectory), Is.False);
            }

            /// <summary>
            /// Tests the RenameFile method
            /// </summary>
            /// <param name="testFile">The file to be created and then renamed</param>
            /// <param name="newFileName">The new name to be given to the file</param>
            private void TestWithFile(string testFile, string newFileName)
            {
                //Calculate the renamed file path
                string testFileDirectory = Path.GetDirectoryName(testFile);
                string renamedFilePath = Path.Combine(testFileDirectory, newFileName);

                try
                {
                    IFileIO fileIO = new FileIO();

                    //Create the test file
                    Stream fileStream = File.Create(testFile);

                    //Close the file stream
                    fileStream.Close();

                    //Verify that the file exists
                    Assert.That(File.Exists(testFile), Is.True);

                    //Rename the test file
                    fileIO.RenameFile(testFile, newFileName);

                    //Verify that the file with the new file name exists and that the old file does not
                    Assert.That(File.Exists(testFile), Is.False);
                    Assert.That(File.Exists(renamedFilePath), Is.True);
                }
                finally
                {
                    //Delete the files
                    File.Delete(testFile);
                    File.Delete(renamedFilePath);
                }

                //Verify that the files have been deleted
                Assert.That(File.Exists(testFile), Is.False);
                Assert.That(File.Exists(renamedFilePath), Is.False);
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
