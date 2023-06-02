using Il2CppSystem.Collections.Generic;
using HarmonyLib;

using Dropship.Translation;
using AmongUs.Data;

namespace Dropship.Patches;

[HarmonyPatch(typeof(LanguageUnit), nameof(LanguageUnit.ParseTSV))]
internal static class AddModTransDataPatch
{
    public static void Postfix(
        LanguageUnit __instance,
        [HarmonyArgument(0)] string tsvText,
        [HarmonyArgument(1)] ref Dictionary<string, string> allStrings,
        [HarmonyArgument(2)] ref Dictionary<StringNames, QuickChatSentenceVariantSet> allQuickChatVariantSets)
    {
        TranslatorManager.AddTranslationData(
            DataManager.Settings.Language.CurrentLanguage,
            allStrings);
    }
}
