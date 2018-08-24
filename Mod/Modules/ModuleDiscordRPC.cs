using Mod.Discord;
using UnityEngine;

namespace Mod.Modules
{
    public class ModuleDiscordRPC : Module
    {
        public override string ID => nameof(ModuleDiscordRPC);
        public override string Name => "Discord RichPresence";
        public override string Description => "Custom aottg status in discord.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;

        protected override void OnModuleEnable()
        {
            var handlers = new DiscordApi.EventHandlers();
            handlers.readyCallback += () => {};
            handlers.disconnectedCallback += (a, b) => { }; 
            handlers.errorCallback += (a, b) => { Debug.Log(b); };
            handlers.joinCallback += (a) =>{};
            handlers.requestCallback += (ref DiscordApi.JoinRequest a) => {};
            handlers.spectateCallback += (a) => { };
            
            DiscordApi.Initialize("378900623875244042", ref handlers, true, null);
        }

        private void OnReady()
        {
            Debug.Log("Discord ready!");
        }

        private void OnDisconnected(int code, string message)
        {
            Debug.Log($"Disconnected from rich presence");
        }

        private void OnError(int code, string message)
        {
            Debug.Log($"Error with rich presence");
        }

        private void OnJoin(string secret)
        {
        }

        private void OnJoinRequest(ref DiscordApi.JoinRequest request)
        {
        }

        private void OnSpectate(string secret)
        {
        }

        protected override void OnModuleUpdate()
        {
            DiscordApi.RunCallbacks();
        }

        protected override void OnModuleDisable()
        {
            DiscordApi.Shutdown();
        }

        private void OnDisable()
        {
            DiscordApi.Shutdown();
        }
    }
}