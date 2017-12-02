using Mod.Exceptions;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandSpectate : Command
    {
        public override string CommandName => "spectate";
        public override void Execute(string[] args)
        {
            if (!PhotonPlayer.TryParse(args[0], out PhotonPlayer player))
                throw new PlayerNotFoundException(args[0]);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (obj.GetPhotonView().owner.ID != player.ID) continue;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
            }
        }
    }
}
