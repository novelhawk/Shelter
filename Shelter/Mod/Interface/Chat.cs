using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Mod.Interface.Components;
using Mod.Managers;
using Photon;
using UnityEngine;

namespace Mod.Interface
{
    public class Chat : Gui
    {
        public const string SystemColor = "04F363";
        private Texture2D _background;
        private Texture2D _whiteTexture;
        private Texture2D _greyTexture;
        private GUIStyle _inputField;
        private GUIStyle _chat;
        private Vector2 _scroll;
        private bool _isWriting;

        public static readonly List<ChatMessage> Messages = new List<ChatMessage>();
        public static string LastMessage { get; private set; } = string.Empty;

        #region Static methods
        
        public static int SendMessage(object sender, object message, PhotonTargets target)
        {
            if (target == PhotonTargets.All)
                target = PhotonTargets.Others;
            FengGameManagerMKII.instance.photonView.RPC(Rpc.Chat, target, message.ToString(), sender.ToString());
            return AddMessage(message, Player.Self);
        }

        public static void SendMessage(object sender, object message, Player player)
        {
            FengGameManagerMKII.instance.photonView.RPC(Rpc.Chat, player, message.ToString(), sender.ToString());
        }

        public static int SendMessage(object message, PhotonTargets target = PhotonTargets.All) => SendMessage(string.Empty, message, target);
        public static void SendMessage(object message, Player player) => SendMessage(string.Empty, message, player);

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

        public static void EditMessage(int? id, object message, Player sender, bool foremost)
        {
            if (id == null) return;
            var old = Messages[id.Value];
            Messages[id.Value] = new ChatMessage($"<color=#{SystemColor}>{message}</color>", sender, old.Time) {IsForemost = foremost};
        }

        public static void EditMessage(int? id, object message, bool foremost)
        {
            if (id == null) return;
            var old = Messages[id.Value];
            Messages[id.Value] = new ChatMessage($"<color=#{SystemColor}>{message}</color>", null  , old.Time) {IsForemost = foremost};
        }

        public static int ReceivePrivateMessage(object message, Player sender)
        {
            Messages.Add(new ChatMessage($"<color=#1068D4>PM<color=#108CD4>></color></color> <color=#{SystemColor}>{sender.Properties.HexName}: {message}</color>", sender));
            return Messages.Count - 1;
        }

        public static int AddMessage(object message, Player sender)
        {
            Messages.Add(new ChatMessage(message, sender));
            return Messages.Count - 1;
        }

        public static int AddMessage(object message)
        {
            Messages.Add(new ChatMessage(message));
            return Messages.Count - 1;
        }

        public static int System(object message, bool foremost = false)
        {
            Messages.Add(new ChatMessage($"<color=#{SystemColor}>{message}</color>") {IsForemost = foremost});
            return Messages.Count - 1;
        } 
        
        [StringFormatMethod("message")]
        public static int System(object message, params object[] args)
        {
            return System(string.Format(message.ToString(), args));
        } 
        
        public static void Clear()
        {
            Messages.Clear();
        }

        #endregion

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
                            System("Command not found.");
                            LastMessage = string.Empty;
                            GUI.FocusControl(string.Empty);
                            _isWriting = !_isWriting;
                            return;
                        }

                        var args = match.Groups[2].Value.Split(' ');
                        if (args[0].Equals(string.Empty)) args = new string[0];

                        if (CommandManager.Execute(cmd, args) != null)
                        {
                            Notify.New("UNHANDLED ERROR", $"Unexpected error running {cmd.CommandName}!\nPlease report the bug on github", 10000);
                            System("Exception thrown on " + cmd.CommandName);
                            System("Please report the bug on discord or on github");
                        }
                    }
                    else if (LastMessage != string.Empty)
                    {
                        SendMessage(CustomFormat(LastMessage));
                    }

                    LastMessage = string.Empty;
                    GUI.FocusControl(string.Empty);
                }
                _scroll = new Vector2(0, 0);
                _mRealScroll = new Vector2(0, 0);
                _isWriting = !_isWriting;
            }

            SmartRect rect;
            List<ChatMessage> messages = Messages.OrderBy(x => x.IsForemost).ToList();
            if (_isWriting)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _background);

                GUI.SetNextControlName(string.Empty);
                rect = new SmartRect(2, Screen.height - 30f - _scroll.y, Screen.width - 2f, 20);
                for (var i = Messages.Count - 1; i >= 0; i--)
                {
                    ChatMessage chatMessage = messages[i];
                    var lines = chatMessage.Message.Split('\n');
                    for (int j = lines.Length - 1; j >= 0; j--)
                    {
                        rect.OY(-15);
                        if (rect.Y > Screen.height - 25) 
                            continue;
                        
                        GUI.Label(rect, $"{(chatMessage.LocalOnly ? "" : $"[{chatMessage.Sender.ID}] ")}{lines[j]}", _chat);
                    }
                }

                GUI.DrawTexture(new Rect(2, Screen.height - 23, 500 + 2, 17), _whiteTexture);
                GUI.SetNextControlName("ChatInput");
                LastMessage = GUI.TextField(new Rect(3, Screen.height - 22, 500, 15), LastMessage, _inputField);
                GUI.FocusControl("ChatInput");
                return;
            }

            GUI.SetNextControlName(string.Empty);

            rect = new SmartRect(2, Screen.height - 25f, Screen.width / 3.5f, 20);
            for (var i = Messages.Count - 1; i >= 0; i--)
            {
                ChatMessage chatMessage = messages[i];
                if (chatMessage.IsForemost || Shelter.Stopwatch.ElapsedMilliseconds - 10000f <= chatMessage.Time)
                {
                    var lines = chatMessage.Message.Split('\n');
                    for (int j = lines.Length - 1; j >= 0; j--)
                    {
                        if (lines[j] == string.Empty)
                            continue;
                        GUI.Label(rect, $"{(chatMessage.LocalOnly ? "" : $"[{chatMessage.Sender.ID}] ")}{lines[j]}",
                            _chat);
                        rect.OY(-15);
                    }
                }
            }

            //TODO: Remove messages after excessive spam
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
