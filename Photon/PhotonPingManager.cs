using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using ExitGames.Client.Photon;
using Mod;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Photon
{
    public class PhotonPingManager
    {
        private const int Attempts = 5;
        private const int MaxMilliseconsPerPing = 800; // enter a value you're sure some server can beat (have a lower rtt)

        public static Region BestRegion
        {
            get
            {
                Region result = null;
                int bestRtt = int.MaxValue;
                foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
                {
                    if (region.Ping != 0 && region.Ping < bestRtt)
                    {
                        bestRtt = region.Ping;
                        result = region;
                    }
                }

                return result;
            }
        }

        public bool Done => this.PingsRunning == 0;
        private int PingsRunning;

        /// <remarks>
        /// Affected by frame-rate of app, as this Coroutine checks the socket for a result once per frame.
        /// </remarks>
        public IEnumerator PingSocket(Region region)
        {
            region.Ping = Attempts * MaxMilliseconsPerPing;

            this.PingsRunning++; // TODO: Add try-catch to make sure the PingsRunning are reduced at the end and that the lib does not crash the app
            PhotonPing ping;
            if (PhotonHandler.PingImplementation == typeof(PingNativeDynamic))
            {
                Shelter.Log("Using constructor for new PingNativeDynamic()"); // it seems on android, the Activator can't find the default Constructor
                ping = new PingNativeDynamic();
            }
            else if (PhotonHandler.PingImplementation == typeof(PingMono))
            {
                ping = new PingMono(); // using this type explicitly saves it from IL2CPP bytecode stripping
            }
#if UNITY_WEBGL
        else if (PhotonHandler.PingImplementation == typeof(PingHttp))
        {
            ping = new PingHttp();
        }
        #endif
            else
            {
                ping = (PhotonPing)Activator.CreateInstance(PhotonHandler.PingImplementation);
            }

            float rttSum = 0.0f;
            int replyCount = 0;

            // all addresses for Photon region servers will contain a :port ending. this needs to be removed first.
            // PhotonPing.StartPing() requires a plain (IP) address without port or protocol-prefix (on all but Windows 8.1 and WebGL platforms).

            string regionAddress = region.HostAndPort;
            int indexOfColon = regionAddress.LastIndexOf(':');
            if (indexOfColon > 1)
                regionAddress = regionAddress.Substring(0, indexOfColon);

            regionAddress = ResolveHost(regionAddress);

            for (int i = 0; i < Attempts; i++)
            {
                bool overtime = false;
                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    ping.StartPing(regionAddress);
                }
                catch (Exception e)
                {
                    Shelter.LogBoth("Exception {0} on pinging {1}.", LogType.Error, e.GetType().Name, regionAddress);
                    Shelter.Log("{0}: {1}", LogType.Error, e.GetType().Name, e.Message);
                    this.PingsRunning--;
                    break;
                }


                while (!ping.Done())
                {
                    if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
                    {
                        overtime = true;
                        break;
                    }
                    yield return 0; // keep this loop tight, to avoid adding local lag to rtt.
                }
                int rtt = (int)sw.ElapsedMilliseconds;
                
                if (i != 0 && ping.Successful && !overtime)
                {
                    rttSum += rtt;
                    replyCount++;
                    region.Ping = (int)((rttSum) / replyCount);
                    //Debug.Log("region " + region.Code + " RTT " + region.Ping + " success: " + ping.Successful + " over: " + overtime);
                }

                yield return new WaitForSeconds(0.1f);
            }

            this.PingsRunning--;

            //Debug.Log("this.PingsRunning: " + this.PingsRunning + " this debug: " + ping.DebugString);
            yield return null;
        }

        /// <summary>
        /// Attempts to resolve a hostname into an IP string or returns empty string if that fails.
        /// </summary>
        /// <param name="hostName">Hostname to resolve.</param>
        /// <returns>IP string or empty string if resolution fails</returns>
        private static string ResolveHost(string hostName)
        {
            string ipv4Address = string.Empty;

            try
            {
                IPAddress[] address = Dns.GetHostAddresses(hostName);

                if (address.Length == 1)
                    return address[0].ToString();

                // if we got more addresses, try to pick a IPv4 one
                foreach (var ipAddress in address)
                {
                    if (ipAddress != null)
                    {
                        // checking ipAddress.ToString() means we don't have to import System.Net.Sockets, which is not available on some platforms (Metro)
                        if (ipAddress.ToString().Contains(":"))
                        {
                            return ipAddress.ToString();
                        }
                        if (string.IsNullOrEmpty(ipv4Address))
                        {
                            ipv4Address = address.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shelter.LogBoth("Couldn't resolve hostname {0} (Exception {1} caught)", LogType.Error, hostName, e.GetType().Name);
            }

            return ipv4Address;
        }
    }
}