using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

namespace Dropship.GuestAccount;

[BepInAutoPlugin("me.yukieiji.guestaccount", "GuestAccount")]
[BepInProcess("Among Us.exe")]
[BepInDependency(DropshipPlugin.Id)]
public partial class GuestAccountPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        this.Harmony.PatchAll();
    }
}
