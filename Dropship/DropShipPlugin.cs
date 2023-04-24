using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;
using UnityEngine.SceneManagement;

using Dropship.Compat;
using Dropship.Network;
using Dropship.Il2Cpp.Attributes;
using Dropship.Utilities;

namespace Dropship;

[BepInAutoPlugin("me.yukieiji.dropship", "Dropship")]
[BepInProcess("Among Us.exe")]
public partial class DropshipPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public DropshipPlugin()
    {
        PluginSingleton.AddAutoCreatePluginSingleton();
        Il2CppRegisterAttribute.AddAutoRegisterHook();
    }

    public override void Load()
    {
        this.Harmony.PatchAll();

        CustomRpcCatcher.Register(new DropshipRpcCatcher());
        DropshipVersionShower.Create();
        ShowModStamp();

        if (BepInExUpdater.UpdateRequired)
        {
            AddComponent<BepInExUpdater>();
        }

        this.AddComponent<DropshipBehavior>();
    }

    private static void ShowModStamp()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp();
            }
        }));
    }
}
