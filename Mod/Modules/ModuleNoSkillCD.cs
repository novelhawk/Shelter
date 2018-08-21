namespace Mod.Modules
{
    public class ModuleNoSkillCD : Module
    {
        public override string ID => nameof(ModuleNoSkillCD);
        public override string Name => "No skill CD";
        public override string Description => "Disables the countdown on using the character special";
        public override bool IsAbusive => true;
        public override bool HasGUI => false;
    }
}