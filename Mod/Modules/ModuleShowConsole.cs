using Mod.Interface;

namespace Mod.Modules
{
    public class ModuleShowConsole : Module
    {
        public override string ID => nameof(ModuleShowConsole);
        public override string Name => "Show Console";
        public override string Description => "Shows the in-game console lower right border of the screen.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;

        protected override void OnModuleEnable()
        {
            if (PhotonNetwork.inRoom)
                Shelter.InterfaceManager.Enable(nameof(Console));
        }

        protected override void OnModuleDisable()
        {
            Shelter.InterfaceManager.Disable(nameof(Console));
        }
    }
}