using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileManager : MonoBehaviour
{
    [Serializable]
    public class ProjectileAttributes
    {
        public ProjectileController projectilePrefab;
        public int maxSizePool;
        public Transform projectilePool;
        public EnemyType enemyType;

        [HideInInspector]
        public List<ProjectileController> inactiveProjectiles = new List<ProjectileController>();
    }

    [SerializeField]
    private List<ProjectileAttributes> projectileList;

    private Dictionary<EnemyType, ProjectileAttributes> projectileDict = new Dictionary<EnemyType, ProjectileAttributes>();

    public static event Action OnGameOver;

    #region Set Up

    public void SetUp()
    {
        projectileDict = new Dictionary<EnemyType, ProjectileAttributes>();

        foreach (var projectile in projectileList)
        {
            projectileDict[projectile.enemyType] = projectile;
        }

        GenerateProjectiles();
    }

    public void GenerateProjectiles()
    {
        foreach (KeyValuePair<EnemyType, ProjectileAttributes> projectileElement in projectileDict)
        {
            EnemyType enemyType = projectileElement.Key;
            ProjectileAttributes projectileAttr = projectileElement.Value;
            for (int i = 0; i < projectileAttr.maxSizePool; i++)
            {
                Vector2 positionToSpawn = Vector2.zero;

                ProjectileController projectile = Instantiate(projectileAttr.projectilePrefab, projectileAttr.projectilePool.position, Quaternion.identity, projectileAttr.projectilePool);
                projectile.gameObject.SetActive(false);
                projectileAttr.inactiveProjectiles.Add(projectile);
                projectile.SetUp(this, enemyType);
            }
        }
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        GamePlayManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= GameOver;
    }

    #endregion

    public ProjectileController GetProjectile(EnemyType enemyType)
    {
        if (!projectileDict.TryGetValue(enemyType, out ProjectileAttributes projectileAttr)
            || projectileAttr.inactiveProjectiles.Count == 0)
            return null;

        ProjectileController proj = projectileAttr.inactiveProjectiles[0];
        projectileAttr.inactiveProjectiles.Remove(proj);

        return proj;
    }

    public void ResetProjectile(ProjectileController projectile, EnemyType enemyType)
    {
        if (!projectileDict.TryGetValue(enemyType, out ProjectileAttributes projectileAttr))
            return;

        projectileAttr.inactiveProjectiles.Add(projectile);
        projectile.gameObject.SetActive(false);
        projectile.transform.position = projectileAttr.projectilePool.position;
    }


    public void GameOver()
    {
        OnGameOver?.Invoke();

        foreach (KeyValuePair<EnemyType, ProjectileAttributes> projectileElement in projectileDict)
        {
            EnemyType enemyType = projectileElement.Key;
            ProjectileAttributes projectileAttr = projectileElement.Value;
            foreach (ProjectileController projectile in projectileAttr.inactiveProjectiles)
            {
                if (!projectile)
                    continue;

                Destroy(projectile.gameObject);
            }

            projectileAttr.inactiveProjectiles.Clear();
        }
    }
}
