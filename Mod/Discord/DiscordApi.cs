//using DiscordRPC;
//using DiscordRPC.IO;
//using DiscordRPC.Logging;
//
//namespace Mod.Discord
//{
//    public class DiscordRpc
//    {
//        private readonly DiscordRpcClient _client;
//        
//        public DiscordRpc()
//        {
//            _client = new DiscordRpcClient("378900623875244042", null, false, -1, new NativeNamedPipeClient());
//
//            System.IO.File.WriteAllBytes("discord-rpc.log", new byte[0]);
//            _client.Logger = new FileLogger("discord-rpc.log");
//            
//            _client.OnReady += (sender, args) => UnityEngine.Debug.Log("OnReady");
//            _client.OnError += (sender, args) => UnityEngine.Debug.Log("OnError");
//            _client.OnClose += (sender, args) => UnityEngine.Debug.Log("OnClose");
//            
//            _client.OnConnectionFailed += (sender, args) => UnityEngine.Debug.Log("OnConnectionFailed");
//            _client.OnConnectionEstablished += (sender, args) => UnityEngine.Debug.Log("OnConnectionEstablished");
//            
//            _client.OnPresenceUpdate += (sender, args) => UnityEngine.Debug.Log($"OnPresenceUpdate {args.Presence}");
//            
//            _client.OnSubscribe += (sender, args) => UnityEngine.Debug.Log("OnSubscribe");
//            _client.OnUnsubscribe += (sender, args) => UnityEngine.Debug.Log("OnUnsubscribe");
//            
//            _client.OnJoin += (sender, args) => UnityEngine.Debug.Log("OnJoin");
//            _client.OnSpectate += (sender, args) => UnityEngine.Debug.Log("OnSpectate");
//            _client.OnJoinRequested += (sender, args) => UnityEngine.Debug.Log("OnJoinRequested");
//            
//            _client.SetPresence(new RichPresence
//            {
//                Details = "TEST1",
//                State = "TEST2"
//            });
//
//            _client.Initialize();
//        }
//
//        public void Update()
//        {
//            _client?.Invoke();
//        }
//    }
//}
//
