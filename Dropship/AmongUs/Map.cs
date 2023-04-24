using System.Linq;

using UnityEngine;

using AmongUs.GameOptions;

using Dropship.Network;
using Dropship.Unity;

namespace Dropship.AmongUs;

public static class Map
{
    public const string SkeldAdmin = "SkeldShip(Clone)/Admin/Ground/admin_bridge/MapRoomConsole";
    public const string SkeldSecurity = "SkeldShip(Clone)/Security/Ground/map_surveillance/SurvConsole";

    public const string MiraHqAdmin = "MiraShip(Clone)/Admin/MapTable/AdminMapConsole";
    public const string MiraHqSecurity = "MiraShip(Clone)/Comms/comms-top/SurvLogConsole";

    public const string PolusAdmin1 = "PolusShip(Clone)/Admin/mapTable/panel_map";
    public const string PolusAdmin2 = "PolusShip(Clone)/Admin/mapTable/panel_map (1)";
    public const string PolusSecurity = "PolusShip(Clone)/Electrical/Surv_Panel";
    public const string PolusVital = "PolusShip(Clone)/Office/panel_vitals";

    public const string AirShipSecurity = "Airship(Clone)/Security/task_cams";
    public const string AirShipVital = "Airship(Clone)/Medbay/panel_vitals";
    public const string AirShipArchiveAdmin = "Airship(Clone)/Records/records_admin_map";
    public const string AirShipCockpitAdmin = "Airship(Clone)/Cockpit/panel_cockpit_map";

    public static void DisableMapModule(string mapModuleName)
    {
        GameObject? obj = GameObject.Find(mapModuleName);
        if (obj != null)
        {
            ObjectHeler.DisableCollider<Collider2D>(obj);
            ObjectHeler.DisableCollider<PolygonCollider2D>(obj);
            ObjectHeler.DisableCollider<BoxCollider2D>(obj);
            ObjectHeler.DisableCollider<CircleCollider2D>(obj);
        }
    }

    public static SystemConsole? GetSecurityConsole()
    {
        // 0 = Skeld
        // 1 = Mira HQ
        // 2 = Polus
        // 3 = Dleks - deactivated
        // 4 = Airship
        var systemConsoleArray = Object.FindObjectsOfType<SystemConsole>();
        switch (GameOptionsManager.Instance.CurrentGameOptions.GetByte(
            ByteOptionNames.MapId))
        {
            case 0:
            case 3:
                return systemConsoleArray.FirstOrDefault(
                    x => x.gameObject.name.Contains("SurvConsole"));
            case 1:
                return systemConsoleArray.FirstOrDefault(
                    x => x.gameObject.name.Contains("SurvLogConsole"));
            case 2:
                return systemConsoleArray.FirstOrDefault(
                    x => x.gameObject.name.Contains("Surv_Panel"));
            case 4:
                return systemConsoleArray.FirstOrDefault(
                    x => x.gameObject.name.Contains("task_cams"));
            default:
                return null;
        }
    }

    public static SystemConsole? GetVitalConsole()
    {
        // 0 = Skeld
        // 1 = Mira HQ
        // 2 = Polus
        // 3 = Dleks - deactivated
        // 4 = Airship
        var systemConsoleArray = Object.FindObjectsOfType<SystemConsole>();
        switch (GameOptionsManager.Instance.CurrentGameOptions.GetByte(
            ByteOptionNames.MapId))
        {
            case 0:
            case 1:
            case 3:
                return null;
            case 2:
            case 4:
                return systemConsoleArray.FirstOrDefault(
                    x => x.gameObject.name.Contains("panel_vitals"));
            default:
                return null;
        }
    }

    public static void RpcCleanDeadBody(byte cleanDeadPlayerId)
    {
        using (var caller = CustomRpcCaller.Create(
            DropshipRpc.CleanDeadBody))
        {
            caller.WriteByte(cleanDeadPlayerId);
        }
        CleanDeadBody(cleanDeadPlayerId);
    }

    internal static void CleanDeadBody(byte targetId)
    {
        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (int i = 0; i < array.Length; ++i)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == targetId)
            {
                Object.Destroy(array[i].gameObject);
                break;
            }
        }
    }
}
