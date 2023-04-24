using System;
using System.Linq;

using BepInEx.Unity.IL2CPP;
using xCloud;

namespace Dropship.Utilities;

internal static class PluginSingleton
{
    internal static void AddAutoCreatePluginSingleton()
    {
        IL2CPPChainloader.Instance.PluginLoad +=
            (_, _, plugin) =>
            {
                CreateSingleton(plugin);
            };
    }

    private static void CreateSingleton<T>(T plugin) where T : BasePlugin
    {
        PluginSingleton<T>.Create(plugin);
    }
}

public static class PluginSingleton<T> where T : BasePlugin
{
    public static T Instance => instance;
    private static T instance;

    internal static void Create(T plugin)
    {
        instance = plugin;
    }
}
