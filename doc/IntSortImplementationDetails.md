#IntSort Implementation Details

The IntSort program reads in a text file containing integers and sorts them without needing to store all the integers in memory. It does this by breaking the integers into chunks, sorting those chunks, writing them to intermediate files, and then merging the intermediate files. It works a lot like a file-based mergesort.

##Shared Functionality

Functionality that is shared between IntGen and IntSort can be found in the LargeSort.Shared project. It consists of the FileIO module, which abstracts file I/O operations (allowing them to be mocked in unit test) and the IntegerFileCreator module, which contains the functionality for storing integers in a text file.

##IntSort Functionality

The CommandLineOptions class defines the command line options for IntSort, and the Program class contains the main entry point, which integrates command line option handling with the integer sorting functionality. The Program class also includes code for displaying a progress bar that displays the current progress of integer sorting.

The ChunkStreamCreator class is used to create a stream of integer chunks that is read from the file. A generator encapsulated in an IEnumerable is used for creating the chunk stream, so as the IEnumerable is iterated over, it reads the input file and pulls in the next chunk of integers. We add on a transformation method that sorts each chunk as it is read from the file, to create a sorted IEnumerable. That sorted IEnumerable is then fed to the ChunkFileCreator class, where each integer chunk is read from the chunk stream and then written to a chunk file. 

That gives us a number of chunk files, where each chunk file contains a single chunk of sorted integers. Now we just need to merge them. The strategy is to merge a maximum number of chunks at a time, taking the next integer from each chunk, and then writing the smallest integer to the output file. Each merge stage is referred to as a generation, and if there are a lot of chunk files and the number of files being merged at one time is small, it may take several generations to merge them all into a single output file.

We merge the chunk files using ChunkFileMerger, which takes care of the logic of merging all the chunk files into a single sorted output file. ChunkFileMerger doesn't contain all the logic in one place, but has uses IntegerFileMerger as a dependency. IntegerFileMerger merges a single generation of integers files into a variable number of output files and ChunkFileMerger will keep calling IntegerFileMerger to merge the output of the previous generation until only one merged file remains.

IntegerFileMerger receives a collection of files to merge and merges one or more groups of files. It uses an IntegerStreamMerger to do the work of merging each group of files. IntegerStreamMerger uses an IntegerStreamReader to read from the input files, finds the smallest integer from the group of input files, and writes the smallest integers to the output files using an IntegerStreamWriter. It uses the IntegerStreamEnumerator for each input file to keep track of where it is in the input files, and to move to the next integer once it has been written to the output file.

At the end of all this, we are left with a single output file containing the sorted integers, and we can do this by only loading a subset of the integers into memory at any given time.