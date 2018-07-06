# LargeSortCSharp
This is the the same problem from the NodeLargeSort project, but solved using C#

It is essentially a version of merge sort that uses files and has variable-length merge chunks, where chunks can be 
merged together in any number instead of two at a time like the typical merge sort.

##Generating Random Integers

dotnet run --project IntGen --count 100 --lowerBound -100 --upperBound 100 data\randomIntegers.txt

##Running Tests

Tests in test projects can be run per project: dotnet test Tests/[Project Directory]

There is also the runTests.sh bash script, which can be run in Bash or in Cmder for Windows using "bash runTests.sh".