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

    [SerializeField] private float buildingRangeLimit = 20f;

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action<bool> AuthorityOnPartyOwnerUpdated;

    public static event Action ClientOnNameUpdated;

    private Color playerColor;

    [SyncVar(hook = nameof(HandleAuthorityOnPartyOwnerUpdated))]
    private bool isPartyOwner = false;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    [SyncVar(hook = nameof(ClientHandleOnNameUpdated))]
    private string playerName;

    public int GetResources()
    {
        return resources;
    }
    public Color GetColor()
    {
        return playerColor;
    }
    public string GetPlayerName()
    {
        return playerName;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        //Check if there is any obstacle on where we want to place.
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            return false;

        //Check if we are close enough to our buildings.
        foreach (Building building in MyBuildings)
            if (Vector3.Distance(point, building.transform.position) <= buildingRangeLimit)
                return true;


        return false;
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned += HandleServerUnitDespawned;
        Building.ServerOnBuildingSpawned += HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned += HandleServerOnBuildingDespawned;

        DontDestroyOnLoad(gameObject);

    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= HandleServerUnitSpawned;
        Unit.ServerOnUnitDespawned -= HandleServerUnitDespawned;
        Building.ServerOnBuildingSpawned -= HandleServerOnBuildingSpawned;
        Building.ServerOnBuildingDespawned -= HandleServerOnBuildingDespawned;

    }
    [Command]
    public void CmdStartGame()
    {
        // If we are the host(partyowner), start the game.
        if (!isPartyOwner)
            return; 

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
    }
    [Server]
    public void SetPlayerName(string newName)
    {
        playerName = newName;
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

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
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
    public override void OnStartClient()
    {
        if (NetworkServer.active) 
            return;

        DontDestroyOnLoad(gameObject);

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly)  
                return; 
        // If client disconnects, remove it from the players list.
        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) 
            return; 

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
    private void HandleAuthorityOnPartyOwnerUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)  
            return; 

        AuthorityOnPartyOwnerUpdated?.Invoke(newState);
    }
    private void ClientHandleOnNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnNameUpdated?.Invoke();
    }


    #endregion
}
