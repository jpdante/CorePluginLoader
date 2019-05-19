using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CorePluginLoader {
    public class PluginLoader<T> : PluginLoaderBase<T> {

        protected readonly List<string> DependencyFindPaths;
        protected readonly SearchMode SearchMode;

        /// <summary>
        /// Builds the plugin loader
        /// </summary>
        /// <param name="searchMode">Search mode for dependency resolution</param>
        public PluginLoader(SearchMode searchMode = SearchMode.AssemblyAndPluginDirectory) {
            SearchMode = searchMode;
            DependencyFindPaths = new List<string>();
            if (SearchMode == SearchMode.AssemblyOnly || SearchMode == SearchMode.AssemblyAndPluginDirectory) {
                DependencyFindPaths.Add(CurrentAssemblyDirectory);
            }
        }
        /// <summary>
        /// Adds a path to dependency resolution
        /// </summary>
        /// <param name="path">Directory path</param>
        public void AddDependencyPath(string path) => DependencyFindPaths.Add(path);

        /// <summary>
        /// Gets all dependency paths
        /// </summary>
        /// <returns>Directory Path Array</returns>
        public string[] GetDependenciesPath() => DependencyFindPaths.ToArray();

        /// <summary>
        /// Removes a path to dependency resolution
        /// </summary>
        /// <param name="path">Directory path</param>
        public void RemoveDependencyPath(string path) => DependencyFindPaths.Remove(path);

        /// <summary>
        /// Loads the plugins in the inserted paths
        /// </summary>
        /// <param name="assemblyFileNames">Paths to the DLL files of the plugins</param>
        /// <returns>IEnumerable instances of plugins.</returns>
        public IEnumerable<T> LoadPlugins(string[] assemblyFileNames) => LoadPlugins(GetPluginsAssemblies(assemblyFileNames));

        /// <summary>
        /// Loads the plugin in the inserted path
        /// </summary>
        /// <param name="assemblyFileName">Path to the DLL file of the plugin</param>
        /// <returns>IEnumerable instances of plugins.</returns>
        public IEnumerable<T> LoadPlugin(string assemblyFileName) => LoadPlugins(GetPluginsAssemblies(new[] {assemblyFileName}));

        protected override void BeforeLoadAssembly(string pluginPath) {
            if (SearchMode == SearchMode.AssemblyAndPluginDirectory || SearchMode == SearchMode.PluginDirectoryOnly) {
                DependencyFindPaths.Add(Path.GetDirectoryName(pluginPath));
            }
        }

        protected override Assembly LoadPluginAssembly(object sender, ResolveEventArgs args) {
            if (SearchMode == SearchMode.None) return null;
            var pluginDependencyName = args.Name.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).First();
            foreach (var path in DependencyFindPaths) {
                var pluginDependencyFullName = Path.Combine(path, $"{pluginDependencyName}.dll");
                if (File.Exists(pluginDependencyFullName)) return Assembly.LoadFile(pluginDependencyFullName);
            }
            return null;
        }

        private static string CurrentAssemblyDirectory => Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
    }

    public enum SearchMode {
        None, AssemblyOnly, AssemblyAndPluginDirectory, PluginDirectoryOnly
    }
}
