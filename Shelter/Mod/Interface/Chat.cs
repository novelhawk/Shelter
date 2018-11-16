using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Mod.Interface.Components;
using Mod.Managers;
using Mod.Modules;
using Photon;
using Photon.Enums;
using UnityEngine;

namespace Mod.Interface
{
    public class Chat : Gui //TODO: Rename to ChatBox
    {
        public const string SystemColor = "04F363";
        private Texture2D _background;
        private Texture2D _whiteTexture;
        private Texture2D _greyTexture;
        private GUIStyle _inputField;
        private GUIStyle _chat;
        private Vector2 _scroll;
        private bool _isWriting;

        public static string LastMessage { get; private set; } = string.Empty;

        private static ChatManager ChatMg => Shelter.Chat;
        

        protected override void OnShow()
        {
            _background = Texture(0, 0, 0, 100);
            _whiteTexture = Texture(255, 255, 255);
            _greyTexture = Texture(136, 136, 136);

            _chat = new GUIStyle
            {
                alignment = TextAnchor.LowerLeft,
                normal = {textColor = Color(255, 255, 255)}
            };
        }

        protected override void Render()
        {
            // Works in OnShow() for ProfileChanger. TODO: Move it to OnShow
            _inputField = new GUIStyle(GUI.skin.textArea) // It creates a new instance every frame. Anyone does it so i don't think it's that much of performance loss. (GUI.skin is available only in OnGUI)
            {
                hover = { background = _greyTexture, textColor = Color(0, 0, 0) }, // Used as 'normal'
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                if (GUI.GetNameOfFocusedControl() == "ChatInput")
                {
                    Match match = Regex.Match(LastMessage, @"^[\\\/](\w+)(?:\s+(.*))?.*?");
                    if (match.Success)
                    {
                        Command cmd = Shelter.CommandManager.GetCommand(match.Groups[1].Value);

                        if (cmd == null)
                        {
                            ChatMg.System("Command not found.");
                            LastMessage = string.Empty;
                            GUI.FocusControl(string.Empty);
                            _isWriting = !_isWriting;
                            return;
                        }

                        var args = match.Groups[2].Value.Split(' ');
                        if (args[0].Equals(string.Empty)) args = new string[0];

                        if (CommandManager.Execute(cmd, args) != null)
                        {
                            Notify.New("UNHANDLED ERROR", $"Unexpected error running {cmd.CommandName}!\nPlease report the bug on github", 10f);
                            ChatMg.System("Exception thrown on " + cmd.CommandName);
                            ChatMg.System("Please report the bug on discord or on github");
                        }
                    }
                    else if (LastMessage != string.Empty)
                    {
                        ChatMg.SendMessage(ChatManager.MessageFormat(LastMessage));
                    }

                    LastMessage = string.Empty;
                    GUI.FocusControl(string.Empty);
                }
                _scroll = new Vector2(0, 0);
                _mRealScroll = new Vector2(0, 0);
                _isWriting = !_isWriting;
            }

            float y;
            if (_isWriting)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _background);

                GUI.SetNextControlName(string.Empty);
                
                y = 30f + _scroll.y;
                for (int i = ChatMg.CurrentIndex + 1; i <= ChatMg.CurrentIndex + ChatMg.Elements; i++)
                {
                    y += 15;
                    if (y < 25)
                        continue;

                    var item = ChatMg.Messages[i % ChatManager.MAX_MESSAGES];
                    ChatManager.DrawMessage(item, y, _chat);
                }

                GUI.DrawTexture(new Rect(2, Screen.height - 23, 500 + 2, 17), _whiteTexture);
                GUI.SetNextControlName("ChatInput");
                LastMessage = GUI.TextField(new Rect(3, Screen.height - 22, 500, 15), LastMessage, _inputField);
                GUI.FocusControl("ChatInput");
                return;
            }

            GUI.SetNextControlName(string.Empty);

            y = 25;
            for (int i = ChatMg.CurrentIndex + 1; i <= ChatMg.CurrentIndex + ChatMg.Elements; i++)
            {
                var item = ChatMg.Messages[i % ChatManager.MAX_MESSAGES];

                if (y > Screen.height * 0.5f || !item.IsVisible)
                    break;

                ChatManager.DrawMessage(item, y, _chat);
                y += 15;
            }
        }


        private float _animation;
        private Vector2 _mRealScroll;
        protected void Update()
        {
            if (!_isWriting) 
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

            if (_scroll != _mRealScroll || _animation != 0f)
            {
                _scroll = Vector2.Lerp(_scroll, _mRealScroll, 1f - _animation * 10 / 3);
                _animation -= Time.deltaTime;

                if (_scroll == _mRealScroll || _animation <= 0f)
                    _animation = 0f;
            }
        }

        protected override void OnHide()
        {
            Destroy(_background);
            Destroy(_greyTexture);
            Destroy(_whiteTexture);
        }
    }
}
