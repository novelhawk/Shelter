// ----------------------------------------------------------------------------
// <copyright file="FriendInfo.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2013 Exit Games GmbH
// </copyright>
// <summary>
//   Collection of values related to a user / friend.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace Photon
{
    /// <summary>
    /// Used to store info about a friend's online state and in which room he/she is.
    /// </summary>
    public class FriendInfo
    {
        public string Name { get; protected internal set; }
        public string Room { get; protected internal set; }
        
        private bool IsInRoom => this.IsOnline && !string.IsNullOrEmpty(this.Room);
        public bool IsOnline { get; protected internal set; }

        public override string ToString()
        {
            return string.Format("{0} is {1} in {2}.", 
                Name, 
                IsOnline ? "online" : "offline",
                IsInRoom ? "a room" : "lobby");
        }
    }
}

