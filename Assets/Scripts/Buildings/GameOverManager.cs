using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverManager : NetworkBehaviour
{
    private List<UnitBase> activeBases = new List<UnitBase>();

    public static event Action ServerOnGameOver;

    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleOnBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleOnBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleOnBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleOnBaseDespawned;
    }

    [Server]
    private void ServerHandleOnBaseSpawned(UnitBase unitBase)
    {
        activeBases.Add(unitBase);
    }

    [Server]
    private void ServerHandleOnBaseDespawned(UnitBase unitBase)
    {
        Debug.Log("Here is fine so far");
        activeBases.Remove(unitBase);

        //Check if there is only 1 base left.
        if (activeBases.Count != 1) 
            return;

        int playerId = activeBases[0].connectionToClient.connectionId;

        RpcGameOver("Player " + (playerId + 1).ToString());

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
