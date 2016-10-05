To run the application for Chapter 6: 

1. IIS must be installed and running.

2. If you are using the non-IDE version, copy the BuyDirect directory (under Project) and all its subdirectories
and contents to the InetPub\WWWRoot directory. Then, start your
Web browser and type "http://localhost/BuyDirect" as the URL.

3. If you are using the Visual Studio.NET 2002 or Visual Studio.NET 2003 versions,
double-click the Setup.exe file to create a virtual root. The application will be installed automatically.
There is no .sln file for the projects. Therefore, if you want to work with the code in Visual
Studio.NET, double click the .vbproj file and create a new solution. If installation fails, create
a new solution and a project for a Web application, and then copy all files to the project.

4. The application requires either a Microsoft Access database or a Microsoft SQL Server.
If you plan to use an Access database (easier), copy the BuyDirect.mdb file to a directory
outside your application directory. 
If you plan to use Microsoft SQL Server, run the MsSQLScript.sql script file to create
all the required database and tables and populate the tables.

5. Edit the Web.config file. By default, the Web.config file contains the following:


  <appSettings>
    <add key="db" value="Access"/>
    <add key="connectionString" 
      value="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\123data\MyBooks\NETProject\Ch06\db\BuyDirect.mdb"/>
    <add key="pageWidth" value="678"/>
    <add key="menuWidth" value="145"/>
  </appSettings>


If you plan to use an Access database, keep the value "Access" for the "db" key,
and replace the Data Source value of the "connectionString" key to the path
of your BuyDirect.mdb file.
If you plan to use Ms SQL Server, change the "db" key's value to "MsSQL",
and then change the value of the connectionString key to reflect the connection string of the database
See the book for information on connection strings.


