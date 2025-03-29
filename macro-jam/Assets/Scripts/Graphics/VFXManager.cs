using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    private GamePlayManager gamePlayManager;
    private EnemyManager enemyManager;


    [SerializeField]
    private VisualEffect impactVFXPrefab;

    public void SetUp(GamePlayManager gpManager, EnemyManager enemyManagerRef)
    {
        enemyManager = enemyManagerRef;
        gamePlayManager = gpManager;
        ProjectileController.OnBulletDetonated += OnBulletDetonate;
    }

    public void OnDestroy()
    {
        ProjectileController.OnBulletDetonated -= OnBulletDetonate;
    }


    public void OnBulletDetonate(ProjectileController bullet)
    {
        PlayImpactVFX(bullet.transform.position, enemyManager.GetEnemyColor(bullet.EnemyType));
    }

    public void PlayImpactVFX(Vector3 positionToSpawnAt, Color color)
    {
        VisualEffect spawnedVisualEffect = Instantiate(impactVFXPrefab, transform);
        spawnedVisualEffect.transform.position = positionToSpawnAt;
        spawnedVisualEffect.SetVector4("ImpactColor", color);
        //might be worth pooling this at some point
        Destroy(spawnedVisualEffect, 5.0f);

    }
}
