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
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        activeBases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        activeBases.Remove(unitBase);

        //Check if there is only 1 base left.
        if (activeBases.Count != 1) 
            return;

        int playerId = activeBases[0].connectionToClient.connectionId;

        RpcGameOver(playerId.ToString());

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
