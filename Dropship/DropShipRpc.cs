using Hazel;

using UnityEngine;

using Dropship.AmongUs;
using Dropship.Network;


namespace Dropship;

public enum DropshipRpc
{
    CleanDeadBody,
    MurderPlayer,
    SnapTo,
    Revive
}

internal sealed class DropshipRpcCatcher : CustomRpcCatcher
{
    public override void Catch(uint cmd, MessageReader reader)
    {
        switch ((DropshipRpc)cmd)
        {
            case DropshipRpc.CleanDeadBody:
                Map.CleanDeadBody(reader.ReadByte());
                break;
            case DropshipRpc.MurderPlayer:
                byte murderPlayerId = reader.ReadByte();
                byte targetPlayerId = reader.ReadByte();
                byte bytedIsSnap = reader.ReadByte();
                Player.UncheckMurderPlayer(
                    murderPlayerId, targetPlayerId, bytedIsSnap);
                break;
            case DropshipRpc.SnapTo:
                byte snapPlayerId = reader.ReadByte();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                Player.UncheckSnapTo(snapPlayerId, new Vector2(x, y));
                break;
            case DropshipRpc.Revive:
                Player.UncheckRevive(reader.ReadByte());
                break;
        }
    }
}
