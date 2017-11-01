using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mod
{
    public class Room
    {
        private readonly string _roomFullName;
        private readonly string _roomName;
        private readonly string _roomMap;
        private readonly bool _isProtected;
        private readonly int _currentPlayers;
        private readonly int _maxPlayers;

        public Room(string roomFullName, bool isPasswordProtected, int currentPlayers, int maxPlayers)
        {
            _roomFullName = roomFullName;
            _roomName = _roomFullName.Split('`')[0];
            _roomMap = _roomFullName.Split('`')[1];
            _isProtected = isPasswordProtected;
            _currentPlayers = currentPlayers;
            _maxPlayers = maxPlayers;
        }

        public static List<Room> List => PhotonNetwork.GetRoomList().Select(room => new Room(room.name, room.name.Split('`')[5] != string.Empty, room.playerCount, room.maxPlayers)).ToList();
        public static readonly Func<List<Room>, List<Room>> GetOrdinatedList = list =>
        {
            //TODO: Change it with IComparable
            var value = list.Where(room => room.IsJoinable && !room.IsProtected).ToList();
            value.AddRange(list.Where(room => room.IsProtected && room.IsJoinable));
            value.AddRange(list.Where(room => room.IsProtected && !room.IsJoinable));
            value.AddRange(list.Where(room => !room.IsJoinable && !room.IsProtected));
            return value;
        };

        public bool Join() => PhotonNetwork.JoinRoom(_roomFullName);
        public string ToString(int alpha)
        {
            var aa = alpha.ToString("X2");
            StringBuilder str = new StringBuilder();
            str.Append("<color=#5D334B" + aa + ">");
            if (IsProtected)
                str.Append("<color=#034C94" + aa + ">[</color><color=#1191D1" + aa + ">PW</color><color=#034C94" + aa + ">]</color> ");
            str.Append(RoomName.RemoveColors());
            str.Append(" || ");
            str.Append(Map);
            str.Append(" || ");
            str.Append(IsJoinable ? "<color=#00FF00" + aa + ">" : "<color=#FF0000" + aa + ">");
            str.Append(CurrentPlayers + "/" + MaxPlayers);
            str.Append("</color></color>");
            return str.ToString();
        }
        public string RoomName => _roomName;
        public string Map => _roomMap;
        public bool IsJoinable => _maxPlayers == 0 || _currentPlayers < _maxPlayers;
        public bool IsProtected => _isProtected;
        public int MaxPlayers => _maxPlayers;
        public int CurrentPlayers => _currentPlayers;
    }
}
