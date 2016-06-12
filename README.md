# NUnit 3.0 Fork - Custom `ITestAssemblyBuilder` #

This NUnit 3.0 fork allows test assemblies to specify a custom `ITestAssemblyBuilder` implementation. Implementing `ITestAssemblyBuilder` allows you to customize test discovery beyond test fixtures and test methods. Scenario's include ordered testing among other things.

## Building
Ensure the Microsoft build tools are in your %PATH% and run 'build.ps1' or 'build.cmd'.

## Usage
Decorate your assembly with an `TestAssemblyBuilderAttribute` pointing to the desired `ITestAssemblyBuilder` interface:

```
[assembly:TestAssemblyBuilder(typeof(MyCustomTestAssemblyBuilder)))]
```
