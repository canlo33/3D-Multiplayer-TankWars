using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    public UnitMovement UnitMovement { get; private set; }
    public Targeter Targeter { get; private set; }
    private HealthSystem healthSystem;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        UnitMovement = GetComponent<UnitMovement>();
        Targeter = GetComponent<Targeter>();
    }

    #region Server

    public override void OnStartServer()
    {
        healthSystem.ServerOnDie += HandleServerOnDie;
        ServerOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        healthSystem.ServerOnDie -= HandleServerOnDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void HandleServerOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        //Check if we are client only and hasAuthority, then trigger the event.
        if (!isClientOnly || !hasAuthority) return;
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    #endregion

}
