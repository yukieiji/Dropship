using System;
using System.Collections.Generic;
using System.Reflection;

using Hazel;

using Dropship.Performance;

namespace Dropship.Network;

public sealed class CustomRpcCaller : IDisposable
{
    private MessageWriter writer;

    public CustomRpcCaller(
        uint modId, uint netId, uint cmd,
        SendOption sendOpt, int target)
    {
        
        Logger<DropshipPlugin>.Debug(
            $"---- Create Rpc caller ModId:{modId} command:{cmd} ----");
        
        this.writer = AmongUsClient.Instance.StartRpcImmediately(
            netId, CustomRpcCatcher.Id, sendOpt, target);
        this.writer.WritePacked(modId);
        this.writer.WritePacked(cmd);
    }

    public void WriteByte(byte data) => this.writer.Write(data);

    public void WriteSByte(sbyte data) => this.writer.Write(data);

    public void WriteShort(short data) => this.writer.Write(data);

    public void WriteUShort(ushort data) => this.writer.Write(data);

    public void WriteInt(int data) => this.writer.Write(data);

    public void WriteUInt(uint data) => this.writer.Write(data);

    public void WriteLong(long data) => this.writer.Write(data);

    public void WriteULong(ulong data) => this.writer.Write(data);

    public void WriteFloat(float data) => this.writer.Write(data);

    public void WriteBool(bool data) => this.writer.Write(data);

    public void WritePackedInt(int data) => this.writer.WritePacked(data);

    public void WritePackedUInt(uint data) => this.writer.WritePacked(data);

    public void Dispose()
    {
        AmongUsClient.Instance.FinishRpcImmediately(this.writer);
        Logger<DropshipPlugin>.Debug($"---- Call Rpc!! ----");
    }

    public static CustomRpcCaller Create(
        uint netId, uint cmd,
        SendOption sendOpt = SendOption.Reliable,
        int target = -1)
    {
        if (!CustomRpcCatcher.RegistedCatcher.TryGetModId(
                Assembly.GetCallingAssembly().FullName, out uint id))
        {
            throw new KeyNotFoundException("CanT find this mod catcher");
        }

        return new CustomRpcCaller(id, netId, cmd, sendOpt, target);
    }

    public static CustomRpcCaller Create(
        uint cmd, SendOption sendOpt = SendOption.Reliable, int target = -1)
    {
        if (!CustomRpcCatcher.RegistedCatcher.TryGetModId(
                Assembly.GetCallingAssembly().FullName, out uint id))
        {
            throw new KeyNotFoundException("CanT find this mod catcher");
        }
        return new CustomRpcCaller(id, CachedPlayerControl.LocalPlayer.PlayerControl.NetId,
            cmd, sendOpt, target);
    }

    public static CustomRpcCaller Create(
        DropshipRpc cmd, SendOption sendOpt = SendOption.Reliable, int target = -1)
    {
        return new CustomRpcCaller(
            0, CachedPlayerControl.LocalPlayer.PlayerControl.NetId,
            (uint)cmd, sendOpt, target);
    }
}
