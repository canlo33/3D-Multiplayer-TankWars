using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    private HealthSystem healthSystem;

    public static event Action<int> ServerOnPlayerDied;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }
    #region Server

    public override void OnStartServer()
    {
        healthSystem.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        healthSystem.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDied?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}
