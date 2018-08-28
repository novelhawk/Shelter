using System;
using System.IO;
using System.Threading;
using Mod;
using UnityEngine;

public class UIMainReferences : MonoBehaviour
{
    public const string Version = "01042015";
    private static bool _done;
    
    private void Start()
    {
        if (!_done)
        {
            _done = true;
            
            float aspectRatio = (float) Screen.currentResolution.width / Screen.currentResolution.height;
            if (Mathf.Abs((float) Screen.width / Screen.height - aspectRatio) > float.Epsilon)
                Screen.SetResolution((int)(Screen.height * aspectRatio), Screen.height, Screen.fullScreen);
            
            GameObject go = new GameObject("Shelter");
            Shelter shelter = go.AddComponent<Shelter>();
            DontDestroyOnLoad(go);
            shelter.InitComponents();


            new Thread(() =>
            {
                LoadAssets("RCAssets");
            }).Start();
        }

        Shelter.OnMainMenu();
    }

    private static void LoadAssets(string asset)
    {
        using (var stream = Shelter.Assembly.GetManifestResourceStream($@"Mod.Resources.Assets.{asset}.unity3d"))
        {
            if (stream == null)
                throw new NullReferenceException("Cannot find resource"); // TODO: Log
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[8192];

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, bytesRead);
                
                ms.Flush();

                FengGameManagerMKII.RCassets = AssetBundle.CreateFromMemoryImmediate(ms.ToArray());
            }
        }
    }
}