using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class MyNetworkManager : NetworkManager
{

    [SerializeField] private GameObject unitBasePrefab;
    [SerializeField] private GameOverManager gameOverManagerPrefab;

    private bool isGameInProgress = false;
    public List<MyPlayer> Players { get; private set; } = new List<MyPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) 
            return; 

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MyPlayer player = conn.identity.GetComponent<MyPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count != 2) 
            return; 

        isGameInProgress = true;

        ServerChangeScene("Level01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyPlayer player = conn.identity.GetComponent<MyPlayer>();

        Players.Add(player);

        player.SetPlayerName("Player " + Players.Count);

        //Give the player a random color.
        player.SetColor(new Color( UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));

        //Make the first player the party owner.
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //After the scene is changed, check if the scene is a level if so then instanciate the GameOverManager and Unit Base.
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            GameOverManager gameOverHandlerInstance = Instantiate(gameOverManagerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (MyPlayer player in Players)
            {
                GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);

                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}


