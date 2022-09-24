using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> Name { get; } = new("Name not found");
    public NetworkVariable<ulong> CSteamId { get; } = new();
    public int LobbyId { get; set; }
}
