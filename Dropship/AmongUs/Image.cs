using UnityEngine;

using AmongUs.GameOptions;

using Dropship.Performance;

namespace Dropship.AmongUs;

public static class Image
{
    public static Sprite GetAdminButtonImage()
    {
        var imageDict = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings;
        switch (GameOptionsManager.Instance.CurrentGameOptions.GetByte(
            ByteOptionNames.MapId))
        {
            case 0:
            case 3:
                return imageDict[ImageNames.AdminMapButton].Image;
            case 1:
                return imageDict[ImageNames.MIRAAdminButton].Image;
            case 2:
                return imageDict[ImageNames.PolusAdminButton].Image;
            default:
                return imageDict[ImageNames.AirshipAdminButton].Image;
        }
    }

    public static Sprite GetSecurityImage()
    {
        var imageDict = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings;
        switch (GameOptionsManager.Instance.CurrentGameOptions.GetByte(
            ByteOptionNames.MapId))
        {
            case 1:
                return imageDict[ImageNames.DoorLogsButton].Image;
            default:
                return imageDict[ImageNames.CamsButton].Image;
        }
    }

    public static Sprite GetVitalImage() =>
        FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[
            ImageNames.VitalsButton].Image;
}
