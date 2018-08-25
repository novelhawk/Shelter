using System;
using System.Collections.Generic;
using System.Linq;
using Mod.Events;

namespace Mod.Managers
{
    public class EventManager
    {
        private Dictionary<string, EventHandler> _events = new Dictionary<string, EventHandler>();
        
        public EventManager()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.Contains("Assembly-CSharp, ")) //Assembly-CSharp, Version=3.6.2.0, Culture=neutral, PublicKeyToken=null
                    continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(EventListener)))
                        continue;

                    foreach (var method in type.GetMethods())
                    {
                        if (method.GetCustomAttributes(typeof(Event), false).FirstOrDefault() is Event attribute)
                            _events.Add(attribute.EventName, Delegate.CreateDelegate(typeof(EventHandler), this, method) as EventHandler);
                    }
                }
            }
        }
        
    }
}