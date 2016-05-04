# LocalizerStringExtractor
Extract all the localizable strings from an AspNetCore project.

This tool uses the [Roslyn compiler](https://github.com/dotnet/roslyn) to retrieve possible localizable strings that are using
the ``IStringLocalizer`` (from the AspNetCode Localization project). To run the tool from command line, you can use ``StringLocalizerExtractor.exe [source (defaults to ./)] --output [the output path with filename]``.

The tool navigates the source directory recursively. Only the files that end with ``.cs`` or ``.cshtml`` (case insensitive) will be analyzed by the tool. At the same time, the tool omits the following directories:
* bin
* obj
* .git
* .vs
* node_modules

# Example
```c#
IStringLocalizer localizer = ...
var helloWorld = localizer["Hello world"];
var helloUser = localizer.GetString("Hello {0}!", "user");
```
In the above example, the tool will extract both keys (``Hello world`` and ``Hello {0}!``)

# Output
Right now, the tool only generates POT files, but I'm going to provide more formats.

#### POT
The POT file this tool generates adds the reference file where every key is defined.

A limitation occurs with the Razor files, because the files code is analyzed, instead of the razor syntax, the reference added to the POT file is not the reference on the razor file.

# Limitations
Because the project does not compiles the source, it is not possible (as long as I'm aware) to determine the type of a property or variable using Roslyn or even to ensure it is the expected type, any property or variable which name ends with **Localizer** (for example ``myLocalizer``) will be considered to be a ``IStringLocalizer`` and any "``GetString``" or "``[]``" call made to that object will be considered a valid localizable string, unless the key is another variable or numeric.

Because some variables may not be initialized or the value may be dynamic ``myString[myVar]`` is currently ignored.
