using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public enum EnemyType 
{
    Blue,
    Red,
    Green,
    Orange
}



public class EnemyManager : MonoBehaviour
{

    [Serializable]
    public class EnemyPool
    {
        /// <summary>
        /// Reference to the prefab of the enemy
        /// </summary>
        public EnemyController enemyController;
        public int maxSizePool;

        [HideInInspector]
        public List<EnemyController> inactiveEnemies = new List<EnemyController>();
    }

    #region Editor Variables


    

    [SerializeField]
    [Tooltip("Enemies per second")]
    private float generationRate;

    [SerializeField]
    private Transform enemyContainer;

    [SerializeField]
    private List<EnemyPool> enemyPoolList;

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<Collider2D> directionPoints;

    #endregion

    private GamePlayManager gamePlayManager;
    private ProjectileManager projectileManager;
    private Transform targetTr;
    private bool isGenerating = false;
    private float currentTime = 0;

    public static event Action OnGameOver;


    [SerializeField]
    private Color enemyBlueColor;
    [SerializeField]
    private Color enemyRedColor;
    [SerializeField]
    private Color enemyGreenColor;
    [SerializeField]
    private Color enemyOrangeColor;



    public void SetUp(GamePlayManager gpManager, ProjectileManager projManager, Transform target)
    {
        gamePlayManager = gpManager;
        projectileManager = projManager;
        targetTr = target;

        GenerateEnemies();
        
        //just for fast testing
        SpawnEnemy();
    }

    public void StartGame()
    {
        isGenerating = true;
    }

    public void GenerateEnemies()
    {
        int poolGroup = 0;

        foreach (EnemyPool pool in enemyPoolList)
        {
            for (int i = 0; i < pool.maxSizePool; i++)
            {
                EnemyController enemy = Instantiate(pool.enemyController, enemyContainer.position, Quaternion.identity, enemyContainer);
                enemy.gameObject.SetActive(false);
                pool.inactiveEnemies.Add(enemy);
                enemy.SetUpEnemy(this, projectileManager, directionPoints, poolGroup, targetTr);
            }

            poolGroup++;
        }
    }

    private void OnEnable()
    {
        GamePlayManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= GameOver;
    }

    private void Update()
    {
        if (!isGenerating)
            return;

        currentTime += Time.deltaTime;
        float generationInterval = 1f / generationRate;

        if (currentTime >= generationInterval)
        {
            SpawnEnemy();
            currentTime = 0;
        }
    }

    private void SpawnEnemy()
    {
        int rndEnemy = UnityEngine.Random.Range(0, enemyPoolList.Count);
        int rndSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Count);

        if (enemyPoolList.Count == 0
            || spawnPoints.Count == 0
            || enemyPoolList[rndEnemy].inactiveEnemies.Count == 0)
            return;

        EnemyController enemy = enemyPoolList[rndEnemy].inactiveEnemies[0];

        Vector2 newPos = spawnPoints[rndSpawnPoint].position;

        enemy.StartEnemy(newPos);
        enemyPoolList[rndEnemy].inactiveEnemies.Remove(enemy);
    }

    public void ResetEnemy(EnemyController enemy)
    {
        enemy.transform.position = enemyContainer.position;
        enemy.gameObject.SetActive(false);
        int group = enemy.GetGroupIndex();
        enemyPoolList[group].inactiveEnemies.Add(enemy);
    }

    public void GameOver()
    {
        isGenerating = false;
        OnGameOver?.Invoke();
    }

    public GamePlayManager.GameState GetGameState() 
    {
        return gamePlayManager.GetGameState();
    }

    public Color GetEnemyColor(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                return enemyBlueColor;
            case EnemyType.Red:
                return enemyRedColor;
            case EnemyType.Green:
                return enemyGreenColor;
            case EnemyType.Orange:
                return enemyOrangeColor;
        }
        return Color.black;
    }
}
