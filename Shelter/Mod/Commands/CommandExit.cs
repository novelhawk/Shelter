using Photon;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandExit : Command
    {
        public override string CommandName => "exit";
        public override string[] Aliases => new[] {"quit"};

        public override void Execute(string[] args)
        {
            if (args.Length > 0)
                Application.Quit();
            
            PhotonNetwork.Disconnect();
        }
    }
}