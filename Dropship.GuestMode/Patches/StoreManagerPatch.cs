using HarmonyLib;

// From Reactor.RemoveAccounts by MIT License : https://github.com/NuclearPowered/Reactor.RemoveAccounts

namespace Dropship.GuestAccount.Patches;

[HarmonyPatch(typeof(StoreManager), nameof(StoreManager.Initialize))]
public static class StoreManagerPatch
{
    public static bool Prefix()
    {
        return false;
    }
}
