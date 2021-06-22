using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackRange;
    private Targeter targeter;

    private void Start()
    {
        targeter = GetComponent<Targeter>();
    }
    #region Server

    public override void OnStartServer()
    {
        GameOverManager.ServerOnGameOver += HandleServerOnGameOver;
    }

    public override void OnStopServer()
    {
        GameOverManager.ServerOnGameOver -= HandleServerOnGameOver;
    }

    [Server]
    private void HandleServerOnGameOver()
    {
        //When the game is over, stop all the units.
        agent.ResetPath();
    }

        //We using Unity functions with ServerCallback instead of Server as otherwise it will keep throwing errors in the console.
        [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.Target;
        if (target != null)
        {
            //Check if we have a target and if it is in attack range, if so then stop, if not then chase the target until it is in attack range;
            if(Vector3.Distance(transform.position, target.transform.position) > attackRange)
                agent.SetDestination(target.transform.position);

            else if (agent.hasPath)
                agent.ResetPath();
            return;
        }            
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
        // This function will check if the position player wants to move towards is legit, then commands the server to move the unit.
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))  return;
        
        agent.SetDestination(hit.position);
    }

    #endregion


}
