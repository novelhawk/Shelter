using Mod.Interface;

namespace Mod.Modules
{
    public class ModuleShowMap : Module
    {
        public override string Name => "Show Minimap";
        public override string Description => "Shows a top-right map with humans and titans icons.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;

        protected override void OnModuleEnable()
        {
            if (!FengGameManagerMKII.settings.IsMapAllowed)
            {
                Notify.New("Map is globally disabled by MC", 5000);
                Disable();
                return;
            }
            
            IN_GAME_MAIN_CAMERA.instance.CreateMinimap();
        }

        protected override void OnModuleDisable()
        {
            if (Minimap.instance != null)
                Minimap.instance.Dispose();
        }
    }
}