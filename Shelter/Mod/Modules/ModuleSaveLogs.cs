namespace Mod.Modules
{
    public class ModuleSaveLogs : Module
    {
        public override string Name => "Save Logs";
        public override string Description => "Keeps a copy of former logs in the /Logs/ directory.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}