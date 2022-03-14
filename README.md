## App-Launcher
### Description
Uses .ini files to generate program categories and lists within those categories to launch different programs with ease.

### How To Use
```ini
[Basic Utilities]
Notepad=notepad.exe
WordPad=wordpad.exe
Calculator=calc.exe

[Text Editing]
Notepad=notepad.exe
WordPad=wordpad.exe

[Mathematics]
Calculator=calc.exe
```

The program sees each section as a category, and each entry inside a section as a different program.
The entry name in a section is the program name, and the value for the entry is it's file path in the device.

To make your own launcher setup, just follow the structure example provided above when making your own launcher setup.
Also, it will never make a new configuration or reset it, but it will auto-generate the one above if there's no file found in the directory of the program.

Example:
```ini
[Category 1]
Example 1=C:\example1_program.exe
Example 2=C:\example2_program.exe

[Category 2]
Example 3=C:\example3_program.exe
Example 4=C:\example4_program.exe
```

This example here will create 2 different categories each with a pair of 2 different "example programs".

Also, clicking on a list item representing a program will launch it using the file path stored in the program after being loaded from the .ini file.

### Download
[Standalone](https://github.com/Lexz-08/App-Launcher/releases/download/1.0/AppLauncher.exe)
