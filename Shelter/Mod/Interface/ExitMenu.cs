using Mod.Keybinds;
using Mod.Managers;
using Photon;
using UnityEngine;

namespace Mod.Interface
{
    public class ExitMenu : Gui
    {
        private GUIStyle _button;
        private Texture2D _buttonBackground;
        private Texture2D _background;

        protected override void OnShow()
        {
            InterfaceManager.OpenMenuCount++;
            
            _background = Texture(0, 0, 0, 80);
            _buttonBackground = Texture(0, 0, 0, 40);
            
            _button = new GUIStyle
            {
                normal =
                {
                    background = _background,
                    textColor = Color(255, 255, 255)
                },
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
        }

        protected override void Render()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _background);
            
            const float btnWidth = 400;
            const float btnHeight = 50;
            const float spacing = 20;
            
            const float numberOfButtons = 3;

            const float centerOffsetY = 80;
            
            var rect = new SmartRect(
                Screen.width / 2f - btnWidth / 2f,
                Screen.height / 2f - ((btnHeight + spacing) * numberOfButtons - spacing) / 2f - centerOffsetY,
                btnWidth,
                btnHeight);

            if (GUI.Button(rect, "Resume", _button))
                Toggle();
            rect.TranslateY(btnHeight + spacing);
            
            if (GUI.Button(rect, "Game Settings", _button))
            {
                Enable(nameof(GameSettingsMenu));
                Toggle();
            }
            rect.TranslateY(btnHeight + spacing);
            
            if (GUI.Button(rect, "Quit", _button))
                PhotonNetwork.Disconnect();
        }

        protected override void OnHide()
        {
            InterfaceManager.OpenMenuCount--;
            
            Destroy(_background);
            Destroy(_buttonBackground);
        }

        private void Update()
        {
            if (InterfaceManager.OpenMenuCount == 0 &&
                PhotonNetwork.inRoom &&
                Shelter.InputManager.IsDown(InputAction.OpenExitMenu))
            {
                Toggle();
            }
        }
    }
}