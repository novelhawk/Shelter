using ExitGames.Client.Photon;
using System;
using System.Net.Sockets;
using UnityEngine;

public class PingMonoEditor : PhotonPing
{
    private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    public override void Dispose()
    {
        try
        {
            this.sock.Close();
        }
        catch
        {
        }
        this.sock = null;
    }

    public override bool Done()
    {
        if (!GotResult && this.sock != null)
        {
            if (this.sock.Available <= 0)
            {
                return false;
            }
            int num = this.sock.Receive(PingBytes, SocketFlags.None);
            if (PingBytes[PingBytes.Length - 1] != PingId || num != PingLength)
            {
                Debug.Log("ReplyMatch is false! ");
            }
            Successful = num == PingBytes.Length && PingBytes[PingBytes.Length - 1] == PingId;
            GotResult = true;
        }
        return true;
    }

    public override bool StartPing(string ip)
    {
        Init();
        try
        {
            this.sock.ReceiveTimeout = 5000;
            this.sock.Connect(ip, 5055);
            PingBytes[PingBytes.Length - 1] = PingId;
            this.sock.Send(PingBytes);
            PingBytes[PingBytes.Length - 1] = (byte) (PingId - 1);
        }
        catch (Exception exception)
        {
            this.sock = null;
            Console.WriteLine(exception);
        }
        return false;
    }
}

