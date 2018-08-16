using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class FengGameManagerMKII
{
    #region Unity & Photon methods

    public void OnGUI()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.NotInRoom)
        {
            bool flag2;
            int num13;
            int num18;
            TextEditor editor;
            int num23;
            Event current;
            bool flag4;
            string str4;
            bool flag5;
            Texture2D textured;
            bool flag6;
            int num30;
            bool flag10;
            if ((int) settings[64] >= 100)
            {
                GameObject obj4;
                float num14;
                Color color;
                Mesh mesh;
                Color[] colorArray;
                float num20;
                float num21;
                float num27;
                int num28;
                int num29;
                float num11 = Screen.width - 300f;
                GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                GUI.DrawTexture(new Rect(7f, 7f, 291f, 586f), textureBackgroundBlue);
                GUI.DrawTexture(new Rect(num11 + 2f, 7f, 291f, 586f), textureBackgroundBlue);
                flag2 = false;
                bool flag3 = false;
                GUI.Box(new Rect(5f, 5f, 295f, 590f), string.Empty);
                GUI.Box(new Rect(num11, 5f, 295f, 590f), string.Empty);
                if (GUI.Button(new Rect(10f, 10f, 60f, 25f), "Script", "box"))
                {
                    settings[68] = 100;
                }

                if (GUI.Button(new Rect(75f, 10f, 65f, 25f), "Controls", "box"))
                {
                    settings[68] = 101;
                }

                if (GUI.Button(new Rect(210f, 10f, 80f, 25f), "Full Screen", "box"))
                {
                    Screen.fullScreen = !Screen.fullScreen;
                    if (Screen.fullScreen)
                    {
                        Screen.SetResolution(960, 600, false);
                    }
                    else
                    {
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    }
                }

                if ((int) settings[68] == 100 || (int) settings[68] == 102)
                {
                    string str2;
                    int num19;
                    GUI.Label(new Rect(115f, 40f, 100f, 20f), "Level Script:", "Label");
                    GUI.Label(new Rect(115f, 115f, 100f, 20f), "Import Data", "Label");
                    GUI.Label(new Rect(12f, 535f, 280f, 60f),
                        "Warning: your current level will be lost if you quit or import data. Make sure to save the level to a text document.",
                        "Label");
                    settings[77] = GUI.TextField(new Rect(10f, 140f, 285f, 350f), (string) settings[77]);
                    if (GUI.Button(new Rect(35f, 500f, 60f, 30f), "Apply"))
                    {
                        foreach (var o in FindObjectsOfType(typeof(GameObject)))
                        {
                            var obj2 = (GameObject) o;
                            if (obj2.name.StartsWith("custom") || obj2.name.StartsWith("base") ||
                                obj2.name.StartsWith("photon") || obj2.name.StartsWith("spawnpoint") ||
                                obj2.name.StartsWith("misc") || obj2.name.StartsWith("racing"))
                            {
                                Destroy(obj2);
                            }
                        }

                        linkHash[3].Clear();
                        settings[186] = 0;
                        string[] strArray = Regex.Replace((string) settings[77], @"\s+", "").Replace("\r\n", "")
                            .Replace("\n", "").Replace("\r", "").Split(';');
                        for (num13 = 0; num13 < strArray.Length; num13++)
                        {
                            string[] strArray2 = strArray[num13].Split(',');
                            if (strArray2[0].StartsWith("custom") || strArray2[0].StartsWith("base") ||
                                strArray2[0].StartsWith("photon") || strArray2[0].StartsWith("spawnpoint") ||
                                strArray2[0].StartsWith("misc") || strArray2[0].StartsWith("racing"))
                            {
                                float num15;
                                float num16;
                                float num17;
                                GameObject obj3 = null;
                                if (strArray2[0].StartsWith("custom"))
                                {
                                    obj3 = (GameObject) Instantiate((GameObject) RCassets.Load(strArray2[1]),
                                        new Vector3(Convert.ToSingle(strArray2[12]), Convert.ToSingle(strArray2[13]),
                                            Convert.ToSingle(strArray2[14])),
                                        new Quaternion(Convert.ToSingle(strArray2[15]), Convert.ToSingle(strArray2[16]),
                                            Convert.ToSingle(strArray2[17]), Convert.ToSingle(strArray2[18])));
                                }
                                else if (strArray2[0].StartsWith("photon"))
                                {
                                    if (strArray2[1].StartsWith("Cannon"))
                                    {
                                        if (strArray2.Length < 15)
                                        {
                                            obj3 = (GameObject) Instantiate(
                                                (GameObject) RCassets.Load(strArray2[1] + "Prop"),
                                                new Vector3(Convert.ToSingle(strArray2[2]),
                                                    Convert.ToSingle(strArray2[3]), Convert.ToSingle(strArray2[4])),
                                                new Quaternion(Convert.ToSingle(strArray2[5]),
                                                    Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]),
                                                    Convert.ToSingle(strArray2[8])));
                                        }
                                        else
                                        {
                                            obj3 = (GameObject) Instantiate(
                                                (GameObject) RCassets.Load(strArray2[1] + "Prop"),
                                                new Vector3(Convert.ToSingle(strArray2[12]),
                                                    Convert.ToSingle(strArray2[13]), Convert.ToSingle(strArray2[14])),
                                                new Quaternion(Convert.ToSingle(strArray2[15]),
                                                    Convert.ToSingle(strArray2[16]), Convert.ToSingle(strArray2[17]),
                                                    Convert.ToSingle(strArray2[18])));
                                        }
                                    }
                                    else
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) RCassets.Load(strArray2[1]),
                                            new Vector3(Convert.ToSingle(strArray2[4]), Convert.ToSingle(strArray2[5]),
                                                Convert.ToSingle(strArray2[6])),
                                            new Quaternion(Convert.ToSingle(strArray2[7]),
                                                Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]),
                                                Convert.ToSingle(strArray2[10])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("spawnpoint"))
                                {
                                    obj3 = (GameObject) Instantiate((GameObject) RCassets.Load(strArray2[1]),
                                        new Vector3(Convert.ToSingle(strArray2[2]), Convert.ToSingle(strArray2[3]),
                                            Convert.ToSingle(strArray2[4])),
                                        new Quaternion(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]),
                                            Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8])));
                                }
                                else if (strArray2[0].StartsWith("base"))
                                {
                                    if (strArray2.Length < 15)
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) Resources.Load(strArray2[1]),
                                            new Vector3(Convert.ToSingle(strArray2[2]), Convert.ToSingle(strArray2[3]),
                                                Convert.ToSingle(strArray2[4])),
                                            new Quaternion(Convert.ToSingle(strArray2[5]),
                                                Convert.ToSingle(strArray2[6]), Convert.ToSingle(strArray2[7]),
                                                Convert.ToSingle(strArray2[8])));
                                    }
                                    else
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) Resources.Load(strArray2[1]),
                                            new Vector3(Convert.ToSingle(strArray2[12]),
                                                Convert.ToSingle(strArray2[13]), Convert.ToSingle(strArray2[14])),
                                            new Quaternion(Convert.ToSingle(strArray2[15]),
                                                Convert.ToSingle(strArray2[16]), Convert.ToSingle(strArray2[17]),
                                                Convert.ToSingle(strArray2[18])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("misc"))
                                {
                                    if (strArray2[1].StartsWith("barrier"))
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) RCassets.Load("barrierEditor"),
                                            new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]),
                                                Convert.ToSingle(strArray2[7])),
                                            new Quaternion(Convert.ToSingle(strArray2[8]),
                                                Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]),
                                                Convert.ToSingle(strArray2[11])));
                                    }
                                    else if (strArray2[1].StartsWith("region"))
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) RCassets.Load("regionEditor"));
                                        obj3.transform.position = new Vector3(Convert.ToSingle(strArray2[6]),
                                            Convert.ToSingle(strArray2[7]), Convert.ToSingle(strArray2[8]));
                                        obj4 = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
                                        obj4.name = "RegionLabel";
                                        obj4.transform.parent = obj3.transform;
                                        num14 = 1f;
                                        if (Convert.ToSingle(strArray2[4]) > 100f)
                                        {
                                            num14 = 0.8f;
                                        }
                                        else if (Convert.ToSingle(strArray2[4]) > 1000f)
                                        {
                                            num14 = 0.5f;
                                        }

                                        obj4.transform.localPosition = new Vector3(0f, num14, 0f);
                                        obj4.transform.localScale = new Vector3(5f / Convert.ToSingle(strArray2[3]),
                                            5f / Convert.ToSingle(strArray2[4]), 5f / Convert.ToSingle(strArray2[5]));
                                        obj4.GetComponent<UILabel>().text = strArray2[2];
                                        obj3.AddComponent<RCRegionLabel>();
                                        obj3.GetComponent<RCRegionLabel>().myLabel = obj4;
                                    }
                                    else if (strArray2[1].StartsWith("racingStart"))
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) RCassets.Load("racingStart"),
                                            new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]),
                                                Convert.ToSingle(strArray2[7])),
                                            new Quaternion(Convert.ToSingle(strArray2[8]),
                                                Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]),
                                                Convert.ToSingle(strArray2[11])));
                                    }
                                    else if (strArray2[1].StartsWith("racingEnd"))
                                    {
                                        obj3 = (GameObject) Instantiate((GameObject) RCassets.Load("racingEnd"),
                                            new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]),
                                                Convert.ToSingle(strArray2[7])),
                                            new Quaternion(Convert.ToSingle(strArray2[8]),
                                                Convert.ToSingle(strArray2[9]), Convert.ToSingle(strArray2[10]),
                                                Convert.ToSingle(strArray2[11])));
                                    }
                                }
                                else if (strArray2[0].StartsWith("racing"))
                                {
                                    obj3 = (GameObject) Instantiate((GameObject) RCassets.Load(strArray2[1]),
                                        new Vector3(Convert.ToSingle(strArray2[5]), Convert.ToSingle(strArray2[6]),
                                            Convert.ToSingle(strArray2[7])),
                                        new Quaternion(Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]),
                                            Convert.ToSingle(strArray2[10]), Convert.ToSingle(strArray2[11])));
                                }

                                if (strArray2[2] != "default" &&
                                    (strArray2[0].StartsWith("custom") ||
                                     strArray2[0].StartsWith("base") && strArray2.Length > 15 ||
                                     strArray2[0].StartsWith("photon") && strArray2.Length > 15))
                                {
                                    foreach (Renderer renderer1 in obj3.GetComponentsInChildren<Renderer>())
                                    {
                                        if (!(renderer1.name.Contains("Particle System") &&
                                              obj3.name.Contains("aot_supply")))
                                        {
                                            renderer1.material = (Material) RCassets.Load(strArray2[2]);
                                            renderer1.material.mainTextureScale =
                                                new Vector2(
                                                    renderer1.material.mainTextureScale.x *
                                                    Convert.ToSingle(strArray2[10]),
                                                    renderer1.material.mainTextureScale.y *
                                                    Convert.ToSingle(strArray2[11]));
                                        }
                                    }
                                }

                                if (strArray2[0].StartsWith("custom") ||
                                    strArray2[0].StartsWith("base") && strArray2.Length > 15 ||
                                    strArray2[0].StartsWith("photon") && strArray2.Length > 15)
                                {
                                    num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[3]);
                                    num15 -= 0.001f;
                                    num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[4]);
                                    num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[5]);
                                    obj3.transform.localScale = new Vector3(num15, num16, num17);
                                    if (strArray2[6] != "0")
                                    {
                                        color = new Color(Convert.ToSingle(strArray2[7]),
                                            Convert.ToSingle(strArray2[8]), Convert.ToSingle(strArray2[9]), 1f);
                                        foreach (MeshFilter filter in obj3.GetComponentsInChildren<MeshFilter>())
                                        {
                                            mesh = filter.mesh;
                                            colorArray = new Color[mesh.vertexCount];
                                            num18 = 0;
                                            while (num18 < mesh.vertexCount)
                                            {
                                                colorArray[num18] = color;
                                                num18++;
                                            }

                                            mesh.colors = colorArray;
                                        }
                                    }

                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," +
                                                strArray2[3] + "," + strArray2[4] + "," + strArray2[5] + "," +
                                                strArray2[6] + "," + strArray2[7] + "," + strArray2[8] + "," +
                                                strArray2[9] + "," + strArray2[10] + "," + strArray2[11];
                                }
                                else if (strArray2[0].StartsWith("misc"))
                                {
                                    if (strArray2[1].StartsWith("barrier") || strArray2[1].StartsWith("racing"))
                                    {
                                        num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[2]);
                                        num15 -= 0.001f;
                                        num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[3]);
                                        num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[4]);
                                        obj3.transform.localScale = new Vector3(num15, num16, num17);
                                        obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," +
                                                    strArray2[3] + "," + strArray2[4];
                                    }
                                    else if (strArray2[1].StartsWith("region"))
                                    {
                                        num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[3]);
                                        num15 -= 0.001f;
                                        num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[4]);
                                        num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[5]);
                                        obj3.transform.localScale = new Vector3(num15, num16, num17);
                                        obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," +
                                                    strArray2[3] + "," + strArray2[4] + "," + strArray2[5];
                                    }
                                }
                                else if (strArray2[0].StartsWith("racing"))
                                {
                                    num15 = obj3.transform.localScale.x * Convert.ToSingle(strArray2[2]);
                                    num15 -= 0.001f;
                                    num16 = obj3.transform.localScale.y * Convert.ToSingle(strArray2[3]);
                                    num17 = obj3.transform.localScale.z * Convert.ToSingle(strArray2[4]);
                                    obj3.transform.localScale = new Vector3(num15, num16, num17);
                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," +
                                                strArray2[3] + "," + strArray2[4];
                                }
                                else if (!(!strArray2[0].StartsWith("photon") || strArray2[1].StartsWith("Cannon")))
                                {
                                    obj3.name = strArray2[0] + "," + strArray2[1] + "," + strArray2[2] + "," +
                                                strArray2[3];
                                }
                                else
                                {
                                    obj3.name = strArray2[0] + "," + strArray2[1];
                                }

                                linkHash[3].Add(obj3.GetInstanceID(), strArray[num13]);
                            }
                            else if (strArray2[0].StartsWith("map") && strArray2[1].StartsWith("disablebounds"))
                            {
                                settings[186] = 1;
                                if (!linkHash[3].ContainsKey("mapbounds"))
                                {
                                    linkHash[3].Add("mapbounds", "map,disablebounds");
                                }
                            }
                        }

                        UnloadAssets();
                        settings[77] = string.Empty;
                    }
                    else if (GUI.Button(new Rect(205f, 500f, 60f, 30f), "Exit"))
                    {
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
                        inputManager.menuOn = false;
                        Destroy(GameObject.Find("MultiplayerManager"));
                        Application.LoadLevel("menu");
                    }
                    else if (GUI.Button(new Rect(15f, 70f, 115f, 30f), "Copy to Clipboard"))
                    {
                        str2 = string.Empty;
                        num19 = 0;
                        foreach (string str3 in linkHash[3].Values)
                        {
                            num19++;
                            str2 = str2 + str3 + ";\n";
                        }

                        editor = new TextEditor
                        {
                            content = new GUIContent(str2)
                        };
                        editor.SelectAll();
                        editor.Copy();
                    }
                    else if (GUI.Button(new Rect(175f, 70f, 115f, 30f), "View Script"))
                    {
                        settings[68] = 102;
                    }

                    if ((int) settings[68] == 102)
                    {
                        str2 = string.Empty;
                        num19 = 0;
                        foreach (string str3 in linkHash[3].Values)
                        {
                            num19++;
                            str2 = str2 + str3 + ";\n";
                        }

                        num20 = Screen.width / 2f - 110.5f;
                        num21 = Screen.height / 2f - 250f;
                        GUI.DrawTexture(new Rect(num20 + 2f, num21 + 2f, 217f, 496f), textureBackgroundBlue);
                        GUI.Box(new Rect(num20, num21, 221f, 500f), string.Empty);
                        if (GUI.Button(new Rect(num20 + 10f, num21 + 460f, 60f, 30f), "Copy"))
                        {
                            editor = new TextEditor
                            {
                                content = new GUIContent(str2)
                            };
                            editor.SelectAll();
                            editor.Copy();
                        }
                        else if (GUI.Button(new Rect(num20 + 151f, num21 + 460f, 60f, 30f), "Done"))
                        {
                            settings[68] = 100;
                        }

                        GUI.TextArea(new Rect(num20 + 5f, num21 + 5f, 211f, 415f), str2);
                        GUI.Label(new Rect(num20 + 10f, num21 + 430f, 150f, 20f),
                            "Object Count: " + Convert.ToString(num19), "Label");
                    }
                }
                else if ((int) settings[68] == 101)
                {
                    GUI.Label(new Rect(92f, 50f, 180f, 20f), "Level Editor Rebinds:", "Label");
                    GUI.Label(new Rect(12f, 80f, 145f, 20f), "Forward:", "Label");
                    GUI.Label(new Rect(12f, 105f, 145f, 20f), "Back:", "Label");
                    GUI.Label(new Rect(12f, 130f, 145f, 20f), "Left:", "Label");
                    GUI.Label(new Rect(12f, 155f, 145f, 20f), "Right:", "Label");
                    GUI.Label(new Rect(12f, 180f, 145f, 20f), "Up:", "Label");
                    GUI.Label(new Rect(12f, 205f, 145f, 20f), "Down:", "Label");
                    GUI.Label(new Rect(12f, 230f, 145f, 20f), "Toggle Cursor:", "Label");
                    GUI.Label(new Rect(12f, 255f, 145f, 20f), "Place Object:", "Label");
                    GUI.Label(new Rect(12f, 280f, 145f, 20f), "Delete Object:", "Label");
                    GUI.Label(new Rect(12f, 305f, 145f, 20f), "Movement-Slow:", "Label");
                    GUI.Label(new Rect(12f, 330f, 145f, 20f), "Rotate Forward:", "Label");
                    GUI.Label(new Rect(12f, 355f, 145f, 20f), "Rotate Backward:", "Label");
                    GUI.Label(new Rect(12f, 380f, 145f, 20f), "Rotate Left:", "Label");
                    GUI.Label(new Rect(12f, 405f, 145f, 20f), "Rotate Right:", "Label");
                    GUI.Label(new Rect(12f, 430f, 145f, 20f), "Rotate CCW:", "Label");
                    GUI.Label(new Rect(12f, 455f, 145f, 20f), "Rotate CW:", "Label");
                    GUI.Label(new Rect(12f, 480f, 145f, 20f), "Movement-Speedup:", "Label");
                    for (num13 = 0; num13 < 17; num13++)
                    {
                        float num22 = 80f + 25f * num13;
                        num23 = 117 + num13;
                        if (num13 == 16)
                        {
                            num23 = 161;
                        }

                        if (GUI.Button(new Rect(135f, num22, 60f, 20f), (string) settings[num23], "box"))
                        {
                            settings[num23] = "waiting...";
                            settings[100] = num23;
                        }
                    }

                    if ((int) settings[100] != 0)
                    {
                        current = Event.current;
                        flag4 = false;
                        str4 = "waiting...";
                        if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
                        {
                            flag4 = true;
                            str4 = current.keyCode.ToString();
                        }
                        else if (Input.GetKey(KeyCode.LeftShift))
                        {
                            flag4 = true;
                            str4 = KeyCode.LeftShift.ToString();
                        }
                        else if (Input.GetKey(KeyCode.RightShift))
                        {
                            flag4 = true;
                            str4 = KeyCode.RightShift.ToString();
                        }
                        else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                        {
                            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                            {
                                flag4 = true;
                                str4 = "Scroll Up";
                            }
                            else
                            {
                                flag4 = true;
                                str4 = "Scroll Down";
                            }
                        }
                        else
                        {
                            num13 = 0;
                            while (num13 < 7)
                            {
                                if (Input.GetKeyDown((KeyCode) (323 + num13)))
                                {
                                    flag4 = true;
                                    str4 = "Mouse" + Convert.ToString(num13);
                                }

                                num13++;
                            }
                        }

                        if (flag4)
                        {
                            for (num13 = 0; num13 < 17; num13++)
                            {
                                num23 = 117 + num13;
                                if (num13 == 16)
                                {
                                    num23 = 161;
                                }

                                if ((int) settings[100] == num23)
                                {
                                    settings[num23] = str4;
                                    settings[100] = 0;
                                    inputRC.setInputLevel(num13, str4);
                                }
                            }
                        }
                    }

                    if (GUI.Button(new Rect(100f, 515f, 110f, 30f), "Save Controls"))
                    {
                        PlayerPrefs.SetString("lforward", (string) settings[117]);
                        PlayerPrefs.SetString("lback", (string) settings[118]);
                        PlayerPrefs.SetString("lleft", (string) settings[119]);
                        PlayerPrefs.SetString("lright", (string) settings[120]);
                        PlayerPrefs.SetString("lup", (string) settings[121]);
                        PlayerPrefs.SetString("ldown", (string) settings[122]);
                        PlayerPrefs.SetString("lcursor", (string) settings[123]);
                        PlayerPrefs.SetString("lplace", (string) settings[124]);
                        PlayerPrefs.SetString("ldel", (string) settings[125]);
                        PlayerPrefs.SetString("lslow", (string) settings[126]);
                        PlayerPrefs.SetString("lrforward", (string) settings[127]);
                        PlayerPrefs.SetString("lrback", (string) settings[128]);
                        PlayerPrefs.SetString("lrleft", (string) settings[129]);
                        PlayerPrefs.SetString("lrright", (string) settings[130]);
                        PlayerPrefs.SetString("lrccw", (string) settings[131]);
                        PlayerPrefs.SetString("lrcw", (string) settings[132]);
                        PlayerPrefs.SetString("lfast", (string) settings[161]);
                    }
                }

                if ((int) settings[64] != 105 && (int) settings[64] != 106)
                {
                    GUI.Label(new Rect(num11 + 13f, 445f, 125f, 20f), "Scale Multipliers:", "Label");
                    GUI.Label(new Rect(num11 + 13f, 470f, 50f, 22f), "Length:", "Label");
                    settings[72] = GUI.TextField(new Rect(num11 + 58f, 470f, 40f, 20f), (string) settings[72]);
                    GUI.Label(new Rect(num11 + 13f, 495f, 50f, 20f), "Width:", "Label");
                    settings[70] = GUI.TextField(new Rect(num11 + 58f, 495f, 40f, 20f), (string) settings[70]);
                    GUI.Label(new Rect(num11 + 13f, 520f, 50f, 22f), "Height:", "Label");
                    settings[71] = GUI.TextField(new Rect(num11 + 58f, 520f, 40f, 20f), (string) settings[71]);
                    if ((int) settings[64] <= 106)
                    {
                        GUI.Label(new Rect(num11 + 155f, 554f, 50f, 22f), "Tiling:", "Label");
                        settings[79] = GUI.TextField(new Rect(num11 + 200f, 554f, 40f, 20f), (string) settings[79]);
                        settings[80] = GUI.TextField(new Rect(num11 + 245f, 554f, 40f, 20f), (string) settings[80]);
                        GUI.Label(new Rect(num11 + 219f, 570f, 10f, 22f), "x:", "Label");
                        GUI.Label(new Rect(num11 + 264f, 570f, 10f, 22f), "y:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 445f, 50f, 20f), "Color:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 470f, 10f, 20f), "R:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 495f, 10f, 20f), "G:", "Label");
                        GUI.Label(new Rect(num11 + 155f, 520f, 10f, 20f), "B:", "Label");
                        settings[73] = GUI.HorizontalSlider(new Rect(num11 + 170f, 475f, 100f, 20f),
                            (float) settings[73], 0f, 1f);
                        settings[74] = GUI.HorizontalSlider(new Rect(num11 + 170f, 500f, 100f, 20f),
                            (float) settings[74], 0f, 1f);
                        settings[75] = GUI.HorizontalSlider(new Rect(num11 + 170f, 525f, 100f, 20f),
                            (float) settings[75], 0f, 1f);
                        GUI.Label(new Rect(num11 + 13f, 554f, 57f, 22f), "Material:", "Label");
                        if (GUI.Button(new Rect(num11 + 66f, 554f, 60f, 20f), (string) settings[69]))
                        {
                            settings[78] = 1;
                        }

                        if ((int) settings[78] == 1)
                        {
                            string[] strArray4 = {"bark", "bark2", "bark3", "bark4"};
                            string[] strArray5 = {"wood1", "wood2", "wood3", "wood4"};
                            string[] strArray6 = {"grass", "grass2", "grass3", "grass4"};
                            string[] strArray7 = {"brick1", "brick2", "brick3", "brick4"};
                            string[] strArray8 = {"metal1", "metal2", "metal3", "metal4"};
                            string[] strArray9 = {"rock1", "rock2", "rock3"};
                            string[] strArray10 =
                            {
                                "stone1", "stone2", "stone3", "stone4", "stone5", "stone6", "stone7", "stone8",
                                "stone9", "stone10"
                            };
                            string[] strArray11 =
                                {"earth1", "earth2", "ice1", "lava1", "crystal1", "crystal2", "empty"};
                            List<string[]> list2 = new List<string[]>
                            {
                                strArray4,
                                strArray5,
                                strArray6,
                                strArray7,
                                strArray8,
                                strArray9,
                                strArray10,
                                strArray11
                            };
                            string[] strArray13 =
                                {"bark", "wood", "grass", "brick", "metal", "rock", "stone", "misc", "transparent"};
                            int index = 78;
                            int num25 = 69;
                            num20 = Screen.width / 2f - 110.5f;
                            num21 = Screen.height / 2f - 220f;
                            int num26 = (int) settings[185];
                            num27 = 10f + 104f * (list2[num26].Length / 3 + 1);
                            num27 = Math.Max(num27, 280f);
                            GUI.DrawTexture(new Rect(num20 + 2f, num21 + 2f, 208f, 446f), textureBackgroundBlue);
                            GUI.Box(new Rect(num20, num21, 212f, 450f), string.Empty);
                            for (num13 = 0; num13 < list2.Count; num13++)
                            {
                                num28 = num13 / 3;
                                num29 = num13 % 3;
                                if (GUI.Button(new Rect(num20 + 5f + 69f * num29, num21 + 5f + 30 * num28, 64f, 25f),
                                    strArray13[num13], "box"))
                                {
                                    settings[185] = num13;
                                }
                            }

                            scroll2 = GUI.BeginScrollView(new Rect(num20, num21 + 110f, 225f, 290f), scroll2,
                                new Rect(num20, num21 + 110f, 212f, num27), true, true);
                            if (num26 != 8)
                            {
                                for (num13 = 0; num13 < list2[num26].Length; num13++)
                                {
                                    num28 = num13 / 3;
                                    num29 = num13 % 3;
                                    GUI.DrawTexture(
                                        new Rect(num20 + 5f + 69f * num29, num21 + 115f + 104f * num28, 64f, 64f),
                                        RCLoadTexture("p" + list2[num26][num13]));
                                    if (GUI.Button(
                                        new Rect(num20 + 5f + 69f * num29, num21 + 184f + 104f * num28, 64f, 30f),
                                        list2[num26][num13]))
                                    {
                                        settings[num25] = list2[num26][num13];
                                        settings[index] = 0;
                                    }
                                }
                            }

                            GUI.EndScrollView();
                            if (GUI.Button(new Rect(num20 + 24f, num21 + 410f, 70f, 30f), "Default"))
                            {
                                settings[num25] = "default";
                                settings[index] = 0;
                            }
                            else if (GUI.Button(new Rect(num20 + 118f, num21 + 410f, 70f, 30f), "Done"))
                            {
                                settings[index] = 0;
                            }
                        }

                        flag5 = false;
                        if ((int) settings[76] == 1)
                        {
                            flag5 = true;
                            textured = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                            textured.SetPixel(0, 0,
                                new Color((float) settings[73], (float) settings[74], (float) settings[75], 1f));
                            textured.Apply();
                            GUI.DrawTexture(new Rect(num11 + 235f, 445f, 30f, 20f), textured, ScaleMode.StretchToFill);
                            Destroy(textured);
                        }

                        flag6 = GUI.Toggle(new Rect(num11 + 193f, 445f, 40f, 20f), flag5, "On");
                        if (flag5 != flag6)
                        {
                            if (flag6)
                            {
                                settings[76] = 1;
                            }
                            else
                            {
                                settings[76] = 0;
                            }
                        }
                    }
                }

                if (GUI.Button(new Rect(num11 + 5f, 10f, 60f, 25f), "General", "box"))
                {
                    settings[64] = 101;
                }
                else if (GUI.Button(new Rect(num11 + 70f, 10f, 70f, 25f), "Geometry", "box"))
                {
                    settings[64] = 102;
                }
                else if (GUI.Button(new Rect(num11 + 145f, 10f, 65f, 25f), "Buildings", "box"))
                {
                    settings[64] = 103;
                }
                else if (GUI.Button(new Rect(num11 + 215f, 10f, 50f, 25f), "Nature", "box"))
                {
                    settings[64] = 104;
                }
                else if (GUI.Button(new Rect(num11 + 5f, 45f, 70f, 25f), "Spawners", "box"))
                {
                    settings[64] = 105;
                }
                else if (GUI.Button(new Rect(num11 + 80f, 45f, 70f, 25f), "Racing", "box"))
                {
                    settings[64] = 108;
                }
                else if (GUI.Button(new Rect(num11 + 155f, 45f, 40f, 25f), "Misc", "box"))
                {
                    settings[64] = 107;
                }
                else if (GUI.Button(new Rect(num11 + 200f, 45f, 70f, 25f), "Credits", "box"))
                {
                    settings[64] = 106;
                }

                if ((int) settings[64] == 101)
                {
                    GameObject obj5;
                    scroll = GUI.BeginScrollView(new Rect(num11, 80f, 305f, 350f), scroll,
                        new Rect(num11, 80f, 300f, 470f), true, true);
                    GUI.Label(new Rect(num11 + 100f, 80f, 120f, 20f), "General Objects:", "Label");
                    GUI.Label(new Rect(num11 + 108f, 245f, 120f, 20f), "Spawn Points:", "Label");
                    GUI.Label(new Rect(num11 + 7f, 415f, 290f, 60f),
                        "* The above titan spawn points apply only to randomly spawned titans specified by the Random Titan #.",
                        "Label");
                    GUI.Label(new Rect(num11 + 7f, 470f, 290f, 60f),
                        "* If team mode is disabled both cyan and magenta spawn points will be randomly chosen for players.",
                        "Label");
                    GUI.DrawTexture(new Rect(num11 + 27f, 110f, 64f, 64f), RCLoadTexture("psupply"));
                    GUI.DrawTexture(new Rect(num11 + 118f, 110f, 64f, 64f), RCLoadTexture("pcannonwall"));
                    GUI.DrawTexture(new Rect(num11 + 209f, 110f, 64f, 64f), RCLoadTexture("pcannonground"));
                    GUI.DrawTexture(new Rect(num11 + 27f, 275f, 64f, 64f), RCLoadTexture("pspawnt"));
                    GUI.DrawTexture(new Rect(num11 + 118f, 275f, 64f, 64f), RCLoadTexture("pspawnplayerC"));
                    GUI.DrawTexture(new Rect(num11 + 209f, 275f, 64f, 64f), RCLoadTexture("pspawnplayerM"));
                    if (GUI.Button(new Rect(num11 + 27f, 179f, 64f, 60f), "Supply"))
                    {
                        flag2 = true;
                        obj5 = (GameObject) Resources.Load("aot_supply");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "base,aot_supply";
                    }
                    else if (GUI.Button(new Rect(num11 + 118f, 179f, 64f, 60f), "Cannon \nWall"))
                    {
                        flag2 = true;
                        obj5 = (GameObject) RCassets.Load("CannonWallProp");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "photon,CannonWall";
                    }
                    else if (GUI.Button(new Rect(num11 + 209f, 179f, 64f, 60f), "Cannon\n Ground"))
                    {
                        flag2 = true;
                        obj5 = (GameObject) RCassets.Load("CannonGroundProp");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "photon,CannonGround";
                    }
                    else if (GUI.Button(new Rect(num11 + 27f, 344f, 64f, 60f), "Titan"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject) RCassets.Load("titan");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "spawnpoint,titan";
                    }
                    else if (GUI.Button(new Rect(num11 + 118f, 344f, 64f, 60f), "Player \nCyan"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject) RCassets.Load("playerC");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "spawnpoint,playerC";
                    }
                    else if (GUI.Button(new Rect(num11 + 209f, 344f, 64f, 60f), "Player \nMagenta"))
                    {
                        flag2 = true;
                        flag3 = true;
                        obj5 = (GameObject) RCassets.Load("playerM");
                        selectedObj = (GameObject) Instantiate(obj5);
                        selectedObj.name = "spawnpoint,playerM";
                    }

                    GUI.EndScrollView();
                }
                else
                {
                    GameObject obj6;
                    if ((int) settings[64] == 107)
                    {
                        bool flag8;
                        GUI.DrawTexture(new Rect(num11 + 30f, 90f, 64f, 64f), RCLoadTexture("pbarrier"));
                        GUI.DrawTexture(new Rect(num11 + 30f, 199f, 64f, 64f), RCLoadTexture("pregion"));
                        GUI.Label(new Rect(num11 + 110f, 243f, 200f, 22f), "Region Name:", "Label");
                        GUI.Label(new Rect(num11 + 110f, 179f, 200f, 22f), "Disable Map Bounds:", "Label");
                        bool flag7 = false;
                        if ((int) settings[186] == 1)
                        {
                            flag7 = true;
                            if (!linkHash[3].ContainsKey("mapbounds"))
                            {
                                linkHash[3].Add("mapbounds", "map,disablebounds");
                            }
                        }
                        else if (linkHash[3].ContainsKey("mapbounds"))
                        {
                            linkHash[3].Remove("mapbounds");
                        }

                        if (GUI.Button(new Rect(num11 + 30f, 159f, 64f, 30f), "Barrier"))
                        {
                            flag2 = true;
                            flag3 = true;
                            obj6 = (GameObject) RCassets.Load("barrierEditor");
                            selectedObj = (GameObject) Instantiate(obj6);
                            selectedObj.name = "misc,barrier";
                        }
                        else if (GUI.Button(new Rect(num11 + 30f, 268f, 64f, 30f), "Region"))
                        {
                            if ((string) settings[191] == string.Empty)
                            {
                                settings[191] = "Region" + UnityEngine.Random.Range(10000, 99999);
                            }

                            flag2 = true;
                            flag3 = true;
                            obj6 = (GameObject) RCassets.Load("regionEditor");
                            selectedObj = (GameObject) Instantiate(obj6);
                            obj4 = (GameObject) Instantiate(Resources.Load("UI/LabelNameOverHead"));
                            obj4.name = "RegionLabel";
                            if (!float.TryParse((string) settings[71], out _))
                            {
                                settings[71] = "1";
                            }

                            if (!float.TryParse((string) settings[70], out _))
                            {
                                settings[70] = "1";
                            }

                            if (!float.TryParse((string) settings[72], out _))
                            {
                                settings[72] = "1";
                            }

                            obj4.transform.parent = selectedObj.transform;
                            num14 = 1f;
                            if (Convert.ToSingle((string) settings[71]) > 100f)
                            {
                                num14 = 0.8f;
                            }
                            else if (Convert.ToSingle((string) settings[71]) > 1000f)
                            {
                                num14 = 0.5f;
                            }

                            obj4.transform.localPosition = new Vector3(0f, num14, 0f);
                            obj4.transform.localScale = new Vector3(5f / Convert.ToSingle((string) settings[70]),
                                5f / Convert.ToSingle((string) settings[71]),
                                5f / Convert.ToSingle((string) settings[72]));
                            obj4.GetComponent<UILabel>().text = (string) settings[191];
                            selectedObj.AddComponent<RCRegionLabel>();
                            selectedObj.GetComponent<RCRegionLabel>().myLabel = obj4;
                            selectedObj.name = "misc,region," + (string) settings[191];
                        }

                        settings[191] = GUI.TextField(new Rect(num11 + 200f, 243f, 75f, 20f), (string) settings[191]);
                        if ((flag8 = GUI.Toggle(new Rect(num11 + 240f, 179f, 40f, 20f), flag7, "On")) != flag7)
                        {
                            if (flag8)
                            {
                                settings[186] = 1;
                            }
                            else
                            {
                                settings[186] = 0;
                            }
                        }
                    }
                    else if ((int) settings[64] == 105)
                    {
                        GameObject obj7;
                        GUI.Label(new Rect(num11 + 95f, 85f, 130f, 20f), "Custom Spawners:", "Label");
                        GUI.DrawTexture(new Rect(num11 + 7.8f, 110f, 64f, 64f), RCLoadTexture("ptitan"));
                        GUI.DrawTexture(new Rect(num11 + 79.6f, 110f, 64f, 64f), RCLoadTexture("pabnormal"));
                        GUI.DrawTexture(new Rect(num11 + 151.4f, 110f, 64f, 64f), RCLoadTexture("pjumper"));
                        GUI.DrawTexture(new Rect(num11 + 223.2f, 110f, 64f, 64f), RCLoadTexture("pcrawler"));
                        GUI.DrawTexture(new Rect(num11 + 7.8f, 224f, 64f, 64f), RCLoadTexture("ppunk"));
                        GUI.DrawTexture(new Rect(num11 + 79.6f, 224f, 64f, 64f), RCLoadTexture("pannie"));
                        if (GUI.Button(new Rect(num11 + 7.8f, 179f, 64f, 30f), "Titan"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnTitan");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnTitan," + (string) settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 79.6f, 179f, 64f, 30f), "Aberrant"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnAbnormal");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnAbnormal," + (string) settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 151.4f, 179f, 64f, 30f), "Jumper"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnJumper");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnJumper," + (string) settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 223.2f, 179f, 64f, 30f), "Crawler"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnCrawler");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnCrawler," + (string) settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 7.8f, 293f, 64f, 30f), "Punk"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnPunk");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnPunk," + (string) settings[83] + "," + num30;
                        }
                        else if (GUI.Button(new Rect(num11 + 79.6f, 293f, 64f, 30f), "Annie"))
                        {
                            if (!float.TryParse((string) settings[83], out float _))
                            {
                                settings[83] = "30";
                            }

                            flag2 = true;
                            flag3 = true;
                            obj7 = (GameObject) RCassets.Load("spawnAnnie");
                            selectedObj = (GameObject) Instantiate(obj7);
                            num30 = (int) settings[84];
                            selectedObj.name = "photon,spawnAnnie," + (string) settings[83] + "," + num30;
                        }

                        GUI.Label(new Rect(num11 + 7f, 379f, 140f, 22f), "Spawn Timer:", "Label");
                        settings[83] = GUI.TextField(new Rect(num11 + 100f, 379f, 50f, 20f), (string) settings[83]);
                        GUI.Label(new Rect(num11 + 7f, 356f, 140f, 22f), "Endless spawn:", "Label");
                        GUI.Label(new Rect(num11 + 7f, 405f, 290f, 80f),
                            "* The above settings apply only to the next placed spawner. You can have unique spawn times and settings for each individual titan spawner.",
                            "Label");
                        bool flag9 = (int) settings[84] == 1;
                        flag10 = GUI.Toggle(new Rect(num11 + 100f, 356f, 40f, 20f), flag9, "On");
                        if (flag9 != flag10)
                        {
                            if (flag10)
                            {
                                settings[84] = 1;
                            }
                            else
                            {
                                settings[84] = 0;
                            }
                        }
                    }
                    else
                    {
                        string[] strArray14;
                        if ((int) settings[64] == 102)
                        {
                            strArray14 = new[]
                            {
                                "cuboid", "plane", "sphere", "cylinder", "capsule", "pyramid", "cone", "prism", "arc90",
                                "arc180", "torus", "tube"
                            };
                            for (num13 = 0; num13 < strArray14.Length; num13++)
                            {
                                num29 = num13 % 4;
                                num28 = num13 / 4;
                                GUI.DrawTexture(new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f),
                                    RCLoadTexture("p" + strArray14[num13]));
                                if (GUI.Button(new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f),
                                    strArray14[num13]))
                                {
                                    flag2 = true;
                                    obj6 = (GameObject) RCassets.Load(strArray14[num13]);
                                    selectedObj = (GameObject) Instantiate(obj6);
                                    selectedObj.name = "custom," + strArray14[num13];
                                }
                            }
                        }
                        else
                        {
                            List<string> list4;
                            GameObject obj8;
                            switch ((int) settings[64])
                            {
                                case 103:
                                    list4 = new List<string> {"arch1", "house1"};
                                    strArray14 = new[]
                                    {
                                        "tower1", "tower2", "tower3", "tower4", "tower5", "house1", "house2", "house3",
                                        "house4", "house5", "house6", "house7", "house8", "house9", "house10",
                                        "house11",
                                        "house12", "house13", "house14", "pillar1", "pillar2", "village1", "village2",
                                        "windmill1", "arch1", "canal1", "castle1", "church1", "cannon1", "statue1",
                                        "statue2", "wagon1",
                                        "elevator1", "bridge1", "dummy1", "spike1", "wall1", "wall2", "wall3", "wall4",
                                        "arena1", "arena2", "arena3", "arena4"
                                    };
                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll,
                                        new Rect(num11, 90f, 300f, num27), true, true);
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f),
                                            strArray14[num13]))
                                        {
                                            flag2 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            if (list4.Contains(strArray14[num13]))
                                            {
                                                selectedObj.name = "customb," + strArray14[num13];
                                            }
                                            else
                                            {
                                                selectedObj.name = "custom," + strArray14[num13];
                                            }
                                        }
                                    }

                                    GUI.EndScrollView();
                                    break;
                                case 104:
                                    list4 = new List<string> {"tree0"};
                                    strArray14 = new[]
                                    {
                                        "leaf0", "leaf1", "leaf2", "field1", "field2", "tree0", "tree1", "tree2",
                                        "tree3", "tree4", "tree5", "tree6", "tree7", "log1", "log2", "trunk1",
                                        "boulder1", "boulder2", "boulder3", "boulder4", "boulder5", "cave1", "cave2"
                                    };
                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll,
                                        new Rect(num11, 90f, 300f, num27), true, true);
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, 90f + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, 159f + 114f * num28, 64f, 30f),
                                            strArray14[num13]))
                                        {
                                            flag2 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            if (list4.Contains(strArray14[num13]))
                                            {
                                                selectedObj.name = "customb," + strArray14[num13];
                                            }
                                            else
                                            {
                                                selectedObj.name = "custom," + strArray14[num13];
                                            }
                                        }
                                    }

                                    GUI.EndScrollView();
                                    break;
                                case 108:
                                    string[] strArray15 =
                                    {
                                        "Cuboid", "Plane", "Sphere", "Cylinder", "Capsule", "Pyramid", "Cone", "Prism",
                                        "Arc90", "Arc180", "Torus", "Tube"
                                    };
                                    strArray14 = new string[12];
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "start" + strArray15[num13];
                                    }

                                    num27 = 110f + 114f * ((strArray14.Length - 1) / 4f);
                                    num27 *= 4f;
                                    num27 += 200f;
                                    scroll = GUI.BeginScrollView(new Rect(num11, 90f, 303f, 350f), scroll,
                                        new Rect(num11, 90f, 300f, num27), true, true);
                                    GUI.Label(new Rect(num11 + 90f, 90f, 200f, 22f), "Racing Start Barrier");
                                    int num33 = 125;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f,
                                                30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }

                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 93f, num33, 200f, 22f), "Racing End Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "end" + strArray15[num13];
                                    }

                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f,
                                                30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }

                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 113f, num33, 200f, 22f), "Kill Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "kill" + strArray15[num13];
                                    }

                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f,
                                                30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }

                                    num33 += 114 * (strArray14.Length / 4) + 10;
                                    GUI.Label(new Rect(num11 + 95f, num33, 200f, 22f), "Checkpoint Trigger");
                                    num33 += 35;
                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        strArray14[num13] = "checkpoint" + strArray15[num13];
                                    }

                                    for (num13 = 0; num13 < strArray14.Length; num13++)
                                    {
                                        num29 = num13 % 4;
                                        num28 = num13 / 4;
                                        GUI.DrawTexture(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 114f * num28, 64f, 64f),
                                            RCLoadTexture("p" + strArray14[num13]));
                                        if (GUI.Button(
                                            new Rect(num11 + 7.8f + 71.8f * num29, num33 + 69f + 114f * num28, 64f,
                                                30f), strArray15[num13]))
                                        {
                                            flag2 = true;
                                            flag3 = true;
                                            obj8 = (GameObject) RCassets.Load(strArray14[num13]);
                                            selectedObj = (GameObject) Instantiate(obj8);
                                            selectedObj.name = "racing," + strArray14[num13];
                                        }
                                    }

                                    GUI.EndScrollView();
                                    break;
                                case 106:
                                    GUI.Label(new Rect(num11 + 10f, 80f, 200f, 22f), "- Tree 2 designed by Ken P.",
                                        "Label");
                                    GUI.Label(new Rect(num11 + 10f, 105f, 250f, 22f),
                                        "- Tower 2, House 5 designed by Matthew Santos", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 130f, 200f, 22f), "- Cannon retextured by Mika",
                                        "Label");
                                    GUI.Label(new Rect(num11 + 10f, 155f, 200f, 22f),
                                        "- Arena 1,2,3 & 4 created by Gun", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 180f, 250f, 22f),
                                        "- Cannon Wall/Ground textured by Bellfox", "Label");
                                    GUI.Label(new Rect(num11 + 10f, 205f, 250f, 120f),
                                        "- House 7 - 14, Statue1, Statue2, Wagon1, Wall 1, Wall 2, Wall 3, Wall 4, CannonWall, CannonGround, Tower5, Bridge1, Dummy1, Spike1 created by meecube",
                                        "Label");
                                    break;
                            }
                        }
                    }
                }

                if (flag2 && selectedObj != null)
                {
                    float y;
                    float num37;
                    float num38;
                    float num39;
                    float z;
                    float num41;
                    string name1;
                    if (!float.TryParse((string) settings[70], out _))
                    {
                        settings[70] = "1";
                    }

                    if (!float.TryParse((string) settings[71], out _))
                    {
                        settings[71] = "1";
                    }

                    if (!float.TryParse((string) settings[72], out _))
                    {
                        settings[72] = "1";
                    }

                    if (!float.TryParse((string) settings[79], out _))
                    {
                        settings[79] = "1";
                    }

                    if (!float.TryParse((string) settings[80], out _))
                    {
                        settings[80] = "1";
                    }

                    if (!flag3)
                    {
                        float a = 1f;
                        if ((string) settings[69] != "default")
                        {
                            if (((string) settings[69]).StartsWith("transparent"))
                            {
                                float.TryParse(((string) settings[69]).Substring(11), out a);
                                foreach (Renderer renderer2 in selectedObj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer2.material = (Material) RCassets.Load("transparent");
                                    renderer2.material.mainTextureScale = new Vector2(
                                        renderer2.material.mainTextureScale.x * Convert.ToSingle((string) settings[79]),
                                        renderer2.material.mainTextureScale.y *
                                        Convert.ToSingle((string) settings[80]));
                                }
                            }
                            else
                            {
                                foreach (Renderer renderer2 in selectedObj.GetComponentsInChildren<Renderer>())
                                {
                                    if (!(renderer2.name.Contains("Particle System") &&
                                          selectedObj.name.Contains("aot_supply")))
                                    {
                                        renderer2.material = (Material) RCassets.Load((string) settings[69]);
                                        renderer2.material.mainTextureScale = new Vector2(
                                            renderer2.material.mainTextureScale.x *
                                            Convert.ToSingle((string) settings[79]),
                                            renderer2.material.mainTextureScale.y *
                                            Convert.ToSingle((string) settings[80]));
                                    }
                                }
                            }
                        }

                        y = 1f;
                        foreach (MeshFilter filter in selectedObj.GetComponentsInChildren<MeshFilter>())
                        {
                            if (selectedObj.name.StartsWith("customb"))
                            {
                                if (y < filter.mesh.bounds.size.y)
                                {
                                    y = filter.mesh.bounds.size.y;
                                }
                            }
                            else if (y < filter.mesh.bounds.size.z)
                            {
                                y = filter.mesh.bounds.size.z;
                            }
                        }

                        num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string) settings[70]);
                        num37 -= 0.001f;
                        num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string) settings[71]);
                        num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string) settings[72]);
                        selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                        if ((int) settings[76] == 1)
                        {
                            color = new Color((float) settings[73], (float) settings[74], (float) settings[75], a);
                            foreach (MeshFilter filter in selectedObj.GetComponentsInChildren<MeshFilter>())
                            {
                                mesh = filter.mesh;
                                colorArray = new Color[mesh.vertexCount];
                                num18 = 0;
                                while (num18 < mesh.vertexCount)
                                {
                                    colorArray[num18] = color;
                                    num18++;
                                }

                                mesh.colors = colorArray;
                            }
                        }

                        z = selectedObj.transform.localScale.z;
                        if (selectedObj.name.Contains("boulder2") || selectedObj.name.Contains("boulder3") ||
                            selectedObj.name.Contains("field2"))
                        {
                            z *= 0.01f;
                        }

                        num41 = 10f + z * y * 1.2f / 2f;
                        selectedObj.transform.position = new Vector3(
                            UnityEngine.Camera.main.transform.position.x + UnityEngine.Camera.main.transform.forward.x * num41,
                            UnityEngine.Camera.main.transform.position.y + UnityEngine.Camera.main.transform.forward.y * 10f,
                            UnityEngine.Camera.main.transform.position.z + UnityEngine.Camera.main.transform.forward.z * num41);
                        selectedObj.transform.rotation =
                            Quaternion.Euler(0f, UnityEngine.Camera.main.transform.rotation.eulerAngles.y, 0f);
                        name1 = selectedObj.name;
                        string[] strArray3 = new string[21];
                        strArray3[0] = name1;
                        strArray3[1] = ",";
                        strArray3[2] = (string) settings[69];
                        strArray3[3] = ",";
                        strArray3[4] = (string) settings[70];
                        strArray3[5] = ",";
                        strArray3[6] = (string) settings[71];
                        strArray3[7] = ",";
                        strArray3[8] = (string) settings[72];
                        strArray3[9] = ",";
                        strArray3[10] = settings[76].ToString();
                        strArray3[11] = ",";
                        float num42 = (float) settings[73];
                        strArray3[12] = num42.ToString(CultureInfo.CurrentCulture);
                        strArray3[13] = ",";
                        num42 = (float) settings[74];
                        strArray3[14] = num42.ToString(CultureInfo.CurrentCulture);
                        strArray3[15] = ",";
                        strArray3[16] = ((float) settings[75]).ToString(CultureInfo.CurrentCulture);
                        strArray3[17] = ",";
                        strArray3[18] = (string) settings[79];
                        strArray3[19] = ",";
                        strArray3[20] = (string) settings[80];
                        selectedObj.name = string.Concat(strArray3);
                        UnloadAssetsEditor();
                    }
                    else if (selectedObj.name.StartsWith("misc"))
                    {
                        if (selectedObj.name.Contains("barrier") || selectedObj.name.Contains("region") ||
                            selectedObj.name.Contains("racing"))
                        {
                            y = 1f;
                            num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string) settings[70]);
                            num37 -= 0.001f;
                            num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string) settings[71]);
                            num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string) settings[72]);
                            selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                            z = selectedObj.transform.localScale.z;
                            num41 = 10f + z * y * 1.2f / 2f;
                            selectedObj.transform.position = new Vector3(
                                UnityEngine.Camera.main.transform.position.x + UnityEngine.Camera.main.transform.forward.x * num41,
                                UnityEngine.Camera.main.transform.position.y + UnityEngine.Camera.main.transform.forward.y * 10f,
                                UnityEngine.Camera.main.transform.position.z + UnityEngine.Camera.main.transform.forward.z * num41);
                            if (!selectedObj.name.Contains("region"))
                            {
                                selectedObj.transform.rotation = Quaternion.Euler(0f,
                                    UnityEngine.Camera.main.transform.rotation.eulerAngles.y, 0f);
                            }

                            name1 = selectedObj.name;
                            selectedObj.name = name1 + "," + (string) settings[70] + "," + (string) settings[71] + "," +
                                               (string) settings[72];
                        }
                    }
                    else if (selectedObj.name.StartsWith("racing"))
                    {
                        y = 1f;
                        num37 = selectedObj.transform.localScale.x * Convert.ToSingle((string) settings[70]);
                        num37 -= 0.001f;
                        num38 = selectedObj.transform.localScale.y * Convert.ToSingle((string) settings[71]);
                        num39 = selectedObj.transform.localScale.z * Convert.ToSingle((string) settings[72]);
                        selectedObj.transform.localScale = new Vector3(num37, num38, num39);
                        z = selectedObj.transform.localScale.z;
                        num41 = 10f + z * y * 1.2f / 2f;
                        selectedObj.transform.position = new Vector3(
                            UnityEngine.Camera.main.transform.position.x + UnityEngine.Camera.main.transform.forward.x * num41,
                            UnityEngine.Camera.main.transform.position.y + UnityEngine.Camera.main.transform.forward.y * 10f,
                            UnityEngine.Camera.main.transform.position.z + UnityEngine.Camera.main.transform.forward.z * num41);
                        selectedObj.transform.rotation =
                            Quaternion.Euler(0f, UnityEngine.Camera.main.transform.rotation.eulerAngles.y, 0f);
                        name1 = selectedObj.name;
                        selectedObj.name = name1 + "," + (string) settings[70] + "," + (string) settings[71] + "," +
                                           (string) settings[72];
                    }
                    else
                    {
                        selectedObj.transform.position = new Vector3(
                            UnityEngine.Camera.main.transform.position.x + UnityEngine.Camera.main.transform.forward.x * 10f,
                            UnityEngine.Camera.main.transform.position.y + UnityEngine.Camera.main.transform.forward.y * 10f,
                            UnityEngine.Camera.main.transform.position.z + UnityEngine.Camera.main.transform.forward.z * 10f);
                        selectedObj.transform.rotation =
                            Quaternion.Euler(0f, UnityEngine.Camera.main.transform.rotation.eulerAngles.y, 0f);
                    }

                    Screen.lockCursor = true;
                    GUI.FocusControl(null);
                }
            }
            else
            {
                float num7;
                float num8;
                if (inputManager != null && inputManager.menuOn)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                    if ((int) settings[64] != 6)
                    {
                        num7 = Screen.width / 2f - 350f;
                        num8 = Screen.height / 2f - 250f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(num7 + 2f, num8 + 2f, 696f, 496f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7, num8, 700f, 500f), string.Empty);
                        if (GUI.Button(new Rect(num7 + 7f, num8 + 7f, 59f, 25f), "General", "box"))
                        {
                            settings[64] = 0;
                        }
                        else if (GUI.Button(new Rect(num7 + 71f, num8 + 7f, 60f, 25f), "Rebinds", "box"))
                        {
                            settings[64] = 1;
                        }
                        else if (GUI.Button(new Rect(num7 + 136f, num8 + 7f, 85f, 25f), "Human Skins", "box"))
                        {
                            settings[64] = 2;
                        }
                        else if (GUI.Button(new Rect(num7 + 226f, num8 + 7f, 85f, 25f), "Titan Skins", "box"))
                        {
                            settings[64] = 3;
                        }
                        else if (GUI.Button(new Rect(num7 + 316f, num8 + 7f, 85f, 25f), "Level Skins", "box"))
                        {
                            settings[64] = 7;
                        }
                        else if (GUI.Button(new Rect(num7 + 406f, num8 + 7f, 85f, 25f), "Custom Map", "box"))
                        {
                            settings[64] = 8;
                        }
                        else if (GUI.Button(new Rect(num7 + 496f, num8 + 7f, 88f, 25f), "Custom Logic", "box"))
                        {
                            settings[64] = 9;
                        }
                        else if (GUI.Button(new Rect(num7 + 589f, num8 + 7f, 95f, 25f), "Game Settings", "box"))
                        {
                            settings[64] = 10;
                        }
                        else if (GUI.Button(new Rect(num7 + 7f, num8 + 37f, 70f, 25f), "Abilities", "box"))
                        {
                            settings[64] = 11;
                        }

                        if ((int) settings[64] == 9)
                        {
                            currentScriptLogic = GUI.TextField(new Rect(num7 + 50f, num8 + 82f, 600f, 270f),
                                currentScriptLogic);
                            if (GUI.Button(new Rect(num7 + 250f, num8 + 365f, 50f, 20f), "Copy"))
                            {
                                editor = new TextEditor
                                {
                                    content = new GUIContent(currentScriptLogic)
                                };
                                editor.SelectAll();
                                editor.Copy();
                            }
                            else if (GUI.Button(new Rect(num7 + 400f, num8 + 365f, 50f, 20f), "Clear"))
                            {
                                currentScriptLogic = string.Empty;
                            }
                        }
                        else if ((int) settings[64] == 11)
                        {
                            GUI.Label(new Rect(num7 + 150f, num8 + 80f, 185f, 22f), "Bomb Mode", "Label");
                            GUI.Label(new Rect(num7 + 80f, num8 + 110f, 80f, 22f), "Color:", "Label");
                            textured = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                            textured.SetPixel(0, 0,
                                new Color((float) settings[246], (float) settings[247], (float) settings[248],
                                    (float) settings[249]));
                            textured.Apply();
                            GUI.DrawTexture(new Rect(num7 + 120f, num8 + 113f, 40f, 15f), textured,
                                ScaleMode.StretchToFill);
                            Destroy(textured);
                            GUI.Label(new Rect(num7 + 72f, num8 + 135f, 20f, 22f), "R:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 160f, 20f, 22f), "G:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 185f, 20f, 22f), "B:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 210f, 20f, 22f), "A:", "Label");
                            settings[246] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 138f, 100f, 20f),
                                (float) settings[246], 0f, 1f);
                            settings[247] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 163f, 100f, 20f),
                                (float) settings[247], 0f, 1f);
                            settings[248] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 188f, 100f, 20f),
                                (float) settings[248], 0f, 1f);
                            settings[249] = GUI.HorizontalSlider(new Rect(num7 + 92f, num8 + 213f, 100f, 20f),
                                (float) settings[249], 0.5f, 1f);
                            GUI.Label(new Rect(num7 + 72f, num8 + 235f, 95f, 22f), "Bomb Radius:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 260f, 95f, 22f), "Bomb Range:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 285f, 95f, 22f), "Bomb Speed:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 310f, 95f, 22f), "Bomb CD:", "Label");
                            GUI.Label(new Rect(num7 + 72f, num8 + 335f, 95f, 22f), "Unused Points:", "Label");
                            num30 = (int) settings[250];
                            GUI.Label(new Rect(num7 + 168f, num8 + 235f, 20f, 22f), num30.ToString(), "Label");
                            num30 = (int) settings[251];
                            GUI.Label(new Rect(num7 + 168f, num8 + 260f, 20f, 22f), num30.ToString(), "Label");
                            num30 = (int) settings[252];
                            GUI.Label(new Rect(num7 + 168f, num8 + 285f, 20f, 22f), num30.ToString(), "Label");
                            GUI.Label(new Rect(num7 + 168f, num8 + 310f, 20f, 22f), ((int) settings[253]).ToString(),
                                "Label");
                            int num43 = 20 - (int) settings[250] - (int) settings[251] - (int) settings[252] -
                                        (int) settings[253];
                            GUI.Label(new Rect(num7 + 168f, num8 + 335f, 20f, 22f), num43.ToString(), "Label");
                            if (GUI.Button(new Rect(num7 + 190f, num8 + 235f, 20f, 20f), "-"))
                            {
                                if ((int) settings[250] > 0)
                                {
                                    settings[250] = (int) settings[250] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 235f, 20f, 20f), "+") &&
                                     (int) settings[250] < 10 && num43 > 0)
                            {
                                settings[250] = (int) settings[250] + 1;
                            }

                            if (GUI.Button(new Rect(num7 + 190f, num8 + 260f, 20f, 20f), "-"))
                            {
                                if ((int) settings[251] > 0)
                                {
                                    settings[251] = (int) settings[251] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 260f, 20f, 20f), "+") &&
                                     (int) settings[251] < 10 && num43 > 0)
                            {
                                settings[251] = (int) settings[251] + 1;
                            }

                            if (GUI.Button(new Rect(num7 + 190f, num8 + 285f, 20f, 20f), "-"))
                            {
                                if ((int) settings[252] > 0)
                                {
                                    settings[252] = (int) settings[252] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 285f, 20f, 20f), "+") &&
                                     (int) settings[252] < 10 && num43 > 0)
                            {
                                settings[252] = (int) settings[252] + 1;
                            }

                            if (GUI.Button(new Rect(num7 + 190f, num8 + 310f, 20f, 20f), "-"))
                            {
                                if ((int) settings[253] > 0)
                                {
                                    settings[253] = (int) settings[253] - 1;
                                }
                            }
                            else if (GUI.Button(new Rect(num7 + 215f, num8 + 310f, 20f, 20f), "+") &&
                                     (int) settings[253] < 10 && num43 > 0)
                            {
                                settings[253] = (int) settings[253] + 1;
                            }
                        }
                        else
                        {
                            float num44;
                            switch ((int) settings[64])
                            {
                                case 2:
                                    GUI.Label(new Rect(num7 + 205f, num8 + 52f, 120f, 30f), "Human Skin Mode:",
                                        "Label");
                                    flag2 = (int) settings[0] == 1;
                                    flag5 = GUI.Toggle(new Rect(num7 + 325f, num8 + 52f, 40f, 20f), flag2, "On");
                                    if (flag2 != flag5)
                                    {
                                        if (flag5)
                                        {
                                            settings[0] = 1;
                                        }
                                        else
                                        {
                                            settings[0] = 0;
                                        }
                                    }

                                    num44 = 44f;
                                    switch ((int) settings[133])
                                    {
                                        case 0:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 1"))
                                            {
                                                settings[133] = 1;
                                            }

                                            settings[3] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[3]);
                                            settings[4] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[4]);
                                            settings[5] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[5]);
                                            settings[6] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[6]);
                                            settings[7] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[7]);
                                            settings[8] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[8]);
                                            settings[14] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f),
                                                    (string) settings[14]);
                                            settings[9] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[9]);
                                            settings[10] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[10]);
                                            settings[11] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[11]);
                                            settings[12] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[12]);
                                            settings[13] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[13]);
                                            settings[94] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[94]);
                                            break;
                                        case 1:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 2"))
                                            {
                                                settings[133] = 2;
                                            }

                                            settings[134] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[134]);
                                            settings[135] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[135]);
                                            settings[136] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[136]);
                                            settings[137] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[137]);
                                            settings[138] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[138]);
                                            settings[139] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[139]);
                                            settings[145] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f),
                                                    (string) settings[145]);
                                            settings[140] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[140]);
                                            settings[141] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[141]);
                                            settings[142] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[142]);
                                            settings[143] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[143]);
                                            settings[144] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[144]);
                                            settings[146] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[146]);
                                            break;
                                        case 2:
                                            if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Human Set 3"))
                                            {
                                                settings[133] = 0;
                                            }

                                            settings[147] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[147]);
                                            settings[148] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[148]);
                                            settings[149] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[149]);
                                            settings[150] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[150]);
                                            settings[151] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[151]);
                                            settings[152] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[152]);
                                            settings[158] =
                                                GUI.TextField(new Rect(num7 + 80f, num8 + 114f + num44 * 6f, 230f, 20f),
                                                    (string) settings[158]);
                                            settings[153] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 0f, 230f, 20f),
                                                    (string) settings[153]);
                                            settings[154] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 1f, 230f, 20f),
                                                    (string) settings[154]);
                                            settings[155] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 2f, 230f, 20f),
                                                    (string) settings[155]);
                                            settings[156] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 3f, 230f, 20f),
                                                    (string) settings[156]);
                                            settings[157] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 4f, 230f, 20f),
                                                    (string) settings[157]);
                                            settings[159] =
                                                GUI.TextField(
                                                    new Rect(num7 + 390f, num8 + 114f + num44 * 5f, 230f, 20f),
                                                    (string) settings[159]);
                                            break;
                                    }

                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 0f, 150f, 20f), "Horse:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 1f, 227f, 20f),
                                        "Hair (model dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 2f, 150f, 20f), "Eyes:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 3f, 240f, 20f),
                                        "Glass (must have a glass enabled):", "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 4f, 150f, 20f), "Face:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 5f, 150f, 20f), "Skin:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 6f, 240f, 20f),
                                        "Hoodie (costume dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 0f, 240f, 20f),
                                        "Costume (model dependent):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 1f, 150f, 20f), "Logo & Cape:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 2f, 240f, 20f),
                                        "3DMG Center & 3DMG/Blade/Gun(left):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 3f, 227f, 20f),
                                        "3DMG/Blade/Gun(right):", "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 4f, 150f, 20f), "Gas:",
                                        "Label");
                                    GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 5f, 150f, 20f),
                                        "Weapon Trail:", "Label");
                                    break;
                                case 3:
                                    int num45;
                                    int num46;
                                    GUI.Label(new Rect(num7 + 270f, num8 + 52f, 120f, 30f), "Titan Skin Mode:",
                                        "Label");
                                    flag6 = (int) settings[1] == 1;
                                    bool flag11 = GUI.Toggle(new Rect(num7 + 390f, num8 + 52f, 40f, 20f), flag6, "On");
                                    if (flag6 != flag11)
                                    {
                                        if (flag11)
                                        {
                                            settings[1] = 1;
                                        }
                                        else
                                        {
                                            settings[1] = 0;
                                        }
                                    }

                                    GUI.Label(new Rect(num7 + 270f, num8 + 77f, 120f, 30f), "Randomized Pairs:",
                                        "Label");
                                    flag6 = (int) settings[32] == 1;
                                    flag11 = GUI.Toggle(new Rect(num7 + 390f, num8 + 77f, 40f, 20f), flag6, "On");
                                    if (flag6 != flag11)
                                    {
                                        if (flag11)
                                        {
                                            settings[32] = 1;
                                        }
                                        else
                                        {
                                            settings[32] = 0;
                                        }
                                    }

                                    GUI.Label(new Rect(num7 + 158f, num8 + 112f, 150f, 20f), "Titan Hair:", "Label");
                                    settings[21] = GUI.TextField(new Rect(num7 + 80f, num8 + 134f, 165f, 20f),
                                        (string) settings[21]);
                                    settings[22] = GUI.TextField(new Rect(num7 + 80f, num8 + 156f, 165f, 20f),
                                        (string) settings[22]);
                                    settings[23] = GUI.TextField(new Rect(num7 + 80f, num8 + 178f, 165f, 20f),
                                        (string) settings[23]);
                                    settings[24] = GUI.TextField(new Rect(num7 + 80f, num8 + 200f, 165f, 20f),
                                        (string) settings[24]);
                                    settings[25] = GUI.TextField(new Rect(num7 + 80f, num8 + 222f, 165f, 20f),
                                        (string) settings[25]);
                                    if (GUI.Button(new Rect(num7 + 250f, num8 + 134f, 60f, 20f),
                                        GetHairType((int) settings[16])))
                                    {
                                        num45 = 16;
                                        num46 = (int) settings[16];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }

                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 156f, 60f, 20f),
                                        GetHairType((int) settings[17])))
                                    {
                                        num45 = 17;
                                        num46 = (int) settings[17];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }

                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 178f, 60f, 20f),
                                        GetHairType((int) settings[18])))
                                    {
                                        num45 = 18;
                                        num46 = (int) settings[18];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }

                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 200f, 60f, 20f),
                                        GetHairType((int) settings[19])))
                                    {
                                        num45 = 19;
                                        num46 = (int) settings[19];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }

                                        settings[num45] = num46;
                                    }
                                    else if (GUI.Button(new Rect(num7 + 250f, num8 + 222f, 60f, 20f),
                                        GetHairType((int) settings[20])))
                                    {
                                        num45 = 20;
                                        num46 = (int) settings[20];
                                        if (num46 >= 9)
                                        {
                                            num46 = -1;
                                        }
                                        else
                                        {
                                            num46++;
                                        }

                                        settings[num45] = num46;
                                    }

                                    GUI.Label(new Rect(num7 + 158f, num8 + 252f, 150f, 20f), "Titan Eye:", "Label");
                                    settings[26] = GUI.TextField(new Rect(num7 + 80f, num8 + 274f, 230f, 20f),
                                        (string) settings[26]);
                                    settings[27] = GUI.TextField(new Rect(num7 + 80f, num8 + 296f, 230f, 20f),
                                        (string) settings[27]);
                                    settings[28] = GUI.TextField(new Rect(num7 + 80f, num8 + 318f, 230f, 20f),
                                        (string) settings[28]);
                                    settings[29] = GUI.TextField(new Rect(num7 + 80f, num8 + 340f, 230f, 20f),
                                        (string) settings[29]);
                                    settings[30] = GUI.TextField(new Rect(num7 + 80f, num8 + 362f, 230f, 20f),
                                        (string) settings[30]);
                                    GUI.Label(new Rect(num7 + 455f, num8 + 112f, 150f, 20f), "Titan Body:", "Label");
                                    settings[86] = GUI.TextField(new Rect(num7 + 390f, num8 + 134f, 230f, 20f),
                                        (string) settings[86]);
                                    settings[87] = GUI.TextField(new Rect(num7 + 390f, num8 + 156f, 230f, 20f),
                                        (string) settings[87]);
                                    settings[88] = GUI.TextField(new Rect(num7 + 390f, num8 + 178f, 230f, 20f),
                                        (string) settings[88]);
                                    settings[89] = GUI.TextField(new Rect(num7 + 390f, num8 + 200f, 230f, 20f),
                                        (string) settings[89]);
                                    settings[90] = GUI.TextField(new Rect(num7 + 390f, num8 + 222f, 230f, 20f),
                                        (string) settings[90]);
                                    GUI.Label(new Rect(num7 + 472f, num8 + 252f, 150f, 20f), "Eren:", "Label");
                                    settings[65] = GUI.TextField(new Rect(num7 + 390f, num8 + 274f, 230f, 20f),
                                        (string) settings[65]);
                                    GUI.Label(new Rect(num7 + 470f, num8 + 296f, 150f, 20f), "Annie:", "Label");
                                    settings[66] = GUI.TextField(new Rect(num7 + 390f, num8 + 318f, 230f, 20f),
                                        (string) settings[66]);
                                    GUI.Label(new Rect(num7 + 465f, num8 + 340f, 150f, 20f), "Colossal:", "Label");
                                    settings[67] = GUI.TextField(new Rect(num7 + 390f, num8 + 362f, 230f, 20f),
                                        (string) settings[67]);
                                    break;
                                case 7:
                                    num44 = 22f;
                                    GUI.Label(new Rect(num7 + 205f, num8 + 52f, 145f, 30f), "Level Skin Mode:",
                                        "Label");
                                    bool flag12 = (int) settings[2] == 1;
                                    bool flag13 = GUI.Toggle(new Rect(num7 + 325f, num8 + 52f, 40f, 20f), flag12, "On");
                                    if (flag12 != flag13)
                                    {
                                        if (flag13)
                                        {
                                            settings[2] = 1;
                                        }
                                        else
                                        {
                                            settings[2] = 0;
                                        }
                                    }

                                    if ((int) settings[188] == 0)
                                    {
                                        if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "Forest Skins"))
                                        {
                                            settings[188] = 1;
                                        }

                                        GUI.Label(new Rect(num7 + 205f, num8 + 77f, 145f, 30f), "Randomized Pairs:",
                                            "Label");
                                        flag12 = (int) settings[50] == 1;
                                        flag13 = GUI.Toggle(new Rect(num7 + 325f, num8 + 77f, 40f, 20f), flag12, "On");
                                        if (flag12 != flag13)
                                        {
                                            if (flag13)
                                            {
                                                settings[50] = 1;
                                            }
                                            else
                                            {
                                                settings[50] = 0;
                                            }
                                        }

                                        scroll = GUI.BeginScrollView(new Rect(num7, num8 + 115f, 712f, 340f), scroll,
                                            new Rect(num7, num8 + 115f, 700f, 475f), true, true);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 0f, 150f, 20f), "Ground:",
                                            "Label");
                                        settings[49] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 1f, 227f, 20f),
                                                (string) settings[49]);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 2f, 150f, 20f),
                                            "Forest Trunks", "Label");
                                        settings[33] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 3f, 227f, 20f),
                                                (string) settings[33]);
                                        settings[34] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 4f, 227f, 20f),
                                                (string) settings[34]);
                                        settings[35] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 5f, 227f, 20f),
                                                (string) settings[35]);
                                        settings[36] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 6f, 227f, 20f),
                                                (string) settings[36]);
                                        settings[37] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 7f, 227f, 20f),
                                                (string) settings[37]);
                                        settings[38] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 8f, 227f, 20f),
                                                (string) settings[38]);
                                        settings[39] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 9f, 227f, 20f),
                                                (string) settings[39]);
                                        settings[40] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 10f, 227f, 20f),
                                                (string) settings[40]);
                                        GUI.Label(new Rect(num7 + 79f, num8 + 117f + num44 * 11f, 150f, 20f),
                                            "Forest Leaves:", "Label");
                                        settings[41] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 12f, 227f, 20f),
                                                (string) settings[41]);
                                        settings[42] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 13f, 227f, 20f),
                                                (string) settings[42]);
                                        settings[43] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 14f, 227f, 20f),
                                                (string) settings[43]);
                                        settings[44] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 15f, 227f, 20f),
                                                (string) settings[44]);
                                        settings[45] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 16f, 227f, 20f),
                                                (string) settings[45]);
                                        settings[46] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 17f, 227f, 20f),
                                                (string) settings[46]);
                                        settings[47] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 18f, 227f, 20f),
                                                (string) settings[47]);
                                        settings[48] =
                                            GUI.TextField(new Rect(num7 + 79f, num8 + 117f + num44 * 19f, 227f, 20f),
                                                (string) settings[48]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 0f, 150f, 20f),
                                            "Skybox Front:", "Label");
                                        settings[163] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 1f, 227f, 20f),
                                                (string) settings[163]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 2f, 150f, 20f),
                                            "Skybox Back:", "Label");
                                        settings[164] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 3f, 227f, 20f),
                                                (string) settings[164]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 4f, 150f, 20f),
                                            "Skybox Left:", "Label");
                                        settings[165] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 5f, 227f, 20f),
                                                (string) settings[165]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 6f, 150f, 20f),
                                            "Skybox Right:", "Label");
                                        settings[166] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 7f, 227f, 20f),
                                                (string) settings[166]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 8f, 150f, 20f),
                                            "Skybox Up:", "Label");
                                        settings[167] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 9f, 227f, 20f),
                                                (string) settings[167]);
                                        GUI.Label(new Rect(num7 + 379f, num8 + 117f + num44 * 10f, 150f, 20f),
                                            "Skybox Down:", "Label");
                                        settings[168] =
                                            GUI.TextField(new Rect(num7 + 379f, num8 + 117f + num44 * 11f, 227f, 20f),
                                                (string) settings[168]);
                                        GUI.EndScrollView();
                                    }
                                    else if ((int) settings[188] == 1)
                                    {
                                        if (GUI.Button(new Rect(num7 + 375f, num8 + 51f, 120f, 22f), "City Skins"))
                                        {
                                            settings[188] = 0;
                                        }

                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 0f, 150f, 20f), "Ground:",
                                            "Label");
                                        settings[59] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 1f, 230f, 20f),
                                                (string) settings[59]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 2f, 150f, 20f), "Wall:",
                                            "Label");
                                        settings[60] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 3f, 230f, 20f),
                                                (string) settings[60]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 4f, 150f, 20f), "Gate:",
                                            "Label");
                                        settings[61] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 5f, 230f, 20f),
                                                (string) settings[61]);
                                        GUI.Label(new Rect(num7 + 80f, num8 + 92f + num44 * 6f, 150f, 20f), "Houses:",
                                            "Label");
                                        settings[51] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 7f, 230f, 20f),
                                                (string) settings[51]);
                                        settings[52] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 8f, 230f, 20f),
                                                (string) settings[52]);
                                        settings[53] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 9f, 230f, 20f),
                                                (string) settings[53]);
                                        settings[54] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 10f, 230f, 20f),
                                                (string) settings[54]);
                                        settings[55] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 11f, 230f, 20f),
                                                (string) settings[55]);
                                        settings[56] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 12f, 230f, 20f),
                                                (string) settings[56]);
                                        settings[57] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 13f, 230f, 20f),
                                                (string) settings[57]);
                                        settings[58] =
                                            GUI.TextField(new Rect(num7 + 80f, num8 + 92f + num44 * 14f, 230f, 20f),
                                                (string) settings[58]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 0f, 150f, 20f),
                                            "Skybox Front:", "Label");
                                        settings[169] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 1f, 230f, 20f),
                                                (string) settings[169]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 2f, 150f, 20f),
                                            "Skybox Back:", "Label");
                                        settings[170] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 3f, 230f, 20f),
                                                (string) settings[170]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 4f, 150f, 20f),
                                            "Skybox Left:", "Label");
                                        settings[171] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 5f, 230f, 20f),
                                                (string) settings[171]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 6f, 150f, 20f),
                                            "Skybox Right:", "Label");
                                        settings[172] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 7f, 230f, 20f),
                                                (string) settings[172]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 8f, 150f, 20f),
                                            "Skybox Up:", "Label");
                                        settings[173] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 9f, 230f, 20f),
                                                (string) settings[173]);
                                        GUI.Label(new Rect(num7 + 390f, num8 + 92f + num44 * 10f, 150f, 20f),
                                            "Skybox Down:", "Label");
                                        settings[174] =
                                            GUI.TextField(new Rect(num7 + 390f, num8 + 92f + num44 * 11f, 230f, 20f),
                                                (string) settings[174]);
                                    }

                                    break;
                                case 4:
                                    GUI.TextArea(new Rect(num7 + 80f, num8 + 52f, 270f, 30f),
                                        "Settings saved to playerprefs!", 100, "Label");
                                    break;
                                case 5:
                                    GUI.TextArea(new Rect(num7 + 80f, num8 + 52f, 270f, 30f),
                                        "Settings reloaded from playerprefs!", 100, "Label");
                                    break;
                                default:
                                    string[] strArray16;
                                    if ((int) settings[64] == 0)
                                    {
                                        bool flag19;
                                        bool flag20;
                                        bool flag21;
                                        bool flag22;
                                        bool flag25;
                                        bool flag32;
                                        GUI.Label(new Rect(num7 + 150f, num8 + 51f, 185f, 22f), "Graphics", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 81f, 185f, 22f),
                                            "Disable custom gas textures:", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 106f, 185f, 22f), "Disable weapon trail:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 131f, 185f, 22f), "Disable wind effect:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 156f, 185f, 22f), "Enable vSync:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 184f, 227f, 20f),
                                            "FPS Cap (0 for disabled):", "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 212f, 150f, 22f), "Texture Quality:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 242f, 150f, 22f), "Overall Quality:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 272f, 185f, 22f), "Disable Mipmapping:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 72f, num8 + 297f, 185f, 65f),
                                            "*Disabling mipmapping will increase custom texture quality at the cost of performance.",
                                            "Label");
                                        qualitySlider = GUI.HorizontalSlider(
                                            new Rect(num7 + 199f, num8 + 247f, 115f, 20f), qualitySlider, 0f, 1f);
                                        PlayerPrefs.SetFloat("GameQuality", qualitySlider);
                                        if (qualitySlider < 0.167f)
                                        {
                                            QualitySettings.SetQualityLevel(0, true);
                                        }
                                        else if (qualitySlider < 0.33f)
                                        {
                                            QualitySettings.SetQualityLevel(1, true);
                                        }
                                        else if (qualitySlider < 0.5f)
                                        {
                                            QualitySettings.SetQualityLevel(2, true);
                                        }
                                        else if (qualitySlider < 0.67f)
                                        {
                                            QualitySettings.SetQualityLevel(3, true);
                                        }
                                        else if (qualitySlider < 0.83f)
                                        {
                                            QualitySettings.SetQualityLevel(4, true);
                                        }
                                        else if (qualitySlider <= 1f)
                                        {
                                            QualitySettings.SetQualityLevel(5, true);
                                        }

                                        if (!(qualitySlider < 0.9f || Level.StartsWith("Custom")))
                                        {
                                            UnityEngine.Camera.main.GetComponent<TiltShift>().enabled = true;
                                        }
                                        else
                                        {
                                            UnityEngine.Camera.main.GetComponent<TiltShift>().enabled = false;
                                        }

                                        bool flag14 = false;
                                        bool flag15 = false;
                                        bool flag16 = false;
                                        bool flag17 = false;
                                        bool flag18 = false;
                                        if ((int) settings[15] == 1)
                                        {
                                            flag14 = true;
                                        }

                                        if ((int) settings[92] == 1)
                                        {
                                            flag15 = true;
                                        }

                                        if ((int) settings[93] == 1)
                                        {
                                            flag16 = true;
                                        }

                                        if ((int) settings[63] == 1)
                                        {
                                            flag17 = true;
                                        }

                                        if ((int) settings[183] == 1)
                                        {
                                            flag18 = true;
                                        }

                                        if ((flag19 = GUI.Toggle(new Rect(num7 + 274f, num8 + 81f, 40f, 20f), flag14,
                                                "On")) != flag14)
                                        {
                                            if (flag19)
                                            {
                                                settings[15] = 1;
                                            }
                                            else
                                            {
                                                settings[15] = 0;
                                            }
                                        }

                                        if ((flag10 = GUI.Toggle(new Rect(num7 + 274f, num8 + 106f, 40f, 20f), flag15,
                                                "On")) != flag15)
                                        {
                                            if (flag10)
                                            {
                                                settings[92] = 1;
                                            }
                                            else
                                            {
                                                settings[92] = 0;
                                            }
                                        }

                                        if ((flag20 = GUI.Toggle(new Rect(num7 + 274f, num8 + 131f, 40f, 20f), flag16,
                                                "On")) != flag16)
                                        {
                                            if (flag20)
                                            {
                                                settings[93] = 1;
                                            }
                                            else
                                            {
                                                settings[93] = 0;
                                            }
                                        }

                                        if ((flag21 = GUI.Toggle(new Rect(num7 + 274f, num8 + 156f, 40f, 20f), flag18,
                                                "On")) != flag18)
                                        {
                                            if (flag21)
                                            {
                                                settings[183] = 1;
                                                QualitySettings.vSyncCount = 1;
                                            }
                                            else
                                            {
                                                settings[183] = 0;
                                                QualitySettings.vSyncCount = 0;
                                            }

                                            Minimap.WaitAndTryRecaptureInstance(0.5f);
                                        }

                                        if ((flag22 = GUI.Toggle(new Rect(num7 + 274f, num8 + 272f, 40f, 20f), flag17,
                                                "On")) != flag17)
                                        {
                                            if (flag22)
                                            {
                                                settings[63] = 1;
                                            }
                                            else
                                            {
                                                settings[63] = 0;
                                            }

                                            linkHash[0].Clear();
                                            linkHash[1].Clear();
                                            linkHash[2].Clear();
                                        }

                                        if (GUI.Button(new Rect(num7 + 254f, num8 + 212f, 60f, 20f),
                                            QualityToString(QualitySettings.masterTextureLimit)))
                                        {
                                            if (QualitySettings.masterTextureLimit <= 0)
                                            {
                                                QualitySettings.masterTextureLimit = 2;
                                            }
                                            else
                                            {
                                                QualitySettings.masterTextureLimit--;
                                            }

                                            linkHash[0].Clear();
                                            linkHash[1].Clear();
                                            linkHash[2].Clear();
                                        }

                                        settings[184] = GUI.TextField(new Rect(num7 + 234f, num8 + 184f, 80f, 20f),
                                            (string) settings[184]);
                                        Application.targetFrameRate = -1;
                                        if (int.TryParse((string) settings[184], out var num47) && num47 > 0)
                                        {
                                            Application.targetFrameRate = num47;
                                        }

                                        GUI.Label(new Rect(num7 + 470f, num8 + 51f, 185f, 22f), "Snapshots", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 81f, 185f, 22f), "Enable Snapshots:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 106f, 185f, 22f), "Show In Game:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 131f, 227f, 22f),
                                            "Snapshot Minimum Damage:", "Label");
                                        settings[95] = GUI.TextField(new Rect(num7 + 563f, num8 + 131f, 65f, 20f),
                                            (string) settings[95]);
                                        bool flag23 = false;
                                        bool flag24 = false;
                                        if (PlayerPrefs.GetInt("EnableSS", 0) == 1)
                                        {
                                            flag23 = true;
                                        }

                                        if (PlayerPrefs.GetInt("showSSInGame", 0) == 1)
                                        {
                                            flag24 = true;
                                        }

                                        if ((flag25 = GUI.Toggle(new Rect(num7 + 588f, num8 + 81f, 40f, 20f), flag23,
                                                "On")) != flag23)
                                        {
                                            if (flag25)
                                            {
                                                PlayerPrefs.SetInt("EnableSS", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("EnableSS", 0);
                                            }
                                        }

                                        bool flag26 = GUI.Toggle(new Rect(num7 + 588f, num8 + 106f, 40f, 20f), flag24,
                                            "On");
                                        if (flag24 != flag26)
                                        {
                                            if (flag26)
                                            {
                                                PlayerPrefs.SetInt("showSSInGame", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("showSSInGame", 0);
                                            }
                                        }

                                        GUI.Label(new Rect(num7 + 485f, num8 + 161f, 185f, 22f), "Other", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 186f, 80f, 20f), "Volume:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 211f, 95f, 20f), "Mouse Speed:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 236f, 95f, 20f), "Camera Dist:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 261f, 80f, 20f), "Camera Tilt:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 283f, 80f, 20f), "Invert Mouse:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 305f, 80f, 20f), "Speedometer:",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 375f, 80f, 20f), "Minimap:", "Label");
                                        GUI.Label(new Rect(num7 + 386f, num8 + 397f, 100f, 20f), "Game Feed:", "Label");
                                        strArray16 = new[] {"Off", "Speed", "Damage"};
                                        settings[189] = GUI.SelectionGrid(new Rect(num7 + 480f, num8 + 305f, 140f, 60f),
                                            (int) settings[189], strArray16, 1, GUI.skin.toggle);
                                        AudioListener.volume = GUI.HorizontalSlider(
                                            new Rect(num7 + 478f, num8 + 191f, 150f, 20f), AudioListener.volume, 0f,
                                            1f);
                                        mouseSlider = GUI.HorizontalSlider(
                                            new Rect(num7 + 478f, num8 + 216f, 150f, 20f), mouseSlider, 0.1f, 1f);
                                        PlayerPrefs.SetFloat("MouseSensitivity", mouseSlider);
                                        IN_GAME_MAIN_CAMERA.sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
                                        distanceSlider = GUI.HorizontalSlider(
                                            new Rect(num7 + 478f, num8 + 241f, 150f, 20f), distanceSlider, 0f, 1f);
                                        PlayerPrefs.SetFloat("cameraDistance", distanceSlider);
                                        IN_GAME_MAIN_CAMERA.cameraDistance = 0.3f + distanceSlider;
                                        bool flag27 = false;
                                        bool flag28 = false;
                                        bool flag29 = false;
                                        bool flag30 = false;
                                        if ((int) settings[231] == 1)
                                        {
                                            flag29 = true;
                                        }

                                        if ((int) settings[244] == 1)
                                        {
                                            flag30 = true;
                                        }

                                        if (PlayerPrefs.HasKey("cameraTilt"))
                                        {
                                            if (PlayerPrefs.GetInt("cameraTilt") == 1)
                                            {
                                                flag27 = true;
                                            }
                                        }
                                        else
                                        {
                                            PlayerPrefs.SetInt("cameraTilt", 1);
                                        }

                                        if (PlayerPrefs.HasKey("invertMouseY"))
                                        {
                                            if (PlayerPrefs.GetInt("invertMouseY") == -1)
                                            {
                                                flag28 = true;
                                            }
                                        }
                                        else
                                        {
                                            PlayerPrefs.SetInt("invertMouseY", 1);
                                        }

                                        bool flag31 = GUI.Toggle(new Rect(num7 + 480f, num8 + 261f, 40f, 20f), flag27,
                                            "On");
                                        if (flag27 != flag31)
                                        {
                                            if (flag31)
                                            {
                                                PlayerPrefs.SetInt("cameraTilt", 1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("cameraTilt", 0);
                                            }
                                        }

                                        if ((flag32 = GUI.Toggle(new Rect(num7 + 480f, num8 + 283f, 40f, 20f), flag28,
                                                "On")) != flag28)
                                        {
                                            if (flag32)
                                            {
                                                PlayerPrefs.SetInt("invertMouseY", -1);
                                            }
                                            else
                                            {
                                                PlayerPrefs.SetInt("invertMouseY", 1);
                                            }
                                        }

                                        bool flag33 = GUI.Toggle(new Rect(num7 + 480f, num8 + 375f, 40f, 20f), flag29,
                                            "On");
                                        if (flag29 != flag33)
                                        {
                                            if (flag33)
                                            {
                                                settings[231] = 1;
                                            }
                                            else
                                            {
                                                settings[231] = 0;
                                            }
                                        }

                                        bool flag34 = GUI.Toggle(new Rect(num7 + 480f, num8 + 397f, 40f, 20f), flag30,
                                            "On");
                                        if (flag30 != flag34)
                                        {
                                            if (flag34)
                                            {
                                                settings[244] = 1;
                                            }
                                            else
                                            {
                                                settings[244] = 0;
                                            }
                                        }

                                        IN_GAME_MAIN_CAMERA.cameraTilt = PlayerPrefs.GetInt("cameraTilt");
                                        IN_GAME_MAIN_CAMERA.invertY = PlayerPrefs.GetInt("invertMouseY");
                                    }
                                    else if ((int) settings[64] == 10)
                                    {
                                        bool flag35;
                                        bool flag36;
                                        GUI.Label(new Rect(num7 + 200f, num8 + 382f, 400f, 22f),
                                            "Master Client only. Changes will take effect upon restart.");
                                        if (GUI.Button(new Rect(num7 + 267.5f, num8 + 50f, 60f, 25f), "Titans"))
                                        {
                                            settings[230] = 0;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 332.5f, num8 + 50f, 40f, 25f), "PVP"))
                                        {
                                            settings[230] = 1;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 377.5f, num8 + 50f, 50f, 25f), "Misc"))
                                        {
                                            settings[230] = 2;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 320f, num8 + 415f, 60f, 30f), "Reset"))
                                        {
                                            settings[192] = 0;
                                            settings[193] = 0;
                                            settings[194] = 0;
                                            settings[195] = 0;
                                            settings[196] = "30";
                                            settings[197] = 0;
                                            settings[198] = "100";
                                            settings[199] = "200";
                                            settings[200] = 0;
                                            settings[201] = "1";
                                            settings[202] = 0;
                                            settings[203] = 0;
                                            settings[204] = "1";
                                            settings[205] = 0;
                                            settings[206] = "1000";
                                            settings[207] = 0;
                                            settings[208] = "1.0";
                                            settings[209] = "3.0";
                                            settings[210] = 0;
                                            settings[211] = "20.0";
                                            settings[212] = "20.0";
                                            settings[213] = "20.0";
                                            settings[214] = "20.0";
                                            settings[215] = "20.0";
                                            settings[216] = 0;
                                            settings[217] = 0;
                                            settings[218] = "1";
                                            settings[219] = 0;
                                            settings[220] = 0;
                                            settings[221] = 0;
                                            settings[222] = "20";
                                            settings[223] = 0;
                                            settings[224] = "10";
                                            settings[225] = string.Empty;
                                            settings[226] = 0;
                                            settings[227] = "50";
                                            settings[228] = 0;
                                            settings[229] = 0;
                                            settings[235] = 0;
                                        }

                                        if ((int) settings[230] == 0)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f),
                                                "Custom Titan Number:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 200f, 22f),
                                                "Amount (Integer):", "Label");
                                            settings[204] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f),
                                                (string) settings[204]);
                                            flag35 = (int) settings[203] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[203] = 1;
                                                }
                                                else
                                                {
                                                    settings[203] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f),
                                                "Custom Titan Spawns:", "Label");
                                            flag35 = (int) settings[210] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[210] = 1;
                                                }
                                                else
                                                {
                                                    settings[210] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 174f, 150f, 22f),
                                                "Normal (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 196f, 150f, 22f),
                                                "Aberrant (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 218f, 150f, 22f),
                                                "Jumper (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 240f, 150f, 22f),
                                                "Crawler (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 262f, 150f, 22f), "Punk (Decimal):",
                                                "Label");
                                            settings[211] = GUI.TextField(new Rect(num7 + 250f, num8 + 174f, 50f, 22f),
                                                (string) settings[211]);
                                            settings[212] = GUI.TextField(new Rect(num7 + 250f, num8 + 196f, 50f, 22f),
                                                (string) settings[212]);
                                            settings[213] = GUI.TextField(new Rect(num7 + 250f, num8 + 218f, 50f, 22f),
                                                (string) settings[213]);
                                            settings[214] = GUI.TextField(new Rect(num7 + 250f, num8 + 240f, 50f, 22f),
                                                (string) settings[214]);
                                            settings[215] = GUI.TextField(new Rect(num7 + 250f, num8 + 262f, 50f, 22f),
                                                (string) settings[215]);
                                            GUI.Label(new Rect(num7 + 100f, num8 + 302f, 160f, 22f), "Titan Size Mode:",
                                                "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 324f, 150f, 22f),
                                                "Minimum (Decimal):", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 346f, 150f, 22f),
                                                "Maximum (Decimal):", "Label");
                                            settings[208] = GUI.TextField(new Rect(num7 + 250f, num8 + 324f, 50f, 22f),
                                                (string) settings[208]);
                                            settings[209] = GUI.TextField(new Rect(num7 + 250f, num8 + 346f, 50f, 22f),
                                                (string) settings[209]);
                                            flag35 = (int) settings[207] == 1;
                                            if ((flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 302f, 40f, 20f),
                                                    flag35, "On")) != flag35)
                                            {
                                                if (flag36)
                                                {
                                                    settings[207] = 1;
                                                }
                                                else
                                                {
                                                    settings[207] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f),
                                                "Titan Health Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 161f, 150f, 22f),
                                                "Minimum (Integer):", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 183f, 150f, 22f),
                                                "Maximum (Integer):", "Label");
                                            settings[198] = GUI.TextField(new Rect(num7 + 550f, num8 + 161f, 50f, 22f),
                                                (string) settings[198]);
                                            settings[199] = GUI.TextField(new Rect(num7 + 550f, num8 + 183f, 50f, 22f),
                                                (string) settings[199]);
                                            strArray16 = new[] {"Off", "Fixed", "Scaled"};
                                            settings[197] = GUI.SelectionGrid(
                                                new Rect(num7 + 550f, num8 + 90f, 100f, 66f), (int) settings[197],
                                                strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 400f, num8 + 223f, 160f, 22f),
                                                "Titan Damage Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 245f, 150f, 22f),
                                                "Damage (Integer):", "Label");
                                            settings[206] = GUI.TextField(new Rect(num7 + 550f, num8 + 245f, 50f, 22f),
                                                (string) settings[206]);
                                            flag35 = (int) settings[205] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 223f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[205] = 1;
                                                }
                                                else
                                                {
                                                    settings[205] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 285f, 160f, 22f),
                                                "Titan Explode Mode:", "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 307f, 160f, 22f),
                                                "Radius (Integer):", "Label");
                                            settings[196] = GUI.TextField(new Rect(num7 + 550f, num8 + 307f, 50f, 22f),
                                                (string) settings[196]);
                                            flag35 = (int) settings[195] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 285f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[195] = 1;
                                                }
                                                else
                                                {
                                                    settings[195] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 347f, 160f, 22f),
                                                "Disable Rock Throwing:", "Label");
                                            flag35 = (int) settings[194] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 347f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[194] = 1;
                                                }
                                                else
                                                {
                                                    settings[194] = 0;
                                                }
                                            }
                                        }
                                        else if ((int) settings[230] == 1)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f), "Point Mode:",
                                                "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 160f, 22f),
                                                "Max Points (Integer):", "Label");
                                            settings[227] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f),
                                                (string) settings[227]);
                                            flag35 = (int) settings[226] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[226] = 1;
                                                }
                                                else
                                                {
                                                    settings[226] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f), "PVP Bomb Mode:",
                                                "Label");
                                            flag35 = (int) settings[192] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[192] = 1;
                                                }
                                                else
                                                {
                                                    settings[192] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 182f, 100f, 66f), "Team Mode:",
                                                "Label");
                                            strArray16 = new[] {"Off", "No Sort", "Size-Lock", "Skill-Lock"};
                                            settings[193] = GUI.SelectionGrid(
                                                new Rect(num7 + 250f, num8 + 182f, 120f, 88f), (int) settings[193],
                                                strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 100f, num8 + 278f, 160f, 22f), "Infection Mode:",
                                                "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 300f, 160f, 22f),
                                                "Starting Titans (Integer):", "Label");
                                            settings[201] = GUI.TextField(new Rect(num7 + 250f, num8 + 300f, 50f, 22f),
                                                (string) settings[201]);
                                            flag35 = (int) settings[200] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 278f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[200] = 1;
                                                }
                                                else
                                                {
                                                    settings[200] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 330f, 160f, 22f), "Friendly Mode:",
                                                "Label");
                                            flag35 = (int) settings[219] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 330f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[219] = 1;
                                                }
                                                else
                                                {
                                                    settings[219] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f), "Sword/AHSS PVP:",
                                                "Label");
                                            strArray16 = new[] {"Off", "Teams", "FFA"};
                                            settings[220] = GUI.SelectionGrid(
                                                new Rect(num7 + 550f, num8 + 90f, 100f, 66f), (int) settings[220],
                                                strArray16, 1, GUI.skin.toggle);
                                            GUI.Label(new Rect(num7 + 400f, num8 + 164f, 160f, 22f),
                                                "No AHSS Air-Reloading:", "Label");
                                            flag35 = (int) settings[228] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 164f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[228] = 1;
                                                }
                                                else
                                                {
                                                    settings[228] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 194f, 160f, 22f),
                                                "Cannons kill humans:", "Label");
                                            flag35 = (int) settings[261] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 194f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[261] = 1;
                                                }
                                                else
                                                {
                                                    settings[261] = 0;
                                                }
                                            }
                                        }
                                        else if ((int) settings[230] == 2)
                                        {
                                            GUI.Label(new Rect(num7 + 100f, num8 + 90f, 160f, 22f),
                                                "Custom Titans/Wave:", "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 112f, 160f, 22f),
                                                "Amount (Integer):", "Label");
                                            settings[218] = GUI.TextField(new Rect(num7 + 250f, num8 + 112f, 50f, 22f),
                                                (string) settings[218]);
                                            flag35 = (int) settings[217] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 90f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[217] = 1;
                                                }
                                                else
                                                {
                                                    settings[217] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 152f, 160f, 22f), "Maximum Waves:",
                                                "Label");
                                            GUI.Label(new Rect(num7 + 100f, num8 + 174f, 160f, 22f),
                                                "Amount (Integer):", "Label");
                                            settings[222] = GUI.TextField(new Rect(num7 + 250f, num8 + 174f, 50f, 22f),
                                                (string) settings[222]);
                                            flag35 = (int) settings[221] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 152f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[221] = 1;
                                                }
                                                else
                                                {
                                                    settings[221] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 214f, 160f, 22f),
                                                "Punks every 5 waves:", "Label");
                                            flag35 = (int) settings[229] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 214f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[229] = 1;
                                                }
                                                else
                                                {
                                                    settings[229] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 100f, num8 + 244f, 160f, 22f),
                                                "Global Minimap Disable:", "Label");
                                            flag35 = (int) settings[235] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 250f, num8 + 244f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[235] = 1;
                                                }
                                                else
                                                {
                                                    settings[235] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 90f, 160f, 22f), "Endless Respawn:",
                                                "Label");
                                            GUI.Label(new Rect(num7 + 400f, num8 + 112f, 160f, 22f),
                                                "Respawn Time (Integer):", "Label");
                                            settings[224] = GUI.TextField(new Rect(num7 + 550f, num8 + 112f, 50f, 22f),
                                                (string) settings[224]);
                                            flag35 = (int) settings[223] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 90f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[223] = 1;
                                                }
                                                else
                                                {
                                                    settings[223] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 152f, 160f, 22f), "Kick Eren Titan:",
                                                "Label");
                                            flag35 = (int) settings[202] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 152f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[202] = 1;
                                                }
                                                else
                                                {
                                                    settings[202] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 182f, 160f, 22f), "Allow Horses:",
                                                "Label");
                                            flag35 = (int) settings[216] == 1;
                                            flag36 = GUI.Toggle(new Rect(num7 + 550f, num8 + 182f, 40f, 20f), flag35,
                                                "On");
                                            if (flag35 != flag36)
                                            {
                                                if (flag36)
                                                {
                                                    settings[216] = 1;
                                                }
                                                else
                                                {
                                                    settings[216] = 0;
                                                }
                                            }

                                            GUI.Label(new Rect(num7 + 400f, num8 + 212f, 180f, 22f),
                                                "Message of the day:", "Label");
                                            settings[225] = GUI.TextField(new Rect(num7 + 400f, num8 + 234f, 200f, 22f),
                                                (string) settings[225]);
                                        }
                                    }
                                    else if ((int) settings[64] == 1)
                                    {
                                        List<string> list7;
                                        float num48;
                                        if (GUI.Button(new Rect(num7 + 233f, num8 + 51f, 55f, 25f), "Human"))
                                        {
                                            settings[190] = 0;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 293f, num8 + 51f, 52f, 25f), "Titan"))
                                        {
                                            settings[190] = 1;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 350f, num8 + 51f, 53f, 25f), "Horse"))
                                        {
                                            settings[190] = 2;
                                        }
                                        else if (GUI.Button(new Rect(num7 + 408f, num8 + 51f, 59f, 25f), "Cannon"))
                                        {
                                            settings[190] = 3;
                                        }

                                        if ((int) settings[190] == 0)
                                        {
                                            list7 = new List<string>
                                            {
                                                "Forward:",
                                                "Backward:",
                                                "Left:",
                                                "Right:",
                                                "Jump:",
                                                "Dodge:",
                                                "Left Hook:",
                                                "Right Hook:",
                                                "Both Hooks:",
                                                "Lock:",
                                                "Attack:",
                                                "Special:",
                                                "Salute:",
                                                "Change Camera:",
                                                "Reset:",
                                                "Pause:",
                                                "Show/Hide Cursor:",
                                                "Fullscreen:",
                                                "Change Blade:",
                                                "Flare Green:",
                                                "Flare Red:",
                                                "Flare Black:",
                                                "Reel in:",
                                                "Reel out:",
                                                "Gas Burst:",
                                                "Minimap Max:",
                                                "Minimap Toggle:",
                                                "Minimap Reset:",
                                                "Open Chat:",
                                                "Live Spectate"
                                            };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 14)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 15;
                                                }

                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f),
                                                    list7[num13], "Label");
                                            }

                                            bool flag37 = (int) settings[97] == 1;
                                            bool flag38 = (int) settings[116] == 1;
                                            bool flag39 = (int) settings[181] == 1;
                                            bool flag40 = GUI.Toggle(new Rect(num7 + 457f, num8 + 261f, 40f, 20f),
                                                flag37, "On");
                                            if (flag37 != flag40)
                                            {
                                                if (flag40)
                                                {
                                                    settings[97] = 1;
                                                }
                                                else
                                                {
                                                    settings[97] = 0;
                                                }
                                            }

                                            bool flag41 = GUI.Toggle(new Rect(num7 + 457f, num8 + 286f, 40f, 20f),
                                                flag38, "On");
                                            if (flag38 != flag41)
                                            {
                                                if (flag41)
                                                {
                                                    settings[116] = 1;
                                                }
                                                else
                                                {
                                                    settings[116] = 0;
                                                }
                                            }

                                            bool flag42 = GUI.Toggle(new Rect(num7 + 457f, num8 + 311f, 40f, 20f),
                                                flag39, "On");
                                            if (flag39 != flag42)
                                            {
                                                if (flag42)
                                                {
                                                    settings[181] = 1;
                                                }
                                                else
                                                {
                                                    settings[181] = 0;
                                                }
                                            }

                                            for (num13 = 0; num13 < 22; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 14)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 15;
                                                }

                                                if (GUI.Button(
                                                    new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f),
                                                    inputManager.getKeyRC(num13), "box"))
                                                {
                                                    settings[100] = num13 + 1;
                                                    inputManager.setNameRC(num13, "waiting...");
                                                }
                                            }

                                            if (GUI.Button(new Rect(num7 + 500f, num8 + 261f, 120f, 20f),
                                                (string) settings[98], "box"))
                                            {
                                                settings[98] = "waiting...";
                                                settings[100] = 98;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 286f, 120f, 20f),
                                                (string) settings[99], "box"))
                                            {
                                                settings[99] = "waiting...";
                                                settings[100] = 99;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 311f, 120f, 20f),
                                                (string) settings[182], "box"))
                                            {
                                                settings[182] = "waiting...";
                                                settings[100] = 182;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 336f, 120f, 20f),
                                                (string) settings[232], "box"))
                                            {
                                                settings[232] = "waiting...";
                                                settings[100] = 232;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 361f, 120f, 20f),
                                                (string) settings[233], "box"))
                                            {
                                                settings[233] = "waiting...";
                                                settings[100] = 233;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 386f, 120f, 20f),
                                                (string) settings[234], "box"))
                                            {
                                                settings[234] = "waiting...";
                                                settings[100] = 234;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 411f, 120f, 20f),
                                                (string) settings[236], "box"))
                                            {
                                                settings[236] = "waiting...";
                                                settings[100] = 236;
                                            }
                                            else if (GUI.Button(new Rect(num7 + 500f, num8 + 436f, 120f, 20f),
                                                (string) settings[262], "box"))
                                            {
                                                settings[262] = "waiting...";
                                                settings[100] = 262;
                                            }

                                            if ((int) settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown &&
                                                    current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode) (323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }

                                                        num13++;
                                                    }
                                                }

                                                if (flag4)
                                                {
                                                    if ((int) settings[100] == 98)
                                                    {
                                                        settings[98] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.reelin, str4);
                                                    }
                                                    else if ((int) settings[100] == 99)
                                                    {
                                                        settings[99] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.reelout, str4);
                                                    }
                                                    else if ((int) settings[100] == 182)
                                                    {
                                                        settings[182] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.dash, str4);
                                                    }
                                                    else if ((int) settings[100] == 232)
                                                    {
                                                        settings[232] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapMaximize, str4);
                                                    }
                                                    else if ((int) settings[100] == 233)
                                                    {
                                                        settings[233] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapToggle, str4);
                                                    }
                                                    else if ((int) settings[100] == 234)
                                                    {
                                                        settings[234] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.mapReset, str4);
                                                    }
                                                    else if ((int) settings[100] == 236)
                                                    {
                                                        settings[236] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.chat, str4);
                                                    }
                                                    else if ((int) settings[100] == 262)
                                                    {
                                                        settings[262] = str4;
                                                        settings[100] = 0;
                                                        inputRC.setInputHuman(InputCodeRC.liveCam, str4);
                                                    }
                                                    else
                                                    {
                                                        for (num13 = 0; num13 < 22; num13++)
                                                        {
                                                            num23 = num13 + 1;
                                                            if ((int) settings[100] == num23)
                                                            {
                                                                inputManager.setKeyRC(num13, str4);
                                                                settings[100] = 0;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int) settings[190] == 1)
                                        {
                                            list7 = new List<string>
                                            {
                                                "Forward:",
                                                "Back:",
                                                "Left:",
                                                "Right:",
                                                "Walk:",
                                                "Jump:",
                                                "Punch:",
                                                "Slam:",
                                                "Grab (front):",
                                                "Grab (back):",
                                                "Grab (nape):",
                                                "Slap:",
                                                "Bite:",
                                                "Cover Nape:"
                                            };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 6)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 7;
                                                }

                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f),
                                                    list7[num13], "Label");
                                            }

                                            for (num13 = 0; num13 < 14; num13++)
                                            {
                                                num23 = 101 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 6)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 7;
                                                }

                                                if (GUI.Button(
                                                    new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f),
                                                    (string) settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }

                                            if ((int) settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown &&
                                                    current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode) (323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }

                                                        num13++;
                                                    }
                                                }

                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 14; num13++)
                                                    {
                                                        num23 = 101 + num13;
                                                        if ((int) settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputTitan(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int) settings[190] == 2)
                                        {
                                            list7 = new List<string>
                                            {
                                                "Forward:",
                                                "Back:",
                                                "Left:",
                                                "Right:",
                                                "Walk:",
                                                "Jump:",
                                                "Mount:"
                                            };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 4;
                                                }

                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f),
                                                    list7[num13], "Label");
                                            }

                                            for (num13 = 0; num13 < 7; num13++)
                                            {
                                                num23 = 237 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 4;
                                                }

                                                if (GUI.Button(
                                                    new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f),
                                                    (string) settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }

                                            if ((int) settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown &&
                                                    current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 7)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode) (323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }

                                                        num13++;
                                                    }
                                                }

                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 7; num13++)
                                                    {
                                                        num23 = 237 + num13;
                                                        if ((int) settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputHorse(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((int) settings[190] == 3)
                                        {
                                            list7 = new List<string>
                                            {
                                                "Rotate Up:",
                                                "Rotate Down:",
                                                "Rotate Left:",
                                                "Rotate Right:",
                                                "Fire:",
                                                "Mount:",
                                                "Slow Rotate:"
                                            };
                                            for (num13 = 0; num13 < list7.Count; num13++)
                                            {
                                                num18 = num13;
                                                num48 = 80f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 390f;
                                                    num18 -= 4;
                                                }

                                                GUI.Label(new Rect(num7 + num48, num8 + 86f + num18 * 25f, 145f, 22f),
                                                    list7[num13], "Label");
                                            }

                                            for (num13 = 0; num13 < 7; num13++)
                                            {
                                                num23 = 254 + num13;
                                                num18 = num13;
                                                num48 = 190f;
                                                if (num18 > 3)
                                                {
                                                    num48 = 500f;
                                                    num18 -= 4;
                                                }

                                                if (GUI.Button(
                                                    new Rect(num7 + num48, num8 + 86f + num18 * 25f, 120f, 20f),
                                                    (string) settings[num23], "box"))
                                                {
                                                    settings[num23] = "waiting...";
                                                    settings[100] = num23;
                                                }
                                            }

                                            if ((int) settings[100] != 0)
                                            {
                                                current = Event.current;
                                                flag4 = false;
                                                str4 = "waiting...";
                                                if (current.type == EventType.KeyDown &&
                                                    current.keyCode != KeyCode.None)
                                                {
                                                    flag4 = true;
                                                    str4 = current.keyCode.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.LeftShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.LeftShift.ToString();
                                                }
                                                else if (Input.GetKey(KeyCode.RightShift))
                                                {
                                                    flag4 = true;
                                                    str4 = KeyCode.RightShift.ToString();
                                                }
                                                else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                                                {
                                                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Up";
                                                    }
                                                    else
                                                    {
                                                        flag4 = true;
                                                        str4 = "Scroll Down";
                                                    }
                                                }
                                                else
                                                {
                                                    num13 = 0;
                                                    while (num13 < 6)
                                                    {
                                                        if (Input.GetKeyDown((KeyCode) (323 + num13)))
                                                        {
                                                            flag4 = true;
                                                            str4 = "Mouse" + Convert.ToString(num13);
                                                        }

                                                        num13++;
                                                    }
                                                }

                                                if (flag4)
                                                {
                                                    for (num13 = 0; num13 < 7; num13++)
                                                    {
                                                        num23 = 254 + num13;
                                                        if ((int) settings[100] == num23)
                                                        {
                                                            settings[num23] = str4;
                                                            settings[100] = 0;
                                                            inputRC.setInputCannon(num13, str4);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if ((int) settings[64] == 8)
                                    {
                                        GUI.Label(new Rect(num7 + 150f, num8 + 51f, 120f, 22f), "Map Settings",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 50f, num8 + 81f, 140f, 20f), "Titan Spawn Cap:",
                                            "Label");
                                        settings[85] = GUI.TextField(new Rect(num7 + 155f, num8 + 81f, 30f, 20f),
                                            (string) settings[85]);
                                        strArray16 = new[] {"1 Round", "Waves", "PVP", "Racing", "Custom"};
                                        RCSettings.gameType = GUI.SelectionGrid(
                                            new Rect(num7 + 190f, num8 + 80f, 140f, 60f), RCSettings.gameType,
                                            strArray16, 2, GUI.skin.toggle);
                                        GUI.Label(new Rect(num7 + 150f, num8 + 155f, 150f, 20f), "Level Script:",
                                            "Label");
                                        currentScript = GUI.TextField(new Rect(num7 + 50f, num8 + 180f, 275f, 220f),
                                            currentScript);
                                        if (GUI.Button(new Rect(num7 + 100f, num8 + 410f, 50f, 25f), "Copy"))
                                        {
                                            editor = new TextEditor
                                            {
                                                content = new GUIContent(currentScript)
                                            };
                                            editor.SelectAll();
                                            editor.Copy();
                                        }
                                        else if (GUI.Button(new Rect(num7 + 225f, num8 + 410f, 50f, 25f), "Clear"))
                                        {
                                            currentScript = string.Empty;
                                        }

                                        GUI.Label(new Rect(num7 + 455f, num8 + 51f, 180f, 20f), "Custom Textures",
                                            "Label");
                                        GUI.Label(new Rect(num7 + 375f, num8 + 81f, 180f, 20f), "Ground Skin:",
                                            "Label");
                                        settings[162] = GUI.TextField(new Rect(num7 + 375f, num8 + 103f, 275f, 20f),
                                            (string) settings[162]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 125f, 150f, 20f), "Skybox Front:",
                                            "Label");
                                        settings[175] = GUI.TextField(new Rect(num7 + 375f, num8 + 147f, 275f, 20f),
                                            (string) settings[175]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 169f, 150f, 20f), "Skybox Back:",
                                            "Label");
                                        settings[176] = GUI.TextField(new Rect(num7 + 375f, num8 + 191f, 275f, 20f),
                                            (string) settings[176]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 213f, 150f, 20f), "Skybox Left:",
                                            "Label");
                                        settings[177] = GUI.TextField(new Rect(num7 + 375f, num8 + 235f, 275f, 20f),
                                            (string) settings[177]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 257f, 150f, 20f), "Skybox Right:",
                                            "Label");
                                        settings[178] = GUI.TextField(new Rect(num7 + 375f, num8 + 279f, 275f, 20f),
                                            (string) settings[178]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 301f, 150f, 20f), "Skybox Up:", "Label");
                                        settings[179] = GUI.TextField(new Rect(num7 + 375f, num8 + 323f, 275f, 20f),
                                            (string) settings[179]);
                                        GUI.Label(new Rect(num7 + 375f, num8 + 345f, 150f, 20f), "Skybox Down:",
                                            "Label");
                                        settings[180] = GUI.TextField(new Rect(num7 + 375f, num8 + 367f, 275f, 20f),
                                            (string) settings[180]);
                                    }

                                    break;
                            }
                        }

                        if (GUI.Button(new Rect(num7 + 408f, num8 + 465f, 42f, 25f), "Save"))
                        {
                            PlayerPrefs.SetInt("human", (int) settings[0]);
                            PlayerPrefs.SetInt("titan", (int) settings[1]);
                            PlayerPrefs.SetInt("level", (int) settings[2]);
                            PlayerPrefs.SetString("horse", (string) settings[3]);
                            PlayerPrefs.SetString("hair", (string) settings[4]);
                            PlayerPrefs.SetString("eye", (string) settings[5]);
                            PlayerPrefs.SetString("glass", (string) settings[6]);
                            PlayerPrefs.SetString("face", (string) settings[7]);
                            PlayerPrefs.SetString("skin", (string) settings[8]);
                            PlayerPrefs.SetString("costume", (string) settings[9]);
                            PlayerPrefs.SetString("logo", (string) settings[10]);
                            PlayerPrefs.SetString("bladel", (string) settings[11]);
                            PlayerPrefs.SetString("blader", (string) settings[12]);
                            PlayerPrefs.SetString("gas", (string) settings[13]);
                            PlayerPrefs.SetString("haircolor", (string) settings[14]);
                            PlayerPrefs.SetInt("gasenable", (int) settings[15]);
                            PlayerPrefs.SetInt("titantype1", (int) settings[16]);
                            PlayerPrefs.SetInt("titantype2", (int) settings[17]);
                            PlayerPrefs.SetInt("titantype3", (int) settings[18]);
                            PlayerPrefs.SetInt("titantype4", (int) settings[19]);
                            PlayerPrefs.SetInt("titantype5", (int) settings[20]);
                            PlayerPrefs.SetString("titanhair1", (string) settings[21]);
                            PlayerPrefs.SetString("titanhair2", (string) settings[22]);
                            PlayerPrefs.SetString("titanhair3", (string) settings[23]);
                            PlayerPrefs.SetString("titanhair4", (string) settings[24]);
                            PlayerPrefs.SetString("titanhair5", (string) settings[25]);
                            PlayerPrefs.SetString("titaneye1", (string) settings[26]);
                            PlayerPrefs.SetString("titaneye2", (string) settings[27]);
                            PlayerPrefs.SetString("titaneye3", (string) settings[28]);
                            PlayerPrefs.SetString("titaneye4", (string) settings[29]);
                            PlayerPrefs.SetString("titaneye5", (string) settings[30]);
                            PlayerPrefs.SetInt("titanR", (int) settings[32]);
                            PlayerPrefs.SetString("tree1", (string) settings[33]);
                            PlayerPrefs.SetString("tree2", (string) settings[34]);
                            PlayerPrefs.SetString("tree3", (string) settings[35]);
                            PlayerPrefs.SetString("tree4", (string) settings[36]);
                            PlayerPrefs.SetString("tree5", (string) settings[37]);
                            PlayerPrefs.SetString("tree6", (string) settings[38]);
                            PlayerPrefs.SetString("tree7", (string) settings[39]);
                            PlayerPrefs.SetString("tree8", (string) settings[40]);
                            PlayerPrefs.SetString("leaf1", (string) settings[41]);
                            PlayerPrefs.SetString("leaf2", (string) settings[42]);
                            PlayerPrefs.SetString("leaf3", (string) settings[43]);
                            PlayerPrefs.SetString("leaf4", (string) settings[44]);
                            PlayerPrefs.SetString("leaf5", (string) settings[45]);
                            PlayerPrefs.SetString("leaf6", (string) settings[46]);
                            PlayerPrefs.SetString("leaf7", (string) settings[47]);
                            PlayerPrefs.SetString("leaf8", (string) settings[48]);
                            PlayerPrefs.SetString("forestG", (string) settings[49]);
                            PlayerPrefs.SetInt("forestR", (int) settings[50]);
                            PlayerPrefs.SetString("house1", (string) settings[51]);
                            PlayerPrefs.SetString("house2", (string) settings[52]);
                            PlayerPrefs.SetString("house3", (string) settings[53]);
                            PlayerPrefs.SetString("house4", (string) settings[54]);
                            PlayerPrefs.SetString("house5", (string) settings[55]);
                            PlayerPrefs.SetString("house6", (string) settings[56]);
                            PlayerPrefs.SetString("house7", (string) settings[57]);
                            PlayerPrefs.SetString("house8", (string) settings[58]);
                            PlayerPrefs.SetString("cityG", (string) settings[59]);
                            PlayerPrefs.SetString("cityW", (string) settings[60]);
                            PlayerPrefs.SetString("cityH", (string) settings[61]);
                            PlayerPrefs.SetInt("skinQ", QualitySettings.masterTextureLimit);
                            PlayerPrefs.SetInt("skinQL", (int) settings[63]);
                            PlayerPrefs.SetString("eren", (string) settings[65]);
                            PlayerPrefs.SetString("annie", (string) settings[66]);
                            PlayerPrefs.SetString("colossal", (string) settings[67]);
                            PlayerPrefs.SetString("hoodie", (string) settings[14]);
                            PlayerPrefs.SetString("cnumber", (string) settings[82]);
                            PlayerPrefs.SetString("cmax", (string) settings[85]);
                            PlayerPrefs.SetString("titanbody1", (string) settings[86]);
                            PlayerPrefs.SetString("titanbody2", (string) settings[87]);
                            PlayerPrefs.SetString("titanbody3", (string) settings[88]);
                            PlayerPrefs.SetString("titanbody4", (string) settings[89]);
                            PlayerPrefs.SetString("titanbody5", (string) settings[90]);
                            PlayerPrefs.SetInt("customlevel", (int) settings[91]);
                            PlayerPrefs.SetInt("traildisable", (int) settings[92]);
                            PlayerPrefs.SetInt("wind", (int) settings[93]);
                            PlayerPrefs.SetString("trailskin", (string) settings[94]);
                            PlayerPrefs.SetString("snapshot", (string) settings[95]);
                            PlayerPrefs.SetString("trailskin2", (string) settings[96]);
                            PlayerPrefs.SetInt("reel", (int) settings[97]);
                            PlayerPrefs.SetString("reelin", (string) settings[98]);
                            PlayerPrefs.SetString("reelout", (string) settings[99]);
                            PlayerPrefs.SetFloat("vol", AudioListener.volume);
                            PlayerPrefs.SetString("tforward", (string) settings[101]);
                            PlayerPrefs.SetString("tback", (string) settings[102]);
                            PlayerPrefs.SetString("tleft", (string) settings[103]);
                            PlayerPrefs.SetString("tright", (string) settings[104]);
                            PlayerPrefs.SetString("twalk", (string) settings[105]);
                            PlayerPrefs.SetString("tjump", (string) settings[106]);
                            PlayerPrefs.SetString("tpunch", (string) settings[107]);
                            PlayerPrefs.SetString("tslam", (string) settings[108]);
                            PlayerPrefs.SetString("tgrabfront", (string) settings[109]);
                            PlayerPrefs.SetString("tgrabback", (string) settings[110]);
                            PlayerPrefs.SetString("tgrabnape", (string) settings[111]);
                            PlayerPrefs.SetString("tantiae", (string) settings[112]);
                            PlayerPrefs.SetString("tbite", (string) settings[113]);
                            PlayerPrefs.SetString("tcover", (string) settings[114]);
                            PlayerPrefs.SetString("tsit", (string) settings[115]);
                            PlayerPrefs.SetInt("reel2", (int) settings[116]);
                            PlayerPrefs.SetInt("humangui", (int) settings[133]);
                            PlayerPrefs.SetString("horse2", (string) settings[134]);
                            PlayerPrefs.SetString("hair2", (string) settings[135]);
                            PlayerPrefs.SetString("eye2", (string) settings[136]);
                            PlayerPrefs.SetString("glass2", (string) settings[137]);
                            PlayerPrefs.SetString("face2", (string) settings[138]);
                            PlayerPrefs.SetString("skin2", (string) settings[139]);
                            PlayerPrefs.SetString("costume2", (string) settings[140]);
                            PlayerPrefs.SetString("logo2", (string) settings[141]);
                            PlayerPrefs.SetString("bladel2", (string) settings[142]);
                            PlayerPrefs.SetString("blader2", (string) settings[143]);
                            PlayerPrefs.SetString("gas2", (string) settings[144]);
                            PlayerPrefs.SetString("hoodie2", (string) settings[145]);
                            PlayerPrefs.SetString("trail2", (string) settings[146]);
                            PlayerPrefs.SetString("horse3", (string) settings[147]);
                            PlayerPrefs.SetString("hair3", (string) settings[148]);
                            PlayerPrefs.SetString("eye3", (string) settings[149]);
                            PlayerPrefs.SetString("glass3", (string) settings[150]);
                            PlayerPrefs.SetString("face3", (string) settings[151]);
                            PlayerPrefs.SetString("skin3", (string) settings[152]);
                            PlayerPrefs.SetString("costume3", (string) settings[153]);
                            PlayerPrefs.SetString("logo3", (string) settings[154]);
                            PlayerPrefs.SetString("bladel3", (string) settings[155]);
                            PlayerPrefs.SetString("blader3", (string) settings[156]);
                            PlayerPrefs.SetString("gas3", (string) settings[157]);
                            PlayerPrefs.SetString("hoodie3", (string) settings[158]);
                            PlayerPrefs.SetString("trail3", (string) settings[159]);
                            PlayerPrefs.SetString("customGround", (string) settings[162]);
                            PlayerPrefs.SetString("forestskyfront", (string) settings[163]);
                            PlayerPrefs.SetString("forestskyback", (string) settings[164]);
                            PlayerPrefs.SetString("forestskyleft", (string) settings[165]);
                            PlayerPrefs.SetString("forestskyright", (string) settings[166]);
                            PlayerPrefs.SetString("forestskyup", (string) settings[167]);
                            PlayerPrefs.SetString("forestskydown", (string) settings[168]);
                            PlayerPrefs.SetString("cityskyfront", (string) settings[169]);
                            PlayerPrefs.SetString("cityskyback", (string) settings[170]);
                            PlayerPrefs.SetString("cityskyleft", (string) settings[171]);
                            PlayerPrefs.SetString("cityskyright", (string) settings[172]);
                            PlayerPrefs.SetString("cityskyup", (string) settings[173]);
                            PlayerPrefs.SetString("cityskydown", (string) settings[174]);
                            PlayerPrefs.SetString("customskyfront", (string) settings[175]);
                            PlayerPrefs.SetString("customskyback", (string) settings[176]);
                            PlayerPrefs.SetString("customskyleft", (string) settings[177]);
                            PlayerPrefs.SetString("customskyright", (string) settings[178]);
                            PlayerPrefs.SetString("customskyup", (string) settings[179]);
                            PlayerPrefs.SetString("customskydown", (string) settings[180]);
                            PlayerPrefs.SetInt("dashenable", (int) settings[181]);
                            PlayerPrefs.SetString("dashkey", (string) settings[182]);
                            PlayerPrefs.SetInt("vsync", (int) settings[183]);
                            PlayerPrefs.SetString("fpscap", (string) settings[184]);
                            PlayerPrefs.SetInt("speedometer", (int) settings[189]);
                            PlayerPrefs.SetInt("bombMode", (int) settings[192]);
                            PlayerPrefs.SetInt("teamMode", (int) settings[193]);
                            PlayerPrefs.SetInt("rockThrow", (int) settings[194]);
                            PlayerPrefs.SetInt("explodeModeOn", (int) settings[195]);
                            PlayerPrefs.SetString("explodeModeNum", (string) settings[196]);
                            PlayerPrefs.SetInt("healthMode", (int) settings[197]);
                            PlayerPrefs.SetString("healthLower", (string) settings[198]);
                            PlayerPrefs.SetString("healthUpper", (string) settings[199]);
                            PlayerPrefs.SetInt("infectionModeOn", (int) settings[200]);
                            PlayerPrefs.SetString("infectionModeNum", (string) settings[201]);
                            PlayerPrefs.SetInt("banEren", (int) settings[202]);
                            PlayerPrefs.SetInt("moreTitanOn", (int) settings[203]);
                            PlayerPrefs.SetString("moreTitanNum", (string) settings[204]);
                            PlayerPrefs.SetInt("damageModeOn", (int) settings[205]);
                            PlayerPrefs.SetString("damageModeNum", (string) settings[206]);
                            PlayerPrefs.SetInt("sizeMode", (int) settings[207]);
                            PlayerPrefs.SetString("sizeLower", (string) settings[208]);
                            PlayerPrefs.SetString("sizeUpper", (string) settings[209]);
                            PlayerPrefs.SetInt("spawnModeOn", (int) settings[210]);
                            PlayerPrefs.SetString("nRate", (string) settings[211]);
                            PlayerPrefs.SetString("aRate", (string) settings[212]);
                            PlayerPrefs.SetString("jRate", (string) settings[213]);
                            PlayerPrefs.SetString("cRate", (string) settings[214]);
                            PlayerPrefs.SetString("pRate", (string) settings[215]);
                            PlayerPrefs.SetInt("horseMode", (int) settings[216]);
                            PlayerPrefs.SetInt("waveModeOn", (int) settings[217]);
                            PlayerPrefs.SetString("waveModeNum", (string) settings[218]);
                            PlayerPrefs.SetInt("friendlyMode", (int) settings[219]);
                            PlayerPrefs.SetInt("pvpMode", (int) settings[220]);
                            PlayerPrefs.SetInt("maxWaveOn", (int) settings[221]);
                            PlayerPrefs.SetString("maxWaveNum", (string) settings[222]);
                            PlayerPrefs.SetInt("endlessModeOn", (int) settings[223]);
                            PlayerPrefs.SetString("endlessModeNum", (string) settings[224]);
                            PlayerPrefs.SetString("motd", (string) settings[225]);
                            PlayerPrefs.SetInt("pointModeOn", (int) settings[226]);
                            PlayerPrefs.SetString("pointModeNum", (string) settings[227]);
                            PlayerPrefs.SetInt("ahssReload", (int) settings[228]);
                            PlayerPrefs.SetInt("punkWaves", (int) settings[229]);
                            PlayerPrefs.SetInt("mapOn", (int) settings[231]);
                            PlayerPrefs.SetString("mapMaximize", (string) settings[232]);
                            PlayerPrefs.SetString("mapToggle", (string) settings[233]);
                            PlayerPrefs.SetString("mapReset", (string) settings[234]);
                            PlayerPrefs.SetInt("globalDisableMinimap", (int) settings[235]);
                            PlayerPrefs.SetString("chatRebind", (string) settings[236]);
                            PlayerPrefs.SetString("hforward", (string) settings[237]);
                            PlayerPrefs.SetString("hback", (string) settings[238]);
                            PlayerPrefs.SetString("hleft", (string) settings[239]);
                            PlayerPrefs.SetString("hright", (string) settings[240]);
                            PlayerPrefs.SetString("hwalk", (string) settings[241]);
                            PlayerPrefs.SetString("hjump", (string) settings[242]);
                            PlayerPrefs.SetString("hmount", (string) settings[243]);
                            PlayerPrefs.SetInt("chatfeed", (int) settings[244]);
                            PlayerPrefs.SetFloat("bombR", (float) settings[246]);
                            PlayerPrefs.SetFloat("bombG", (float) settings[247]);
                            PlayerPrefs.SetFloat("bombB", (float) settings[248]);
                            PlayerPrefs.SetFloat("bombA", (float) settings[249]);
                            PlayerPrefs.SetInt("bombRadius", (int) settings[250]);
                            PlayerPrefs.SetInt("bombRange", (int) settings[251]);
                            PlayerPrefs.SetInt("bombSpeed", (int) settings[252]);
                            PlayerPrefs.SetInt("bombCD", (int) settings[253]);
                            PlayerPrefs.SetString("cannonUp", (string) settings[254]);
                            PlayerPrefs.SetString("cannonDown", (string) settings[255]);
                            PlayerPrefs.SetString("cannonLeft", (string) settings[256]);
                            PlayerPrefs.SetString("cannonRight", (string) settings[257]);
                            PlayerPrefs.SetString("cannonFire", (string) settings[258]);
                            PlayerPrefs.SetString("cannonMount", (string) settings[259]);
                            PlayerPrefs.SetString("cannonSlow", (string) settings[260]);
                            PlayerPrefs.SetInt("deadlyCannon", (int) settings[261]);
                            PlayerPrefs.SetString("liveCam", (string) settings[262]);
                            settings[64] = 4;
                        }
                        else if (GUI.Button(new Rect(num7 + 455f, num8 + 465f, 40f, 25f), "Load"))
                        {
                            LoadConfig();
                            settings[64] = 5;
                        }
                        else if (GUI.Button(new Rect(num7 + 500f, num8 + 465f, 60f, 25f), "Default"))
                        {
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().setToDefault();
                        }
                        else if (GUI.Button(new Rect(num7 + 565f, num8 + 465f, 75f, 25f), "Continue"))
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                Time.timeScale = 1f;
                            }

                            if (!UnityEngine.Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
                            {
                                Screen.showCursor = true;
                                Screen.lockCursor = true;
                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn =
                                    false;
                                UnityEngine.Camera.main.GetComponent<SpectatorMovement>().disable = false;
                                UnityEngine.Camera.main.GetComponent<MouseLook>().disable = false;
                            }
                            else
                            {
                                IN_GAME_MAIN_CAMERA.isPausing = false;
                                if (IN_GAME_MAIN_CAMERA.cameraMode == CameraType.TPS)
                                {
                                    Screen.showCursor = false;
                                    Screen.lockCursor = true;
                                }
                                else
                                {
                                    Screen.showCursor = false;
                                    Screen.lockCursor = false;
                                }

                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn =
                                    false;
                                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>()
                                    .justUPDATEME();
                            }
                        }
                        else if (GUI.Button(new Rect(num7 + 645f, num8 + 465f, 40f, 25f), "Quit"))
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                            {
                                Time.timeScale = 1f;
                            }
                            else
                            {
                                PhotonNetwork.Disconnect();
                            }

                            Screen.lockCursor = false;
                            Screen.showCursor = true;
                            IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
                            gameStart = false;
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                            DestroyAllExistingCloths();
                            Destroy(GameObject.Find("MultiplayerManager"));
                            Application.LoadLevel("menu");
                        }
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    if (Time.timeScale <= 0.1f)
                    {
                        num7 = Screen.width / 2f;
                        num8 = Screen.height / 2f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(num7 - 98f, num8 - 48f, 196f, 96f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7 - 100f, num8 - 50f, 200f, 100f), string.Empty);
                        if (pauseWaitTime <= 3f)
                        {
                            GUI.Label(new Rect(num7 - 43f, num8 - 15f, 200f, 22f), "Unpausing in:");
                            GUI.Label(new Rect(num7 - 8f, num8 + 5f, 200f, 22f), pauseWaitTime.ToString("F1"));
                        }
                        else
                        {
                            GUI.Label(new Rect(num7 - 43f, num8 - 10f, 200f, 22f), "Game Paused.");
                        }
                    }
                    else if (!(logicLoaded && customLevelLoaded))
                    {
                        num7 = Screen.width / 2f;
                        num8 = Screen.height / 2f;
                        GUI.backgroundColor = new Color(0.08f, 0.3f, 0.4f, 1f);
                        GUI.DrawTexture(new Rect(num7 - 98f, num8 - 48f, 196f, 146f), textureBackgroundBlue);
                        GUI.Box(new Rect(num7 - 100f, num8 - 50f, 200f, 150f), string.Empty);
                        int length = Player.Self.Properties.CurrentLevel.Length;
                        int num50 = PhotonNetwork.masterClient.Properties.CurrentLevel.Length;
                        GUI.Label(new Rect(num7 - 60f, num8 - 30f, 200f, 22f),
                            "Loading Level (" + length + "/" + num50 + ")");
                        retryTime += Time.deltaTime;
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        if (GUI.Button(new Rect(num7 - 20f, num8 + 50f, 40f, 30f), "Quit"))
                        {
                            PhotonNetwork.Disconnect();
                            Screen.lockCursor = false;
                            Screen.showCursor = true;
                            IN_GAME_MAIN_CAMERA.GameType = GameType.NotInRoom;
                            gameStart = false;
                            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
                            DestroyAllExistingCloths();
                            Destroy(GameObject.Find("MultiplayerManager"));
                            Application.LoadLevel("menu");
                        }
                    }
                }
            }
        }
    }

    #endregion
}