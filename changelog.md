# Changelog for SharpScss

## 1.4.0 (25 June 2018)
  - Update libsass to 3.5.4

## 1.3.8
  - Fix runtime for MacOSX

## 1.3.7
  - Improve detection of x86 runtime on Windows for net20, net35, net40+

## 1.3.6
  - Fix native library on Windows x86/x64 to return the correct version `Scss.Version`

## 1.3.5
  - Better support for NET20, NET35, NET40 by copying by default the win7-x64 libsass.dll to the folder.
    The platform runtime can be changed in your project by using the SharpScssRuntime variable set by default to `win7-x64`
    
    The possible values for SharpScssRuntime are:
    - `win7-x86`
    - `win7-x64`
    - `osx.10.10-x64`
    - `ubuntu.14.04-x64`
    
    For the netstandard1.3+ platforms (.NET Core), it is based on the RID of your `netcoreapp` project

## 1.3.4
  - upgrade to libsass 3.4.4