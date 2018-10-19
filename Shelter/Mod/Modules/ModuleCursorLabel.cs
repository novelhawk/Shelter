using UnityEngine;

namespace Mod.Modules
{
    public class ModuleCursorLabel : Module
    {
        public override string Name => "Custom Cursor Label";
        public override string Description => "Allows direct modification of the cursor's underlying label.";
        public override bool IsAbusive => false;
        public override bool HasGUI => true;

        private const string Default = "{0:0}";
        public static string LabelFormat = Default;

        protected override void OnModuleEnable()
        {
            LabelFormat = PlayerPrefs.GetString(PlayerPref + ".format", Default);
        }

        protected override void OnModuleDisable()
        {
            LabelFormat = Default;
        }

        public override void Render(Rect windowRect)
        {
            var format = LabelFormat;
            
            GUILayout.BeginArea(windowRect);
            GUILayout.Label("{0} is distance, {1} is speed (u/m), {2} is damage (K)");
            format = GUILayout.TextField(format);
            if (GUILayout.Button("Premade: Speed"))
                format = "{0:0}\n{1:F1} u/s";
            if (GUILayout.Button("Premade: Damage"))
                format = "{0:0}\n{2:F1} K";
            GUILayout.EndArea();

            if (format != LabelFormat)
            {
                LabelFormat = format;
                PlayerPrefs.SetString(PlayerPref + ".format", format);
            }
        }
    }
}