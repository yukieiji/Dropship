using System;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx.Unity.IL2CPP;

namespace Dropship.Utilities;

internal static class DropshipVersionShower
{
    private static TextMeshPro showText;

    internal static void Create()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((_, _) =>
        {
            var original = UnityEngine.Object.FindObjectOfType<VersionShower>();
            if (!original)
            {
                return;
            }

            var gameObject = new GameObject($"Dropship VersionShower{Guid.NewGuid()}");
            gameObject.transform.parent = original.transform.parent;

            var aspectPosition = gameObject.AddComponent<AspectPosition>();

            aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftTop;

            var originalAspectPosition = original.GetComponent<AspectPosition>();
            var originalPosition = originalAspectPosition.DistanceFromEdge;
            originalPosition.y = 0.15f;
            originalAspectPosition.DistanceFromEdge = originalPosition;
            originalAspectPosition.AdjustPosition();

            var position = originalPosition;
            position.x += 10.075f - 0.1f;
            position.y += 2.75f - 0.15f;
            position.z -= 1;
            aspectPosition.DistanceFromEdge = position;

            aspectPosition.AdjustPosition();

            showText = gameObject.AddComponent<TextMeshPro>();
            showText.fontSize = 2;

            UpdateText();
        }));
    }

    internal static void UpdateText()
    {
        if (showText == null) { return; }
        
        showText.text =
            string.Concat(
             $"Dropship : {DropshipPlugin.Version}", "\n",
             $"Mods: {IL2CPPChainloader.Instance.Plugins.Count}");
    }
}
