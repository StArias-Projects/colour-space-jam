using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    #region Editor Variables

    [SerializeField]
    [Tooltip("Shots per second")]
    private float fireRate;

    #endregion

    private bool isShooting = false;
    private float currentFireTime;
    private Transform targetTr;
    private EnemyController enemyController;
    private ProjectileManager projectileManager;

    #region Set Up

    public void SetUpWeapon(EnemyController enemyC, ProjectileManager projManager, Transform target)
    {
        enemyController = enemyC;
        isShooting = true;
        targetTr = target;
        projectileManager = projManager;
    }

    #endregion

    private void Update()
    {
        if (!isShooting || !targetTr)
            return;

        currentFireTime += Time.deltaTime;
        float fireInterval = 1f / fireRate;

        if (currentFireTime >= fireInterval)
        {
            if (targetTr)
            {
                Vector2 dir = (targetTr.position - transform.position).normalized;
                Shoot(dir);
                currentFireTime = 0;
            }
            else
            {
                Debug.LogWarning("Target is null");
            }
        }
    }

    private void Shoot(Vector2 dir)
    {
        ProjectileController proj = projectileManager.GetProjectile(enemyController.GetEnemyType());
        if (!proj)
            return;

        proj.ShootProjectile(transform.position, dir);
    }
}
