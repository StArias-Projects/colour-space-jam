using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;



public enum ProjectileType
{
    Small,
    Ray,
}


[Serializable]
public class ProjectileAttributes
{
    public ProjectileController projectilePrefab;
    public int maxSizePool;
    public Transform projectilePool;
    public ProjectileType projectileType;

    [HideInInspector]
    public List<ProjectileController> inactiveProjectiles = new();
}

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private List<ProjectileAttributes> projectileList;

    private Dictionary<ProjectileType, ProjectileAttributes> projectileDict = new();
    private EnemyManager enemyManager;
    protected VFXManager vfxManager;
    public static event Action OnGameOver;

    #region Set Up

    public void SetUp(EnemyManager enemyManagerRef, VFXManager vfxManagerRef)
    {
        vfxManager = vfxManagerRef;
        enemyManager = enemyManagerRef;
        projectileDict = new Dictionary<ProjectileType, ProjectileAttributes>();

        foreach (var projectile in projectileList)
        {
            projectileDict[projectile.projectileType] = projectile;
        }

        GenerateProjectiles();
    }

    public void GenerateProjectiles()
    {
        foreach (KeyValuePair<ProjectileType, ProjectileAttributes> projectileElement in projectileDict)
        {
            ProjectileType projectileType = projectileElement.Key;
            ProjectileAttributes projectileAttr = projectileElement.Value;
            for (int i = 0; i < projectileAttr.maxSizePool; i++)
            {
                ProjectileController projectile = Instantiate(projectileAttr.projectilePrefab, projectileAttr.projectilePool.position, Quaternion.identity, projectileAttr.projectilePool);
                projectile.gameObject.SetActive(false);
                projectileAttr.inactiveProjectiles.Add(projectile);
                projectile.SetUp(this, enemyManager, vfxManager);
            }
        }
    }

    #endregion

    public ProjectileController GetProjectile(ProjectileType projectileType)
    {
        if (!projectileDict.TryGetValue(projectileType, out ProjectileAttributes projectileAttr)
            || projectileAttr.inactiveProjectiles.Count == 0)
            return null;

        ProjectileController proj = projectileAttr.inactiveProjectiles[0];
        projectileAttr.inactiveProjectiles.Remove(proj);

        return proj;
    }

    public void ResetProjectile(ProjectileController projectile, ProjectileType projectileType)
    {
        if (!projectileDict.TryGetValue(projectileType, out ProjectileAttributes projectileAttr))
            return;

        projectileAttr.inactiveProjectiles.Add(projectile);
        projectile.OnReset();
        projectile.gameObject.SetActive(false);
        projectile.transform.position = projectileAttr.projectilePool.position;
    }

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

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public GameState GetGameState() 
    {
        return enemyManager.GetGameState();
    }
}
