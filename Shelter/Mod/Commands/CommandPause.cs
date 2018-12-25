using Photon.Enums;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandPause : Command
    {
        public override string CommandName => "pause";
        public override string[] Aliases => new[] {"unpause"};

        public override void Execute(string[] args)
        {
            GameManager.instance.photonView.RPC(Rpc.Pause, PhotonTargets.All, Time.timeScale >= 1);
        }
    }
}
