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

    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();

    [SerializeField] private float buildingRangeLimit = 5f;

    public event Action<int> ClientOnResourcesUpdated;

    private Color playerColor;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    public int GetResources()
    {
        return resources;
    }
    public Color GetColor()
    {
        return playerColor;
    }
    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        //Check if there is any obstacle on where we want to place.
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            return false;

        //Check if we are close enough to our buildings.
        foreach (Building building in buildings)
        {
            if (Vector3.Distance(point, building.transform.position) <= buildingRangeLimit)
                return true;
        }
        return false;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned += HandleServerUnitDespawned;
        Building.ServerOnBuildingSpawned += HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned += HandleServerOnBuildingDespawned;

    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned -= HandleServerUnitDespawned;
        Building.ServerOnBuildingSpawned -= HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned -= HandleServerOnBuildingDespawned;

    }
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }
    [Server]
    public void SetColor(Color color)
    {
        playerColor = color;
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
        //Check if the building exists and we have enough resource for it.
        if (buildingToPlace == null) 
            return;
        if (buildingToPlace.GetCost() > resources)
            return;

        //Check if the position we want to place the building is valid or not.
        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, position)) 
            return; 

        //Spawn the unit on the network and deduct the cost from player resources.
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetCost());

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
