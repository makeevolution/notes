# Vim

- Change all occurences of foo with bar in whole file with(out) confirmation: ```:%s/foo/bar/gc``` (```:%s/foo/bar/g```)
- Search case insensitive: ```/word\c```
- Search for complete word: ```/\<word/>```
- Forward search: ```n```, backward search: ```N```
- If you want to replace a particular subword of a word e.g. 123 in 123456, select the 123 using ctrl + V, press c, type the replacement. Then you can go to normal mode, press n to go to the next occurrence of this word, and press . to replace
