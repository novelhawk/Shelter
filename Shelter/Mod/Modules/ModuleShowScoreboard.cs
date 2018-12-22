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

        private Gui _scoreboard;
        private bool _alwaysShow;

        protected override void OnModuleEnable()
        {
            _scoreboard = Shelter.InterfaceManager.GetGUI(nameof(Scoreboard));
            _alwaysShow = PlayerPrefs.GetInt(PlayerPref + ".alwaysShow", 0) == 1;
            if (_alwaysShow)
                _scoreboard.Enable();
        }

        protected override void OnModuleUpdate()
        {
            if (_alwaysShow)
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
        
        public override void Render(Rect windowRect)
        {
            GUILayout.BeginArea(windowRect);
            
            var show = GUILayout.Toggle(_alwaysShow, "Always show the Scoreboard (otherwise show only on Tab)");
            if (show != _alwaysShow)
            {
                _alwaysShow = show;
                
                PlayerPrefs.SetInt(PlayerPref + ".alwaysShow", show ? 1 : 0);
                if (show)
                    _scoreboard.Enable();
            }
            
            GUILayout.EndArea();
        }
    }
}