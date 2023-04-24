using System.Collections.Generic;

using Dropship.API.Interface;

namespace Dropship.Cosmic;

public static class CosmicManager
{
    internal static IReadOnlyDictionary<
        string, ICustomCosmicData<HatData>> CustomHat => hat;
    internal static IReadOnlyDictionary<
        string, ICustomCosmicData<VisorData>> CustomVisor => visor;
    internal static IReadOnlyDictionary<
        string, ICustomCosmicData<NamePlateData>> CustomNp => np;

    private static Dictionary<string, ICustomCosmicData<HatData>> hat = new();
    private static Dictionary<string, ICustomCosmicData<VisorData>> visor = new();
    private static Dictionary<string, ICustomCosmicData<NamePlateData>> np = new();

    public static void AddHatData(string id, ICustomCosmicData<HatData> newHat)
    {
        Logger<DropshipPlugin>.Info($"Add Visor:{id}");
        hat.Add(id, newHat);
    }
    public static void AddVisorData(string id, ICustomCosmicData<VisorData> newVisor)
    {
        Logger<DropshipPlugin>.Info($"Add Visor:{id}");
        visor.Add(id, newVisor);
    }
    public static void AddNamePlateData(string id, ICustomCosmicData<NamePlateData> newNp)
    {
        Logger<DropshipPlugin>.Info($"Add Visor:{id}");
        np.Add(id, newNp);
    }
}
