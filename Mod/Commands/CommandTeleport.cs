using Mod.Exceptions;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandTeleport : Command
    {
        public override string CommandName => "tp";
        public override string[] Aliases => new[] {"teleport"};

        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                throw new CommandArgumentException(CommandName, "/tp [id]");
            if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                throw new PlayerNotFoundException(args[0]);
            if (player.isLocal)
                throw new TargetCannotBeLocalException("Non puoi teletrasportarti da te stesso.");

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (Equals(go.GetPhotonView().owner, player))
                {
                    IN_GAME_MAIN_CAMERA.instance.main_object.transform.position = go.transform.position;
                    IN_GAME_MAIN_CAMERA.instance.main_object.transform.rotation = go.transform.rotation;
                }
            }
        }
    }
}
