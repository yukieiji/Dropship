using System;
using System.Reflection;
using System.Collections.Generic;

using HarmonyLib;

using Il2CppInterop.Runtime.Injection;
using BepInEx.Unity.IL2CPP;

namespace Dropship.Il2Cpp.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class Il2CppRegisterAttribute : Attribute
{
    public Type[] Interfaces { get; private set; }

    private static HashSet<Assembly> registered = new HashSet<Assembly>();

    public Il2CppRegisterAttribute()
    {
        Interfaces = Type.EmptyTypes;
    }

    public Il2CppRegisterAttribute(params Type[] interfaces)
    {
        Interfaces = interfaces;
    }

    public static void RegisterAssembly(Assembly dll)
    {
        foreach (Type type in dll.GetTypes())
        {
            Il2CppRegisterAttribute attribute =
                CustomAttributeExtensions.GetCustomAttribute<Il2CppRegisterAttribute>(type);
            if (attribute != null)
            {
                RegisterType(type, attribute.Interfaces);
            }
        }
    }

    public static void RegisterType(
        Type targetType, Type[] interfaces)
    {
        Type targetBase = targetType.BaseType;

        Il2CppRegisterAttribute baseAttribute =
            targetType == null ?
            null :
            CustomAttributeExtensions.GetCustomAttribute<Il2CppRegisterAttribute>(targetBase);

        if (baseAttribute != null)
        {
            RegisterType(targetBase, baseAttribute.Interfaces);
        }

        if (ClassInjector.IsTypeRegisteredInIl2Cpp(targetType)) { return; }

        try
        {
            ClassInjector.RegisterTypeInIl2Cpp(
                targetType, new RegisterTypeOptions
                {
                    Interfaces = interfaces,
                    LogSuccess = true
                }
            );
        }
        catch (Exception e)
        {
            string excStr = targetType.FullDescription();
            Logger<DropshipPlugin>.Error($"Il2Cpp Register Error!!  Exc:{e}  {excStr}");
        }
    }

    internal static void AddAutoRegisterHook()
    {
        IL2CPPChainloader.Instance.PluginLoad += 
            (_, assembly, _) => RegisterAssembly(assembly);
    }
}
