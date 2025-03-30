using System;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rigidBody;

    

    
    

    [SerializeField]
    protected float maxLifeTime;

    [SerializeField]
    protected float attackPower;

    [SerializeField]
    protected ProjectileType projectileType;

    private float currentLifeTime = 0;
    protected ProjectileManager projectileManager;
    protected EnemyManager enemyManager;
    protected Vector2 projDir;


    public EnemyType enemyColor { get; private set; }
    
    protected bool isBounced = false;

    public static event Action<float> OnPlayerHit;
    public static event Action<Vector3, EnemyType> OnBulletDetonated;
    public static event Action<EnemyType> OnEnemyKilled;
    public static event Action OnProjectileReflected;
    

 

    public void SetUp(ProjectileManager manager, EnemyManager enemyManagerRef)
    {    
        enemyManager = enemyManagerRef;
        projectileManager = manager;
    }

    public void OnReset()
    {
        isBounced = false;
    }

    private void OnEnable()
    {
        if (!projectileManager)
            return;

        ProjectileManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        if (!projectileManager)
            return;

        ProjectileManager.OnGameOver -= OnGameOver;
    }

   

    void Update()
    {
        if (!projectileManager)
            return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
            projectileManager.ResetProjectile(this, projectileType);
    }

   

    

    public virtual void ShootProjectile(Vector2 pos, Vector2 dir, EnemyType type)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        projDir = dir;
        enemyColor = type;
        
    }

    private void OnGameOver()
    {
        if (!projectileManager)
            return;

        projectileManager.ResetProjectile(this, projectileType);
    }


    protected void TriggerOnPlayerHit(float amount)
    {
        OnPlayerHit?.Invoke(amount);
    }
    protected void TriggerOnBulletDetonated(Vector3 explosionPos)
    {
        OnBulletDetonated?.Invoke(explosionPos, enemyColor);
    }
 
    protected void TriggerOnEnemyKilled(EnemyType enemyType)
    {
        OnEnemyKilled?.Invoke(enemyType);
    }
    protected void TriggerOnProjectileReflected()
    {
        OnProjectileReflected?.Invoke();
    }
}
