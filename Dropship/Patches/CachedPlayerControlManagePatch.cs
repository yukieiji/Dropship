using System.Reflection;
using System.Linq;
using HarmonyLib;

using Dropship.Il2Cpp;
using Dropship.Performance;

namespace Dropship.Patches;

[HarmonyPatch]
internal static class CacheLocalPlayerPatch
{
    [HarmonyTargetMethod]
    public static MethodBase TargetMethod()
    {
        return EnumeratorPatchHelper.GetMoveNextMethod(
            typeof(PlayerControl), nameof(PlayerControl.Start));
    }

    [HarmonyPostfix]
    public static void SetLocalPlayer()
    {
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (!localPlayer)
        {
            CachedPlayerControl.LocalPlayer = null;
            return;
        }

        CachedPlayerControl cached = CachedPlayerControl.AllPlayerControls.FirstOrDefault(
            p => p.PlayerControl.Pointer == localPlayer.Pointer);
        if (cached != null)
        {
            CachedPlayerControl.LocalPlayer = cached;
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
internal static class CachedPlayerControlSetUpPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) { return; }

        new CachedPlayerControl(__instance);

#if DEBUG
        foreach (var cachedPlayer in CachedPlayerControl.AllPlayerControls)
        {
            if (!cachedPlayer.PlayerControl ||
                !cachedPlayer.PlayerPhysics ||
                !cachedPlayer.NetTransform ||
                !cachedPlayer.transform)
            {
                Logger<DropshipPlugin>.Debug(
                    $"CachedPlayer {cachedPlayer.PlayerControl.name} has null fields");
            }
        }
#endif
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Deserialize))]
internal static class CachedPlayerControlPlayerIdDeserializePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        CachedPlayerControl.PlayerPtrs[__instance.Pointer].PlayerId = __instance.PlayerId;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
internal static class CachedPlayerControlDestroyPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) { return; }
        CachedPlayerControl.Remove(__instance);
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.AddPlayer))]
	internal static class CachedPlayerControlDataAddPatch
	{
		public static void Postfix()
		{
			foreach (CachedPlayerControl cachedPlayer in CachedPlayerControl.AllPlayerControls)
			{
				cachedPlayer.Data = cachedPlayer.PlayerControl.Data;
				cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
			}
		}
	}

	[HarmonyPatch(typeof(GameData), nameof(GameData.Deserialize))]
	internal static class CachedPlayerControlDataDeserializePatch
	{
		public static void Postfix()
		{
			foreach (CachedPlayerControl cachedPlayer in CachedPlayerControl.AllPlayerControls)
			{
				cachedPlayer.Data = cachedPlayer.PlayerControl.Data;
				cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
			}
		}
	}
