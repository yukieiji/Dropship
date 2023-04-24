using Dropship.API;

namespace Dropship.Random;

public static class Rng
{
    public static RngBase Instance
    {
        get => instance ??= new DonetRng();
        set
        {
            instance = value;
        }
    }
    private static RngBase instance;
}

internal class DonetRng : RngBase
{
    private System.Random rng;

    internal DonetRng()
    {
        rng = new System.Random();
    }
    public override int Next() => rng.Next();

    public override int Next(int maxExclusive) => rng.Next(maxExclusive);

    public override int Next(int minInclusive, int maxExclusive)
        => rng.Next(minInclusive, maxExclusive);
}
