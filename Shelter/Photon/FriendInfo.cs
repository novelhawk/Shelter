namespace Photon
{
    public class FriendInfo
    {
        public override string ToString()
        {
            return $"{this.Name}\t is: {(this.IsOnline ? (!this.IsInRoom ? "on master" : "playing") : "offline")}";
        }

        private bool IsInRoom => this.IsOnline && !string.IsNullOrEmpty(this.Room);
        public bool IsOnline { get; protected internal set; }
        public string Name { get; protected internal set; }
        public string Room { get; protected internal set; }
    }
}

