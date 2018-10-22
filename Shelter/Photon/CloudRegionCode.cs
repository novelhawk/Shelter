// ----------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

namespace Photon
{
    /// <summary>Currently available <a href="https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/regions">Photon Cloud regions</a> as enum.</summary>
    /// <remarks>
    /// This is used in PhotonNetwork.ConnectToRegion.
    /// </remarks>
    public enum CloudRegionCode
    {
        /// <summary>European servers in Amsterdam.</summary>
        Europe = 0,
        /// <summary>US servers (East Coast).</summary>
        UsEast = 1,
        /// <summary>Asian servers in Singapore.</summary>
        Asia = 2,
        /// <summary>Japanese servers in Tokyo.</summary>
        Japan = 3,
        /// <summary>Australian servers in Melbourne.</summary>
        Australia = 5,
        ///<summary>USA West, San José, usw</summary>
        UsWest = 6,
        ///<summary>South America, Sao Paulo, sa</summary>
        SouthAmerica = 7,
        ///<summary>Canada East, Montreal, cae</summary>
        CanadaEast = 8,
        ///<summary>South Korea, Seoul, kr</summary>
        SouthKorea = 9,
        ///<summary>India, Chennai, in</summary>
        India = 10,
        /// <summary>Russia, ru</summary>
        Russia = 11,
        /// <summary>Russia East, rue</summary>
        RussiaEast = 12,

        /// <summary>No region selected.</summary>
        None = 4
    };
}

