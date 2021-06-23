using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];
    public List<Unit> MyUnits { get; private set; } = new List<Unit>();
    public List<Building> MyBuildings { get; private set; } = new List<Building>();

    public event Action<int> ClientOnResourcesUpdated;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    public int GetResources()
    {
        return resources;
    }
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned += HandleServerUnitDespawned;
    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned -= HandleServerUnitDespawned;
    }
    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void HandleServerUnitSpawned(Unit unit)
    {
        // Check if this unit belongs to the client that is trying access it, if so then add it to the unit list.
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)  return; 
        MyUnits.Add(unit);
    }
    private void HandleServerUnitDespawned(Unit unit)
    {
        // Check if this unit belongs to the client that is trying access it, if so then remove it from the unit list.
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)  return; 
        MyUnits.Add(unit);
    }
    private void HandleServerOnBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)  
            return; 

        MyBuildings.Add(building);

    }
    private void HandleServerOnBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return; 

        MyBuildings.Remove(building);

    }
    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        Building buildingToPlace = null;
        //Get the building with the matching Id from the building array.
        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) 
            return;
        //Instatiate it and spawn it on the network.
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += HandleAuthorityOnUnitSpawned;
        Unit.AuthorityOnUnitDespawned += HandleAuthorityOnUnitDespawned;
        Building.ServerOnBuildingSpawned += HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned += HandleServerOnBuildingDespawned;
        Building.AuthorityOnBuildingSpawned += HandleAuthorityOnBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += HandleAuthorityOnBuildingDespawned;
    }
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= HandleAuthorityOnUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= HandleAuthorityOnUnitDespawned;
        Building.ServerOnBuildingSpawned -= HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned -= HandleServerOnBuildingDespawned;
    }
    private void HandleAuthorityOnUnitSpawned(Unit unit)
    {
        MyUnits.Add(unit);
    }
    private void HandleAuthorityOnUnitDespawned(Unit unit)
    {
        MyUnits.Add(unit);
    }
    private void HandleAuthorityOnBuildingSpawned(Building building)
    {
        MyBuildings.Add(building);
    }
    private void HandleAuthorityOnBuildingDespawned(Building building)
    {
        MyBuildings.Remove(building);
    }

    #endregion
}
