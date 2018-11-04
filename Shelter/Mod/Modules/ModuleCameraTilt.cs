namespace Mod.Modules
{
    public class ModuleCameraTilt : Module
    {
        public override string Name => "Camera Tilt";
        public override string Description => "Enables Camera Tilt while hooking an object.";
        public override bool IsAbusive => false;
        public override bool HasGUI => false;
    }
}