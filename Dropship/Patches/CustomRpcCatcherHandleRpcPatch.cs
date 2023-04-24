using System.Linq;
using System.Reflection;

using HarmonyLib;
using Hazel;
using Dropship.Network;

namespace Dropship.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal static class CustomRpcCatcherHandleRpcPatch
{
    [HarmonyPrefix, HarmonyPriority(Priority.First)]
    public static bool Prefix(
        PlayerControl __instance,
        [HarmonyArgument(0)] byte callId,
        [HarmonyArgument(1)] MessageReader reader)
    {
        if (callId != CustomRpcCatcher.Id ||
            __instance == null || 
            reader == null) { return true; }

        CustomRpcCatcher.Catch(reader);

        return false;
    }
}
