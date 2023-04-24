using HarmonyLib;

namespace Dropship.Patches;

[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
internal static class HideKillSnapPatch
{
    private static bool hideNextAnimation = false;

    public static void SetAnimationState(bool isHide)
    {
        hideNextAnimation = isHide;
    }

    public static void Prefix(
        [HarmonyArgument(0)] ref PlayerControl source,
        [HarmonyArgument(1)] ref PlayerControl target)
    {
        if (hideNextAnimation)
        {
            source = target;
        }
        SetAnimationState(false);
    }
}
