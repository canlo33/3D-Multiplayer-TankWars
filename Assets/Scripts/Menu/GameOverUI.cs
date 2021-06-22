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
        GameOverManager.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverManager.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        //Check if the player is also the server too, if so then stop hosting, if not then stop client.
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = winner + " has won!";

        gameOverDisplayParent.SetActive(true);
    }
}
