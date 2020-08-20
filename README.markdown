SlimJim
=====

SlimJim genereates Visual Studio solution files that include related projects from disparate repositories.

To install SlimJim, run the following command in the [NuGet](http://nuget.org/) Package Manager Console

    PM> Install-Package SlimJim

SlimJim is meant to be invoked from the command line. It will search a given directory recursively for .csproj files, and include them in a new solution file if:

* The name matches one of the target project files.
* The project has a dependency on one or more of the target project files.
* One of the target project files has a dependency on it.
* Any project to be included in the solution has a reference to it. 

Invocation
----------

* `--target ProjectName`				Required; include one or more target projects in your solution. All dependencies will be included for them. Repeat switch for multiple targets.
* `--root C:\Projects\Root`				Optional; indicates the directory which contains the root project that you want to build a solution file for. Defaults to the working directory.
* `--narrow SubFolder`					Optional; limit the search process to certain folders in the root directory. Repeat for multiple folders.
* `--search C:\Additional\Search\Path`  Optional; specify one or more directories to search for projects that depend on or are depdended on by the target projects. Repeat for multiple directories.
* `--ignore Path`						Optional; indicates that directories with names matching the given regex pattern will be ignored. Repeat for multiple patterns.
* `--out SolnDir` 						Optional; specify a directory other than the working or target directory to write the generated solution file.
* `--version 2008`         				Optional; supported versions: 2008, 2010, 2012, 2013, 2015, 2017, 2019 (default).
* `--projecttype`						Optional; specify project file type(s) to search for. Supported: '.csproj', '.vbproj', '.vcxproj' (default: all)
* `--name SolutionName`                 Optional; alternate name for generated solution file. If omitted, defaults to target project name.
* `--minimal`   			 			Optional; indicates that all afferent assembly references are skipped. By default, they are omitted.
* `--all`								Optional; indicates that all efferent assembly references should be included in the solution. By default, they are omitted.
* `--ignore`							Optional; indicates that directories with names matching the given regex pattern will be ignored. Repeat for multiple patterns.
* `--open`								Optional; opens the solution in Visual Studio after generating it.
* `--convert`							Optional; modifies relevant csproj files to use Project References instead of Assembly References.
* `--unconvert`							Optional; reverses changes made by --convert flag.
* `--fixhintpaths`						Optional; adjust HintPath metadata in relevant csproj files to point to directory relative to generated Solution.
* `--restorehintpaths`					Optional; reverses changes made by --fixhintpaths flag.

Known issues 
------------
* No error handling
