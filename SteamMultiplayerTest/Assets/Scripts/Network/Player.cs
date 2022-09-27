using Unity.Collections;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> Name { get; } = new("Name not found");
    public NetworkVariable<ulong> CSteamId { get; } = new();
}
