
- What to do if you need to have a Main method on your class but also have an XUnit test to test the class
https://stackoverflow.com/questions/42372342/xunit-test-project-with-only-1-main-method-program-has-more-than-one-entry-poi
 - If you use XUnit and want to debug the program from main, but have the problem "Program has more than one entry point defined", then do the following: https://andrewlock.net/fixing-the-error-program-has-more-than-one-entry-point-defined-for-console-apps-containing-xunit-tests/ i.e. add <GenerateProgramFile>false</GenerateProgramFile> to propertygroup of your csproj file.
 - See also softwareTesting/BestPractices folder for more info on how to write code such that it is testable
