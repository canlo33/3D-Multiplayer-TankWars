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

    public static event Action<UnitBase> ClientOnBaseSpawned;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }
    #region Server

    public override void OnStartServer()
    {
        healthSystem.ServerOnDie += ServerHandleOnDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        healthSystem.ServerOnDie -= ServerHandleOnDie;
    }
    public override void OnStartAuthority()
    {
        ClientOnBaseSpawned?.Invoke(this);
    }
    [Server]
    private void ServerHandleOnDie()
    {
        ServerOnPlayerDied?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion

}
