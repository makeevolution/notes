# Git

- Multiple .gitignore files for different branches. There's a trick to have multiple gitignores for different branches: https://gist.github.com/wizioo/c89847c7894ede628071 or https://gist.github.com/Tealk/13ec8effe72f45f45165143cc64a3048. Summary:

In general, the solution is to make a general `.gitignore` file and add `.gitignore.branch_name` files for the branches I want to add specific file exclusion.
I'll use post-checkout hook to copy those `.gitignore.branch_name` in place` of `.git/info/exclude` each time I go to the branch with `git checkout branch_name`.

0. On top an existing `.gitignore` which all branches use, create new `.gitignore` files for each branch you want the extra ignores to be and name it like this : `.gitignore.branch_name`
1. Then, add the extra files you want the branch to ignore in the `.gitignore.branch_name`
2. In your git repo, go to `.git/hooks/`
3. Edit or create `post-checkout` file and copy the content found in this gist.
4. Don't forget to make it executable with `chmod 755 post-checkout`
5. Just go to the branch you want and type `git status --ignored`. If you make the file in the branch you want it to work on, first checkout to another branch and checkout back to the branch to make the ignore take effect.

This is useful if you have a development branch, which has files that you dont want production to track, when development is merged to production. It uses post-checkout hook functionality of Git. The disadvantage is that all branches will include an extra .gitignore.production file, maybe undesired non-cleanliness.

- Untracking existing file: After first cleaning cache using ```git rm -r --cached <FILE>```, do ```git status``` and sometimes in the changes to be commited part, the files are marked as fully to be deleted instead! This is because the file has been already commited before in history. Thus need to then do ```git reset HEAD <FILE>``` to fully untrack the file WITHOUT deleting the file from the filesystem. Check if the file is truly untracked using ```git status --untracked```, although it won't show if you didn't make any changes to the file; use ```git clean -n``` to check if the file is untracked. To fully ignore the file, include the file in ```.gitignore```.

Ignoring makes the file truly invisible to git and unavailable to be tracked by git (it will not even show up in ```git status``` although can be seen using ```git status --ignored```), while untracked status makes it ready to be added to be tracked by git. Be careful in interpreting ```git status```, it only shows status of changed files, so if you did not change anything in the file, the file won't show there regardless if is tracked, untracked or ignored! So don't rely on ```git status``` to find out the status of a file.

To list all files being currently tracked by git (i.e. in the staging area), run ```git ls-files -s```

To list all untracked (not in staging area) and ignored (not considered by git at all) files, run ```git ls-files --others```

To list only ignored files, run ```git ls-files --others --exclude-standard```

To list only untracked files (but not ignored), run ```git clean -n```

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
