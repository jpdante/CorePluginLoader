using System;
using System.Collections.Generic;
using System.IO;

namespace CorePluginLoader.Testing {
    public class Program {

        public static Program Context;
        public List<IPlugin> Plugins;

        public static void Main(string[] args) {
            new Program().Start();
        }

        public void Start() {
            //Allows the host to be accessed by any assembly
            Context = this;
            Console.WriteLine("Loading plugins...");
            //Creates the plugin loader
            var pluginLoader = new PluginLoader<IPlugin>();
            //Generates a path where the plugins are located
            var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins/");
            if (!Directory.Exists(pluginsDirectory)) Directory.CreateDirectory(pluginsDirectory);
            Plugins = new List<IPlugin>();
            //Load the plugins
            var plugins = pluginLoader.LoadPlugins(Directory.GetFiles(pluginsDirectory, "*.dll", SearchOption.AllDirectories));
            //Adds the loaded plugins to a list
            Plugins.AddRange(plugins);
            if(Plugins.Count == 0) Console.WriteLine("No plugins found.");
            //Start the plugins
            foreach (var plugin in Plugins) {
                Console.WriteLine($"Starting Plugin: {plugin.PluginName}");
                plugin.Start();
            }
            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();
        }
    }
}
