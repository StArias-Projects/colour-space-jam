using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    private GamePlayManager gamePlayManager;
    private EnemyManager enemyManager;


    [SerializeField]
    private ParticleSystem impactVFXPrefab;

    [SerializeField]
    private ParticleSystem firingVFXPrefab;

    public void SetUp(GamePlayManager gpManager, EnemyManager enemyManagerRef)
    {
        enemyManager = enemyManagerRef;
        gamePlayManager = gpManager;
        ProjectileController.OnBulletDetonated += OnBulletDetonate;
        ProjectileController.OnBulletFired += PlayFiringVFX;
    }

    public void OnDestroy()
    {
        ProjectileController.OnBulletDetonated -= OnBulletDetonate;
        ProjectileController.OnBulletFired -= PlayFiringVFX;
    }


    public void OnBulletDetonate(Vector3 positionOfDetonation, EnemyType enemyType)
    {
        PlayImpactVFX(positionOfDetonation, enemyManager.GetEnemyColor(enemyType));
    }

    public void PlayFiringVFX(Vector3 positionToSpawnAt, Quaternion rot, EnemyType enemyType)
    {
        ParticleSystem spawnedVisualEffect = Instantiate(firingVFXPrefab, positionToSpawnAt, rot,transform);
        var main = spawnedVisualEffect.main;
        main.startColor = enemyManager.GetEnemyColor(enemyType);
        //might be worth pooling this at some point
        spawnedVisualEffect.Play();
        Destroy(spawnedVisualEffect, 5.0f);

    }

    public void PlayImpactVFX(Vector3 positionToSpawnAt, Color color)
    {
        ParticleSystem spawnedVisualEffect = Instantiate(impactVFXPrefab, transform);
        spawnedVisualEffect.transform.position = positionToSpawnAt;
        var main = spawnedVisualEffect.main;
        main.startColor = color;
        //might be worth pooling this at some point
        spawnedVisualEffect.Play();
        Destroy(spawnedVisualEffect, 5.0f);

    }
}
