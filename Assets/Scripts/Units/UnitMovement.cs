using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    #region Server

    //We using Unity functions with ServerCallback instead of Server as otherwise it will keep throwing errors in the console.
    [ServerCallback]
    private void Update()
    {
        //Check if we near enough to the target position, then stop moving.
        if (!agent.hasPath)
            return;
        if (agent.remainingDistance > agent.stoppingDistance)
            return;
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        // This function will check if the position player wants to move is legit, then commands the server to move the unit.
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))  return;
        
        agent.SetDestination(hit.position);
    }

    #endregion


}
