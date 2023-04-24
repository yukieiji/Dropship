using System.Linq;

using UnityEngine;

using Dropship.Network;
using Dropship.Patches;
using Dropship.Performance;
using Dropship.Il2Cpp.Extension;

namespace Dropship.AmongUs;

public static class Player
{
    public static PlayerControl GetPlayerControl(byte playerId)
    {
        return CachedPlayerControl.AllPlayerControls.FirstOrDefault(
            cp => cp.PlayerId == playerId);
    }

    public static float GetTaskProgress(byte playerId)
    {
        return GetTaskProgress(GetPlayerControl(playerId));
    }

    public static float GetTaskProgress(PlayerControl player)
    {
        return GetTaskProgress(player.Data);
    }

    public static float GetTaskProgress(GameData.PlayerInfo player)
    {
        int taskNum = 0;
        int compNum = 0;

        foreach (GameData.TaskInfo task in player.Tasks.GetFastEnumerator())
        {
            ++taskNum;

            if (task.Complete)
            {
                ++compNum;
            }
        }
        return (float)compNum / (float)taskNum;
    }

    public static void RpcUnceckMurderPlayer(
        byte murderPlayerId, byte targetPlayerId, bool isSnap = true)
    {
        byte bytedIsSnap = isSnap ? byte.MinValue : byte.MaxValue;

        using (var caller = CustomRpcCaller.Create(
            DropshipRpc.MurderPlayer))
        {
            caller.WriteByte(murderPlayerId);
            caller.WriteByte(targetPlayerId);
            caller.WriteByte(bytedIsSnap);
        }
        UncheckMurderPlayer(murderPlayerId, targetPlayerId, bytedIsSnap);
    }

    public static void RpcUnceckMurderPlayer(
        PlayerControl murder, PlayerControl target, bool isSnap = true)
    {
        RpcUnceckMurderPlayer(murder.PlayerId, target.PlayerId, isSnap);
    }

    public static void RpcUnceckSnapTo(
        byte targetPlayerId, Vector2 snapPos)
    {
        using (var caller = CustomRpcCaller.Create(
            DropshipRpc.SnapTo))
        {
            caller.WriteByte(targetPlayerId);
            caller.WriteFloat(snapPos.x);
            caller.WriteFloat(snapPos.y);
        }
        UncheckSnapTo(targetPlayerId, snapPos);
    }

    public static void RpcUnceckSnapTo(
        PlayerControl target, Vector2 snapPos)
    {
        RpcUnceckSnapTo(target.PlayerId, snapPos);
    }

    public static void RpcUncheckRevive(PlayerControl target)
    {
        RpcUncheckRevive(target.PlayerId);
    }

    public static void RpcUncheckRevive(byte targetPlayerId)
    {
        using (var caller = CustomRpcCaller.Create(
            DropshipRpc.Revive))
        {
            caller.WriteByte(targetPlayerId);
        }
        UncheckRevive(targetPlayerId);
    }

    internal static void UncheckMurderPlayer(
        byte murderPlayerId, byte targetPlayerId, byte bytedIsSnapState)
    {
        PlayerControl source = GetPlayerControl(murderPlayerId);
        PlayerControl target = GetPlayerControl(targetPlayerId);

        if (source != null && target != null)
        {
            HideKillSnapPatch.SetAnimationState(
                bytedIsSnapState == byte.MaxValue);
            source.MurderPlayer(target);
        }
    }

    internal static void UncheckSnapTo(
        byte targetPlayerId, Vector2 pos)
    {
        PlayerControl target = GetPlayerControl(targetPlayerId);

        if (target != null)
        {
            target.NetTransform.SnapTo(pos);
        }
    }

    internal static void UncheckRevive(byte targetId)
    {
        PlayerControl target = GetPlayerControl(targetId);

        if (target != null)
        {
            target.Revive();

            // なんか起きて失敗
            if (target.Data == null ||
                target.Data.IsDead ||
                target.Data.Disconnected) { return; }

            // 死体は消しておく
            Map.CleanDeadBody(target.PlayerId);
        }
    }
}
