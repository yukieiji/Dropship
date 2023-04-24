using HarmonyLib;
using UnityEngine;

// From Reactor.RemoveAccounts by MIT License : https://github.com/NuclearPowered/Reactor.RemoveAccounts

namespace Dropship.GuestAccount.Patches;

[HarmonyPatch(typeof(FriendsListButton), nameof(FriendsListButton.Awake))]
public static class FriendsListDestroy
{
    public static void Prefix(FriendsListButton __instance)
    {
        Object.Destroy(__instance.gameObject);
    }
}
