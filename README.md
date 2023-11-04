# OrderProjectsInSlnFile

MIT License

Copyright(c) 2023 Klara Koščević

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Introduction
Merge conflicts are a common challenge when collaborating on software projects. These conflicts occur when multiple contributors make changes to a project simultaneously, leading to conflicts in the code. 

This extension primary purpose is to prevent merge conflicts when new projects are added to a solution. By ensuring that projects are sorted alphabetically, this extension minimizes the chances of conflicts arising during collaborative development.

## Instructions for Using the Project-Sorting Extension

The project sorting extension allows you to quickly and easily sort projects within Visual Studio solution (.sln files). The extension can be run in two ways: automatically or manually. 
Here's how to do it:

### Automatically:
Automatic extension startup depends on several factors, including Visual Studio settings. 
The way it functions is as follows:
1.	Close solution in Visual Studio
2.	The extension will automatically sort projects when Visual Studio is closed if the "Automatically sort .sln file on closing solution" setting is enabled, which is default state. Sorting will not be performed if projects are already sorted in the .sln file, even when the setting is enabled. 
3.	If the projects are not sorted before the sorting process is done, a prompt window will open asking for confirmation. 

### Manually:
1.	Open solution in Visual Studio
2.	Go to the toolbar and select "Tools".
3.	In the drop-down menu find and click "Sort projects in .sln file". If projects have already been sorted or if only a single project exists, the sort button will not be available.
4.	After pressing the sorting button, a confirmation message box will open.
5.	Sorting changes the content of currently open solution file and Visual Studio will prompt on this change outside the environment, asking to reload the solution or ignore changes. You should press Reload to sync environment with actual content, although there will be no changes in the environment.

 ![ManuallyExtension](https://github.com/klkoscevic/SolutionFileSorter/blob/22d11ca7e7c3cb54bf52b9c289bfa9d4b39de650/ManuallyExtension.png)


### Settings
You can also configure the display of notifications shown when sorting is complete in the extension settings. By default, the window is always displayed after each sorting. 
To change it go to "Tools" -> Options" - "Sort projects in .sln file". In the settings, you have the option to change the display of message boxes and automatic sorting.

![SettingsExtension](https://github.com/klkoscevic/SolutionFileSorter/blob/6f5addf98f87a147323cf0e1fdcf96f36cf585fe/SettingsExtension.png)


## Instructions for using the console application to sort projects
This console application allows you to sort projects in .sln file on Windows and Linux operating systems. 
To run the application, open Command Prompt on a Windows operating system or a terminal on a Linux operating system.
Use the following syntax to run the application:
 
![SyntaxConsoleApp](https://github.com/klkoscevic/SolutionFileSorter/blob/22d11ca7e7c3cb54bf52b9c289bfa9d4b39de650/SyntaxConsole.png)

To view available options and user support, you can use the following argument:

![InfoConsoleApp](https://github.com/klkoscevic/SolutionFileSorter/blob/1a62c803fc9943fa078a26eb43e5a9e6fc57cbd7/InfoConsole.png)

### Warnings and notifications
The application will notify you of the following situations:
1. Successful sorting: If all arguments are correct and the sorting is successful, you will receive a notification that the solution file is sorted or has already been sorted and the application will exit with code 0.
2. No argument: If you call aplication with no argument or with /?, the application will exit with code 1.
3. Incorrect solution file path: If you enter wrong path or solution file does not exist, corresponding error will be shown and the application will exit with code 2.
4. Incorrect culture: If you enter an invalid culture, corresponding error will be show and the application will exit with code 3.
5. File corrupted: If solution file is corrupted, the error will be shown and application will exit with code 4.

You are now ready to use the console application to sort projects. Follow the instructions and enjoy sorting your projects quickly!
