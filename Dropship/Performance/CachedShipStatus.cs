using System.Collections.Generic;

// from TOR : https://github.com/Eisbison/TheOtherRoles/blob/main/TheOtherRoles/Utilities/MapUtilities.cs

namespace Dropship.Performance;

public static class CachedShipStatus
{
    public static ShipStatus Instance { get; private set; }
    public static bool IsNull => Instance == null;

    public static Dictionary<SystemTypes, ISystemType> Systems
    {
        get
        {
            if (allSystems.Count == 0) { SetSystem(); }
            return allSystems;
        }
    }

    private static readonly Dictionary<SystemTypes, ISystemType> allSystems = new Dictionary<SystemTypes, ISystemType>();

    public static void SetUp(ShipStatus instance)
    {
        Instance = instance;
    }

    public static void Destroy()
    {
        Instance = null;
        allSystems.Clear();
    }

    private static void SetSystem()
    {
        if (!Instance) { return; }

        var systems = Instance.Systems;
        if (systems.Count <= 0) { return; }

        foreach (var systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) { continue; }
            allSystems[systemTypes] = systems[systemTypes];
        }
    }
}
