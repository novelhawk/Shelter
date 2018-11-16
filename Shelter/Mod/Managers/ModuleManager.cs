using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mod.Managers
{
    public static class ModuleManager
    {
        public static readonly List<Module> Modules = new List<Module>();
        
        public static void LoadModules()
        {
            GameObject obj = new GameObject("Modules");
            foreach (var type in Shelter.Assembly.GetTypes())
            {
                if (type.Namespace != $"{nameof(Mod)}.{nameof(Mod.Modules)}")
                    continue;

                if (type.IsSubclassOf(typeof(Module)))
                    Modules.Add(obj.AddComponent(type) as Module);
            }
            Object.DontDestroyOnLoad(obj);
        }

        public static bool Enabled(string id)
        {
            return Modules.FirstOrDefault(x => x.ID == id)?.Enabled ?? false;
        }
    }
}