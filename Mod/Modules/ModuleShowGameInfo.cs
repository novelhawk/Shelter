using Mod.Interface;

namespace Mod.Modules
{
    public class ModuleShowGameInfo : Module
    {
        public override string ID => nameof(ModuleRacingInterface);
        public override string Name => "Show GameInfo";
        public override string Description => "Shows the top right GameInfo panel.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false; //TODO: Add custom infos selection

        protected override void OnModuleEnable()
        {
            if (PhotonNetwork.inRoom)
                Shelter.InterfaceManager.Enable(nameof(GameInfo));
        }

        protected override void OnModuleDisable()
        {
            Shelter.InterfaceManager.Disable(nameof(GameInfo));
        }
    }
}