# Ludiq.Dependencies

A simple utility to import Unity project dependencies. Given a source (ex.: a git repository), this utility will import the content of any of its subfolders to a destination in your project, rename the namespaces if need be, and preserve any file you choose to.

## Installation

1. Copy the `Dependencies` folder into your project.
2. Enjoy!

All assets from this utility are found in the `Assets > Create` menu.

## Dependency Parameters

Parameter		| Description
------------|-------------
Source Path	|The path, relative to the source root, from which imported files will be copied.
Destination Path	|The path, relative to the project's `Assets` folder, where imported files will be copied.
Preserve Files		|A list of file patterns (ex.: `config_*.txt`) that will not be overwritten when importing. Useful if, for example, the dependency has a configuration file that you want to avoid resetting whenever you update.
Source Namespace		|The namespace from the source to be replaced.
Destination Namespace	|The replacement namespace for in imported files.
Repository Path	|The repository URL, used as a parameter for `git clone`.

## Example: Git dependency

Say we wanted to use [Rotorz Reorderable List](https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity) in our Unity project. Instead of manually going to the Bitbucket repository, downloading the zip, then copying the folder, we can setup the utility to pull the files we need directly from git. Then, importing and updating the dependency will be a 1-click operation.

Here's how to set it up:

1. From the menu, choose `Assets > Create > Git Dependency`
2. Fill in the information in the inspector (table below)
3. Click `Import`

Parameter		| Description
------------|-------------
Source Path	|`Editor`
Destination Path	|`Dependencies/ReorderableList/Editor`
Preserve Files		| (None)
Source Namespace		|`Rotorz.ReorderableList`
Destination Namespace	|`MyProject.ReorderableList`
Repository Path	|https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity


That's it! In the background, the utility will perform a git clone, strip it of all git folders and files, copy it to your chosen destination folder, then replace every `namespace` and `using` directive with from `Rotorz` to `MyProject`.

## Example: Multiple dependencies

If you have multiple dependencies, you can use a `Dependency Group` asset from the same menu. In its inspectors, add each dependency in other you want them to be imported, then click `Import All`.

For example, if we wanted to import [Ludiq.Reflection](https://github.com/lazlo-bonin/ludiq-reflection), we would also need to import its own dependency, [Ludiq.Controls](https://github.com/lazlo-bonin/ludiq-controls). To do that in one click, the steps would be:

1. Create a `Ludiq.Reflection` dependency with the parameters in the table below
2. Create a `Ludiq.Controls` dependency with the parameters in the table below
3. From the menu, choose `Assets > Create > Dependency Group`, and assign both dependencies in the list (the order is irrelevant)
4. Click `Import All`.

Parameter		| Ludiq.Reflection | Ludiq.Controls
------------|-------------|----
Source Path	|`Reflection`|`Controls`
Destination Path	|`Dependencies/Ludiq/Reflection`|`Dependencies/Ludiq/Controls`
Preserve Files		| (None)|(None)
Source Namespace		|(Empty)|(Empty)
Destination Namespace	|(Empty)|(Empty)
Repository Path	|https://github.com/lazlo-bonin/ludiq-reflection|https://github.com/lazlo-bonin/ludiq-controls

## Example: Self-updating dependency

**WARNING: Currently throws an file permission exception. Do not use.**

Why not use `Ludiq.Dependencies` to update... itself?

Create a git dependency and give it the following parameters:

Parameter		| Description
------------|-------------
Source Path	|`Dependencies`
Destination Path	|Wherever you copied the `Dependencies` folder when installing
Preserve Files		| (None)
Source Namespace		|`Ludiq.Dependencies`
Destination Namespace	|(Empty, since we're not redistributing)
Repository Path	|https://github.com/lazlo-bonin/ludiq-dependencies

## Contributing

I'll happily accept pull requests if you have improvements or fixes to suggest.

### To-do

- Testing
- Documentation for local dependencies
- Git credentials for private repositories
- More dependency sources (HTTP / FTP / etc.)
- Namespace replacement in JavaScript files
- Fix self-updating dependency

##  License

The whole source is under MIT License, which basically means you can freely use and redistribute it in your commercial and non-commercial projects. See [the license file](LICENSE) for the boring details. You must keep the license file and copyright notice in copies of the library.

If you use it in a plugin that you redistribute, please change the namespace to avoid version conflicts with your users. For example, change `Ludiq.Dependencies` to `MyPlugin.Dependencies`.
