# Bootstrapper

This is the intermediary step between Godot and the .NET extension during initialization. Depending on the configuration, the bootstrapper can do different things, but generally the end goal is to load the .NET runtime and the extension:

- In the editor, it will:
  - Find and load hostfxr
  - Load the .NET extension DLL on initialization.

- In an export template, it will:
  - Load the compiled project .NET DLL directly.

## To-do

There are some things which still need to be done in order to ensure this bootstrapper is ready for general use. See the list below for more info.

- [ ] Export template loading
- [ ] Support for desktop
  - [x] Windows
  - [ ] Linux/BSD
  - [ ] MacOS
- [ ] Support for mobile
  - [ ] Android
  - [ ] iOS
- [ ] Support for web
- [ ] Switch to SCons
