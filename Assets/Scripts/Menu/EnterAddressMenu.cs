using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterAddressMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField addressInput;


    private void OnEnable()
    {
        MyNetworkManager.ClientOnConnected += HandleClientOnConnected;
        MyNetworkManager.ClientOnDisconnected += HandleClientOnDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientOnConnected;
        MyNetworkManager.ClientOnDisconnected -= HandleClientOnDisconnected;
    }

    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientOnConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        menuPanel.SetActive(false);
    }

    private void HandleClientOnDisconnected()
    {
        joinButton.interactable = true;
    }
}
