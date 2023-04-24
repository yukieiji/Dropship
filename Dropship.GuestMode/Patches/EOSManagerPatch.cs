using System.Reflection;
using System.Linq;

using AmongUs.Data;
using Epic.OnlineServices;
using InnerNet;

using HarmonyLib;

using Dropship.Il2Cpp;

// From Reactor.RemoveAccounts by MIT License : https://github.com/NuclearPowered/Reactor.RemoveAccounts

namespace Dropship.GuestAccount.Patches;

[HarmonyPatch]
public static class EOSManagerRunLoginPatch
{
    public static MethodBase TargetMethod()
    {
        return EnumeratorPatchHelper.GetMoveNextMethod(
            typeof(EOSManager), nameof(EOSManager.RunLogin));
    }

    public static bool Prefix(ref bool __result)
    {
        var eosManager = EOSManager.Instance;

        DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        DataManager.Player.Onboarding.LastAcceptedPrivacyPolicyVersion = 
            Constants.PrivacyPolicyVersion;

        eosManager.userId = new ProductUserId();

        eosManager.hasRunLoginFlow = true;
        eosManager.loginFlowFinished = true;

        AccountManager.Instance.privacyPolicyBg.gameObject.SetActive(false);
        eosManager.CloseStartupWaitScreen();
        eosManager.HideCallbackWaitAnim();
        eosManager.IsAllowedOnline(true);

        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Awake))]
public static class EOSManagerAwakePatch
{
    private static readonly PropertyInfo localUserIdProperty = 
        typeof(EpicManager).GetProperty(
            "localUserId", BindingFlags.Static | BindingFlags.Public);

    public static bool Prefix(EOSManager __instance)
    {
        new DestroyableSingleton<EOSManager>(__instance.Pointer).Awake();

        __instance.platformInitialized = true;
        
        localUserIdProperty?.SetValue(null, new EpicAccountId());
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
public static class EOSManagerProductUserIdPatch
{
    public static bool Prefix(out string __result)
    {
        __result = string.Empty;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.UserIDToken), MethodType.Getter)]
public static class EOSManagerUserIDTokenPatch
{
    public static bool Prefix(out string __result)
    {
        __result = null;
        return false;
    }
}
