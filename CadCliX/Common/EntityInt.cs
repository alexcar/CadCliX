namespace CadCliX.Common;

public abstract class EntityInt : Entity<int>
{
    protected EntityInt() : base()
    {
    }

    protected EntityInt(int id) : base(id)
    {
    }
}
