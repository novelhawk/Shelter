using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class WebRpcResponse
{
    public WebRpcResponse(OperationResponse response)
    {
        object obj2;
        response.Parameters.TryGetValue(209, out obj2);
        this.Name = obj2 as string;
        response.Parameters.TryGetValue(207, out obj2);
        this.ReturnCode = (obj2 == null) ? -1 : ((byte) obj2);
        response.Parameters.TryGetValue(208, out obj2);
        this.Parameters = obj2 as Dictionary<string, object>;
        response.Parameters.TryGetValue(206, out obj2);
        this.DebugMessage = obj2 as string;
    }

    public string ToStringFull()
    {
        object[] args = new object[] { this.Name, SupportClass.DictionaryToString(this.Parameters), this.ReturnCode, this.DebugMessage };
        return string.Format("{0}={2}: {1} \"{3}\"", args);
    }

    public string DebugMessage { get; private set; }

    public string Name { get; private set; }

    public Dictionary<string, object> Parameters { get; private set; }

    public int ReturnCode { get; private set; }
}

