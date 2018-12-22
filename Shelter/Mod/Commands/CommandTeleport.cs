using Mod.Exceptions;
using Mod.Interface;
using Photon;
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
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetPhotonView().owner.ID == player.ID)
                {
                    IN_GAME_MAIN_CAMERA.instance.main_object.transform.position = go.transform.position;
                    IN_GAME_MAIN_CAMERA.instance.main_object.transform.rotation = go.transform.rotation;
                    Notify.New($"You teleported to {player}!", 1.3f, 35F);
                    return;
                }
            }
            
            Shelter.Chat.System("Player is not alive!");
        }
    }
}
