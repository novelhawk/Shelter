using System.Linq;
using UnityEngine;

namespace Mod.Modules
{
    public class ModuleWind : Module
    {
        public override string ID => nameof(ModuleWind);
        public override string Name => "Enable Wind";
        public override string Description => "Adds wind post-processing.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;

        protected override void OnModuleEnable()
        {
            if (Player.Self.Properties.Alive == false || Player.Self.Hero == null)
                return;
                
            var windRenderer = Player.Self.Hero.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.name.Contains("speed"));
            if (windRenderer != null)
                windRenderer.enabled = true;
        }

        protected override void OnModuleDisable()
        {
            if (Player.Self.Properties.Alive == false || Player.Self.Hero == null)
                return;
            
            var windRenderer = Player.Self.Hero.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.name.Contains("speed"));
            if (windRenderer != null)
                windRenderer.enabled = false;
        }
    }
}