namespace Mod.Modules
{
    public class ModuleDisableTP : Module
    {
        public override string Name => "Disable TP";
        public override string Description => "Prevents MC from teleporting you.";
        public override bool IsAbusive => true;
        public override bool HasGUI => false;
    }
}