# Linux

- Change ```python3``` to ```python``` if ```python``` already exists:
  - Find the location of your ```python3``` and ```python``` (if it exists) using ```which python3``` and ```which python```. Say they're in ```usr/bin/python3``` and ```/usr/bin/python```
  - Then move the current symlink of python command to another command (if it exists) ```sudo mv /usr/bin/python /usr/bin/python2```
  - Then create a new symlink to ```python3``` i.e. ```sudo ln -s /usr/bin/python3.6 /usr/bin/python```
  - May break some things on already running servers, only use when making docker containers
- Change ```python3``` to ```python``` if ```python``` doesn't exist:
  - Create ```alias python=python3``` in ```.bashrc``` and ```source``` it