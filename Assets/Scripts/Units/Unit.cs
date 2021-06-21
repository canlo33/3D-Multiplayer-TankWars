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
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    public static event Action<Unit> ServerOnUnitySpawned;
    public static event Action<Unit> ServerOnUnityDespawned;

    public static event Action<Unit> AuthorityOnUnitySpawned;
    public static event Action<Unit> AuthorityOnUnityDespawned;

    private void Start()
    {
        UnitMovement = GetComponent<UnitMovement>();
        Targeter = GetComponent<Targeter>();
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitySpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        ServerOnUnityDespawned?.Invoke(this);
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        //Check if we are client only and hasAuthority, then trigger the event. As we dont want the server to duplicate the list because server will be both a client and server so it will have
        // two lists if we dont check it.
        if (!isClientOnly || !hasAuthority) return;
        AuthorityOnUnitySpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        //Check if we are client only and hasAuthority, then trigger the event.
        if (!isClientOnly || !hasAuthority) return;
        AuthorityOnUnityDespawned?.Invoke(this);
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
