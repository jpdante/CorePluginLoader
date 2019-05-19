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
