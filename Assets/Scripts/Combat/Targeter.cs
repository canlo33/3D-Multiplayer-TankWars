using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class Targeter : NetworkBehaviour
{
    public Targetable Target { get; private set; }

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        // Check if the object is targetable, then set it as target.
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget))
            return; 
        Target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        Target = null;
    }

    #endregion

    #region Client

    #endregion
}
