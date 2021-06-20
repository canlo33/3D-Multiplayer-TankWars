using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform unitSpawnPoint;
    [SerializeField] private LayerMask layerMask;

    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        // This command will Spawn a unit at the spawnpoint and tie it to the client.
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        //Spawn the unit at the SpawnPoint and give authory of the unit to the related Client via connectionToClient.
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        //Check if we clink the left mouse button and if we have authority, then spawn a unit.
        if (eventData.button != PointerEventData.InputButton.Left)  return; 
        if (!hasAuthority)  return;
        CmdSpawnUnit();
    }

    #endregion

    private void Update()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        //    Debug.Log(hit.collider.name);
    }
}