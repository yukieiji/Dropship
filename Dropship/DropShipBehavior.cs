using System;
using System.Linq;

using UnityEngine;

using BepInEx.Unity.IL2CPP;

using Dropship.Il2Cpp.Attributes;

namespace Dropship;

[Il2CppRegister]
internal sealed class DropshipBehavior : MonoBehaviour
{
    public DropshipBehavior(IntPtr ptr) : base(ptr)
    { }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ReloadAllConfig();
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Logger<DropshipPlugin>.Dump();
        }
    }

    private void ReloadAllConfig()
    {
        Logger<DropshipPlugin>.Info("Reloading all configs");

        foreach (var pluginInfo in IL2CPPChainloader.Instance.Plugins.Values)
        {
            var config = ((BasePlugin)pluginInfo.Instance).Config;
            if (!config.Any())
            {
                continue;
            }

            try
            {
                config.Reload();
            }
            catch (Exception e)
            {
                Logger<DropshipPlugin>.Warning(
                    $"Exception occured during reload of {pluginInfo.Metadata.Name}: {e}");
            }
        }
    }
}
