namespace Mod.Modules
{
    public class ModuleInfiniteGas : Module
    {
        public override string Name => "Infinite Gas";
        public override string Description => "Makes the gas infinite.";
        public override bool IsAbusive => true;
        public override bool HasGUI => false;
    }
}