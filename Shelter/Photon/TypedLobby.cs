// ----------------------------------------------------------------------------
// <copyright file="LoadBalancingPeer.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Provides operations to use the LoadBalancing and Cloud photon servers.
//   No logic is implemented here.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace Photon
{
    /// <summary>Refers to a specific lobby (and type) on the server.</summary>
    /// <remarks>
    /// The name and type are the unique identifier for a lobby.<br/>
    /// Join a lobby via PhotonNetwork.JoinLobby(TypedLobby lobby).<br/>
    /// The current lobby is stored in PhotonNetwork.lobby.
    /// </remarks>
    public class TypedLobby
    {
        public static TypedLobby Default => new TypedLobby();
        
        private readonly string _name;
        private readonly LobbyType _type;

        private TypedLobby()
        {
            _name = string.Empty;
            _type = LobbyType.Default;
        }

        public TypedLobby(string name, LobbyType type)
        {
            _name = name;
            _type = type;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("Lobby '{0}'[{1}]", _name, _type);

        /// <summary>Name of the lobby this game gets added to. Default: null, attached to default lobby. Lobbies are unique per lobbyName plus lobbyType, so the same name can be used when several types are existing.</summary>
        public string Name => _name;
        
        /// <summary>Type of the (named)lobby this game gets added to</summary>
        public LobbyType Type => _type;

        /// <summary>True if the lobby has all default values</summary>
        public bool IsDefault => _type == LobbyType.Default && string.IsNullOrEmpty(_name);
    }
}

