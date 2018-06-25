# SharpScss [![Build status](https://ci.appveyor.com/api/projects/status/github/xoofx/sharpscss?svg=true)](https://ci.appveyor.com/project/xoofx/sharpscss/branch/master)  [![NuGet](https://img.shields.io/nuget/v/SharpScss.svg)](https://www.nuget.org/packages/SharpScss/)

SharpScss is a P/Invoke .NET wrapper around [libsass](https://github.com/sass/libsass) to convert SCSS to CSS supporting NET2.0/NET3.5/NET4.x+ and CoreCLR platform

Based on the version of `libsass 3.5.4`

## Features

- Pure P/Invoke .NET wrapper, no C++/CLI involved
- Supports converting from a string or from a file
- Supports include paths
- Supports for source maps
- Supports for `libsass` user custom importer callback in `ScssOptions.TryImport`
- Supports for .NET2.0, .NET3.5, .NET4.x+ and CoreCLR (`netstandard1.3` and `netstandard2.0` from dotnet)
- Supports `Windows x86`, `Windows x64`, `linux-x64` and `osx-x64`

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

You can use also custom dynamic import through the delegate `ScssOptions.TryImport`:

``` 
var result = Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
{
	InputFile = "test.scss",
	TryImport = (string file, string path, out string scss, out string map) =>
	{
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
- `linux-x64`
- `osx-x64`

On .NET Core (`netcoreapp`), the runtime is selected based on the [Runtime Identifier - RID](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog) of your project.

- You can add to your csproj the specific targeting runtimes your `netcoreapp` with `<RuntimeIdentifiers>win-x86;linux-x64</RuntimeIdentifiers>` or `<RuntimeIdentifier>` if you have only one runtime to target (See [Additions to the csproj format for .NET Core](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/csproj))

On .NET20, .NET35, .NET45, it is using a project variable `SharpScssRuntime`

By default, `SharpScssRuntime` selects `win-x86` if `Prefer 32 Bit` is selected in your project (variable `Prefer32Bit` in your csproj), otherwise it will use the `win-x64`

In case you are running SharpScss on Mono on a non Windows platform, you will have to set `SharpScssRuntime` to the proper supported SharpScss RID listed above.

## Build

Currently, the compiled version of libsass shipped with SharpScss is a custom build from the fork [xoofx/libsass/3.5-stable](https://github.com/xoofx/libsass/tree/3.5-stable)

This fork is mainly allowing to compile libsass without the MSVC C/C++ Runtime on Windows.

## License

This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause). 

## Author

Alexandre Mutel aka [xoofx](http://xoofx.com)