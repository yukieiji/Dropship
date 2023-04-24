using System.Collections.Generic;
using System.Linq;

using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using Il2CppSystem;
using TMPro;
using UnityEngine.Events;

using Dropship.API.Interface;
using Dropship.Cosmic;
using Dropship.Performance;
using Dropship.Unity;

namespace Dropship.Patches.CosmicTabs;

[HarmonyPatch]
internal static class NameplatesTabPatch
{
    private static List<TMP_Text> npCollectionText = new List<TMP_Text>();

    private static float inventoryTop = 1.5f;
    private static float inventoryBottom = -2.5f;


    [HarmonyPrefix]
    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
    public static bool NameplatesTabOnEnablePrefix(NameplatesTab __instance)
    {
        var customNp = CosmicManager.CustomNp;
        if (!customNp.Any()) { return true; }

        inventoryTop = __instance.scroller.Inner.position.y - 0.5f;
        inventoryBottom = __instance.scroller.Inner.position.y - 4.5f;

        NamePlateData[] unlockedNamePlate = FastDestroyableSingleton<
            HatManager>.Instance.GetUnlockedNamePlates();
        var npPackage = new Dictionary<string, List<NamePlateData>>();

        ObjectHeler.DestoryAll(npCollectionText);
        ObjectHeler.DestoryAll(__instance.ColorChips.ToArray());

        npCollectionText.Clear();
        __instance.ColorChips.Clear();

        foreach (NamePlateData npData in unlockedNamePlate)
        {
            bool result = customNp.TryGetValue(
                npData.ProductId, out ICustomCosmicData<NamePlateData> np);

            string collection = result ?
                np.CollectionName : 
                InventoryTabHelper.InnerslothPackageName;

            if (!npPackage.ContainsKey(collection))
            {
                npPackage.Add(collection, new List<NamePlateData>());
            }
            npPackage[collection].Add(npData);
        }

        float yOffset = __instance.YStart;

        var orderedKeys = npPackage.Keys.OrderBy((string x) => {
            if (x == InventoryTabHelper.InnerslothPackageName)
            {
                return 0;
            }
            else
            {
                return 100;
            }
        });

        float headerSize = InventoryTabHelper.HeaderSize;

        foreach (string key in orderedKeys)
        {
            createNameplateTab(npPackage[key], key, yOffset, __instance);
            yOffset = 
                (yOffset - (headerSize * __instance.YOffset)) - 
                ((npPackage[key].Count - 1) / __instance.NumPerRow) * __instance.YOffset -
                headerSize;
        }

        __instance.scroller.ContentYBounds.max = -(yOffset + 3.0f + headerSize);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.Update))]
    public static void HatsTabUpdatePostfix()
    {
        InventoryTabHelper.HideCollectionText(
            npCollectionText, inventoryTop, inventoryBottom);
    }

    private static void createNameplateTab(
        List<NamePlateData> namePlates, string packageName,
        float yStart, NameplatesTab __instance)
    {
        float offset = yStart;

        InventoryTabHelper.AddCollectionText(
            __instance, yStart, packageName,
            ref npCollectionText, ref offset);

        int numHats = namePlates.Count;

        PlayerCustomizationData playerSkinData = DataManager.Player.Customization;

        for (int i = 0; i < numHats; i++)
        {
            NamePlateData np = namePlates[i];

            ColorChip colorChip = InventoryTabHelper.CreateColorChip(
                __instance, i, offset);

            colorChip.Button.ClickMask = __instance.scroller.Hitbox;

            if (ActiveInputManager.currentControlType == 
                ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener(
                    (UnityAction)(() => __instance.SelectNameplate(np)));
                colorChip.Button.OnMouseOut.AddListener(
                    (UnityAction)(
                        () => __instance.SelectNameplate(
                            FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById(
                                playerSkinData.NamePlate))));
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.ClickEquip()));
            }
            else
            {
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.SelectNameplate(np)));
            }

            __instance.StartCoroutine(
                np.CoLoadViewData((Action<NamePlateViewData>)((n) => {
                    colorChip.GetComponent<NameplateChip>().image.sprite = n.Image;
                })));
            colorChip.ProductId = np.ProdId;
            __instance.ColorChips.Add(colorChip);
        }
    }
}
