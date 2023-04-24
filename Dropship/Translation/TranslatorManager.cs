using System.Collections.Generic;

using Il2CppCollection = Il2CppSystem.Collections.Generic;

using Dropship.Performance;

namespace Dropship.Translation;

public static class TranslatorManager
{
    private static List<TranslatorBase> allTrans = new List<TranslatorBase>();

    public static void Register(TranslatorBase translator)
    {
        if (!allTrans.Contains(translator))
        {
            allTrans.Add(translator);

            allTrans.Sort((a, b) => b.Priority - a.Priority);
        }
    }

    public static void AddAditionalTransData(
        Dictionary<string, string> newData)
    {
        if (!TranslationController.InstanceExists)
        {
            Logger<DropshipPlugin>.Error($"Can't Add new data");
        }
        AddData(
            FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.AllStrings,
            newData);
    }

    internal static void AddTranslationData(
        SupportedLangs languageId,
        Il2CppCollection.Dictionary<string, string> allData)
    {
        foreach (TranslatorBase translator in allTrans)
        {
            SupportedLangs useLang = 
                translator.IsSupport(languageId) ? languageId : translator.DefaultLang;
            AddData(
                allData,
                translator.GetTranslation(useLang));
        }
    }

    private static void AddData(
        Il2CppCollection.Dictionary<string, string> allData,
        Dictionary<string, string> newData)
    {
        foreach (var (key, data) in newData)
        {
            if (allData.ContainsKey(key))
            {
                Logger<DropshipPlugin>.Error(
                    $"Detect:Translation Data conflict!!  Key:{key} Data:{data}");
            }
            else
            {
                allData.Add(key, data);
            }
        }
    }
}
