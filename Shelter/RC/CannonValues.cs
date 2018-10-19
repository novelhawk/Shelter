namespace RC
{
    public struct CannonValues
    {
        public readonly string settings;
        public readonly int viewID;

        public CannonValues(int id, string str)
        {
            this.viewID = id;
            this.settings = str;
        }
    }
}

