// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System.Text;
using ExitGames.Client.Photon;
using Mod;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Photon
{
    /// <summary>
    /// Internally used class, containing de/serialization methods for various Unity-specific classes.
    /// Adding those to the Photon serialization protocol allows you to send them in events, etc.
    /// </summary>
    internal static class CustomTypes
    {
        internal static void Register()
        {
            PhotonPeer.RegisterType(typeof(Vector2),       87, SerializeVector2,       DeserializeVector2);
            PhotonPeer.RegisterType(typeof(Vector3),       86, SerializeVector3,       DeserializeVector3);
            PhotonPeer.RegisterType(typeof(Vector4),       88, SerializeVector4,       DeserializeVector4);
            PhotonPeer.RegisterType(typeof(Player),        80, SerializePhotonPlayer,  DeserializePhotonPlayer);
            PhotonPeer.RegisterType(typeof(Quaternion),    81, SerializeQuaternion,    DeserializeQuaternion);
            PhotonPeer.RegisterType(typeof(StringBuilder), 84, SerializeStringBuilder, DeserializeStringBuilder);
        }

        #region Vector2

        private static byte[] SerializeVector2(object obj)
        {
            Vector2 vector = (Vector2) obj;
            byte[] target = new byte[sizeof(float) * 2];
            int targetOffset = 0;
            Protocol.Serialize(vector.x, target, ref targetOffset);
            Protocol.Serialize(vector.y, target, ref targetOffset);
            return target;
        }

        private static object DeserializeVector2(byte[] bytes)
        {
            Vector2 vector = new Vector2();
            if (bytes.Length < sizeof(float) * 2)
            {
                Shelter.LogBoth("Malformed data received. Vector2 with {0} bytes", LogType.Error, bytes.Length);
                return Vector2.zero;
            }
            int offset = 0;
            Protocol.Deserialize(out vector.x, bytes, ref offset);
            Protocol.Deserialize(out vector.y, bytes, ref offset);
            return vector;
        }

        #endregion

        #region Vector3

        private static byte[] SerializeVector3(object obj)
        {
            Vector3 vector = (Vector3) obj;
            int targetOffset = 0;
            byte[] target = new byte[sizeof(float) * 3];
            Protocol.Serialize(vector.x, target, ref targetOffset);
            Protocol.Serialize(vector.y, target, ref targetOffset);
            Protocol.Serialize(vector.z, target, ref targetOffset);
            return target;
        }

        private static object DeserializeVector3(byte[] bytes)
        {
            Vector3 vector = new Vector3();
            if (bytes.Length < sizeof(float) * 3)
            {
                Shelter.LogBoth("Malformed data received. Vector3 with {0} bytes", LogType.Error, bytes.Length);
                return Vector3.zero;
            }
            int offset = 0;
            Protocol.Deserialize(out vector.x, bytes, ref offset);
            Protocol.Deserialize(out vector.y, bytes, ref offset);
            Protocol.Deserialize(out vector.z, bytes, ref offset);
            return vector;
        }

        #endregion

        #region Vector4

        private static byte[] SerializeVector4(object obj)
        {
            Vector4 vector = (Vector4) obj;
            int targetOffset = 0;
            byte[] target = new byte[sizeof(float) * 4];
            Protocol.Serialize(vector.x, target, ref targetOffset);
            Protocol.Serialize(vector.y, target, ref targetOffset);
            Protocol.Serialize(vector.z, target, ref targetOffset);
            Protocol.Serialize(vector.w, target, ref targetOffset);
            return target;
        }

        private static object DeserializeVector4(byte[] bytes)
        {
            Shelter.LogBoth("We deserialized a Vector4. That should never happen ({0} bytes)", LogType.Warning, bytes.Length);
            Vector4 vector = new Vector4();
            if (bytes.Length < sizeof(float) * 4)
            {
                Shelter.LogBoth("Malformed data received. Vector4 with {0} bytes", LogType.Error, bytes.Length);
                return Vector3.zero;
            }
            int offset = 0;
            Protocol.Deserialize(out vector.x, bytes, ref offset);
            Protocol.Deserialize(out vector.y, bytes, ref offset);
            Protocol.Deserialize(out vector.z, bytes, ref offset);
            Protocol.Deserialize(out vector.w, bytes, ref offset);
            return vector;
        }

        #endregion

        #region PhotonPlayer

        private static byte[] SerializePhotonPlayer(object obj)
        {
            int id = ((Player) obj).ID;
            byte[] target = new byte[sizeof(int)];
            int targetOffset = 0;
            Protocol.Serialize(id, target, ref targetOffset);
            return target;
        }

        private static object DeserializePhotonPlayer(byte[] bytes)
        {
            if (bytes.Length < sizeof(int))
            {
                Shelter.LogBoth("Malformed data received. PhotonPlayer with {0} bytes", LogType.Error, bytes.Length);
                return Vector2.zero;
            }
            int offset = 0;
            Protocol.Deserialize(out int id, bytes, ref offset);
            if (PhotonNetwork.networkingPeer.mActors.ContainsKey(id))
                return PhotonNetwork.networkingPeer.mActors[id];
            return null;
        }

        #endregion

        #region Quaternion

        private static byte[] SerializeQuaternion(object obj)
        {
            Quaternion quaternion = (Quaternion) obj;
            byte[] target = new byte[sizeof(float) * 4];
            int targetOffset = 0;
            Protocol.Serialize(quaternion.w, target, ref targetOffset);
            Protocol.Serialize(quaternion.x, target, ref targetOffset);
            Protocol.Serialize(quaternion.y, target, ref targetOffset);
            Protocol.Serialize(quaternion.z, target, ref targetOffset);
            return target;
        }

        private static object DeserializeQuaternion(byte[] bytes)
        {
            Quaternion quaternion = new Quaternion();
            if (bytes.Length < sizeof(float) * 4)
            {
                Shelter.LogBoth("Malformed data received. Quaternion with {0} bytes", LogType.Error, bytes.Length);
                return Vector2.zero;
            }
            int offset = 0;
            Protocol.Deserialize(out quaternion.w, bytes, ref offset);
            Protocol.Deserialize(out quaternion.x, bytes, ref offset);
            Protocol.Deserialize(out quaternion.y, bytes, ref offset);
            Protocol.Deserialize(out quaternion.z, bytes, ref offset);
            return quaternion;
        }

        #endregion

        #region StringBuilder

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
        
            string str = Encoding.UTF8.GetString(bytes);
            Shelter.Log("Content: {0}", LogType.Warning, str);
            return new StringBuilder(str);
        }

        #endregion
    }
}

