using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    private Camera mainCamera;

    #region Client

    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        // Check if the player has authority and right clicked on the platform, then move the unit to that position
        if (!hasAuthority)  return; 

        if (!Mouse.current.rightButton.wasPressedThisFrame)  return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))  return; 

        CmdMove(hit.point);
    }

    #endregion

    #region Server

    [Command]
    private void CmdMove(Vector3 position)
    {
        // This function will check if the position player wants to move is legit, then commands the server to move the unit.
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }

    #endregion


}
