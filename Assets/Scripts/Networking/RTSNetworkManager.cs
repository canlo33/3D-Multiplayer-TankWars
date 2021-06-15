using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RTSNetworkManager : NetworkManager
{

    [SerializeField] private GameObject unitSpawnerPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        //As soon as a new player connects to the game, spawn a unit spawner for them and link it to that client only.
        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }
}

