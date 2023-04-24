using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppObject = Il2CppSystem.Object;

namespace Dropship.Translation.Extension;

public static class StringIdExtension
{
    public static string GetString(this TranslationController cont, string id)
    {
        return cont.GetString(id, string.Empty, Array.Empty<Il2CppObject>());
    }

    public static string GetString(
        this TranslationController cont, string id, params object[] parts)
    {
        return cont.GetString(
            id, defaultStr: string.Empty, 
            (Il2CppReferenceArray<Il2CppObject>)parts);;
    }
}
