﻿using HarmonyLib;

// From Reactor.RemoveAccounts by MIT License : https://github.com/NuclearPowered/Reactor.RemoveAccounts

namespace Dropship.GuestAccount.Patches;

[HarmonyPatch(
    typeof(HttpMatchmakerManager),
    nameof(HttpMatchmakerManager.TryReadCachedToken))]
public static class HttpMatchmakerManagerTryReadCachedTokenPatch
{
    public static bool Prefix(out bool __result, out string matchmakerToken)
    {
        __result = true;
        matchmakerToken = "RemoveAccounts";
        return false;
    }
}
