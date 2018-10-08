namespace Mod.Modules
{
    public class ModuleEnableSkins : Module // TODO: Split Human, Titan and Level skin enables (With the GUI)
    {
        public override string ID => nameof(ModuleEnableSkins);
        public override string Name => "Enable Skins";
        public override string Description => "Enables players and map reskin.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}