using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEngine;

using Dropship.Il2Cpp.Extension;
using Dropship.Unity.Extension;

namespace Dropship.Unity;

public static class AssetHelper
{
    private static Dictionary<string, AssetBundle> cachedBundle = new Dictionary<string, AssetBundle>();

    public static T GetAssetFromEmbeddedAssetBundle<T>(
        string bundleName, string objName) where T : Object
    {
        if (!cachedBundle.TryGetValue(bundleName, out AssetBundle bundle))
        {
            using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(
                bundleName);
            bundle = LoadAndRegisterAssetBundleFromStream(bundleName, stream);
        }
        return bundle.LoadAsset<T>(objName);
    }

    public static T GetAssetFromStorageAssetBundle<T>(
        string path, string objName) where T : Object
    {
        if (!cachedBundle.TryGetValue(path, out AssetBundle bundle))
        {
            bundle = LoadAndRegisterAssetBundleFromStorage(path);
        }
        return bundle.LoadAsset<T>(objName);
    }

    public static AssetBundle LoadAndRegisterAssetBundleFromStream(
        string key, Stream stream)
    {
        AssetBundle bundle = AssetBundle.LoadFromStream(stream.ToIl2Cpp());
        bundle.DontUnload();
        Register(key, bundle);
        return bundle;
    }

    public static AssetBundle LoadAndRegisterAssetBundleFromStorage(
        string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        bundle.DontUnload();
        Register(path, bundle);
        return bundle;
    }

    internal static void Register(string key, AssetBundle bundle)
    {
        cachedBundle.Add(key, bundle);
    }
}
