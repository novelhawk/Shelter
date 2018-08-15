﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using Mod.Interface.Components;
using Mod.Managers;
using UnityEngine;

namespace Mod.Interface
{
    public class Chat : Gui
    {
        public const string SystemColor = "04F363";
        private Texture2D _mBackground;
        private Texture2D _mWhiteTexture;
        private Texture2D _mGreyTexture;
        private bool _mWriting;
        private Vector2 _mScroll;
        private GUIStyle _mInputStyle;

        private static readonly List<ChatMessage> Messages = new List<ChatMessage>();
        public static string Message { get; private set; } = string.Empty;

        #region Static methods

        public static void SendMessage(object sender, object message, PhotonTargets target)
        {
            FengGameManagerMKII.instance.photonView.RPC("Chat", target, message, sender.ToString());
        }

        public static void SendMessage(object sender, object message, Player player)
        {
            FengGameManagerMKII.instance.photonView.RPC("Chat", player, message, sender.ToString());
        }

        public static void SendMessage(object message, PhotonTargets target = PhotonTargets.All)
        {
            SendMessage(string.Empty, message, target);
        }

        public static void SendMessage(object message, Player player)
        {
            SendMessage(string.Empty, message, player);
        }

        public static void SendMessageAsPlayer(object message) //TODO: todo
        {
            FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, CustomFormat(message.ToString()), string.Empty);
        }

        private static string CustomFormat(string message)
        {
            var format = Shelter.Profile.ChatFormat; //TODO: Make it automatic
            format = format.Replace("$(profileName)", Shelter.Profile.ProfileName);
            format = format.Replace("$(name)", Shelter.Profile.HexName);
            format = format.Replace("$(guild)", Shelter.Profile.Guild);
            format = format.Replace("$(chatName)", Shelter.Profile.ChatName);
            format = format.Replace("$(message)", message);
            return format.Replace("$(friendName)", Shelter.Profile.FriendName);
        }

        public static void AddMessage(object message)
        {
            Messages.Insert(0, new ChatMessage(message).CheckHTMLTags());
        }

        public static void ReceiveMessageFromPlayer(Player sender, object message)
        {
            Messages.Insert(0, new ChatMessage($"{sender.HexName}: {message}", sender).CheckHTMLTags());
        }

        public static void ReceivePrivateMessage(Player sender, object message)
        {
            Messages.Insert(0, new ChatMessage($"<color=#1068D4>PM<color=#108CD4>></color></color> <color=#{SystemColor}>{sender.HexName}: {message}</color>", sender).CheckHTMLTags());
        }

        public static void ReceiveMessage(Player sender, object message)
        {
            Messages.Insert(0, new ChatMessage($"{message}", sender).CheckHTMLTags());
        }

        public static void System(object message)
        {
            AddMessage($"<color=#{SystemColor}>{message}</color>");
        } 
        
        public static void Clear()
        {
            Messages.Clear();
        }

        #endregion

        protected override void OnShow()
        {
            _mBackground = Texture(0, 0, 0, 100);
            _mWhiteTexture = Texture(255, 255, 255);
            _mGreyTexture = Texture(136, 136, 136);
        }

        protected override void Render()
        {
            // Works in OnShow() for ProfileChanger. TODO: Move it to OnShow
            _mInputStyle = new GUIStyle(GUI.skin.textArea) // It creates a new instance every frame. Anyone does it so i don't think it's that much of performance loss. (GUI.skin is available only in OnGUI)
            {
                hover = { background = _mGreyTexture, textColor = Color(0, 0, 0) }, // Used as 'normal'
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                if (GUI.GetNameOfFocusedControl() == "ChatInput")
                {
                    if (Message.StartsWith("/") || Message.StartsWith("\\"))
                    {
                        Match match = Regex.Match(Message, @"[\\\/](\w+)(?:\s+(.*))?.*?");
                        Command cmd = Shelter.CommandManager.GetCommand(match.Groups[1].Value); //Core.CommandManager.FirstOrDefault(cmds => cmds.Commands.FirstOrDefault(x => x.EqualsIgnoreCase(match.Groups[1].Value)) != null);

                        if (cmd == null)
                        {
                            System("Command not found.");
                            Message = string.Empty;
                            GUI.FocusControl(string.Empty);
                            _mWriting = !_mWriting;
                            return;
                        }

                        var args = match.Groups[2].Value.Split(' ');
                        if (args[0].Equals(string.Empty)) args = new string[0];

                        if (CommandManager.Execute(cmd, args) != null)
                        {
                            Notify.New("UNHANDLED ERROR", $"Unexpected error running {cmd.CommandName}!\nPlease report the bug to the developer", 10000);
                            System("Exception thrown on " + cmd.CommandName/*Shelter.Lang.Get("message.commandexecutionerror.text", match.Groups[1].Value)*/);
                            Shelter.Log(/*Shelter.Lang.Get("message.exeptionthrown.text", e.GetType().Name), ErrorType.Warning*/);

                        }
                    }
                    else if (Message != string.Empty)
                        SendMessageAsPlayer(Message);

                    Message = string.Empty;
                    GUI.FocusControl(string.Empty);
                }
                _mScroll = new Vector2(0, 0);
                _mRealScroll = new Vector2(0, 0);
                _mWriting = !_mWriting;
            }

            SmartRect rect;
            if (_mWriting)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _mBackground);

                GUI.SetNextControlName(string.Empty);
                rect = new SmartRect(2, Screen.height - 30f - _mScroll.y, Screen.width - 2f, 20);
                foreach (ChatMessage chatMessage in Messages)
                {
                    rect.OY(-15);
                    if (rect.Y > Screen.height - 25) continue;
                    GUI.Label(rect, $"{(chatMessage.LocalOnly? "" : $"[{chatMessage.Sender.ID}] ")}{chatMessage.Message}", new GUIStyle { alignment = TextAnchor.LowerLeft, normal = { textColor = Color(255, 255, 255) } });
                }

                GUI.DrawTexture(new Rect(2, Screen.height - 23, 500 + 2, 17), _mWhiteTexture);
                GUI.SetNextControlName("ChatInput");
                Message = GUI.TextField(new Rect(3, Screen.height - 22, 500, 15), Message, _mInputStyle);
                GUI.FocusControl("ChatInput");
                return;
            }

            GUI.SetNextControlName(string.Empty);

            rect = new SmartRect(2, Screen.height - 25f, Screen.width / 3.5f, 20);
            foreach (ChatMessage chatMessage in Messages)
            {
                if (Shelter.Stopwatch.ElapsedMilliseconds - 10000f <= chatMessage.Time)
                    GUI.Label(rect, $"{(chatMessage.LocalOnly ? "" : $"[{chatMessage.Sender.ID}] ")}{chatMessage.Message}", new GUIStyle { alignment = TextAnchor.LowerLeft, normal = { textColor = Color(255, 255, 255) } });
                rect.OY(-15);
            }
            //TODO: Remove messages after excessive spam
        }


        private float _animation;
        private Vector2 _mRealScroll;
        protected void Update()
        {
            if (!_mWriting) 
                return;
            
            const int moveBy = 50;
            if (Input.mouseScrollDelta.y > 0)
            {
                _mRealScroll -= new Vector2(0, moveBy);
                _animation = .3f;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                _mRealScroll += new Vector2(0, moveBy);
                _animation = .3f;
            }

            if (_mScroll != _mRealScroll || _animation != 0f)
            {
                _mScroll = Vector2.Lerp(_mScroll, _mRealScroll, 1f - _animation * 10 / 3);
                _animation -= Time.deltaTime;

                if (_mScroll == _mRealScroll || _animation <= 0f)
                    _animation = 0f;
            }
        }

        protected override void OnHide()
        {
            Destroy(_mBackground);
            Destroy(_mGreyTexture);
            Destroy(_mWhiteTexture);
        }
    }
}
