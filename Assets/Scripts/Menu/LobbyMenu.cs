using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Button startGameButton;

    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
    private void Start()
    {
        MyNetworkManager.ClientOnConnected += HandleClientOnConnected;
        MyPlayer.AuthorityOnPartyOwnerUpdated += AuthorityHandleOnPartyOwnerUpdated;
        MyPlayer.ClientOnNameUpdated += ClientHandleOnNameUpdated;
    }

    private void OnDestroy()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientOnConnected;
        MyPlayer.AuthorityOnPartyOwnerUpdated -= AuthorityHandleOnPartyOwnerUpdated;
        MyPlayer.ClientOnNameUpdated -= ClientHandleOnNameUpdated;
    }

    private void HandleClientOnConnected()
    {
        lobbyPanel.SetActive(true);
    }

    private void AuthorityHandleOnPartyOwnerUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }
    private void ClientHandleOnNameUpdated()
    {

        List<MyPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
            playerNameTexts[i].text = players[i].GetPlayerName();

        for (int i = players.Count; i < playerNameTexts.Length; i++)
            playerNameTexts[i].text = "Waiting For Player...";

        startGameButton.interactable = players.Count == 2;
        startGameButton.GetComponent<Animator>().enabled = players.Count == 2;
    }


    public void LeaveLobby()
    {
        //If we are the host, then stop hosting
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();

        //If we are the client, just reload the menu scene.
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<MyPlayer>().CmdStartGame();
    }

}
