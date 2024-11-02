# SharpScss [![Build Status](https://github.com/xoofx/SharpScss/workflows/ci/badge.svg?branch=master)](https://github.com/xoofx/SharpScss/actions)  [![NuGet](https://img.shields.io/nuget/v/SharpScss.svg)](https://www.nuget.org/packages/SharpScss/)

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/xoofx/SharpScss/master/img/SharpScss.png">

SharpScss is a P/Invoke .NET wrapper around [libsass](https://github.com/sass/libsass) to convert SCSS to CSS.

> Based on the version of `libsass 3.6.6`

## Features

- Pure P/Invoke .NET wrapper, no C++/CLI involved
- Supports converting from a string or from a file
- Supports include paths
- Supports for source maps
- Supports for `libsass` user custom importer callback in `ScssOptions.TryImport`
- Supports for `.NET Standard 2.0+`
- Supports the following platforms:
    - `win-x86`
    - `win-x64`
    - `win-arm`
    - `win-arm64`
    - `linux-x64`
    - `linux-arm`
    - `linux-arm64`
    - `linux-musl-x64`
    - `linux-musl-arm`
    - `linux-musl-arm64`
    - `osx-x64`
    - `osx-arm64`

For older .NET2.0, .NET3.5, .NET4.x+ and `netstandard1.3`, you need to download the `1.4.0` version.

## Download

SharpScss is available on [![NuGet](https://img.shields.io/nuget/v/SharpScss.svg)](https://www.nuget.org/packages/SharpScss/)

## Usage

SharpScss API is simply composed of a main `Scss` class:

- `Scss.ConvertToCss`: to convert a `SCSS` string to a `CSS`  

```
var result = Scss.ConvertToCss("div {color: #FFF;}")
Console.WriteLine(result.Css);
```

- `Scss.ConvertFileToCss`: to convert a `SCSS` file to a `CSS`  

```
var result = Scss.ConvertFileToCss("test.scss")
Console.WriteLine(result.Css);
```

Using the `ScssOptions` you can specify additional parameters:

```
var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions()
{
	InputFile = "Test.scss",
	OutputFile = "Test.css", // Note: It will not generate the file, 
                             // only used for exception reporting
                             // includes and source maps
	GenerateSourceMap = true
});
Console.WriteLine(result.Css);
Console.WriteLine(result.SourceMap);
```

You can use also custom dynamic import through the delegate `ScssOptions.TryImport`. Note that in that cases `ScssOptions.IncludePaths` is not used 
and it is the responsability of the `TryImport` to perform the resolution (e.g on a virtual file system):

``` 
var result = Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
{
	InputFile = "test.scss",
	TryImport = (ref string file, string path, out string scss, out string map) =>
	{
        // Add resolve the file
        // file = resolvedFilePath; // Can change the file resolved
		scss = ...; // TODO: handle the loading of scss for the specified file
		map = null;
		return true;
	}
});
```

## Runtime

SharpScss depends on the native runtime `libsass`. This runtime is compiled for the following platform/runtime:

- `win-x86`
- `win-x64`
- `win-arm`
- `win-arm64`
- `linux-x64`
- `linux-arm`
- `linux-arm64`
- `linux-musl-x64`
- `linux-musl-arm`
- `linux-musl-arm64`
- `osx-x64`
- `osx-arm64`

On .NET Core (`net8.0`), the runtime is selected based on the [Runtime Identifier - RID](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog) of your project.

- You can add to your csproj the specific targeting runtimes your `net8.0` with `<RuntimeIdentifiers>win-x86;linux-x64</RuntimeIdentifiers>` or `<RuntimeIdentifier>` if you have only one runtime to target (See [Additions to the csproj format for .NET Core](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/csproj))

## Build

Currently, the compiled version of libsass shipped with SharpScss is a custom build from the fork [xoofx/libsass](https://github.com/xoofx/libsass)

This fork is mainly allowing to compile libsass without the MSVC C/C++ Runtime on Windows and provide a GitHub CI action to compile all different platforms.

## License

This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause). 

## Author

Alexandre Mutel aka [xoofx](https://xoofx.github.io)
