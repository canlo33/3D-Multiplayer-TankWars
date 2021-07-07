using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText;

    private MyPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<MyPlayer>();

        HandleClientOnResourcesUpdated(player.GetResources());

        player.ClientOnResourcesUpdated += HandleClientOnResourcesUpdated;
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= HandleClientOnResourcesUpdated;
    }

    private void HandleClientOnResourcesUpdated(int resources)
    {
        //When Resources changed, update the UI text.
        resourcesText.text = "Resources: " + resources;
    }
}
