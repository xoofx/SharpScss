# SharpScss

SharpScss is a P/Invoke .NET wrapper around [libsass](https://github.com/sass/libsass) to convert SCSS to CSS supporting NET2.0/NET3.5/NET4.x+ and CoreCLR platform

## Features

- Pure P/Invoke .NET wrapper, no C++/CLI involved
- Supports converting from a string or from a file
- Supports include paths
- Supports for source maps
- Supports for `libsass` user custom importer callback in `ScssOptions.TryImport`
- Supports for .NET2.0, .NET3.5, .NET4.x+ and CoreCLR (dotnet5.4 profile)
- Supports Windows x86 and x64 (requires C++ VS2013 runtime installed)

## Download

SharpScss is available as a [NuGet](https://nuget.org) package.

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

## Build

Currently, the compiled version of libsass shipped with SharpScss is a custom build from the fork [xoofx/libsass](https://github.com/xoofx/libsass/tree/develop) branch [develop](https://github.com/xoofx/libsass/tree/develop).

This fork is fixing two issues reported to [libsass](https://github.com/sass/libsass):

- [#1973](https://github.com/sass/libsass/pull/1973): Add sass_string_alloc public API function to allow to allocate a string owned by libsass
- [#1974](https://github.com/sass/libsass/pull/1974): Fix sass_option_push_include_path / sass_option_push_plugin_path  

## TODO Tasks

- [ ] Build and package libsass binaries for Linux
- [ ] Build and package libsass binaries for MacOSX

## License
This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause). 

## Author

Alexandre Mutel aka [xoofx](http://xoofx.com)