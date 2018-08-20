namespace Mod.Modules
{
    public class ModuleNameScaling : Module
    {
        public override string ID => nameof(ModuleNameScaling);
        public override string Name => "Scale Names";
        public override string Description => "Scales name size based on the distance";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}