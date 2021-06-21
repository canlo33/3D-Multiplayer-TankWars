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
