using System;
using Mod.Events.EventArgs;

namespace Mod.Interface.Components
{
    public interface IComponent
    {
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
    }
}