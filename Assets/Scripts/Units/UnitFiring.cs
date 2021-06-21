using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    private Targeter targeter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float rotationSpeed = 20f;
    private float lastFireTime;

    private void Start()
    {
        targeter = GetComponent<Targeter>();
    }
    #region Server
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.Target;

        //Check if the attack conditions are met.
        if (target == null)
            return;

        if (!CanFireAtTarget())
            return;
        // Calculate the target angle to look at.
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        // Rotate towards the target angle.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Fire the projectile.
        if (lastFireTime + (1 / fireRate) < Time.time)
        {
            //Rotate the projectile towards the target point.
            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            //Spawn it on the network.
            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return Vector3.Distance(transform.position, targeter.Target.transform.position) <= fireRange;
    }
    #endregion
}