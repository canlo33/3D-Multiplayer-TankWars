using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{

    [SerializeField] private GameObject unitSpawnerPrefab;
    [SerializeField] private GameOverManager gameOverManagerPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //Give the player a random color.
        MyPlayer player = conn.identity.GetComponent<MyPlayer>();

        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        player.SetColor(color);

        //As soon as a new player connects to the game, spawn a unit spawner for them and link it to that client only.
        GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //After the scene is changed, check if the scene is a level if so then instanciate the GameOverManager.
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            GameOverManager gameOverManager = Instantiate(gameOverManagerPrefab);

            NetworkServer.Spawn(gameOverManager.gameObject);
        }
    }

}

