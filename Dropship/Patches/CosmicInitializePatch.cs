using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

using Dropship.API.Interface;
using Dropship.Cosmic;

namespace Dropship.Patches;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
internal static class CosmicInitializePatch
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix(HatManager __instance)
    {
        try
        {
            List<HatData> hatData = __instance.allHats.ToList();
            foreach (ICustomCosmicData<HatData> hat in CosmicManager.CustomHat.Values)
            {
                hatData.Add(hat.GetData());
            }
            __instance.allHats = hatData.ToArray();
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Error($"Can't Add HatData Exc:{e}");
        }

        try
        {
            List<NamePlateData> npData = __instance.allNamePlates.ToList();
            foreach (ICustomCosmicData<NamePlateData> np in CosmicManager.CustomNp.Values)
            {
                npData.Add(np.GetData());
            }
            __instance.allNamePlates = npData.ToArray();
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Error($"Can't Add NamePlate Exc:{e}");
        }

        try
        {
            List<VisorData> visorData = __instance.allVisors.ToList();
            foreach (ICustomCosmicData<VisorData> vi in CosmicManager.CustomVisor.Values)
            {
                visorData.Add(vi.GetData());
            }
            __instance.allVisors = visorData.ToArray();
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Error($"Can't Add visor Exc:{e}");
        }
    }
}
