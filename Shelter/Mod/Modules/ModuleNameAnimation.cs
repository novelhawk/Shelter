using Game;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using Animator = Mod.Animation.Animator;

namespace Mod.Modules
{
    public class ModuleNameAnimation : Module
    {
        public override string Name => "Name Animation";
        public override string Description => "Animates your in game name with an animation of your choice.";
        public override bool IsAbusive => false;
        public override bool HasGUI => true;

        private float _updatesPerSecond = 30;
        private int _shades = 25;

        private Animator _animator;

        protected override void OnModuleEnable()
        {
            _animator = new Animator(Shelter.AnimationManager.Animation, 20);
            _animator.ComputeNext();
        }
        
        private float _lastUpdate;
        protected override void OnModuleUpdate()
        {
            if (!PhotonNetwork.inRoom || Time.time - _lastUpdate < 1f / _updatesPerSecond)
                return;
            
            Player.Self.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Name, _animator.Current}
            });
            
            _animator.ComputeNext();
            _lastUpdate = Time.time;
        }

        protected override void OnModuleDisable()
        {
            _animator = null;
            Player.Self.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Name, Shelter.Profile.Name}
            });
        }

        private Texture2D _animationButton;
        private Texture2D _selectedAnimationButton;
        private GUIStyle _button;
        private GUIStyle _selectedButton;
        private GUIStyle _sliderText;
        protected override void OnGuiOpen()
        {
            _animationButton = Gui.Texture(0, 20, 0, 80);
            _selectedAnimationButton = Gui.Texture(100, 220, 255);
            
            _button = new GUIStyle
            {
                normal = {background = _animationButton, textColor = Gui.Color(220, 90, 255)},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15
            };
            _selectedButton = new GUIStyle(_button)
            {
                normal = {background = _selectedAnimationButton}
            };
            
            _sliderText = new GUIStyle
            {
                normal = {textColor = Color.black},
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            };
        }

        public override void Render(Rect windowRect)
        {
            SmartRect rect = new SmartRect(windowRect.x, windowRect.y - 30, windowRect.width, 30);
            
            for (var i = 0; i < Shelter.AnimationManager.Animations.Count; i++)
            {
                var info = Shelter.AnimationManager.Animations[i];
                if (GUI.Button(rect.OY(30), info.Name, Shelter.AnimationManager.Selected != i ? _button : _selectedButton))
                {
                    Shelter.AnimationManager.Selected = i;
                    _animator = new Animator(info, _shades);
                    _animator.ComputeNext();
                }
            }
                
            GUI.Label(rect.OY(40), $"Shades: {_shades}", _sliderText);
            _shades = (int) GUI.HorizontalSlider(rect.OY(20), _shades, 1, 100);
            
            GUI.Label(rect.OY(20), $"Frequency: {_updatesPerSecond:0.00} hZ", _sliderText);
            _updatesPerSecond = GUI.HorizontalSlider(rect.OY(20), _updatesPerSecond, 1, 100); //TODO: Color viewer and Animation selector
        }

        protected override void OnGuiClose()
        {
            Destroy(_animationButton);
            Destroy(_selectedAnimationButton);
        }
    }
}