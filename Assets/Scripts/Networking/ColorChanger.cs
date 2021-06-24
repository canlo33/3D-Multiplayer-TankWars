using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleClientColorUpdated))]
    private Color playerColor = new Color();

    #region Server

    public override void OnStartServer()
    {
        MyPlayer player = connectionToClient.identity.GetComponent<MyPlayer>();

        playerColor = player.GetColor();
    }

    #endregion

    #region Client

    private void HandleClientColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
            renderer.material.SetColor("_BaseColor", newColor);
    }

    #endregion
}
