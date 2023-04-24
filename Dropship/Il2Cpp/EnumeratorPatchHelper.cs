using System;
using System.Linq;
using System.Reflection;

using HarmonyLib;

namespace Dropship.Il2Cpp;

public static class EnumeratorPatchHelper
{
    public static MethodBase GetMoveNextMethod(
        Type classType, string name)
    {
        var targetType = classType.GetNestedTypes(AccessTools.all).FirstOrDefault(
            t => t.Name.Contains(name));
        return AccessTools.Method(targetType, nameof(Il2CppSystem.Collections.IEnumerator.MoveNext));
    }
}
