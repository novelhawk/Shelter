namespace Mod.Modules
{
    public class ModuleWind : Module
    {
        public override string ID => nameof(ModuleWind);
        public override string Name => "Enable Wind";
        public override string Description => "Adds wind post-processing.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}