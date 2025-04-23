## Must have in project
- Ardalis.Guard for: Guard/validation in domain
	- Your domain objects must be always valid, and validation of it when instantiating, should throw exception instead of return false when the validation fails; we can achieve this cleanly using the Guard library
	- Why throw exception? This is because the "client"/"user" of the domain object is the application code/layer and not the end user, so we expect the application layer to always give valid inputs (since we are the ones who built it, we are perfect human beings so we are always going to be accurate (or not!)!)
	- So a validation failure will then/should be considered as an exceptional event, a bug, and hence we throw instead of return false
- Directory.Build.props (standardize settings for all modules)
- Directory.Build.targets (standardize target builds for all modules)
- global.json
- editorconfig https://www.jetbrains.com/help/rider/Using_EditorConfig.html

## Must remember during coding
- Return Result class instead of bool/null/etc.
- Domain classes must not contain public functions
- async await
	- bolt in configureawait to specify intent with thread (do you wanna use calling thread for executing continuation or not)
	- bolt in cancellationToken
- When designing classes, and having to work with old projects with disabled nullable:
	- set \#nullable enable on top of your class
	- When you write your auto properties, if you intend your property to be optional/nullable (e.g. a person with an optional address property), mark it as nullable, to clarify intent to the compiler (see [[using null operator#^whFX0yE6o0DfZBzru00hB]] for example)
	- 
