using ExitGames.Client.Photon;
using Mod.Exceptions;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandTest : Command
    {
        public override string CommandName => "test";

        public override void Execute(string[] args)
        {
            Player.Self.SetCustomProperties(new Hashtable
            {
                {PlayerProperty.Name, "TEST123"}
            });
        }
    }
}
