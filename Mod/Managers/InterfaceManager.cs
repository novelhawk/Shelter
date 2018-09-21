using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Mod.Interface;
using Console = Mod.Interface.Console;
using Object = UnityEngine.Object;

namespace Mod.Managers
{
    public class InterfaceManager
    {
        private readonly List<Gui> _interfaces = new List<Gui>();

        public InterfaceManager()
        {
            GameObject go = new GameObject("Interface");
            _interfaces.AddRange(new Gui[]
            {
                go.AddComponent<Console>(),
                go.AddComponent<Notify>(),
                go.AddComponent<Connecting>(),
                go.AddComponent<Navigator>(),
                go.AddComponent<InGameMenu>(),
                go.AddComponent<Scoreboard>(),
                go.AddComponent<Chat>(),
                go.AddComponent<Loading>(),
                go.AddComponent<ProfileChanger>(),
                go.AddComponent<ServerList>(),
                go.AddComponent<CreateRoom>(),
                go.AddComponent<MainMenu>(),
                go.AddComponent<Background>(),
                go.AddComponent<GameInfo>(),
            });
            Object.DontDestroyOnLoad(go);
        }

        public bool IsVisible(string name)
        {
            Gui gui = _interfaces.FirstOrDefault(g => g.Name == name);
            if (gui != null && gui.Visible)
                return true;
            return false;
        }

        public Gui GetGUI(string name)
        {
            return _interfaces.FirstOrDefault(g => g.Name == name);
        }
        
        public void Enable(string name)
        {
            Gui gui = _interfaces.FirstOrDefault(g => g.Name == name);
            if (gui != null && !gui.Visible)
                gui.Enable();
        }

        public void Disable(string name)
        {
            Gui gui = _interfaces.FirstOrDefault(g => g.Name == name);
            if (gui != null && gui.Visible)
                gui.Disable();
        }

        public void DisableAll()
        {
            foreach (Gui gui in _interfaces)
                if (gui != null && gui.Visible)
                    gui.Disable();
        }
    }
}
