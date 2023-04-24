using System.Collections.Generic;
using System.Linq;

using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using TMPro;
using UnityEngine.Events;

using Dropship.API.Interface;
using Dropship.Cosmic;
using Dropship.Performance;
using Dropship.Unity;

namespace Dropship.Patches.CosmicTabs;

[HarmonyPatch]
internal static class HatsTabPatch
{
    private static List<TMP_Text> hatCollectionText = new List<TMP_Text>();

    private static float inventoryTop = 1.5f;
    private static float inventoryBottom = -2.5f;


    [HarmonyPrefix]
    [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
    public static bool HatsTabOnEnablePrefix(HatsTab __instance)
    {
        var customHat = CosmicManager.CustomHat;
        if (!customHat.Any()) { return true; }

        inventoryTop = __instance.scroller.Inner.position.y - 0.5f;
        inventoryBottom = __instance.scroller.Inner.position.y - 4.5f;

        HatData[] unlockedHats = FastDestroyableSingleton<
            HatManager>.Instance.GetUnlockedHats();
        var hatPackage = new Dictionary<string, List<HatData>>();

        ObjectHeler.DestoryAll(hatCollectionText);
        ObjectHeler.DestoryAll(__instance.ColorChips.ToArray());

        hatCollectionText.Clear();
        __instance.ColorChips.Clear();

        foreach (HatData hatData in unlockedHats)
        {
            bool result = customHat.TryGetValue(
                hatData.ProductId, out ICustomCosmicData<HatData> hat);

            string collection = result ? 
                hat.CollectionName : 
                InventoryTabHelper.InnerslothPackageName;

            if (!hatPackage.ContainsKey(collection))
            {
                hatPackage.Add(collection, new List<HatData>());
            }
            hatPackage[collection].Add(hatData);
        }

        float yOffset = __instance.YStart;

        var orderedKeys = hatPackage.Keys.OrderBy((string x) => {
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
            createHatTab(hatPackage[key], key, yOffset, __instance);
            yOffset = 
                (yOffset - (headerSize * __instance.YOffset)) - 
                ((hatPackage[key].Count - 1) / __instance.NumPerRow) * __instance.YOffset -
                headerSize;
        }

        __instance.scroller.ContentYBounds.max = -(yOffset + 3.0f + headerSize);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.Update))]
    public static void HatsTabUpdatePostfix()
    {
        InventoryTabHelper.HideCollectionText(
            hatCollectionText, inventoryTop, inventoryBottom);
    }

    private static void createHatTab(
        List<HatData> hats, string packageName, float yStart, HatsTab __instance)
    {
        float offset = yStart;

        InventoryTabHelper.AddCollectionText(
            __instance, yStart, packageName,
            ref hatCollectionText, ref offset);

        int numHats = hats.Count;

        PlayerCustomizationData playerSkinData = DataManager.Player.Customization;

        for (int i = 0; i < numHats; i++)
        {
            HatData hat = hats[i];

            ColorChip colorChip = InventoryTabHelper.CreateColorChip(
                __instance, i, offset);

            int color = __instance.HasLocalPlayer() ?
                PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId :
                playerSkinData.colorID;

            if (ActiveInputManager.currentControlType == 
                ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener(
                    (UnityAction)(() => __instance.SelectHat(hat)));
                colorChip.Button.OnMouseOut.AddListener(
                    (UnityAction)(
                        () =>
                        {
                            __instance.SelectHat(
                                FastDestroyableSingleton<HatManager>.Instance.GetHatById(
                                    playerSkinData.Hat));
                        }));
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.ClickEquip()));
            }
            else
            {
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.SelectHat(hat)));
            }

            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.ScrollingUI);
            colorChip.Inner.SetHat(hat, color);
            colorChip.Inner.transform.localPosition = hat.ChipOffset;
            colorChip.Tag = hat;
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            __instance.ColorChips.Add(colorChip);
        }
    }
}
