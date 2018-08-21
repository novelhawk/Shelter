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
            handlers.readyCallback += () =>
            {
                Debug.Log("readyCallback");
            };
            handlers.disconnectedCallback += (a, b) =>
            {
                Debug.Log("disconnectedCallback");
            };
            handlers.errorCallback += (a, b) =>
            {
                Debug.Log($"errorCallback({a}, {b})");
            };
            DiscordApi.Initialize("378900623875244042", ref handlers, true, null);
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