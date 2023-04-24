using HarmonyLib;

using Dropship.Performance;

namespace Dropship.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
internal static class CachedShipStatusSetUp
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix(ShipStatus __instance)
    {
        CachedShipStatus.SetUp(__instance);
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
internal static class CachedShipStatusDestroy
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix()
    {
        CachedShipStatus.Destroy();
    }
}
