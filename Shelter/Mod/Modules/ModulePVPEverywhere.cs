namespace Mod.Modules
{
    public class ModulePVPEverywhere : Module
    {
        public override string Name => "Enable PvP";
        public override string Description => "Enables PvP without MC's authorization.";
        public override bool IsAbusive => true;
        public override bool HasGUI => false;
    }
}