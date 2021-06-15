﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UnitSelectionHandler : MonoBehaviour
{
    private List<Unit> selectedUnits = new List<Unit>();
    [SerializeField] private LayerMask layerMask;
    private Camera mainCamera;    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        SelectDeselectUnits();
    }
    private void SelectDeselectUnits()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }
    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))  return; 
        if (!hit.collider.TryGetComponent<Unit>(out Unit unit))  return; 
        if (!unit.hasAuthority)  return; 
        selectedUnits.Add(unit);
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}