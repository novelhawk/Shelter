using System;
using Game;
using ExitGames.Client.Photon;
using Mod;
using Photon.Enums;

namespace RC
{
    public class RCAction
    {
        private int actionClass;
        private int actionType;
        private RCEvent nextEvent;
        private RCActionHelper[] parameters;

        public RCAction(int category, int type, RCEvent next, RCActionHelper[] helpers)
        {
            return;
            this.actionClass = category;
            this.actionType = type;
            this.nextEvent = next;
            this.parameters = helpers;
        }

        public void callException(string str)
        {
            return;
        }

        public void doAction()
        {
            return;
            switch (this.actionClass)
            {
                case 0:
                    this.nextEvent.checkEvent();
                    break;

                case 1:
                {
                    string key = this.parameters[0].returnString(null);
                    int num2 = this.parameters[1].returnInt(null);
                    switch (this.actionType)
                    {
                        case 0:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                GameManager.intVariables.Add(key, num2);
                            }
                            else
                            {
                                GameManager.intVariables[key] = num2;
                            }
                            return;

                        case 1:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) GameManager.intVariables[key] + num2;
                            }
                            return;

                        case 2:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) GameManager.intVariables[key] - num2;
                            }
                            return;

                        case 3:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) GameManager.intVariables[key] * num2;
                            }
                            return;

                        case 4:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) GameManager.intVariables[key] / num2;
                            }
                            return;

                        case 5:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) GameManager.intVariables[key] % num2;
                            }
                            return;

                        case 6:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                            }
                            else
                            {
                                GameManager.intVariables[key] = (int) Math.Pow((int)GameManager.intVariables[key], num2);
                            }
                            return;

                        case 12:
                            if (!GameManager.intVariables.ContainsKey(key))
                            {
                                GameManager.intVariables.Add(key, UnityEngine.Random.Range(num2, this.parameters[2].returnInt(null)));
                            }
                            else
                            {
                                GameManager.intVariables[key] = UnityEngine.Random.Range(num2, this.parameters[2].returnInt(null));
                            }
                            return;
                    }
                    break;
                }
                case 2:
                {
                    string str2 = this.parameters[0].returnString(null);
                    bool flag2 = this.parameters[1].returnBool(null);
                    switch (this.actionType)
                    {
                        case 11:
                            if (!GameManager.boolVariables.ContainsKey(str2))
                            {
                                this.callException("Variable not found: " + str2);
                            }
                            else
                            {
                                GameManager.boolVariables[str2] = !(bool) GameManager.boolVariables[str2];
                            }
                            return;

                        case 12:
                            if (!GameManager.boolVariables.ContainsKey(str2))
                            {
                                GameManager.boolVariables.Add(str2, Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
                            }
                            else
                            {
                                GameManager.boolVariables[str2] = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
                            }
                            return;

                        case 0:
                            if (!GameManager.boolVariables.ContainsKey(str2))
                            {
                                GameManager.boolVariables.Add(str2, flag2);
                            }
                            else
                            {
                                GameManager.boolVariables[str2] = flag2;
                            }
                            return;
                    }
                    break;
                }
                case 3:
                {
                    string str3 = this.parameters[0].returnString(null);
                    switch (this.actionType)
                    {
                        case 7:
                        {
                            string str5 = string.Empty;
                            for (int i = 1; i < this.parameters.Length; i++)
                            {
                                str5 = str5 + this.parameters[i].returnString(null);
                            }
                            if (!GameManager.stringVariables.ContainsKey(str3))
                            {
                                GameManager.stringVariables.Add(str3, str5);
                            }
                            else
                            {
                                GameManager.stringVariables[str3] = str5;
                            }
                            return;
                        }
                        case 8:
                        {
                            string str6 = this.parameters[1].returnString(null);
                            if (!GameManager.stringVariables.ContainsKey(str3))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.stringVariables[str3] = (string) GameManager.stringVariables[str3] + str6;
                            }
                            return;
                        }
                        case 9:
                            this.parameters[1].returnString(null);
                            if (!GameManager.stringVariables.ContainsKey(str3))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.stringVariables[str3] = ((string) GameManager.stringVariables[str3]).Replace(this.parameters[1].returnString(null), this.parameters[2].returnString(null));
                            }
                            return;

                        case 0:
                        {
                            string str4 = this.parameters[1].returnString(null);
                            if (!GameManager.stringVariables.ContainsKey(str3))
                            {
                                GameManager.stringVariables.Add(str3, str4);
                            }
                            else
                            {
                                GameManager.stringVariables[str3] = str4;
                            }
                            return;
                        }
                    }
                    break;
                }
                case 4:
                {
                    string str9 = this.parameters[0].returnString(null);
                    float num4 = this.parameters[1].returnFloat(null);
                    switch (this.actionType)
                    {
                        case 0:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                GameManager.floatVariables.Add(str9, num4);
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = num4;
                            }
                            return;

                        case 1:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) GameManager.floatVariables[str9] + num4;
                            }
                            return;

                        case 2:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) GameManager.floatVariables[str9] - num4;
                            }
                            return;

                        case 3:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) GameManager.floatVariables[str9] * num4;
                            }
                            return;

                        case 4:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) GameManager.floatVariables[str9] / num4;
                            }
                            return;

                        case 5:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) GameManager.floatVariables[str9] % num4;
                            }
                            return;

                        case 6:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                this.callException("No Variable");
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = (float) Math.Pow((int)GameManager.floatVariables[str9], num4);
                            }
                            return;

                        case 12:
                            if (!GameManager.floatVariables.ContainsKey(str9))
                            {
                                GameManager.floatVariables.Add(str9, UnityEngine.Random.Range(num4, this.parameters[2].returnFloat(null)));
                            }
                            else
                            {
                                GameManager.floatVariables[str9] = UnityEngine.Random.Range(num4, this.parameters[2].returnFloat(null));
                            }
                            return;
                    }
                    break;
                }
                case 5:
                {
                    string str10 = this.parameters[0].returnString(null);
                    Player player = this.parameters[1].returnPlayer(null);
                    if (this.actionType == 0)
                    {
                        if (!GameManager.playerVariables.ContainsKey(str10))
                        {
                            GameManager.playerVariables.Add(str10, player);
                        }
                        else
                        {
                            GameManager.playerVariables[str10] = player;
                        }
                        break;
                    }
                    break;
                }
                case 6:
                {
                    string str11 = this.parameters[0].returnString(null);
                    TITAN titan = this.parameters[1].returnTitan(null);
                    if (this.actionType == 0)
                    {
                        if (!GameManager.titanVariables.ContainsKey(str11))
                        {
                            GameManager.titanVariables.Add(str11, titan);
                        }
                        else
                        {
                            GameManager.titanVariables[str11] = titan;
                        }
                        break;
                    }
                    break;
                }
                case 7:
                {
                    Player targetPlayer = this.parameters[0].returnPlayer(null);
                    switch (this.actionType)
                    {
                        case 0:
                        {
                            int iD = targetPlayer.ID;
                            if (GameManager.heroHash.ContainsKey(iD))
                            {
                                HERO hero = (HERO) GameManager.heroHash[iD];
                                hero.markDie();
                                hero.photonView.RPC(Rpc.DieRC, PhotonTargets.All, -1, this.parameters[1].returnString(null) + " ");
                            }
                            else
                            {
                                this.callException("Player Not Alive");
                            }
                            return;
                        }
                        case 1:
                            GameManager.instance.photonView.RPC(Rpc.Respawn, targetPlayer);
                            return;

                        case 2:
                            GameManager.instance.photonView.RPC(Rpc.SpawnPlayerAt, targetPlayer, this.parameters[1].returnFloat(null), this.parameters[2].returnFloat(null), this.parameters[3].returnFloat(null));
                            return;

                        case 3:
                        {
                            int num6 = targetPlayer.ID;
                            if (GameManager.heroHash.ContainsKey(num6))
                            {
                                HERO hero2 = (HERO) GameManager.heroHash[num6];
                                hero2.photonView.RPC(Rpc.MoveTo, targetPlayer, this.parameters[1].returnFloat(null), this.parameters[2].returnFloat(null), this.parameters[3].returnFloat(null));
                            }
                            else
                            {
                                this.callException("Player Not Alive");
                            }
                            return;
                        }
                        case 4:
                        {
                            Hashtable propertiesToSet = new Hashtable
                            {
                                { PlayerProperty.Kills, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(propertiesToSet);
                            return;
                        }
                        case 5:
                        {
                            Hashtable hashtable2 = new Hashtable
                            {
                                { PlayerProperty.Deaths, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable2);
                            return;
                        }
                        case 6:
                        {
                            Hashtable hashtable3 = new Hashtable
                            {
                                { PlayerProperty.MaxDamage, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable3);
                            return;
                        }
                        case 7:
                        {
                            Hashtable hashtable4 = new Hashtable
                            {
                                { PlayerProperty.TotalDamage, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable4);
                            return;
                        }
                        case 8:
                        {
                            Hashtable hashtable5 = new Hashtable
                            {
                                { PlayerProperty.Name, this.parameters[1].returnString(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable5);
                            return;
                        }
                        case 9:
                        {
                            Hashtable hashtable6 = new Hashtable
                            {
                                { PlayerProperty.Guild, this.parameters[1].returnString(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable6);
                            return;
                        }
                        case 10:
                        {
                            Hashtable hashtable7 = new Hashtable
                            {
                                { PlayerProperty.RCTeam, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable7);
                            return;
                        }
                        case 11:
                        {
                            Hashtable hashtable8 = new Hashtable
                            {
                                { PlayerProperty.RCInt, this.parameters[1].returnInt(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable8);
                            return;
                        }
                        case 12:
                        {
                            Hashtable hashtable9 = new Hashtable
                            {
                                { PlayerProperty.RCBool, this.parameters[1].returnBool(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable9);
                            return;
                        }
                        case 13:
                        {
                            Hashtable hashtable10 = new Hashtable
                            {
                                { PlayerProperty.RCString, this.parameters[1].returnString(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable10);
                            return;
                        }
                        case 14:
                        {
                            Hashtable hashtable11 = new Hashtable
                            {
                                { PlayerProperty.RCTeam, this.parameters[1].returnFloat(null) }
                            };
                            targetPlayer.SetCustomProperties(hashtable11);
                            return;
                        }
                    }
                    break;
                }
                case 8:
                    switch (this.actionType)
                    {
                        case 0:
                        {
                            TITAN titan2 = this.parameters[0].returnTitan(null);
                            titan2.photonView.RPC(Rpc.TitanGetHit, titan2.photonView.owner, this.parameters[1].returnPlayer(null).ID, this.parameters[2].returnInt(null));
                            return;
                        }
                        case 1:
                            GameManager.instance.SpawnTitanAction(this.parameters[0].returnInt(null), this.parameters[1].returnFloat(null), this.parameters[2].returnInt(null), this.parameters[3].returnInt(null));
                            return;

                        case 2:
                            GameManager.instance.SpawnTitanAtAction(this.parameters[0].returnInt(null), this.parameters[1].returnFloat(null), this.parameters[2].returnInt(null), this.parameters[3].returnInt(null), this.parameters[4].returnFloat(null), this.parameters[5].returnFloat(null), this.parameters[6].returnFloat(null));
                            return;

                        case 3:
                        {
                            TITAN titan3 = this.parameters[0].returnTitan(null);
                            int num7 = this.parameters[1].returnInt(null);
                            titan3.currentHealth = num7;
                            if (titan3.maxHealth == 0)
                            {
                                titan3.maxHealth = titan3.currentHealth;
                            }
                            titan3.photonView.RPC(Rpc.UpdateHealthLabel, PhotonTargets.AllBuffered, titan3.currentHealth, titan3.maxHealth);
                            return;
                        }
                        case 4:
                        {
                            TITAN titan4 = this.parameters[0].returnTitan(null);
                            if (titan4.photonView.isMine)
                            {
                                titan4.moveTo(this.parameters[1].returnFloat(null), this.parameters[2].returnFloat(null), this.parameters[3].returnFloat(null));
                            }
                            else
                            {
                                titan4.photonView.RPC(Rpc.MoveTo, titan4.photonView.owner, this.parameters[1].returnFloat(null), this.parameters[2].returnFloat(null), this.parameters[3].returnFloat(null));
                            }
                            return;
                        }
                    }
                    break;

                case 9:
                    switch (this.actionType)
                    {
                        case 0:
                            GameManager.instance.photonView.RPC(Rpc.Chat, PhotonTargets.All, this.parameters[0].returnString(null), string.Empty);
                            return;

                        case 1:
                            GameManager.instance.GameWin();
                            if (this.parameters[0].returnBool(null))
                            {
                                GameManager.intVariables.Clear();
                                GameManager.boolVariables.Clear();
                                GameManager.stringVariables.Clear();
                                GameManager.floatVariables.Clear();
                                GameManager.playerVariables.Clear();
                                GameManager.titanVariables.Clear();
                            }
                            return;

                        case 2:
                            GameManager.instance.GameLose();
                            if (this.parameters[0].returnBool(null))
                            {
                                GameManager.intVariables.Clear();
                                GameManager.boolVariables.Clear();
                                GameManager.stringVariables.Clear();
                                GameManager.floatVariables.Clear();
                                GameManager.playerVariables.Clear();
                                GameManager.titanVariables.Clear();
                            }
                            return;

                        case 3:
                            if (this.parameters[0].returnBool(null))
                            {
                                GameManager.intVariables.Clear();
                                GameManager.boolVariables.Clear();
                                GameManager.stringVariables.Clear();
                                GameManager.floatVariables.Clear();
                                GameManager.playerVariables.Clear();
                                GameManager.titanVariables.Clear();
                            }
                            GameManager.instance.RestartGame();
                            return;
                    }
                    break;
            }
        }
    }
}

