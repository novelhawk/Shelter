using System;
using Mod.Interface;
using UnityEngine;

namespace Mod.Modules
{
    public class ModuleShowScoreboard : Module
    {
        public override string ID => nameof(ModuleShowScoreboard);
        public override string Name => "Show Scoreboard";
        public override string Description => "Shows the scoreboard top left";
        public override bool IsAbusive => false;
        public override bool HasGUI => true;

        private Gui _scoreboard;
        private bool _showOnlyOnTab;
        public override Action<Rect> GetGUI()
        {
            return rect =>
            {
                GUILayout.BeginArea(rect);
                
                GUILayout.Label("Show scoreboard only when Tab is pressed:");
                var newValue = GUILayout.Toggle(_showOnlyOnTab, "On/Off");
                if (newValue != _showOnlyOnTab)
                {
                    if (!newValue)
                        _scoreboard.Enable();
                    _showOnlyOnTab = newValue;
                }
                GUILayout.EndArea();
            };
        }

        protected override void OnModuleEnable()
        {
            _scoreboard = Shelter.InterfaceManager.GetGUI(nameof(Scoreboard));
            if (!_showOnlyOnTab)
                _scoreboard.Enable();
        }

        protected override void OnModuleUpdate()
        {
            if (!_showOnlyOnTab)
                return;
            
            if (Input.GetKey(KeyCode.Tab) && !_scoreboard.Visible)
                _scoreboard.Enable();
            else if (!Input.GetKey(KeyCode.Tab) && _scoreboard.Visible)
                _scoreboard.Disable();
        }
        
        protected override void OnModuleDisable()
        {
            _scoreboard.Disable();
            _scoreboard = null;
        }
    }
}