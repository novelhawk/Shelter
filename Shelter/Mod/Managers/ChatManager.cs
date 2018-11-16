using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Mod.Interface.Components;
using Mod.Profiles;
using Photon.Enums;
using UnityEngine;

namespace Mod.Managers
{
    public class ChatManager
    {
        public const int MAX_MESSAGES = 50;
        
        // Space is pre-allocated, even tho sizeof(ChatMessage) is not that much I'd like to use the approch List<T> uses
        // TODO: Make array start with 0 elements and use Array.Copy to create a copy with 1 more entry (or add by 5 to reduce performance hit)
        public readonly ChatMessage[] Messages = new ChatMessage[MAX_MESSAGES];
        public int CurrentIndex = MAX_MESSAGES - 1;
        public int Elements = 0;

        public ChatMessage AddMessage(in object message) => AddMessage(null, message);
        /// <summary>
        /// Add a message to the ChatBox
        /// </summary>
        /// <param name="sender">The sender of the message</param>
        /// <param name="message">The <see cref="string"/> message content</param>
        /// <param name="isForemost">Specifies whether the message should be pinned</param>
        public ChatMessage AddMessage(in Player sender, in object message, in bool isForemost = false)
        {
            var msg = Utility.ValidateUnityTags(message.ToString());
            var ret = Messages[CurrentIndex--] = new ChatMessage(CurrentIndex, sender, msg, isForemost);
            if (CurrentIndex < 0)
                CurrentIndex = MAX_MESSAGES - 1;
            if (Elements < MAX_MESSAGES)
                Elements++;
            return ret;
        }

        public void Clear()
        {
            Array.Clear(Messages, 0, MAX_MESSAGES);
            Elements = 0;
            CurrentIndex = MAX_MESSAGES - 1;
        }

        public ChatMessage SendMessage(in string message) => SendMessage(message, string.Empty);
        public ChatMessage SendMessage(in string message, in string sender) => SendMessage(message, sender, PhotonTargets.All);

        public ChatMessage SendMessage(in string message, in string sender, in Player player)
        {
            GameManager.instance.photonView.RPC(Rpc.Chat, player, message, sender);
            return AddMessage(Player.Self, message);
        }
        
        public ChatMessage SendMessage(in string message, in string sender, PhotonTargets target)
        {
            ChatMessage ret = default;
            if (target >= PhotonTargets.All)
                ret = AddMessage(Player.Self, message);
            if (target == PhotonTargets.All)
                target = PhotonTargets.Others;
            if (target == PhotonTargets.AllBuffered)
                target = PhotonTargets.OthersBuffered;
            GameManager.instance.photonView.RPC(Rpc.Chat, target, message, sender);
            return ret;
        }

        public ChatMessage Create()
        {
            var ret = Messages[CurrentIndex--] = new ChatMessage(CurrentIndex);
            if (CurrentIndex < 0)
                CurrentIndex = MAX_MESSAGES - 1;
            if (Elements < MAX_MESSAGES)
                Elements++;
            return ret;
        }

        public void Edit(ChatMessage? message, in string content, in bool isForemost = false)
        {
            if (message == null)
                return;
            Edit(message.Value, content, isForemost);
        }
        
        public void Edit(ChatMessage message, in string content, in bool isForemost = false) 
        {
            message.IsForemost = isForemost;
            message.Content = content;
            Messages[message.ID] = message;
        }
        
        [StringFormatMethod("message")]
        public ChatMessage System(string message, params object[] args) => System(string.Format(message, args));
        public ChatMessage System(object message) => System(message.ToString());
        public ChatMessage System(string message)
        {
            const string SystemColor = "04F363";
            return AddMessage($"<color=#{SystemColor}>{message}</color>");
        }

        /// <summary>
        /// Formats the message with user's specified pattern.
        /// </summary>
        /// <param name="content">Message content</param>
        /// <returns>Formatted message</returns>
        public static string MessageFormat(in string content)
        {
            var profile = Shelter.Profile;

            var matches = Regex.Matches(profile.ChatFormat, @"\$\((\w+)\)");
            if (matches.Count <= 0)
                return profile.ChatFormat;

            StringBuilder builder = new StringBuilder(profile.ChatFormat);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var match = matches[i];
                if (!match.Success)
                    continue;
                var propName = match.Groups[1].Value;
                
                string value;
                if (propName.EqualsIgnoreCase("Content"))
                    value = content; 
                else if (!profile.Properties.TryGetValue(propName, out value))
                    continue;
                
                builder.Remove(match.Index, match.Length);
                builder.Insert(match.Index, value ?? string.Empty);
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Draws the message to the screen
        /// </summary>
        /// <param name="message"><see cref="ChatMessage"/> that will get drawn</param>
        /// <param name="dist">Distance between the bottom of the screen and the message</param>
        /// <param name="style">Label's <see cref="GUIStyle"/></param>
        public static void DrawMessage(in ChatMessage message, in float dist, in GUIStyle style)
        {
            const float x = 0;
            const float width = 500;
            
            GUI.Label(new Rect(x, Screen.height - dist, width, 20), message.ToString(), style);
        }

        public void ReceivePrivateMessage(string s, Player infoSender)
        {
            throw new NotImplementedException();
        }
    }
}