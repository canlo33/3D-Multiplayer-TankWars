using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent;
    [SerializeField] private TMP_Text winnerNameText;

    private void Start()
    {
        GameOverManager.ClientOnGameOver += ClientHandleOnGameOver;
    }

    private void OnDestroy()
    {
        GameOverManager.ClientOnGameOver -= ClientHandleOnGameOver;
    }

    public void LeaveGame()
    {
        //Check if the player is also the server too, if so then stop hosting, if not then stop client.
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();

        else
            NetworkManager.singleton.StopClient();

    }

    private void ClientHandleOnGameOver(string winner)
    {
        winnerNameText.text = winner + " has won!";

        gameOverDisplayParent.SetActive(true);
    }
}
