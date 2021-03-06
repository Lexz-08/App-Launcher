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

### Controls
  - `View File in Explorer`
    - Opens `File Explorer` with the folder containing the .ini config file **(which stays with the program at all times)** already viewed
  - `Add Category`
    - Opens a prompt where you enter a category name, and confirming will create a new, empty category in the launcher.
  - `Add Program`
    - Opens a prompt where you enter a program name and path, and confirming will create a new program in the current category in the launcher.
  - `Mouse Left-Click`
    - **Category**
      - Selects a different category of programs in the launcher.
    - **Program**
      - Launches the selected program in the current category in the launcher.
  - `Mouse Right-Click` ***-Opens a [ContextMenu](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/contextmenu?view=netframeworkdesktop-4.8)-***
    - ***Modify***
      - Opens a category/program info prompt for modifying the selected category or prompt.
      - ***Modifying a category no longer requires a restart of the application.***
    - ***Remove***
      - Simply removes the selected category/program from the launcher and the .ini file configuration.

### Download
[Standalone](https://github.com/Lexz-08/App-Launcher/releases/download/1.2/Standalone.zip)
