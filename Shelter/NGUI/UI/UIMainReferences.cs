using System;
using System.Collections;
using System.IO;
using Mod;
using UnityEngine;
using LogType = Mod.Logging.LogType;

// ReSharper disable once CheckNamespace
public class UIMainReferences : MonoBehaviour
{
    private const string RCAssetsUrl = @"http://iishawk.it/AoTTG/RCAssets.unity3d";
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
            go.AddComponent<Shelter>();
            DontDestroyOnLoad(go);
            
            Shelter.InitComponents();

            Application.RegisterLogCallback(UnityLogHandle);
            
            StartCoroutine(LoadRCAssets());
        }

        Shelter.OnMainMenu();
    }
    
    private void UnityLogHandle(string log, string stacktrace, UnityEngine.LogType type)
    {
        if (log == "The referenced script on this Behaviour is missing!")
            return;
        
        switch (type)
        {
            case UnityEngine.LogType.Error:
                Shelter.Log(log, LogType.Error);
                break;
            case UnityEngine.LogType.Assert:
                Shelter.Log(log, LogType.Error);
                break;
            case UnityEngine.LogType.Warning:
                Shelter.Log(log, LogType.Warning);
                break;
            case UnityEngine.LogType.Log:
                Shelter.Log(log);
                break;
            case UnityEngine.LogType.Exception:
                Shelter.Log($"{log} {stacktrace}", LogType.Error);
                break;
            default:
                Shelter.Log($"LogLevel {type} does not exist.", LogType.Error);
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private static IEnumerator LoadRCAssets()
    {
        var download = $"{Application.dataPath}/RCAssets.unity3d";
        if (File.Exists(download))
            download = "file://" + download;
        else
        {
            Shelter.LogBoth("RC Assets were not found. Downloading from {0}", LogType.Warning, RCAssetsUrl);
            Shelter.LogBoth("(If this doesn't work download it yourself and move it to {0} folder)", LogType.Info, Application.dataPath);
            download = RCAssetsUrl;
        }
        
        while (!Caching.ready)
            yield return null;
        using (WWW www = WWW.LoadFromCacheOrDownload(download, 1))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("Error on WWW request (RCAssets):" + www.error);
            GameManager.RCassets = www.assetBundle;
        }
    }
}