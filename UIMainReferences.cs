using System;
using System.Collections;
using System.IO;
using Mod;
using Mod.Interface;
using Mod.Logging;
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

            Application.RegisterLogCallback(UnityLogHandle);
            
            StartCoroutine(LoadRCAssets());
        }

        Shelter.OnMainMenu();
    }
    
    private void UnityLogHandle(string log, string stacktrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                Shelter.Log(log, LogLevel.Error);
                break;
            case LogType.Assert:
                Shelter.Log(log, LogLevel.Error);
                break;
            case LogType.Warning:
                if (!log.Contains("Behaviour is missing!"))
                    Shelter.Log(log, LogLevel.Warning);
                break;
            case LogType.Log:
                Shelter.Log(log);
                break;
            case LogType.Exception:
                Shelter.Log($"{log} {stacktrace}", LogLevel.Error);
                break;
            default:
                Shelter.Log($"LogLevel {type} does not exist.", LogLevel.Error);
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private static IEnumerator LoadRCAssets()
    {
        var url = "http://iishawk.it/AoTTG/RCAssets.unity3d";
        var file = Application.dataPath + "/RCAssets.unity3d";
        if (File.Exists(file))
            url = "file://" + file;
        
        while (!Caching.ready)
            yield return null;
        using (WWW www = WWW.LoadFromCacheOrDownload(url, 1))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("Error on WWW request (RCAssets):" + www.error);
            FengGameManagerMKII.RCassets = www.assetBundle;
        }
    }
}