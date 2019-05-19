using System;
using System.Collections.Generic;
using System.Text;

namespace CorePluginLoader.Testing {
    public interface IPlugin {
        string PluginName { get; }
        void Start();
    }
}
