using System;
using System.Collections.Generic;

namespace Mod.Managers
{
    public class EventManager
    {
        private Dictionary<string, EventHandler> _events = new Dictionary<string, EventHandler>();
        
        public void Subscribe<T>(string eventName, EventHandler<T> handler) where T: EventArgs
        {
        }
        
        public void Subscribe(string eventName, EventHandler handler)
        {
            
        }

        public void Fire(string eventName)
        {
        }
    }
}