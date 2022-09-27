using Unity.Collections;
using Unity.Netcode;

namespace Network
{
    public class ClientPlayer : NetworkBehaviour
    {
        public NetworkVariable<FixedString64Bytes> Name { get; } = new("Name not found");
        public NetworkVariable<ulong> CSteamId { get; } = new();
    }
}
