using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private int resourceGenerationRate = 10;
    [SerializeField] private float timerMax = 2f;

    private float timer;
    private MyPlayer player;

    public override void OnStartServer()
    {
        timer = timerMax;
        player = connectionToClient.identity.GetComponent<MyPlayer>();

        healthSystem.ServerOnDie += ServerHandleDie;
        GameOverManager.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        healthSystem.ServerOnDie -= ServerHandleDie;
        GameOverManager.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            player.SetResources(player.GetResources() + resourceGenerationRate);

            timer += timerMax;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}
