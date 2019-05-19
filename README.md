# CorePluginLoader

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/CorePluginLoader/
[main-nuget-badge]: https://img.shields.io/nuget/v/CorePluginLoader.svg

As some know .net core has some problems solving dependencies on dynamically loaded libraries, this project provides a simple API for loading plugins and their dependencies. This project does **NOT** isolate the libraries of the main project, in case you are looking for libraries loader that isolates the libraries of the main project I advise to look at the project [.NET Core Plugins](https://github.com/natemcmaster/DotNetCorePlugins)

The motivation for development was to make it easier to add plugins to the project when they are still in the initial development phase or need to be done quickly, the library facilitates loading by automatically managing everything when possible.

## Getting started
You can either install the library via NuGet or compile the library manually.
```
dotnet add package CorePluginLoader
```
Use of the main API:
```csharp
var pluginLoader = new PluginLoader<T>();
var plugins = pluginLoader.LoadPlugin(assemblyFileName: "./Plugins/MyPlugin/MyPlugin.dll");
```
* `T:` Plugin class or interface type
* `assemblyFileName:` Path to the assembly DLL file

See examples of projects in [Examples/](./Examples/)

## Usage
To make use of plugins you need at least two projects
1) The Host that loads the plugins
2) The Plugin

Usually a third abstraction project is used for the interaction between the Host and the Plugin, but this project focuses on the memory sharing between the Plugin and the Host.
That's because I want to allow a plugin to call methods, get types, and so on, from other plugins. If the memory were isolated for each plugin this would not be possible in an easy way.
### Host
`Program.cs`
```csharp
using System;
using System.IO;
using CorePluginLoader;

namespace HostApplication {
    public class Program {
        public static void Main(string[] args) {
            // Set plugins directory
            var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins/");
            // Create plugin loader
            var pluginLoader = new PluginLoader<IPlugin>();
            // Load plugins
            var pluginsInstances = pluginLoader.LoadPlugins(Directory.GetFiles(pluginsDirectory, "*.dll", SearchOption.AllDirectories));
            // Show loaded plugins
            foreach (var plugin in pluginsInstances) {
                Console.WriteLine($"Plugin loaded: {plugin.PluginName}");   
            }
            Console.ReadKey();
        }
    }
}
```
`IPlugin.cs`
```csharp
namespace HostApplication {
    public interface IPlugin {
        string PluginName { get; }
    }
}
```

### Plugins
`Plugin.cs`
```csharp
using HostApplication;

namespace Plugin {
    public class Plugin : IPlugin {
        public string PluginName => "MyPlugin";
    }
}
```