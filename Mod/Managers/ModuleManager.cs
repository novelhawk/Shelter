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
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.Contains("Assembly-CSharp, ")) //Assembly-CSharp, Version=3.6.2.0, Culture=neutral, PublicKeyToken=null
                    continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Namespace != "Mod.Modules")
                        continue;

                    if (obj.AddComponent(type) is Module module)
                        _modules.Add(module);
                }
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