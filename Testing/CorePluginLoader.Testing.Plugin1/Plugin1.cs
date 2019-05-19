using System;
using MySql.Data.MySqlClient;

namespace CorePluginLoader.Testing.Plugin1 {
    public class Plugin1 : IPlugin {

        public string PluginName => "Plugin1";

        //On plugin start
        public void Start() {
            Console.WriteLine($"[{PluginName}] Plugin started!");
            //Force plugin system search for the dependency
            var mySqlConnection = new MySqlConnection("");
        }

        //The command can be called by other plugins or by the host
        public void Command() {
            Console.WriteLine($"[{PluginName}] Command called!");
        }
    }
}
