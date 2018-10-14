namespace Mod.Modules
{
    public class ModuleSnapshot : Module // TODO: Add minimum damage required
    {
        public override string Name => "Kill Screenshot";
        public override string Description => "Takes a screenshot on titan kill.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}