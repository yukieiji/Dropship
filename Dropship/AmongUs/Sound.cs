
using UnityEngine;

using Dropship.Performance;

namespace Dropship.AmongUs;

public static class Sound
{
    public static AudioClip KillSE => 
        CachedPlayerControl.LocalPlayer.PlayerControl.KillSfx;

    public static AudioClip GuardianAngelBlockSE =>
        FastDestroyableSingleton<RoleManager>.Instance.protectAnim.UseSound;
}

