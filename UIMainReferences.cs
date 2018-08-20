using System;
using System.Collections;
using Mod;
using UnityEngine;
using UnityEngine.UI;

public class UIMainReferences : MonoBehaviour
{
    public const string Version = "01042015";
    private static bool isFirstLaunch = true;
    public GameObject panelCredits;
    public GameObject PanelDisconnect;
    public GameObject panelMain;
    public GameObject PanelMultiJoinPrivate;
    public GameObject PanelMultiPWD;
    public GameObject panelMultiROOM;
    public GameObject panelMultiSet;
    public GameObject panelMultiStart;
    public GameObject PanelMultiWait;
    public GameObject panelOption;
    public GameObject panelSingleSet;
    public GameObject PanelSnapShot;

    public IEnumerator LoadRCAssets()
    {
        string url = Application.dataPath + "/RCAssets.unity3d";
        if (!Application.isPlaying)
            url = "File://" + url;
        while (!Caching.ready)
            yield return null;
        using (WWW iteratorVariable2 = WWW.LoadFromCacheOrDownload(url, 1))
        {
            yield return iteratorVariable2;
            if (iteratorVariable2.error != null)
                throw new Exception("Error on WWW request (RCAssets):" + iteratorVariable2.error);
            FengGameManagerMKII.RCassets = iteratorVariable2.assetBundle;
        }
    }

    private void Start()
    {
        const float aspectRatio = 1920f / 1080;
        if (Mathf.Abs((float) Screen.width / Screen.height - aspectRatio) > float.Epsilon)
            Screen.SetResolution((int)(Screen.height * aspectRatio), Screen.height, Screen.fullScreen);
        
        
        if (isFirstLaunch)
        {
            GameObject go = new GameObject("Shelter");
            Shelter shelter = go.AddComponent<Shelter>();
            DontDestroyOnLoad(go);
            shelter.InitComponents();

            GameObject target = (GameObject)Instantiate(Resources.Load("InputManagerController"));
            target.name = "InputManagerController";
            DontDestroyOnLoad(target);
            StartCoroutine(LoadRCAssets());
            isFirstLaunch = false;
        }

        Shelter.OnMainMenu();
    }
}