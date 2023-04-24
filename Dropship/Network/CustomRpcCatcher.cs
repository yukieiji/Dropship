using System.Reflection;

using Hazel;

namespace Dropship.Network;

public abstract class CustomRpcCatcher
{
    public const byte Id = byte.MaxValue;

    internal static CustomRpcCatcherContainer RegistedCatcher => catcher;
    private static CustomRpcCatcherContainer catcher = new CustomRpcCatcherContainer();

    public abstract void Catch(uint cmd, MessageReader reader);

    public static void Register(CustomRpcCatcher newCatcher)
    {
        string name = Assembly.GetCallingAssembly().FullName;

        if (catcher.IsRegisted(name))
        {
            Logger<DropshipPlugin>.Error("This Mod RPCCatcher Already registed!!");
            return;
        }
        catcher.Add(newCatcher, name);
    }

    internal static void Catch(MessageReader reader)
    {
        uint modId = reader.ReadPackedUInt32();
        uint cmd = reader.ReadPackedUInt32();

        if (!catcher.TryGetCatcher(modId, out CustomRpcCatcher rpcCathcer))
        {
            Logger<DropshipPlugin>.Error($"---- Custom Rpc CallMissing ----");
            return;
        }

        Logger<DropshipPlugin>.Debug($"---- Catch Rpc   ModId:{modId} Cmd:{cmd} ----");
        rpcCathcer.Catch(cmd, reader);
    }
}
