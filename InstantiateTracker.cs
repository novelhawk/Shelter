using UnityEngine;

public class InstantiateTracker
{
    public static readonly InstantiateTracker instance = new InstantiateTracker();
    private Player[] players = new Player[0];

    public bool checkObj(string key, global::Player player, int[] viewIDS)
    {
        if (player.IsMasterClient || player.isLocal)
        {
            return true;
        }
        int num = player.ID * PhotonNetwork.MAX_VIEW_IDS;
        int num2 = num + PhotonNetwork.MAX_VIEW_IDS;
        foreach (int num3 in viewIDS)
        {
            if (num3 <= num || num3 >= num2)
            {
                if (PhotonNetwork.isMasterClient)
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning invalid photon view.");
                }
                return false;
            }
        }
        key = key.ToLower();
        switch (key)
        {
            case "rcasset/bombmain":
            case "rcasset/bombexplodemain":
                if (RCSettings.bombMode <= 0)
                {
                    if (!(!PhotonNetwork.isMasterClient || FengGameManagerMKII.instance.restartingBomb))
                    {
                        FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning bomb item (" + key + ").");
                    }
                    return false;
                }
                return this.Instantiated(player, GameResource.bomb);

            case "hook":
            case "aottg_hero 1":
                return this.Instantiated(player, GameResource.general);

            case "hitmeat2":
                return this.Instantiated(player, GameResource.bloodEffect);

            case "hitmeat":
            case "redcross":
            case "redcross1":
                return this.Instantiated(player, GameResource.bladeHit);

            case "fx/flarebullet1":
            case "fx/flarebullet2":
            case "fx/flarebullet3":
                return this.Instantiated(player, GameResource.flare);

            case "fx/shotgun":
            case "fx/shotgun 1":
                return this.Instantiated(player, GameResource.shotGun);

            case "fx/fxtitanspawn":
            case "fx/boom1":
            case "fx/boom3":
            case "fx/boom5":
            case "fx/rockthrow":
            case "fx/bite":
                if (LevelInfoManager.GetInfo(FengGameManagerMKII.Level).PlayerTitansAllowed || RCSettings.infectionMode > 0 || IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight)
                {
                    return this.Instantiated(player, GameResource.effect);
                }
                if (!(!PhotonNetwork.isMasterClient || FengGameManagerMKII.instance.restartingTitan))
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning titan effects.");
                }
                return false;

            case "fx/boom2":
            case "fx/boom4":
            case "fx/fxtitandie":
            case "fx/fxtitandie1":
            case "fx/boost_smoke":
            case "fx/thunder":
                return this.Instantiated(player, GameResource.effect);

            case "rcasset/cannonballobject":
                return this.Instantiated(player, GameResource.bomb);

            case "rcasset/cannonwall":
            case "rcasset/cannonground":
                if (PhotonNetwork.isMasterClient && !(FengGameManagerMKII.instance.allowedToCannon.ContainsKey(player.ID) || FengGameManagerMKII.instance.restartingMC))
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning cannon item (" + key + ").");
                }
                return this.Instantiated(player, GameResource.general);

            case "rcasset/cannonwallprop":
            case "rcasset/cannongroundprop":
                if (PhotonNetwork.isMasterClient)
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning MC item (" + key + ").");
                }
                return false;

            case "titan_eren":
                if (!(player.Properties.Character.ToUpper() != "EREN"))
                {
                    if (RCSettings.banEren > 0)
                    {
                        if (!(!PhotonNetwork.isMasterClient || FengGameManagerMKII.instance.restartingEren))
                        {
                            FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning titan eren (" + key + ").");
                        }
                        return false;
                    }
                    return this.Instantiated(player, GameResource.general);
                }
                if (PhotonNetwork.isMasterClient)
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning titan eren (" + key + ").");
                }
                return false;

            case "fx/justSmoke":
            case "bloodexplore":
            case "bloodsplatter":
                return this.Instantiated(player, GameResource.effect);

            case "hitmeatbig":
                if (!(player.Properties.Character.ToUpper() != "EREN"))
                {
                    if (RCSettings.banEren > 0)
                    {
                        if (!(!PhotonNetwork.isMasterClient || FengGameManagerMKII.instance.restartingEren))
                        {
                            FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning eren effect (" + key + ").");
                        }
                        return false;
                    }
                    return this.Instantiated(player, GameResource.effect);
                }
                if (PhotonNetwork.isMasterClient)
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning eren effect (" + key + ").");
                }
                return false;

            case "fx/colossal_steam_dmg":
            case "fx/colossal_steam":
            case "fx/boom1_ct_kick":
                if (!PhotonNetwork.isMasterClient || IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight)
                {
                    return this.Instantiated(player, GameResource.effect);
                }
                FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning colossal effect (" + key + ").");
                return false;

            case "rock":
                if (!PhotonNetwork.isMasterClient || IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight)
                {
                    return this.Instantiated(player, GameResource.general);
                }
                FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning MC item (" + key + ").");
                return false;

            case "horse":
                if (LevelInfoManager.GetInfo(FengGameManagerMKII.Level).Horse || RCSettings.horseMode != 0)
                {
                    return this.Instantiated(player, GameResource.general);
                }
                if (!(!PhotonNetwork.isMasterClient || FengGameManagerMKII.instance.restartingHorse))
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning horse (" + key + ").");
                }
                return false;

            case "titan_ver3.1":
                int num4;
                if (!PhotonNetwork.isMasterClient)
                {
                    if (FengGameManagerMKII.masterRC && IN_GAME_MAIN_CAMERA.GameMode != GameMode.BossFight)
                    {
                        num4 = 0;
                        foreach (TITAN titan in FengGameManagerMKII.instance.GetTitans())
                        {
                            if (titan.photonView.owner == player)
                            {
                                num4++;
                            }
                        }
                        if (num4 > 1)
                        {
                            return false;
                        }
                    }
                    break;
                }
                if (LevelInfoManager.GetInfo(FengGameManagerMKII.Level).PlayerTitansAllowed || RCSettings.infectionMode > 0 || IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight || FengGameManagerMKII.instance.restartingTitan)
                {
                    if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFight)
                    {
                        break;
                    }
                    num4 = 0;
                    foreach (TITAN titan in FengGameManagerMKII.instance.GetTitans())
                    {
                        if (titan.photonView.owner == player)
                        {
                            num4++;
                        }
                    }
                    if (num4 <= 1)
                    {
                        break;
                    }
                    FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning titan (" + key + ").");
                    return false;
                }
                FengGameManagerMKII.instance.KickPlayerRC(player, false, "spawning titan (" + key + ").");
                return false;

            case "colossal_titan":
            case "female_titan":
            case "titan_eren_trost":
            case "aot_supply":
            case "monsterprefab":
            case "titan_new_1":
            case "titan_new_2":
                if (!PhotonNetwork.isMasterClient)
                {
                    if (FengGameManagerMKII.masterRC)
                    {
                        return false;
                    }
                    return this.Instantiated(player, GameResource.general);
                }
                FengGameManagerMKII.instance.KickPlayerRC(player, true, "spawning MC item (" + key + ").");
                return false;

            default:
                return false;
        }
        return this.Instantiated(player, GameResource.general);
    }

    public void Dispose()
    {
        this.players = null;
        this.players = new Player[0];
    }

    public bool Instantiated(global::Player owner, GameResource type)
    {
        int num;
        if (this.TryGetPlayer(owner.ID, out num))
        {
            if (this.players[num].IsThingExcessive(type))
            {
                global::Player player = owner;
                if (player != null && PhotonNetwork.isMasterClient)
                {
                    FengGameManagerMKII.instance.KickPlayerRC(player, true, "spamming instantiate (" + type + ").");
                }
                RCextensions.RemoveAt<Player>(ref this.players, num);
                return false;
            }
        }
        else
        {
            RCextensions.Add<Player>(ref this.players, new Player(owner.ID));
            this.players[this.players.Length - 1].IsThingExcessive(type);
        }
        return true;
    }

    public bool PropertiesChanged(global::Player owner)
    {
        int num;
        if (this.TryGetPlayer(owner.ID, out num))
        {
            if (this.players[num].IsThingExcessive(GameResource.name))
            {
                return false;
            }
        }
        else
        {
            RCextensions.Add<Player>(ref this.players, new Player(owner.ID));
            this.players[this.players.Length - 1].IsThingExcessive(GameResource.name);
        }
        return true;
    }

    public void resetPropertyTracking(int ID)
    {
        int num;
        if (this.TryGetPlayer(ID, out num))
        {
            this.players[num].resetNameTracking();
        }
    }

    private bool TryGetPlayer(int id, out int result)
    {
        for (int i = 0; i < this.players.Length; i++)
        {
            if (this.players[i].id == id)
            {
                result = i;
                return true;
            }
        }
        result = -1;
        return false;
    }

    public void TryRemovePlayer(int playerId)
    {
        for (int i = 0; i < this.players.Length; i++)
        {
            if (this.players[i].id == playerId)
            {
                RCextensions.RemoveAt<Player>(ref this.players, i);
                break;
            }
        }
    }

    private class AhssShots : ThingToCheck
    {
        private float lastShot = Time.time;
        private int shots = 1;

        public AhssShots()
        {
            type = GameResource.shotGun;
        }

        public override bool KickWorthy()
        {
            if (Time.time - this.lastShot < 1f)
            {
                this.shots++;
                if (this.shots > 2)
                {
                    return true;
                }
            }
            else
            {
                this.shots = 0;
            }
            this.lastShot = Time.time;
            return false;
        }

        public override void reset()
        {
        }
    }

    private class BladeHitEffect : ThingToCheck
    {
        private float accumTime = 0f;
        private float lastHit = Time.time;

        public BladeHitEffect()
        {
            type = GameResource.bladeHit;
        }

        public override bool KickWorthy()
        {
            float num = Time.time - this.lastHit;
            this.lastHit = Time.time;
            if (num <= 0.3f)
            {
                this.accumTime += num;
                return this.accumTime >= 1.25f;
            }
            this.accumTime = 0f;
            return false;
        }

        public override void reset()
        {
        }
    }

    private class BloodEffect : ThingToCheck
    {
        private float accumTime = 0f;
        private float lastHit = Time.time;

        public BloodEffect()
        {
            type = GameResource.bloodEffect;
        }

        public override bool KickWorthy()
        {
            float num = Time.time - this.lastHit;
            this.lastHit = Time.time;
            if (num <= 0.3f)
            {
                this.accumTime += num;
                return this.accumTime >= 2f;
            }
            this.accumTime = 0f;
            return false;
        }

        public override void reset()
        {
        }
    }

    private class ExcessiveBombs : ThingToCheck
    {
        private int count = 1;
        private float lastClear = Time.time;

        public ExcessiveBombs()
        {
            type = GameResource.bomb;
        }

        public override bool KickWorthy()
        {
            if (Time.time - this.lastClear > 5f)
            {
                this.count = 0;
                this.lastClear = Time.time;
            }
            this.count++;
            return this.count > 20;
        }

        public override void reset()
        {
        }
    }

    private class ExcessiveEffect : ThingToCheck
    {
        private int effectCounter = 1;
        private float lastEffectTime = Time.time;

        public ExcessiveEffect()
        {
            type = GameResource.effect;
        }

        public override bool KickWorthy()
        {
            if (Time.time - this.lastEffectTime >= 2f)
            {
                this.effectCounter = 0;
                this.lastEffectTime = Time.time;
            }
            this.effectCounter++;
            return this.effectCounter > 8;
        }

        public override void reset()
        {
        }
    }

    private class ExcessiveFlares : ThingToCheck
    {
        private int flares = 1;
        private float lastFlare = Time.time;

        public ExcessiveFlares()
        {
            type = GameResource.flare;
        }

        public override bool KickWorthy()
        {
            if (Time.time - this.lastFlare >= 10f)
            {
                this.flares = 0;
                this.lastFlare = Time.time;
            }
            this.flares++;
            return this.flares > 4;
        }

        public override void reset()
        {
        }
    }

    private class ExcessiveNameChange : ThingToCheck
    {
        private float lastNameChange = Time.time;
        private int nameChanges = 1;

        public ExcessiveNameChange()
        {
            type = GameResource.name;
        }

        public override bool KickWorthy()
        {
            float num = Time.time - this.lastNameChange;
            this.lastNameChange = Time.time;
            if (num >= 5f)
            {
                this.nameChanges = 0;
            }
            this.nameChanges++;
            return this.nameChanges > 5;
        }

        public override void reset()
        {
            this.nameChanges = 0;
            this.lastNameChange = Time.time;
        }
    }

    public enum GameResource
    {
        none,
        shotGun,
        effect,
        flare,
        bladeHit,
        bloodEffect,
        general,
        name,
        bomb
    }

    private class GenerallyExcessive : ThingToCheck
    {
        private int count = 1;
        private float lastClear = Time.time;

        public GenerallyExcessive()
        {
            type = GameResource.general;
        }

        public override bool KickWorthy()
        {
            if (Time.time - this.lastClear > 5f)
            {
                this.count = 0;
                this.lastClear = Time.time;
            }
            this.count++;
            return this.count > 35;
        }

        public override void reset()
        {
        }
    }

    private class Player
    {
        public int id;
        private ThingToCheck[] thingsToCheck;

        public Player(int id)
        {
            this.id = id;
            this.thingsToCheck = new ThingToCheck[0];
        }

        private ThingToCheck GameResourceToThing(GameResource gr)
        {
            switch (gr)
            {
                case GameResource.shotGun:
                    return new AhssShots();

                case GameResource.effect:
                    return new ExcessiveEffect();

                case GameResource.flare:
                    return new ExcessiveFlares();

                case GameResource.bladeHit:
                    return new BladeHitEffect();

                case GameResource.bloodEffect:
                    return new BloodEffect();

                case GameResource.general:
                    return new GenerallyExcessive();

                case GameResource.name:
                    return new ExcessiveNameChange();

                case GameResource.bomb:
                    return new ExcessiveBombs();
            }
            return null;
        }

        private int GetThingToCheck(GameResource type)
        {
            for (int i = 0; i < this.thingsToCheck.Length; i++)
            {
                if (this.thingsToCheck[i].type == type)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsThingExcessive(GameResource gr)
        {
            int thingToCheck = this.GetThingToCheck(gr);
            if (thingToCheck > -1)
            {
                return this.thingsToCheck[thingToCheck].KickWorthy();
            }
            RCextensions.Add<ThingToCheck>(ref this.thingsToCheck, this.GameResourceToThing(gr));
            return false;
        }

        public void resetNameTracking()
        {
            int thingToCheck = this.GetThingToCheck(GameResource.name);
            if (thingToCheck > -1)
            {
                this.thingsToCheck[thingToCheck].reset();
            }
        }
    }

    private abstract class ThingToCheck
    {
        public GameResource type = GameResource.none;

        public abstract bool KickWorthy();
        public abstract void reset();
    }
}

