using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        GameOverManager.ClientOnGameOver += HandleClientOnGameOver;
    }
    private void OnDestroy()
    {
        GameOverManager.ClientOnGameOver -= HandleClientOnGameOver;
    }
    private void HandleClientOnGameOver(string winnerPlayerName)
    {
        enabled = false;
    }

    private void Update()
    {
        // Check if the player is clicking on a legit place 
        if (!Mouse.current.rightButton.wasPressedThisFrame) 
            return;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) 
            return;
        //Check if player clicked to a targetable object, if so then set it as target.
        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }
        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        //Move each unit in the selected unit list to the mouse position.
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.UnitMovement.CmdMove(point);
        }
    }
    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.Targeter.CmdSetTarget(target.gameObject);
        }
    }

}
