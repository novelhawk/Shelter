using ExitGames.Client.Photon;
using Mod.Interface;
using UnityEngine;
using Animator = Mod.Animation.Animator;
using MonoBehaviour = Photon.MonoBehaviour;

namespace Mod
{
    public class AnimationTest : MonoBehaviour
    {
        private Animator _animator;
        
        public AnimationTest()
        {
            _animator = new Animator(Shelter.AnimationManager.Animation, 15);
            _animator.ComputeNext();
        }

        private float _lastUpdate;
        private void Update()
        {
            if (!PhotonNetwork.inRoom || Time.time - _lastUpdate < .15f)
                return;
            
            if (_animator == null)
                _animator = new Animator(Shelter.AnimationManager.Animation, 10);
            Player.Self.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Name, _animator.Name}
            });
                
            _animator.ComputeNext();
            _lastUpdate = Time.time;
        }
    }
}