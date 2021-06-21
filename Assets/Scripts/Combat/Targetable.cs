using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimPoint;

    public Transform GetAimAtPoint()
    {
        return aimPoint;
    }
}
