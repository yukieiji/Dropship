using System;

using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;

namespace Dropship.Il2Cpp.Extension;

public static class Il2CppObjectExtension
{
    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(
            self.GetType(),
            nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(
                self, Array.Empty<object>());
    }
}
