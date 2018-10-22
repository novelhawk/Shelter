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

using System;

namespace Photon
{
    /// <summary>
    /// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
    /// </summary>
    /// <remarks>
    /// On Photon, user authentication is optional but can be useful in many cases.
    /// If you want to FindFriends, a unique ID per user is very practical.
    ///
    /// There are basically three options for user authentification: None at all, the client sets some UserId
    /// or you can use some account web-service to authenticate a user (and set the UserId server-side).
    ///
    /// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
    /// values to Photon which will verify them before granting access or disconnecting the client.
    ///
    /// The AuthValues are sent in OpAuthenticate when you connect, so they must be set before you connect.
    /// Should you not set any AuthValues, PUN will create them and set the playerName as userId in them.
    /// If the AuthValues.userId is null or empty when it's sent to the server, then the Photon Server assigns a userId!
    ///
    /// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
    /// https://www.photonengine.com/dashboard
    /// </remarks>
    public class AuthenticationValues
    {
        /// <summary>The type of custom authentication provider that should be used. Currently only "Custom" or "None" (turns this off).</summary>
        public CustomAuthenticationType AuthType;
        
        /// <summary>This string must contain any (http get) parameters expected by the used authentication service. By default, username and token.</summary>
        /// <remarks>Standard http get parameters are used here and passed on to the service that's defined in the server (Photon Cloud Dashboard).</remarks>
        public string AuthParameters;
        
        /// <summary>After initial authentication, Photon provides a token for this client / user, which is subsequently used as (cached) validation.</summary>
        public string Secret;
        
        /// <summary>Data to be passed-on to the auth service via POST. Default: null (not sent). Either string or byte[] (see setters).</summary>
        public object AuthPostData { get; private set; }

        public void SetAuthParameters(string user, string token)
        {
            this.AuthParameters = "username=" + Uri.EscapeDataString(user) + "&token=" + Uri.EscapeDataString(token);
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary. Each SetAuthPostData replaces any previous value.</remarks>
        /// <param name="stringData">String data to be used in the body of the POST request. Null or empty string will set AuthPostData to null.</param>
        public void SetAuthPostData(string stringData)
        {
            this.AuthPostData = !string.IsNullOrEmpty(stringData) ? stringData : null;
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary. Each SetAuthPostData replaces any previous value.</remarks>
        /// <param name="byteData">Binary token / auth-data to pass on.</param>
        public void SetAuthPostData(byte[] byteData)
        {
            this.AuthPostData = byteData;
        }

        public override string ToString()
        {
            return this.AuthParameters + " s: " + this.Secret;
        }
    }
}

