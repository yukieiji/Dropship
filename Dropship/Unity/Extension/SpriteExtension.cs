using UnityEngine;

namespace Dropship.Unity.Extension;

public static class SpriteExtension
{
    public static void Register(this Sprite self, string key)
    {
        SpriteHelper.Register(key, self);
    }
}
