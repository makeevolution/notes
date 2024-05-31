https://github.com/thangchung/clean-code-dotnet
https://github.com/zedr/clean-code-python

| Group                                        | Role    | Permissions (unused yet) | Frontend UI tabs that should be seen     | Test suites and executions that can be seen                                                                                                           |
|----------------------------------------------|---------|--------------------------|------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| VFI                                          | Admin   |                          | Define, execute and admin tab            | All                                                                                                                                                      |
| VFAB                                         | VFAdmin |                          | Define, execute tabs                     | All                                                                                                                                                      |
| VFOD (i.e. a group created for Non VF users) | VFUser  |                          | Execute test suite and view all jobs tab | Only suites whose group membership is the same as the user, and only executions that are part of a test suite, whose group membership is the same as the user |
