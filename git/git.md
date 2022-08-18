# Git

- Best practice for good workflow: fork upstream to create origin/master and local master. Then create branches for each feature you are working on. https://www.youtube.com/watch?v=deEYHVpE1c8. Procedure is:
  
  git fork the real repository (i.e. the upstream), by going to the real git repo on github and press "fork"
  
  git clone your fork to have it locally

  git add remote upstream (link to real repository)

  git remote -v, here make sure you track two remotes i.e. origin and upstream, where origin is your own remote and upstream is the production code (i.e. real) remote

- How to update local and origin master of forked/cloned git repo correctly: https://www.youtube.com/watch?v=deEYHVpE1c8. Summary is:

  git remote -v, here make sure you track two remotes i.e. origin and upstream, where origin is your own remote and upstream is the production code (i.e. real) remote

  git fetch

  git merge upstream/master master

  git push origin master

- Always clean after checkout: git clean -d -n and git clean -d -f (-d is to remove untracked directories too)

- Always remember to git pull frequently from master to your origin/master and your local master, also to your feature branches. This is so that the feature branch you are working on are always the updated version.

-  git log --oneline --all --branches

-  To update only one file in your computer from any remote branch: git fetch, and then git log --oneline --all --branches, find the hash key of commit you want to update from, then git checkout (hashkey) (path to file)

-  Move git branch to HEAD: git branch -f branch_name HEAD

-  If you accidentally pushed something to origin, and would like to remove that: git push --force origin <hash key of commit previous to accident>:<branch name>. DO THIS ONLY FOR YOUR OWN PRIVATE BRANCH, IF YOU DO IT ON SHARED BRANCH, YOU RISK MESSING UP OTHERS' WORK!

-  If you committed something (locally), then want to remove that commit BUT keep the changes (i.e. like an uncommit), do git reset HEAD~

-  git submodule, if cloning new, do git submodule init, git submodule update

-  git filename too long when pull fix, add longpaths=true in config file of .git folder: https://www.javaprogramto.com/2020/04/git-filename-too-long.html#:~:text=Filename%20too%20long%20-%20Solution%201%20-%20Git,git%20config%20core.longpaths%20true%20%22%20in%20git%20bash

- git merge: if we are on a branch, and we want to merge master into ours, we say git merge master. This automatically does a recursive merge. This means there are no dominant branch. If we changed line 1 and they didn't, the merge result is our version. If we didn't change line 1, and they did, the merge result is their version. If both of us did, merge fails and needs resolution manually. https://stackoverflow.com/questions/42099431/what-is-the-dominant-branch-when-doing-a-git-merge. 
  
  This assumes both branches can be traced to a base/common commit.

- Git merge theirs (i.e. favor branchB): git merge -X theirs branchB

- Git rebase is basically: change the base of your feature branch to the latest commit of master, and then put all commit history of your feature branch to the master once it is ready to be put on master branch. Not recommended for collaboration master (who wants to see your messy git commit messages?)

- Change commit history: git rebase -i master

- checkout remote branch: git checkout --track origin/branchname

- untrack file from git: git rm -r --cached filename

- diff between two files: git diff mybranch master -- myfile.cs

- git diff file name only: git diff --name-only SHA1 SHA2

- git mergetool on command line: left top (LOCAL) is the status of tther branch you want to merge to, right top (REMOTE) is your stuff, bottom is merge result. Move cursor to bottom window using ctrl+w+direction, and to use other branch's version for the resolution, type :diffget LOCAL, to use yours use :diffget REMOTE, save by :wqa. Do this for every colored lines that represent conflicts.

- Overwrite one file in current branch with that of another branch: git checkout OtherBranchName \<filePath\>

- Rewrite commit history: If you want to edit some commits on a particular branch, do ```git rebase --strategy recursive --strategy-option ours -i --root```, using ours strategy so that if there are merge conflicts during rebase, the original version is retained (i.e. git doesn't attempt to merge . Make your changes in the commit you want, and if there are merge conflicts like files are modified/deleted, always add them to maintain the original version!
