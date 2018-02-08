# ATM_Store

Download / install  
git: https://git-scm.com/downloads

Create Your Github account

Navigate to https://github.com/tsi4402bnl/ATM_Store

Fork the project

Clone The project: Open Git bash
Navigate to Folder of Your choice and run command:
```
git clone https://github.com/YourUserName/ATM_Store.git
```
* It will download the project
* replace YourUserName with Your user name 

in git bash navigate to newly created folder:
```
cd ATM_Store
```
*Git bash will show something like:
```
"*******@************ MINGW64 ~/*******************************************/ATM_Store (master)"
```

Create a new branch
```
git checkout master
git checkout -b MK-001
```
* be sure to be on master branch when creating a new one (1st command)
* Replace MK-001 with Any unique branch name (I used MK - first letter of my name, surname and 001 - number)
* Git bash will show something like:
```
Switched to a new branch 'MK-001'
```


###### Now You are ready to make all the changes to project

if accidently You made changes in master branch, do something like this
```
git stash
git checkout -b MK-001
git stash pop
```

When done run command:
```
git status
```
Choose files which need to be submitted to repository
```
git add README.md
```
*README.md - is the file name to be subitted
*You can also open Git GUI app on folder to see files which have been changed

When all files are added run
```
git commit -m 'MK-001 : Creating Read me git instructions'
```
* MK-001 - branch name
* "Creating Read me git instructions" - small description of changes made

To actually finish the submition, run
```
git push -v origin MK-001:MK-001-creating-read-me
```
*"creating-read-me" - small branch description / branch name.

