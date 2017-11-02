﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Mod.Interface;

namespace Mod.Managers
{
    public class InterfaceManager
    {
        private readonly List<Gui> guis = new List<Gui>();

        public InterfaceManager()
        {
            GameObject go = new GameObject("Interface");
            guis.AddRange(new Gui[]
            {
                go.AddComponent<Scoreboard>(),
                go.AddComponent<Loading>(),
                go.AddComponent<ServerList>(),
                go.AddComponent<CreateRoom>(),
                go.AddComponent<MainMenu>(),
                go.AddComponent<Background>(),
            });
            Object.DontDestroyOnLoad(go);
        }

        public bool IsVisible(string name)
        {
            Gui gui = guis.FirstOrDefault(g => g.Name == name);
            if (gui != null && gui.Visible)
                return true;
            return false;
        }

        public void Enable(string name)
        {
            Gui gui = guis.FirstOrDefault(g => g.Name == name);
            if (gui != null && !gui.Visible)
                gui.Enable();
        }

        public void Disable(string name)
        {
            Gui gui = guis.FirstOrDefault(g => g.Name == name);
            if (gui != null && gui.Visible)
                gui.Disable();
        }

        public void OnJoinedGame()
        {
            Enable("Scoreboard");
            Disable("CreateRoom");
            Disable("Loading");
            Disable("MainMenu");
            Disable("ServerList");
            Disable("Background");
        }
    }
}