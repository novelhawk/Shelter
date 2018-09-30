using System.Text;
using ExitGames.Client.Photon;
using Mod;
using UnityEngine;
using LogType = Mod.Logging.LogType;

internal static class CustomTypes
{
    private static object DeserializePhotonPlayer(byte[] bytes)
    {
        int num2;
        int offset = 0;
        Protocol.Deserialize(out num2, bytes, ref offset);
        if (PhotonNetwork.networkingPeer.mActors.ContainsKey(num2))
        {
            return PhotonNetwork.networkingPeer.mActors[num2];
        }
        return null;
    }

    private static object DeserializeQuaternion(byte[] bytes)
    {
        Quaternion quaternion = new Quaternion();
        int offset = 0;
        Protocol.Deserialize(out quaternion.w, bytes, ref offset);
        Protocol.Deserialize(out quaternion.x, bytes, ref offset);
        Protocol.Deserialize(out quaternion.y, bytes, ref offset);
        Protocol.Deserialize(out quaternion.z, bytes, ref offset);
        return quaternion;
    }

    private static object DeserializeVector2(byte[] bytes)
    {
        Vector2 vector = new Vector2();
        int offset = 0;
        Protocol.Deserialize(out vector.x, bytes, ref offset);
        Protocol.Deserialize(out vector.y, bytes, ref offset);
        return vector;
    }

    private static object DeserializeVector3(byte[] bytes)
    {
        Vector3 vector = new Vector3();
        int offset = 0;
        Protocol.Deserialize(out vector.x, bytes, ref offset);
        Protocol.Deserialize(out vector.y, bytes, ref offset);
        Protocol.Deserialize(out vector.z, bytes, ref offset);
        return vector;
    }

    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Vector2),       87, SerializeVector2,       DeserializeVector2);
        PhotonPeer.RegisterType(typeof(Vector3),       86, SerializeVector3,       DeserializeVector3);
        PhotonPeer.RegisterType(typeof(Vector4),       88, SerializeVector4,       DeserializeVector4);
        PhotonPeer.RegisterType(typeof(Quaternion),    81, SerializeQuaternion,    DeserializeQuaternion);
        PhotonPeer.RegisterType(typeof(Player),        80, SerializePhotonPlayer,  DeserializePhotonPlayer);
        PhotonPeer.RegisterType(typeof(StringBuilder), 84, SerializeStringBuilder, DeserializeStringBuilder);
    }

    private static byte[] SerializeStringBuilder(object obj)
    {
        var builder = (StringBuilder) obj;
        Shelter.LogBoth("We serialized a StringBuilder. That should never happen. Content: {0}", LogType.Warning, builder);
        
        byte[] target = new byte[builder.Length];
        Encoding.UTF8.GetBytes(builder.ToString()).CopyTo(target, 0);
        return target;
    }

    private static object DeserializeStringBuilder(byte[] bytes)
    {
        Shelter.LogBoth("We deserialized a StringBuilder. That should never happen.", LogType.Warning);
        
        string str;
        Shelter.Log("Content: {0}", LogType.Warning, str = Encoding.UTF8.GetString(bytes));
        return new StringBuilder(str);
    }

    private static object DeserializeVector4(byte[] serializedcustomobject)
    {
        return Vector4.zero;
    }

    private static byte[] SerializeVector4(object customobject)
    {
        Vector4 vector = (Vector4) customobject;
        int targetOffset = 0;
        byte[] target = new byte[4 * 4];
        Protocol.Serialize(vector.x, target, ref targetOffset);
        Protocol.Serialize(vector.y, target, ref targetOffset);
        Protocol.Serialize(vector.z, target, ref targetOffset);
        Protocol.Serialize(vector.w, target, ref targetOffset);
        return target;
    }

    private static byte[] SerializePhotonPlayer(object customobject)
    {
        int iD = ((Player) customobject).ID;
        byte[] target = new byte[4];
        int targetOffset = 0;
        Protocol.Serialize(iD, target, ref targetOffset);
        return target;
    }

    private static byte[] SerializeQuaternion(object obj)
    {
        Quaternion quaternion = (Quaternion) obj;
        byte[] target = new byte[16];
        int targetOffset = 0;
        Protocol.Serialize(quaternion.w, target, ref targetOffset);
        Protocol.Serialize(quaternion.x, target, ref targetOffset);
        Protocol.Serialize(quaternion.y, target, ref targetOffset);
        Protocol.Serialize(quaternion.z, target, ref targetOffset);
        return target;
    }

    private static byte[] SerializeVector2(object customobject)
    {
        Vector2 vector = (Vector2) customobject;
        byte[] target = new byte[8];
        int targetOffset = 0;
        Protocol.Serialize(vector.x, target, ref targetOffset);
        Protocol.Serialize(vector.y, target, ref targetOffset);
        return target;
    }

    private static byte[] SerializeVector3(object customobject)
    {
        Vector3 vector = (Vector3) customobject;
        int targetOffset = 0;
        byte[] target = new byte[12];
        Protocol.Serialize(vector.x, target, ref targetOffset);
        Protocol.Serialize(vector.y, target, ref targetOffset);
        Protocol.Serialize(vector.z, target, ref targetOffset);
        return target;
    }
}

