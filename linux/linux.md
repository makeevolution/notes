https://bizanosa.com/debian-10-initial-server-setup-vps-vultr/ Linux

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
  
- Use ```-a``` for AND, ```-o``` for OR, and ```!``` for NOT, inside ```[ ]``` expressions.

- To group multiple conditionals, use ```{ }```. Example:
  ```sh
  if { [ $X -eq $Y ] || [ $Y -eq $Z ] || [ $X -eq $Z ]; }; then
    echo "ISOSCELES"
  fi
  ```
  This example also shows that outside ```[ ]```, use AND and OR syntax using the normal way. More info: https://unix.stackexchange.com/questions/28781/what-is-the-syntax-of-a-complex-condition-in-shell

- Looping:
  ```sh
  for i in {1..50}; do
    echo $i;
  done
  ```
- ```cut``` to filter out columns from lines of text, or characters from a line of text.
  Examples:
  ```sh
  Hello
  World
  how are you
  
  cut -c 3-4 
  
  l
  r
  w
  ```
  
  ```sh
  Hello
  World
  how are you
  
  cut -c 2,7 
  
  e
  o
  oe
  ```
  
  ```sh
  New York is a state in the Northeastern and Mid-Atlantic regions of the United States. 
  
  cut -d " " -f 1,3
  
  New York is
  ```
  
- Print middle of text output
  ```sh
  From fairest creatures we desire increase,
  That thereby beauty's rose might never die,
  But as the riper should by time decease,
  His tender heir might bear his memory:
  But thou contracted to thine own bright eyes,
  Feed'st thy light's flame with self-substantial fuel,
  Making a famine where abundance lies,
  Thy self thy foe, to thy sweet self too cruel:
  Thou that art now the world's fresh ornament,
  And only herald to the gaudy spring,
  Within thine own bud buriest thy content,
  And tender churl mak'st waste in niggarding:
  Pity the world, or else this glutton be,
  To eat the world's due, by the grave and thee.
  When forty winters shall besiege thy brow,
  And dig deep trenches in thy beauty's field,
  Thy youth's proud livery so gazed on now,
  Will be a tattered weed of small worth held:
  Then being asked, where all thy beauty lies,
  Where all the treasure of thy lusty days;
  To say within thine own deep sunken eyes,
  Were an all-eating shame, and thriftless praise.
  How much more praise deserved thy beauty's use,
  If thou couldst answer 'This fair child of mine
  Shall sum my count, and make my old excuse'

  To print the 12th up to 22th lines use tail and head, with + in tail to output everything except the 12 lines from top (without +, it outputs the 12 lines  from the bottom):
  tail -n +12 | head -n 11
  ```

------------

list disk usage `df -h` or `du -h`
show all files and folder sizes in a folder `du -sh * | sort -n`

------------

To match regex in a non greedy way:
doing `(.*?)\=` on `i am a message test=123` captures `i am a message test =`

------------

## How to setup new server to add new user and allow ssh for that user from another server without password: 
https://bizanosa.com/debian-10-initial-server-setup-vps-vultr/
1. `adduser newuser`
2. `usermod -aG sudo newuser`
3. Re-login into server as `newuser`
4. `sudo mkdir ~/.ssh`
5. `sudo nano ~/.ssh/authorized_keys`, and copy public key of server you want to ssh from into this file
6. `sudo chmod 700 -R ~/.ssh`
7. `sudo chmod 600 ~/.ssh/authorized_keys`
8. `sudo chown -R joe:joe /home/joe`
9. `sudo vi /etc/ssh/sshd_config`, allow publickeyauth and authkey file
10. (optional) disable root by setting `PermitRootLogin no` on the above file, and `sudo service ssh restart`
11. (optional) disable passwordauthentication for the user by setting `PasswordAuthentication no`, and restart ssh service
