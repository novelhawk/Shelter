using ExitGames.Client.Photon;
using Mod.Exceptions;
using UnityEngine;

namespace Mod.Commands
{
    public class CommandTest : Command
    {
        public override string CommandName => "test";
        private const string PlayerRespawnTag = "playerRespawn";

        public override void Execute(string[] args)
        {
            GameObject[] spawns = GameObject.FindGameObjectsWithTag(PlayerRespawnTag);
            foreach (GameObject spawn in spawns)
                spawn.transform.position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));

                
            foreach (Object o in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (o is GameObject obj)
                {
                    if (obj.name.Contains("TREE") || obj.name.Contains("aot_supply"))
                        Object.Destroy(obj);
//                    if (obj.name == "Cube_001" && obj.transform.parent.gameObject.tag != "player" &&
//                        obj.renderer != null)
//                    {
//                        groundList.Add(obj);
//                        obj.renderer.material.mainTexture = ((Material) RCassets.Load("grass")).mainTexture;
//                    }
                }
                    
            }
        }
    }
}
