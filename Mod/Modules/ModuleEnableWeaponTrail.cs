namespace Mod.Modules
{
    public class ModuleEnableWeaponTrail : Module
    {
        public override string ID => nameof(ModuleEnableWeaponTrail);
        public override string Name => "Enable Weapon Trail";
        public override string Description => "Shows a trail behind the blades.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}