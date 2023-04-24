using UnityEngine;

namespace Dropship.Unity.Extension;

public static class ObjectExtension
{
    public static void DontUnload<T>(this T self) where T : Object
    {
        self.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
    }
}
