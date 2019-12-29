
# Strokes Key Mapper - Karabiner-like utility for Windows

Strokes is a Windows application that let's you map keys and key combinations to other keys and key combinations. It's inspired by Karabiner, an OSX application that lets you do the same thing.

I made this because I can't see any good Windows solutions for doing these things. On OSX, I have Karabiner. On Linux, a I have AutoKey. On Windows, all I have is strange 10 year old applications and registry hacks that only work for single key strokes.

This is in the bare minimal state to be useful. Finding out how to send and receive keys was hard enough that I assumed it alone was keeping most people from actually creating this tool. My hope is that people will jump in to flesh it out. I've gotten it to the point where I can just use it for my own needs so I may not make it much fancier than this.

## Installation

Copy the sample `.strokes.json` file to your home directory (the dir that contains Documents, Videos, etc.) and [install Strokes][sample-installer]. You can launch it by searching for Strokes.

There isn't much to see at the moment. It just pops up an empty window and a console that prints out some debugging information, but basic keybinds are working.

## Philosophy

There are a few things that should always be true about this application (until someone tells me why I'm wrong).

- It should get everything it needs from a single JSON config file located at `~/.strokes.json`. Sample configuration file named prototype-config.json is included in this repo.
- It should use decimal [virtual key codes][keycodes] internally, but rely on virutal key code names in configuration.

## Disclosure

I'm not a Windows user. I only use Windows to game and occasionally program/stream for some niche use cases. I'm an avid Vim user and I have a keybind setup that I use to navigate my desktop environment. That is important enough to me that I decided to make this to enable my workflow on Windows, regardless of how often I'll use it. Also, it was a good excuse to try out F#.

Consequently, I don't know a lot of the best practices or standard anythigns for Windows. I don't know if I created the installer correctly and I don't know what APIs people should or shouldn't be using. I welcome all feedback in this area.

# Contributing
Here are some outstanding things that need doing. This definitely isn't an exhaustive list.

- Add support for other kinds of events. For example, in Karabiner, you can set delays and different actions based on whether a key was pressed in combination or alone.
- Add a GUI to generate the configuartion file and find out how config files work on Windows. I just stuck it in the home dir like I would have on a Unix OS.
- Add an icon for the package.
- Finish implementing conditional keybinds. I've got the ability to tell the name of the foreground application already. It looks like it may only work for some windows applications though, and I'm not actually respecting conditional anything when executing key strokes.
- Make failures far more resillient.

There are some [docs on the wiki][wiki_docs] that should explain the design and how the code
is structured. Hopefully it's enough to get started.

This is my first Windows anything and I did everything through Windows GUIs. What works for me is checking the project out with git, opening the .sln file using Visual Studio, and hitting Start. I'm not sure what state I have on my machine. If something goes wrong then it's probably FSharp.Data missing, which means you'll have to use nuget to install that.

## Dependencies
- FSharp.Data for JSON parsing, installed through nuget

[global-key-listener]: https://stackoverflow.com/questions/17579658/how-to-intercept-all-the-keyboard-events-and-prevent-losing-focus-in-a-winforms
[keycodes]: https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
[sendinput-ffi-tips]: https://stackoverflow.com/questions/12761169/send-keys-through-sendinput-in-user32-dll
[consume-keys]: https://www.codeproject.com/Articles/14485/Low-level-Windows-API-hooks-from-C-to-stop-unwante
[scancodes-over-vks]: https://stackoverflow.com/questions/49224390/c-sendinput-doesnt-manage-alt-codes-properly
[scancodes-demystified]: http://www.quadibloc.com/comp/scan.htm
[correct-native-api-model]: https://github.com/michaelnoonan/inputsimulator/blob/master/WindowsInput/Native/KEYBDINPUT.cs#L13
[ApplicationFrameHost-issue]: https://github.com/ActivityWatch/activitywatch/issues/182
[nuget-package-manager]: https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio
[sample-installer]: https://github.com/naddeoa/strokes/releases/download/sample/StrokesInstaller.msi
[wiki_docs]: https://github.com/naddeoa/strokes/wiki
