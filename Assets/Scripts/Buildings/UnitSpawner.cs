using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private Transform unitSpawnPoint;
    [SerializeField] private TMP_Text remainingUnitsText;
    [SerializeField] private Image fillImage;
    [SerializeField] private float unitSpawnDuration = 5f;
    [SerializeField] private float spawnMoveRange = 7f;

    private HealthSystem healthSystem;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }


    #region Server

    public override void OnStartServer()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.ServerOnDie += HandleServerOnDie;
    }
    public override void OnStopServer()
    {
        healthSystem.ServerOnDie -= HandleServerOnDie;
    }
    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) { return; }

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration) { return; }

        GameObject unitInstance = Instantiate(
            unitPrefab.gameObject,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;
    }
    [Server]
    private void HandleServerOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        //Check if the queue is already full
        if (queuedUnits == maxUnitQueue)
            return;

        MyPlayer player = connectionToClient.identity.GetComponent<MyPlayer>();

        //Check if we have enough resources.
        if (unitPrefab.GetCost() > player.GetResources()) 
            return;
        //Add to the queue and deduct the unit cost.
        queuedUnits++;
        
        player.SetResources(player.GetResources() - unitPrefab.GetCost());

    }
    #endregion

    #region Client
    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < fillImage.fillAmount)
        {
            fillImage.fillAmount = newProgress;
        }
        else
        {
            fillImage.fillAmount = Mathf.SmoothDamp(
                fillImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
            );
        }
    }
    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Check if we clink the left mouse button and if we have authority, then spawn a unit.
        if (eventData.button != PointerEventData.InputButton.Left)  return; 
        if (!hasAuthority)  return;
        CmdSpawnUnit();
    }

    #endregion

}