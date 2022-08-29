# Linux

- Change ```python3``` to ```python``` if ```python``` already exists:
  - Find the location of your ```python3``` and ```python``` (if it exists) using ```which python3``` and ```which python```. Say they're in ```usr/bin/python3``` and ```/usr/bin/python```
  - Then move the current symlink of python command to another command (if it exists) ```sudo mv /usr/bin/python /usr/bin/python2```
  - Then create a new symlink to ```python3``` i.e. ```sudo ln -s /usr/bin/python3.6 /usr/bin/python```
  - May break some things on already running servers, only use when making docker containers
- Change ```python3``` to ```python``` if ```python``` doesn't exist:
  - Create ```alias python=python3``` in ```.bashrc``` and ```source``` it

- = and == are for string comparisons
  -eq is for numeric comparisons
  -eq is in the same family as -lt, -le, -gt, -ge, and -ne

  == is specific to bash (not present in sh (Bourne shell), ...). Using POSIX = is preferred for compatibility. In bash the two are equivalent, and in sh = is the only one that will work.
```sh
  $ a=foo
  $ [ "$a" = foo ]; echo "$?"       # POSIX sh
  0
  $ [ "$a" == foo ]; echo "$?"      # bash-specific
  0
  $ [ "$a" -eq foo ]; echo "$?"     # wrong
  -bash: [: foo: integer expression expected
  2
  ```
  better practice for sh for compatibility of your docker .sh entry point across different systems
