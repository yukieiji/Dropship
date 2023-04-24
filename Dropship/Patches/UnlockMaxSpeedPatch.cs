using HarmonyLib;

using AmongUs.GameOptions;

namespace Dropship.Patches;

[HarmonyPatch(typeof(PlayerPhysics), "TrueSpeed", MethodType.Getter)]
internal static class UnlockMaxSpeedPatch
{
    private const float maxModSpeed = 1000.0f;

    public static bool Prefix(
        PlayerPhysics __instance,
        ref float __result)
    {
        // 最大速度 = 基本速度 * PlayerControl.GameOptions.PlayerSpeedMod * 3.0f;
        __result = __instance.Speed * 
            maxModSpeed * 
            GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.PlayerSpeedMod);
        return false;
    }
}
