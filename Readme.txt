The projects for Chapter 1,2,3,4,5 come in three versions:
1. Visual Studio.NET 2002, located in the VSNET2002 directory under each chapter.
2. Visual Studio.NET 2003, located in the VSNET2003 directory under each chapter.
3. non-IDE that has to be compiled using the vbc.exe program, located in the Project directory under each chapter.


Visual Studio.NET 2002 and Visual Studio.NET 2003 versions 
For the applications in Chapters 1,2,3,4,5
To run Visual Studio.NET 2002 and Visual Studio.NET 2003 versions, double-click the solution (.sln) file.
For the application in Chapter 6, read the CreateVSNetProject.doc under Chapter6 directory.

Non-IDE versions
A build.bat file is provided to compile each project. For the build.bat file to run successfully,
your computer's PATH environment variable must include the path to the vbc.exe program. In most
cases, the PATH environment has been updated when you install the .NET Framework.
To set the PATH environment variable, do the following:
1. Find the location of the vbc.exe program.
2. Open the Control Panel and double-click the System applet.
3. Click the Advanced tab and then click the Environment Variables button. 
4. If you can see the Path environment variable, add the path to the vbc.exe program 
to the end of its current value.
5. If you cannot find the Path environment variable, add it and assign the path to the vbc.exe program
as its value.

Note that the new value of the Path environment variable only affects the command prompt that
is opened after the value is changed. 

If you have installed Visual Studio.NET 2002 or Visual Studio.NET 2003, and you want to compile a 
project using the build.bat file, you can also use the .NET command prompt. The advantage is you don't 
need to change the PATH environment variable. To invoke the .NET command prompt,
click Start-->Visual Studio.NET-->Visual Studio.NET Tools-->.NET Command Prompt. 

For updates, visit www.brainysoftware.com
