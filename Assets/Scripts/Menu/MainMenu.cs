using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;

    public void HostLobby()
    {
        mainMenuPanel.SetActive(false);

        NetworkManager.singleton.StartHost();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
