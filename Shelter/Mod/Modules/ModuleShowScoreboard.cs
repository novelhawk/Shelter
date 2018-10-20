using Mod.Interface;
using UnityEngine;

namespace Mod.Modules
{
    public class ModuleShowScoreboard : Module
    {
        public override string Name => "Show Scoreboard";
        public override string Description => "Shows the scoreboard top left";
        public override bool IsAbusive => false;
        public override bool HasGUI => true;

        protected override void OnModuleEnable()
        {
            _scoreboard = Shelter.InterfaceManager.GetGUI(nameof(Scoreboard));
            _showOnTab = PlayerPrefs.GetInt(PlayerPref + ".showOnTab", 0) == 1;
            if (!_showOnTab)
                _scoreboard.Enable();
        }

        protected override void OnModuleUpdate()
        {
            if (!_showOnTab)
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

        private Gui _scoreboard;
        private bool _showOnTab;
        
        public override void Render(Rect windowRect)
        {
            GUILayout.BeginArea(windowRect);
            
            GUILayout.Label("Show scoreboard only when Tab is pressed:");
            var newValue = GUILayout.Toggle(_showOnTab, "On/Off");
            if (newValue != _showOnTab)
            {
                if (!newValue)
                    _scoreboard.Enable();
                PlayerPrefs.SetInt(PlayerPref + ".showOnTab", _showOnTab ? 1 : 0);
                _showOnTab = newValue;
            }
            GUILayout.EndArea();
        }
    }
}