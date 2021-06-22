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
        Unit.ServerOnUnitSpawned += ServerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerUnitDespawned;
    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerUnitDespawned;
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
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityUnitDespawned;
    }
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityUnitDespawned;
    }
    private void AuthorityUnitSpawned(Unit unit)
    {
        MyUnits.Add(unit);
    }
    private void AuthorityUnitDespawned(Unit unit)
    {
        MyUnits.Add(unit);
    }
    #endregion
}
