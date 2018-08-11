namespace Mod
{
    public struct Hero
    {
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public UNIFORM_TYPE Uniform { get; set; }
        public string PartChestObjectMesh { get; set; }
        public string PartChestObjectTexture { get; set; }
        public bool HasCape { get; set; }
        public string BodyTexture { get; set; }
        public CostumeHair Hair { get; set; }
        public int EyeTextureID { get; set; }
        public int BeardTextureID { get; set; }
        public int GlassTextureID { get; set; }
        public int SkinColor { get; set; }
        public int HairColor { get; set; }
        public Division Division { get; set; }
        public int CostumeId { get; set; }
        public string Skill { get; set; }
        public int Speed { get; set; }
        public int Gas { get; set; }
        public int Acceleration { get; set; }
        public int BladeDuration { get; set; }
    }
}
