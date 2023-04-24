namespace Dropship.API;

public abstract class RngBase
{
    public abstract int Next();

    public abstract int Next(int maxExclusive);

    public abstract int Next(int minInclusive, int maxExclusive);
}
