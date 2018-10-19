using System;
using System.Security;
using Photon;
using UnityEngine;

namespace Mod.Discord
{
    public static class DiscordRpc
    {
        private static readonly string[] _mainMenuTexts = {
            "Shelter menu is shiny",
            "Those buttons are very tempting",
            "I'm too shy to join a room",
            "Maybe I should play singleplayer...",
            "Am I supposed to click this button?",
            "Titans scare me too much, I can't play!",
            "Server list.. What does this mean?"
        };
        public static void SendMainMenu()
        {
            DiscordApi.UpdatePresence(new DiscordApi.RichPresence
            {
                details = "In Main Menu",
                startTimestamp = Shelter.StartTime,
                largeImageKey = "mainmenu",
                largeImageText = _mainMenuTexts[UnityEngine.Random.Range(0, _mainMenuTexts.Length)]
            });
        }

        public static void SendInGameMulti()
        {
            string largeImageKey = "mainmenu", map = FengGameManagerMKII.Level;
            if (FengGameManagerMKII.Level.Contains("Forest") && !FengGameManagerMKII.Level.Contains("Custom"))
            {
                largeImageKey = "forest";
                map = "Forest";
            }

            if (FengGameManagerMKII.Level.Contains("City"))
            {
                largeImageKey = "city";
                map = "City";
            }

            if (FengGameManagerMKII.Level == "OutSide")
            {
                largeImageKey = "outsidethewalls";
                map = "Outside the Walls";
            }
            
            
            DiscordApi.UpdatePresence(new DiscordApi.RichPresence
            {
                details = "Multiplayer",
                state = PhotonNetwork.Room.Name.RemoveColors().MaxChars(25),
                partyId = PhotonNetwork.Room.FullName,
                partySize = Room.CurrentPlayers,
                partyMax = PhotonNetwork.Room.MaxPlayers,
                startTimestamp = Shelter.StartTime,
                largeImageKey = largeImageKey,
                largeImageText = "Map: " + map
            });
        }
    }
}