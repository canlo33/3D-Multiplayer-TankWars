using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onUnitSelected;
    [SerializeField] private UnityEvent onUnitDeselected;

    #region Client

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;

        onUnitSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;

        onUnitDeselected?.Invoke();
    }

    #endregion
}
