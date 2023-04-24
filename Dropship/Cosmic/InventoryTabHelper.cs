using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace Dropship.Cosmic;

internal static class InventoryTabHelper
{
    internal const float HeaderSize = 0.8f;
    internal const string InnerslothPackageName = "innerslothMake";

    private const float headerX = 0.8f;
    private const float inventoryZ = -2f;

    private static TMP_Text textTemplate;

    internal static void AddCollectionText(
       InventoryTab instance,
       float yPos,
       string packageName,
       ref List<TMP_Text> textList,
       ref float offset)
    {
        if (textTemplate == null)
        {
            textTemplate = PlayerCustomizationMenu.Instance.itemName;
        }

        TMP_Text title = Object.Instantiate(
            textTemplate, instance.scroller.Inner);
        title.transform.parent = instance.scroller.Inner;
        title.transform.localPosition = new Vector3(headerX, yPos, inventoryZ);
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize *= 1.25f;
        title.fontWeight = FontWeight.Thin;
        title.enableAutoSizing = false;
        title.autoSizeTextContainer = true;
        title.text = packageName;
        offset -= HeaderSize * instance.YOffset;

        textList.Add(title);
    }

    internal static void HideCollectionText(
        List<TMP_Text> packageText,
        float inventoryTop,
        float inventoryBottom)
    {
        // Manually hide all custom TMPro.TMP_Text objects that are outside the ScrollRect
        foreach (TMP_Text customText in packageText)
        {
            if (customText != null && 
                customText.transform != null && 
                customText.gameObject != null)
            {
                bool active = 
                    customText.transform.position.y <= inventoryTop && 
                    customText.transform.position.y >= inventoryBottom;
                float epsilon = Mathf.Min(
                    Mathf.Abs(customText.transform.position.y - inventoryTop),
                    Mathf.Abs(customText.transform.position.y - inventoryBottom));
                if (active != customText.gameObject.active && epsilon > 0.1f)
                {
                    customText.gameObject.SetActive(active);
                }
            }
        }
    }

    internal static ColorChip CreateColorChip(
        InventoryTab instance, int setIndex, float offset)
    {
        float xPos = instance.XRange.Lerp(
            (setIndex % instance.NumPerRow) / (instance.NumPerRow - 1f));
        float yPos = offset - (setIndex / instance.NumPerRow) * instance.YOffset;
        ColorChip colorChip = Object.Instantiate(
            instance.ColorTabPrefab, instance.scroller.Inner);

        colorChip.transform.localPosition = new Vector3(xPos, yPos, inventoryZ);

        return colorChip;
    }
}
