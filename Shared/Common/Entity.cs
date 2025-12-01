namespace Shared.Common
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode();
    }
}
