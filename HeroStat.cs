public struct HeroStat
{
    public string SkillName;
    public int Speed;
    public int Gas;
    public int Blade;
    public int Acceleration;

    public HeroStat(string skillName, int speed, int gas, int blade, int acceleration)
    {
        Acceleration = acceleration;
        Blade = blade;
        Gas = gas;
        Speed = speed;
        SkillName = skillName;
    }

    public static HeroStat GetInfo(string name)
    {
        switch (name.ToUpperInvariant())
        {
            case "CUSTOM_DEFAULT":
                return CustomDefault;
            case "AHSS":
                return AHSS;
            
            case "MIKASA":
                return Mikasa;
            case "LEVI":
                return Levi;
            case "ARMIN":
                return Armin;
            case "MARCO":
                return Marco;
            case "JEAN":
                return Jean;
            case "EREN":
                return Eren;
            case "PETRA":
                return Petra;
            case "SASHA":
                return Sasha;
            default:
                return Levi;
        }
    }

    public static HeroStat CustomDefault => new HeroStat("petra", 100, 100, 100, 10);
    private static HeroStat AHSS => new HeroStat("sasha", 100, 100, 100, 100);

    private static HeroStat Armin => new HeroStat("armin", 75, 150, 125, 85);
    private static HeroStat Eren => new HeroStat("eren", 100, 90, 90, 100);
    private static HeroStat Jean => new HeroStat("jean", 100, 150, 80, 100);
    private static HeroStat Levi => new HeroStat("levi", 95, 100, 100, 150);
    private static HeroStat Marco => new HeroStat("marco", 110, 100, 115, 95);
    private static HeroStat Mikasa => new HeroStat("mikasa", 125, 75, 75, 135);
    private static HeroStat Petra => new HeroStat("petra", 80, 110, 100, 140);
    private static HeroStat Sasha => new HeroStat("sasha", 140, 100, 100, 115);
}

