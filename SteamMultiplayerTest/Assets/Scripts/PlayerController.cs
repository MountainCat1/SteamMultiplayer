using System;
using Network;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    private void Start()
    {
        if (!NetworkObject.IsOwner)
        {
            enabled = false;
            return;
        }
    }
}