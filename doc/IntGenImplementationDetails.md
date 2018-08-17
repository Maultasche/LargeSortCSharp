# IntGen Implementation Details

The IntGen program generates random integers to serve as input to the IntSort program. It's quite a bit simpler than IntSort.

## Shared Functionality

Functionality that is shared between IntGen and IntSort can be found in the LargeSort.Shared project. It consists of the FileIO module, which abstracts file I/O operations (allowing them to be mocked in unit test), the IntegerFileCreator module, which contains the functionality for storing integers in a text file, and the ConsoleInfo class, which is used in both programs to save and restore console settings.

## IntGen Functionality

The CommandLineOptions class defines the command line options for IntGen, and the Program class contains the main entry point, which integrates command line option handling with the integer generation functionality. The Program class also includes code for displaying a progress bar that displays the current progress of integer generation.

The RandomIntegerGenerator class contains a generator that generates random integers and encapsulates the generator as an IEnumerable, which generates the random integers as it is iterated over. That random integer generator IEnumerable is passed to the IntegerFileCreator class, which iterates over the random number generator, writing the results to a text file.