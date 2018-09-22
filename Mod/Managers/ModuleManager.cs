using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mod.Managers
{
    public sealed class ModuleManager
    {
        private static readonly List<Module> _modules = new List<Module>();
        
        public ModuleManager()
        {
            GameObject obj = new GameObject("Modules");
            foreach (var type in Shelter.Assembly.GetTypes())
            {
                if (type.Namespace != $"{nameof(Mod)}.{nameof(Modules)}")
                    continue;

                if (type.IsSubclassOf(typeof(Module)))
                    _modules.Add(obj.AddComponent(type) as Module);
            }
            Object.DontDestroyOnLoad(obj);
        }

        public Module GetModule(string id)
        {
            return _modules.FirstOrDefault(x => x.ID == id);
        }

        public bool Enabled(string id)
        {
            return _modules.Any(x => x.ID == id && x.Enabled);
        }

        public static IEnumerable<Module> Modules => _modules; 
    }
}