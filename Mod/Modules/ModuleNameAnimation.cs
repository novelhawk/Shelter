using System;
using System.ComponentModel;
using ExitGames.Client.Photon;
using Mod.Interface;
using UnityEngine;
using Animator = Mod.Animation.Animator;

namespace Mod.Modules
{
    public class ModuleNameAnimation : Module
    {
        public override string ID => nameof(ModuleNameAnimation);
        public override string Name => "Name Animation";
        public override string Description => "Animates your in game name with an animation of your choice.";
        public override bool IsAbusive => false;
        public override bool HasGUI => true;

        private int _updatesPerSecond = 3;
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

        public override Action<Rect> GetGUI()
        {
            Texture2D background = Gui.Texture(255, 255, 255, 160); // Hopefully disposed by GC.. TODO: Need proper disposal (Should be after game but on GUI close is good enough)
            return rect =>
            {
                GUI.DrawTexture(rect, background);
                GUILayout.BeginArea(rect);
                GUILayout.Label("[THIS UI IS WIP]");
                GUILayout.Label("Animation:");
                GUILayout.Label($"Shades: {_shades}");
                _shades = (int) GUILayout.HorizontalSlider(_shades, 1, 50);
                GUILayout.Label($"UpdatesPerSecond: {_updatesPerSecond}");
                _updatesPerSecond = (int) GUILayout.HorizontalSlider(_updatesPerSecond, 1, 100); //TODO: Color viewer and Animation selector
                if (GUILayout.Button("Apply shades"))
                {
                    _animator = new Animator(Shelter.AnimationManager.Animation, _shades);
                    _animator.ComputeNext();
                }
                GUILayout.EndArea();
            };
        }

        protected override void OnModuleDisable()
        {
            _animator = null;
            Player.Self.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Name, Shelter.Profile.Name}
            });
        }
    }
}