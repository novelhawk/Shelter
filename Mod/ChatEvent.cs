//using System;
//
//namespace Mod
//{
//    public class ChatEvent : Attribute
//    {
//        private readonly ChatEventType _type;
//
//        protected ChatEvent(ChatEventType type)
//        {
//            _type = type;
//        }
//
//        public virtual bool OnEvent(string message)
//        {
//            return true;
//        }
//
//        public virtual bool OnEvent(string message, Player sender)
//        {
//            return true;
//        }
//
//        public virtual void UserInput(string message)
//        {
//            
//        }
//
//        public ChatEventType Type => _type;
//    } 
//
//    public enum ChatEventType
//    {
//        Both,
//        OnSend,
//        OnReceive
//    }
//}