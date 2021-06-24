using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private LayerMask floorMask = new LayerMask();
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    private MyPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;
    private Camera mainCamera;
    private BoxCollider buildingCollider;

    private void Start()
    {
        mainCamera = Camera.main;
        buildingCollider = building.GetComponent<BoxCollider>();
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetCost().ToString();
    }

    private void Update()
    {
        if (player == null)
            player = NetworkClient.connection.identity.GetComponent<MyPlayer>();

        if (player.GetResources() < building.GetCost()) 
            return; 

        if (buildingPreviewInstance == null) 
            return; 

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //If we click on the button and hold, Instatiate the building preview.
        if (eventData.button != PointerEventData.InputButton.Left)  
            return; 

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Once we let go of the left click, try to place the building and destroy the building preview object.
        if (buildingPreviewInstance == null) 
            return; 

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        //Update the position of the building preview on the floor.
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))  
            return; 

        buildingPreviewInstance.transform.position = hit.point;

        if (!buildingPreviewInstance.activeSelf)    
            buildingPreviewInstance.SetActive(true);

        //Change the color of the building preview. If the conditions are valid make it green, else make it red.
        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

        buildingRendererInstance.material.SetColor("_BaseColor", color);


    }
}
