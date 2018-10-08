namespace Mod.Modules
{
    public class ModuleWeaponTrail : Module
    {
        public override string ID => nameof(ModuleWeaponTrail);
        public override string Name => "Enable Weapon Trail";
        public override string Description => "Shows a trail behind the blades.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}