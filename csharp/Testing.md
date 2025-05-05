### General
- What to do if you need to have a Main method on your class but also have an XUnit test to test the class
https://stackoverflow.com/questions/42372342/xunit-test-project-with-only-1-main-method-program-has-more-than-one-entry-poi
 - If you use XUnit and want to debug the program from main, but have the problem "Program has more than one entry point defined", then do the following: https://andrewlock.net/fixing-the-error-program-has-more-than-one-entry-point-defined-for-console-apps-containing-xunit-tests/ i.e. add <GenerateProgramFile>false</GenerateProgramFile> to propertygroup of your csproj file.
 - See also softwareTesting/BestPractices folder for more info on how to write code such that it is testable

### How to expose internal members in src project in your test project
![[Drawing 2025-05-04 19.10.38.excalidraw.png]]
But, if your project is a submodule of another project with Directory.Builds.Props, doing this may generate duplicate assembly info errors like ![[Pasted image 20250504200058.png]]
This is because, your new AssemblyAttribute tag will trigger the compiler to create an assemblyinfo file with its own assumptions on targetframework etc., which may conflict with the ones from Directory.Builds.Props

Solution is to AssemblyInfo file yourself and set autogenerate assemblyinfo to false, and set your internalsvisibleto in that file instead![[Pasted image 20250504195913.png]]
