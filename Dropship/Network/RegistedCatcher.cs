using System.Collections.Generic;

namespace Dropship.Network;

internal sealed class CustomRpcCatcherContainer
{
    private Dictionary<string, uint> registerModId = new Dictionary<string, uint>();
    private Dictionary<uint, CustomRpcCatcher> registerCatcher = new Dictionary<
        uint, CustomRpcCatcher>();

    private uint modId = 0;

    public CustomRpcCatcherContainer()
    {
        this.registerModId.Clear();
        this.registerCatcher.Clear();
    }

    internal bool TryGetModId(string name, out uint id) =>
        this.registerModId.TryGetValue(name, out id);

    internal bool TryGetCatcher(uint id, out CustomRpcCatcher catcher) =>
        this.registerCatcher.TryGetValue(id, out catcher);

    internal void Add(CustomRpcCatcher catcher, string name)
    {
        uint id = this.modId;
        this.modId++;

        this.registerCatcher.Add(id, catcher);
        this.registerModId.Add(name, id);
    }

    internal bool IsRegisted(string name)
        =>
        this.registerModId.ContainsKey(name) ||
        this.registerCatcher.ContainsKey(this.modId);
}
