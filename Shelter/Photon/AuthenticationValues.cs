using System;

namespace Photon
{
    public class AuthenticationValues
    {
        public string AuthParameters;
        public CustomAuthenticationType AuthType;
        public string Secret;

        public void SetAuthParameters(string user, string token)
        {
            this.AuthParameters = "username=" + Uri.EscapeDataString(user) + "&token=" + Uri.EscapeDataString(token);
        }

        public void SetAuthPostData(string stringData)
        {
            this.AuthPostData = !string.IsNullOrEmpty(stringData) ? stringData : null;
        }

        public void SetAuthPostData(byte[] byteData)
        {
            this.AuthPostData = byteData;
        }

        public override string ToString()
        {
            return this.AuthParameters + " s: " + this.Secret;
        }

        public object AuthPostData { get; private set; }
    }
}

