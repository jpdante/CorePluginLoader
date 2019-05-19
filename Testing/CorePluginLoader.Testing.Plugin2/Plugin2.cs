using System;

namespace CorePluginLoader.Testing.Plugin2 {
    public class Plugin2 : IPlugin {

        public string PluginName => "Plugin2";

        //On plugin start
        public void Start() {
            Console.WriteLine($"[{PluginName}] Plugin started!");
            //Searches for plugins of type "Plugin1" to call the command
            foreach (var plugin in Program.Context.Plugins) {
                if (plugin is Plugin1.Plugin1 plugin1) {
                    //Call the command
                    plugin1.Command();
                }
            }
        }
    }
}
