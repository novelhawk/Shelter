namespace Game
{
    public struct CostumeHair
    {
        public int ID { get; }
        public string Texture { get; }
        public string Clothes { get; }
        public bool HasClothes => Clothes != string.Empty;

        public static readonly CostumeHair[] FemaleHairs;
        public static readonly CostumeHair[] MaleHairs;

        private CostumeHair(int id, string texture) : this(id, texture, string.Empty) {}
        private CostumeHair(int id, string texture, string clothes)
        {
            ID = id;
            Texture = texture;
            Clothes = clothes;
        }

        static CostumeHair()
        {
            int i = 0;
            MaleHairs = new[]
            {
                new CostumeHair(i++, "hair_boy1"),
                new CostumeHair(i++, "hair_boy2"),
                new CostumeHair(i++, "hair_boy3"),
                new CostumeHair(i++, "hair_boy4"),
                new CostumeHair(i++, "hair_eren"),
                new CostumeHair(i++, "hair_armin"),
                new CostumeHair(i++, "hair_jean"),
                new CostumeHair(i++, "hair_levi"),
                new CostumeHair(i++, "hair_marco"),
                new CostumeHair(i++, "hair_mike"),
                new CostumeHair(i, string.Empty),
            };

            i = 0;
            FemaleHairs = new[]
            {
                new CostumeHair(i++, "hair_girl1"), 
                new CostumeHair(i++, "hair_girl2"), 
                new CostumeHair(i++, "hair_girl3"), 
                new CostumeHair(i++, "hair_girl4"), 
                new CostumeHair(i++, "hair_girl5", "hair_girl5_cloth"), 
                new CostumeHair(i++, "hair_annie"), 
                new CostumeHair(i++, "hair_hanji", "hair_hanji_cloth"), 
                new CostumeHair(i++, "hair_mikasa"), 
                new CostumeHair(i++, "hair_petra"), 
                new CostumeHair(i++, "hair_rico"), 
                new CostumeHair(i, "hair_sasha", "hair_sasha_cloth"), 
            };
        }
    }
}

