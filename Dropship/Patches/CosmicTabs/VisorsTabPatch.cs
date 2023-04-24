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
internal static class VisorsTabPatch
{
    private static List<TMP_Text> visorCollectionText = new List<TMP_Text>();

    private static float inventoryTop = 1.5f;
    private static float inventoryBottom = -2.5f;


    [HarmonyPrefix]
    [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
    public static bool VisorsTabOnEnablePrefix(VisorsTab __instance)
    {
        var customVisor = CosmicManager.CustomVisor;
        if (!customVisor.Any()) { return true; }

        inventoryTop = __instance.scroller.Inner.position.y - 0.5f;
        inventoryBottom = __instance.scroller.Inner.position.y - 4.5f;

        VisorData[] unlockedVisor = FastDestroyableSingleton<
            HatManager>.Instance.GetUnlockedVisors();
        var visorPackage = new Dictionary<string, List<VisorData>>();

        ObjectHeler.DestoryAll(visorCollectionText);
        ObjectHeler.DestoryAll(__instance.ColorChips.ToArray());

        visorCollectionText.Clear();
        __instance.ColorChips.Clear();

        foreach (VisorData visorData in unlockedVisor)
        {
            bool result = customVisor.TryGetValue(
                visorData.ProductId, out ICustomCosmicData<VisorData> visor);

            string collection = result ?
                visor.CollectionName : 
                InventoryTabHelper.InnerslothPackageName;

            if (!visorPackage.ContainsKey(collection))
            {
                visorPackage.Add(collection, new List<VisorData>());
            }
            visorPackage[collection].Add(visorData);
        }

        float yOffset = __instance.YStart;

        var orderedKeys = visorPackage.Keys.OrderBy((string x) => {
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
            createVisorTab(visorPackage[key], key, yOffset, __instance);
            yOffset = 
                (yOffset - (headerSize * __instance.YOffset)) - 
                ((visorPackage[key].Count - 1) / __instance.NumPerRow) * __instance.YOffset -
                headerSize;
        }

        __instance.scroller.ContentYBounds.max = -(yOffset + 3.0f + headerSize);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.Update))]
    public static void HatsTabUpdatePostfix()
    {
        InventoryTabHelper.HideCollectionText(
            visorCollectionText, inventoryTop, inventoryBottom);
    }

    private static void createVisorTab(
        List<VisorData> visores, string packageName, float yStart, VisorsTab __instance)
    {
        float offset = yStart;

        InventoryTabHelper.AddCollectionText(
            __instance, yStart, packageName,
            ref visorCollectionText, ref offset);

        int numVisor = visores.Count;

        PlayerCustomizationData playerSkinData = DataManager.Player.Customization;

        for (int i = 0; i < numVisor; i++)
        {
            VisorData vi = visores[i];

            ColorChip colorChip = InventoryTabHelper.CreateColorChip(
                __instance, i, offset);

            if (ActiveInputManager.currentControlType == 
                ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener(
                    (UnityAction)(() => __instance.SelectVisor(vi)));
                colorChip.Button.OnMouseOut.AddListener(
                    (UnityAction)(
                        () => __instance.SelectVisor(
                            FastDestroyableSingleton<HatManager>.Instance.GetVisorById(
                                playerSkinData.Visor))));
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.ClickEquip()));
            }
            else
            {
                colorChip.Button.OnClick.AddListener(
                    (UnityAction)(() => __instance.SelectVisor(vi)));
            }

            colorChip.Inner.transform.localPosition = vi.ChipOffset;
            colorChip.ProductId = vi.ProductId;
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.Tag = vi.ProdId;

            int color = __instance.HasLocalPlayer() ?
                PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId :
                playerSkinData.colorID;

            __instance.StartCoroutine(
                vi.CoLoadViewData((Action<VisorViewData>)((v) => {
                    colorChip.Inner.FrontLayer.sprite = v.IdleFrame;
                    __instance.UpdateSpriteMaterialColor(colorChip, v, color);
                })));

            __instance.ColorChips.Add(colorChip);
        }
    }
}
