namespace Controller
{
    public class Building
    {
        public BuildingTypes Type;
        public bool isOwned;
        private Player _owner;
        public Player owner
        {
            get
            {
                if ( Type != BuildingTypes.Contryside)
                { return _owner; }
                return null;
            }
        }

        public (int X, int Y) fieldPosition;

        public Building((int X, int Y) fpos)
        {
            fieldPosition = fpos;
            if (Type == BuildingTypes.Camp || Type == BuildingTypes.Contryside)
                isOwned = false;
            isOwned = true;
        }

        public event ChangeOwnerHandler ChangeOwner;

        public void setPlayer(Player player)
        {
            if (player != owner)
            {
                var prevOwner = _owner;
                _owner = player;
                ChangeOwner?.Invoke(this, prevOwner);
            }
        }
    }

    

    public enum BuildingTypes
    {
        Camp = 0,
        Contryside = 1,
        Spawn = 2,
        AttackFlag = 3,
        MoveFlag = 4,
    }

    public delegate void ChangeOwnerHandler(Building sender, Player prevOwner);
}