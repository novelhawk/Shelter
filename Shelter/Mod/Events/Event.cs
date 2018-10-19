using System;

namespace Mod.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class Event : Attribute
    {
        public string EventName { get; }
        
        public Event(string eventName)
        {
            EventName = eventName;
        }
    }
}