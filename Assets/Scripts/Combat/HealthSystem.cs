using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    [SerializeField] private int maxHealth = 100;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthChanged;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDied += HandleServerOnPlayerDied;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDied -= HandleServerOnPlayerDied;
    }

    private void HandleServerOnPlayerDied(int connectionId)
    {
        //Check if the player who died is also the same player who owns this unit,if not then return but if it is the same player, then destroy all the units it has.
        if (connectionToClient.connectionId != connectionId)
            return;

        //Check if this is already the unitbase, if so then return because unitbase is already destroyed at this point.
        bool isUnitBase = gameObject.TryGetComponent(out UnitBase unitbase);
        if (!isUnitBase)
            DealDamage(currentHealth);
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth != 0) 
            return;

        ServerOnDie?.Invoke();

    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthChanged?.Invoke(newHealth, maxHealth);
    }

    #endregion

}
