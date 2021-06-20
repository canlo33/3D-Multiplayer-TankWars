using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    public List<Unit> MyUnits { get; private set; } = new List<Unit>();

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitySpawned += ServerUnitSpawned;
        Unit.ServerOnUnityDespawned += ServerUnitDespawned;
    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitySpawned -= ServerUnitSpawned;
        Unit.ServerOnUnityDespawned -= ServerUnitDespawned;
    }
    private void ServerUnitSpawned(Unit unit)
    {
        // Check if this unit belongs to the client that is trying access it, if so then add it to the unit list.
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)  return; 
        MyUnits.Add(unit);
    }
    private void ServerUnitDespawned(Unit unit)
    {
        // Check if this unit belongs to the client that is trying access it, if so then remove it from the unit list.
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)  return; 
        MyUnits.Add(unit);
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (!isClientOnly) return;
        Unit.AuthorityOnUnitySpawned += AuthorityUnitSpawned;
        Unit.AuthorityOnUnityDespawned += AuthorityUnitDespawned;
    }
    public override void OnStopClient()
    {
        if (!isClientOnly) return;
        Unit.AuthorityOnUnitySpawned -= AuthorityUnitSpawned;
        Unit.AuthorityOnUnityDespawned -= AuthorityUnitDespawned;
    }
    private void AuthorityUnitSpawned(Unit unit)
    {
        if (!hasAuthority) return;
        MyUnits.Add(unit);
    }
    private void AuthorityUnitDespawned(Unit unit)
    {
        if (!hasAuthority) return;
        MyUnits.Add(unit);
    }
    #endregion
}
