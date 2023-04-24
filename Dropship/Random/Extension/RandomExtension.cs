using System.Collections.Generic;
using System.Linq;

namespace Dropship.Random.Extension;

public static class RandomExtension
{
    public static T GetRandomItem<T>(this IEnumerable<T> self)
    {
        var list = self as IList<T> ?? self.ToList();
        return list.Count == 0 ? default(T) : list[Rng.Instance.Next(0, list.Count)];
    }

    public static int GetRandomIndex<T>(this IEnumerable<T> self)
    {
        var list = self as IList<T> ?? self.ToList();
        return list.Count == 0 ? 0 : Rng.Instance.Next(0, list.Count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
    {
        return self.OrderBy(x => Rng.Instance.Next());
    }
}
