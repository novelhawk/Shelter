namespace Mod.Modules
{
    public class ModuleTiltShift : Module
    {
        public override string Name => "Enable Tilt Shift";
        public override string Description => "Enables a motion blur like effect.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;

        protected override void OnModuleEnable()
        {
            if (Shelter.TryFind("MainCamera", out var mainCamera))
                mainCamera.GetComponent<TiltShift>().enabled = true;
        }
        
        protected override void OnModuleDisable()
        {
            if (Shelter.TryFind("MainCamera", out var mainCamera))
                mainCamera.GetComponent<TiltShift>().enabled = false;
        }
    }
}