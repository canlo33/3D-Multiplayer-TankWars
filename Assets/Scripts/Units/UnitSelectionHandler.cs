using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UnitSelectionHandler : MonoBehaviour
{
    public List<Unit> SelectedUnits { get; private set; } = new List<Unit>();
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RectTransform selectionBox;
    private Vector2 startDraggingPosition;
    private MyPlayer player;
    private Camera mainCamera;    
    private void Start()
    {
        mainCamera = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<MyPlayer>();
    }
    private void Update()
    {
        if(player == null)
            player = NetworkClient.connection.identity.GetComponent<MyPlayer>();
        SelectDeselectUnits();
    }
    private void SelectDeselectUnits()
    {
        // If mouse is left clicked, deselect all the unit and empty the selected unit list.
        // Then save the mouse click position in case the player start dragging while the mouse button is being held.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (Unit selectedUnit in SelectedUnits)
                {
                    selectedUnit.Deselect();
                }
                SelectedUnits.Clear();
            }
            selectionBox.gameObject.SetActive(true);
            startDraggingPosition = Mouse.current.position.ReadValue();
            UpdateSelectionBox();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
            UpdateSelectionBox();
    }
    private void UpdateSelectionBox()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        // Calculate the size of the selection box. 
        float selectionBoxWidth = mousePosition.x - startDraggingPosition.x;
        float selectionBoxHeight = mousePosition.y - startDraggingPosition.y;
        //Set the size of the selection box and its position. Make sure the position starts from the mouse position and the values are absolute, not negative
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth), Mathf.Abs(selectionBoxHeight));
        selectionBox.anchoredPosition = startDraggingPosition + new Vector2(selectionBoxWidth / 2, selectionBoxHeight / 2);
    }

    private void ClearSelectionArea()
    {
        selectionBox.gameObject.SetActive(false);

        //Check if we selected a single unit.
        if(selectionBox.sizeDelta.magnitude == 0)
        {
            //Check if we are actually clicked on a unit and we have authority on that unit or not. If so, unselect the all other units in the selected units list.
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.hasAuthority) return;
            SelectedUnits.Add(unit);
            foreach (Unit selectedUnit in SelectedUnits)
                selectedUnit.Select();
            return;
        }

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in player.MyUnits)
        {
            //Check if the unit is alreadt in the list, if so then move to the next.
            if (SelectedUnits.Contains(unit))
                continue;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
            //Check if the unit is withing the selectionbox or not.
            if (screenPosition.x > min.x && screenPosition.x < max.x)
                if (screenPosition.y > min.y && screenPosition.y < max.y)
                    {
                        SelectedUnits.Add(unit);
                        unit.Select();
                    }
        }
    }
}