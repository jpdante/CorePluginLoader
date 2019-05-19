using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorePluginLoader {
    public abstract class PluginLoaderBase<T> {

        protected PluginLoaderBase() {
            //Adds an assembly resolver to the plugins
            AppDomain.CurrentDomain.AssemblyResolve += LoadPluginAssembly;
        }

        /// <summary>
        /// Called every time an assembly or dependency needs to be loaded
        /// </summary>
        /// <param name="sender">Class that called</param>
        /// <param name="args">Arguments for assembly resolution</param>
        /// <returns>The assembly loaded or null in case of failure</returns>
        protected abstract Assembly LoadPluginAssembly(object sender, ResolveEventArgs args);

        /// <summary>
        /// Called before loading assembly
        /// </summary>
        /// <param name="pluginPath">Path to the assembly's DLL</param>
        protected virtual void BeforeLoadAssembly(string pluginPath) {}

        /// <summary>
        /// Called after loading assembly
        /// </summary>
        /// <param name="pluginPath">Path to the assembly's DLL</param>
        /// <param name="pluginAssembly">Assembly loaded from DLL</param>
        protected virtual void AfterLoadAssembly(string pluginPath, Assembly pluginAssembly) {}

        /// <summary>
        /// Called before the plugin is instantiated
        /// </summary>
        /// <param name="pluginAssembly">Plugin Assembly</param>
        /// <param name="pluginType">Plugin Type</param>
        protected virtual void BeforeLoadPlugin(Assembly pluginAssembly, Type pluginType) {}

        /// <summary>
        /// Called after the plugin is instantiated
        /// </summary>
        /// <param name="pluginAssembly">Plugin Assembly</param>
        /// <param name="pluginType">Plugin Type</param>
        /// <param name="plugin">Plugin Instance</param>
        protected virtual void AfterLoadPlugin(Assembly pluginAssembly, Type pluginType, T plugin) {}

        /// <summary>
        /// Gets the assemblies in the reported paths
        /// </summary>
        /// <param name="pluginPaths">IEnumerable of plugins(DLL) paths</param>
        /// <returns>Plugins assemblies</returns>
        protected IEnumerable<Assembly> GetPluginsAssemblies(IEnumerable<string> pluginPaths) {
            foreach (var pluginPath in pluginPaths) {
                if (!File.Exists(pluginPath)) continue;
                BeforeLoadAssembly(pluginPath);
                var assembly = Assembly.LoadFile(pluginPath);
                AfterLoadAssembly(pluginPath, assembly);
                yield return assembly;
            }
        }

        /// <summary>
        /// Get instances of the "T" class plugins through a search
        /// </summary>
        /// <param name="assemblies">Plugins assemblies</param>
        /// <returns>IEnumerable of plugins instances</returns>
        protected IEnumerable<T> LoadPlugins(IEnumerable<Assembly> assemblies) {
            foreach (var pluginAssembly in assemblies) {
                foreach (var pluginType in pluginAssembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract)) {
                    BeforeLoadPlugin(pluginAssembly, pluginType);
                    var plugin = (T)Activator.CreateInstance(pluginType);
                    AfterLoadPlugin(pluginAssembly, pluginType, plugin);
                    yield return plugin;
                }
            }
        }
    }
}
