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



## Instructions for Using the Project-Sorting Extension

The project sorting extension allows you to quickly and easily sort projects within Visual Studio solution (.sln files). The extension can be run in two ways: automatically or manually. 
Here's how to do it:

### Automatically:
Automatic extension startup depends on several factors, including Visual Studio settings. 
The way it functions is as follows:
1.	Open Visual Studio and open the solution
2.	The extension will automatically sort projects when Visual Studio is closed if the "Sort Project in .sln file after closing solution" setting is set to "True". By default, this setting is set to "False".
3.	Even if the "Sort Project in .sln file after closing solution" setting is set to "True", sorting will not be performed if the projects have already been sorted within the .sln file. 
4.	If the projects are not sorted before the sorting process is done, a prompt window will open asking for confirmation. 

### Manually:
1.	Open Visual Studio and open the solution.
2.	Go to the toolbar and select "Tools".
3.	In the drop-down menu find and click "Sort Project".
4.	If the projects have already been sorted sorted or if only a single project exists, the sort button will not be available.
5.	After pressing the sorting button, a window connected to the sorting process will open.
6.	You might notice changes in the .sln file after the sorting is complete. In that case, a window allowing you to reload the solution will open so you can see the changes made.

 ![image](https://github.com/klkoscevic/SolutionFileSorter/assets/102737720/f04341f7-883d-43a3-aeea-eb72a3e4d21a)


### Settings
You can also configure the display of notifications shown when sorting is complete in the extension settings. By default, the window is always displayed after each sorting. 
Here’s how to change it:
1.	Open Visual Studio and open the solution.
2.	Go to the toolbar and select "Tools".
3.	Find "Sort Project" and select "Settings".
4.	You can change the "Do not show message box" option according to whether you would like to see sorting notifications or not.
  
![image](https://github.com/klkoscevic/SolutionFileSorter/assets/102737720/53768de3-4c47-4491-b804-eba1a92c98da)


## Instructions for using the console application to sort projects
This console application allows you to sort projects within Visual Studio solutions on Windows and Linux operating systems. 
Here’s how to use it:
Launching the application
To run the application, open Command Prompt on a Windows operating system or a terminal on a Linux operating system.
Use the following syntax to run the application:
 
![image](https://github.com/klkoscevic/SolutionFileSorter/assets/102737720/da5faaa6-473a-436f-a26f-2fcee0aa054b)

To view available options and user support, you can use the following argument:

 ![image](https://github.com/klkoscevic/SolutionFileSorter/assets/102737720/7c55ffb8-af32-4915-919a-a2bde7d1688e)

### Warnings and notifications
The application will notify you of the following situations:
1.	Incorrect solution file path: If you enter the wrong path or the solution file does not exist, the application will inform you about it and explain the reason why the sorting was not performed.
2.	Incorrect culture: If you enter the wrong culture, the application will warn you the sorting will not be completed.
3.	Successful sorting: After all entries are correct and the sorting is successful, you will receive a notification that the solution file is sorted or has already previously been sorted.
You are now ready to use the console application to sort projects. Follow the instructions and enjoy sorting your projects quickly!
