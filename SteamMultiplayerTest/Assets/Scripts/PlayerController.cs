using System;
using Network;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 1f;

    private Transform _transform;
    
    #region Unity Methods

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        if (!NetworkObject.IsOwner)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    #endregion

    #region Private Methods

    private void HandleMovement()
    {
        var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        
        var newPosition = _transform.position;
        
        newPosition.x += movement.x * speed * Time.deltaTime;

        _transform.position = newPosition;
    }

    #endregion
}