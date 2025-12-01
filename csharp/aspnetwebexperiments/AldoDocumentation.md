# Aldo Documentation

For first time run, run `dotnet run` from the terminal
After first time run, can use the launchSettings.json to automatically get to swagger

The OrderCompletionApi follows the Hexagonal architecture pattern:
Presentation (controllers) -> UseCase -> Ports -> Adapters -> External dependencies (e.g. database)

Some design decisions:
- I use async await since it is best practice to improve performance, especially given lots of I/O operations like db
  calls and external api calls
  - To support this I decide not to directly change the interfaces but rather extend on it to respect Open Closed Principle
- Added a new state Notifying, such that if after notifying the database call to update the order to Completed fails, then
  the background worker 
- Changed the request method of the controller from POST to PATCH since we are updating the resource, not adding a new
  resource
- I use Result pattern so the method can return true or false with reasons of success/failure, and keep exceptions for truly
  exceptional cases
- I add Serilog since one of your values is to invest in logging
- I add a timeout so that the client is not waiting forever if notification or updating db takes a long time
- Added a Global exception handler so that if any unexpected errors occur, the whole stack is not emitted to the client and
  risk leaking sensitive information
- Added retries with backoff to the notification
- Changed Finished to Completed in the state for more readable code: we need to be consistent in naming to avoid confusion
  e.g. in this case between Completed and Finished
- I added Notifying state to add robustness to the system; we do not want to double notify in the case that the notification is 
  successful but the save to db fails (where the re-notification would've occurred by the background worker re-calling the service,
  or a user hitting the endpoint again with the same payload)

  Some things I think we could do differently:
- I notice the second migration file uses datetime as OrderDate type. From my previous experience, I had issues where
  multiple apps running in different timezones
  write to a db with a datetime information, and so if app B wants to compare a given record's datetime with the time
  in the app B's region, it cannot do it reliably  
  since the app doesn't know the timezone the record is made/whether the app has converted the date to some other
  timezone. I think it's better to change the type to TIMESTAMP, which in MySQL will translate all writes to UTC, and
  we are comparing to UTC consistently.
- Change the GetOrderById method to one that only loads relevant orders (i.e. orders submitted or notifying); otherwise
  currently we load all orders requested; if there are lots of orders it will be an issue. But I couldn't change this
  method since the already existing tests use it, and I am asked not to edit the test logic.
- Write more tests for the notification adapter
- Integration test (using Specflow perhaps)