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
    }

    private void Update()
    {
        // Check if the player is clicking on a legit place 
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

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
}
