using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UIElements;

public class EnemyWeaponController : MonoBehaviour
{
    #region Editor Variables

    [SerializeField]
    [Tooltip("Shots per second")]
    private float fireRate;

    [SerializeField]
    private int shotsInBurst = 1;

    [SerializeField]
    private float secondsBetweenShotsInBurst = .1f;

    [SerializeField]
    bool fireInForwardDirection = false;
    [SerializeField]
    private float secondsOfChargeTimeBeforeShot = .5f;

    [SerializeField]
    private ProjectileType projectileThisShoots;

    #endregion

    public bool IsShooting { get; private set; }
    private float currentFireTime;
    private Transform targetTr;
    private EnemyController enemyController;
    private ProjectileManager projectileManager;
    private int shotsInBustSoFar = 0;
    private EnemySFX sfxController;

    #region Set Up

    public void SetUpWeapon(EnemyController enemyC, ProjectileManager projManager, EnemySFX sfx, Transform target)
    {
        enemyController = enemyC;
        sfxController = sfx;
        IsShooting = false;
        targetTr = target;
        projectileManager = projManager;
    }

    #endregion

    private void Update()
    {
        if (IsShooting || !targetTr || enemyController.IsDead() || projectileManager.GetGameState() != GameState.Playing)
            return;

        currentFireTime += Time.deltaTime;
        float fireInterval = 1f / fireRate;

        if (currentFireTime >= fireInterval)
        {
            if (targetTr)
            {
                Vector2 dir = (targetTr.position - transform.position).normalized;
                StartChargingShot(dir);
                currentFireTime = 0;
            }
            else
            {
                Debug.LogWarning("Target is null");
            }
        }
    }

    private void StartChargingShot(Vector2 dir)
    {
        shotsInBustSoFar = 0;
        IsShooting = true;
        Sequence firingSequence = DOTween.Sequence();
        firingSequence.Append(transform.DOScale(Vector3.one * 1.5f, secondsOfChargeTimeBeforeShot).OnComplete(() =>
        {
            if (fireInForwardDirection)
            {
                dir = transform.right;
            }
            Shoot(dir);
        }).SetEase(Ease.InSine));
        firingSequence.Append(transform.DOScale(Vector3.one, secondsOfChargeTimeBeforeShot / 2f).SetEase(Ease.OutSine));
    }

    private void Shoot(Vector2 dir)
    {
        if (enemyController.IsDead()) return;

        shotsInBustSoFar += 1;
        ProjectileController proj = projectileManager.GetProjectile(projectileThisShoots);
        if (!proj)
            return;

        proj.ShootProjectile(transform.position, dir, enemyController.GetEnemyType());

        if (shotsInBustSoFar < shotsInBurst && gameObject.activeSelf)
        {
            StartCoroutine(ShootAfterDelay(secondsBetweenShotsInBurst, dir));
        }
        else
        {
            IsShooting = false;
        }
    }

    IEnumerator ShootAfterDelay(float seconds, Vector2 dir)
    {
        yield return new WaitForSeconds(seconds);

        if (fireInForwardDirection)
        {
            dir = transform.right;
        }
        Shoot(dir);
    }
}
