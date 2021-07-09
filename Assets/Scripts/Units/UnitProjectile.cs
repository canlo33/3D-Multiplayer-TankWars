using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private int projectileDamage = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * launchForce;
    }
    public override void OnStartServer()
    {
        Invoke(nameof(SelfDestruct), destroyAfterSeconds);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        ProcessTrigger(other.gameObject);
    }

    private void ProcessTrigger(GameObject other)
    {
        //Check if we hit our own unit, if so then ignore.
        if (other.TryGetComponent(out NetworkIdentity networkIdentity))
            if (networkIdentity.connectionToClient == connectionToClient)
                return;

        //Deal damage to the enemy unit.
        if (other.TryGetComponent(out HealthSystem healthSystem))
            healthSystem.DealDamage(projectileDamage);

        //Destroy yourself after the collision.
        SelfDestruct();
    }

    [Server]
    private void SelfDestruct()
    {
        NetworkServer.Destroy(gameObject);
    }
}
