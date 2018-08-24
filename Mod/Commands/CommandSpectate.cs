using Mod.Exceptions;
using Mod.Interface;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandSpectate : Command
    {
        public override string CommandName => "spectate";
        public override void Execute(string[] args)
        {
            if (args.Length < 1)
                args = new[] { Player.Self.ID.ToString() };
            if (!Player.TryParse(args[0], out Player player))
                throw new PlayerNotFoundException(args[0]);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (obj.GetPhotonView().owner.ID != player.ID) 
                    continue;
                
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
                Notify.New($"You are spectating to {player}!", 1300, 35F);
                return;
            }
            
            Notify.New("Couldn't find the player's HERO!", 1300, 35F);
        }
    }
}
