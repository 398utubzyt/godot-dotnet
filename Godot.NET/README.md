# Godot.NET

This is the .NET side of the extension. It is split into four primary components:

- [GDExtension Generator](#gdextension-generator)
- [Godot.NET.Sdk](#godotnetsdk)
- [Godot.Roslyn](#godotroslyn)
- [GodotSharp](#godotsharp)

Each play a different part in Godot's .NET functionality.

## [GDExtension Generator](./GDExtensionGenerator/)

This is the generator for GDExtension, which takes in an `extension_api.json` or a `gdextension_interface.h` and generates source code for [GodotSharp](#godotsharp).

## [Godot.NET.Sdk](./Godot.NET.Sdk/)

This is the SDK that will be used by Godot users to help create their projects. It provides the Roslyn source generators and .NET project settings which are necessary for proper functionality.

## [Godot.Roslyn](./Godot.Roslyn/)

This is a .NET Roslyn project which provides source generators and analyzers to ensure code quality and prevent errors within Godot. This also generates user script/class at compile-time to allow for a faster runtime.

## [GodotSharp](./GodotSharp/)

This is the main GDExtension plugin. This is what interacts with Godot itself and provides runtime functionality and Godot bindings. Every .NET project in Godot is must reference this library.
