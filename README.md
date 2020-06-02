# Edit Profiles #

The most of my work projects use [Omicron Test Universe](https://www.omicronenergy.com/en/products/test-universe/) as main test tool.  
However modifying, or correcting the test files manually is cumbersome.  
Edit Profiles can automate this process (for now only for ExeCute modules).  
> Current version is v2.0.2

### Usage: ###

* Enter "Find what:" field your search criteria separated by "|" character.
* Enter "Replace with:" field your replacement items in this field separated by "|" character.
* Or "Select" a .csv file to instead of typing register numbers. (this option would not update values)
* If your Omicron Control Center (*.occ) file(s) is/are password protected enter the password to "Password to edit file(s)" field.
* Use "removeProgID=moduleName" to delete named Omicron test module from your test file.
  * ProgIDs:
    * removeExecute
    * removeSequencer
    * removeRamp
    * removePulse

### What will you need? ###

* [Microsoft .NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
* [Omicron Test Universe](https://www.omicronenergy.com/en/products/test-universe/) v3.10 or higher installed in your computer. 

### Contact Us ###

* [Contact us](http://www.beckwithelectric.com/)