using UnityEngine;

using Il2CppInterop.Runtime;

namespace Dropship.Unity.Extension;

public static class AssetExtension
{
    public static T LoadAsset<T>(this AssetBundle self, string name) where T : Object
    {
        return self.LoadAsset(name, Il2CppType.Of<T>()).Cast<T>();
    }

    public static void Register(this AssetBundle self, string key)
    {
        AssetHelper.Register(key, self);
    }
}
