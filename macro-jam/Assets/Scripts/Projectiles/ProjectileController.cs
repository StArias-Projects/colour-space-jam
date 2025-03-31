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
    public ProjectileType projectileType;

    private float currentLifeTime = 0;
    private Vector2 velocityBeforPaused = Vector2.zero;

    protected ProjectileManager projectileManager;
    protected EnemyManager enemyManager;
    protected Vector2 projDir;
    protected VFXManager vfxManager;

    public EnemyType EnemyColor { get; private set; }
    
    protected bool isBounced = false;

    public static event Action<float> OnPlayerHit;
    public static event Action<Vector3, EnemyType> OnBulletDetonated;
    public static event Action<Vector3, Quaternion,EnemyType> OnBulletFired;
    public static event Action<EnemyType> OnEnemyKilled;
    public static event Action OnProjectileReflected;

    public void SetUp(ProjectileManager manager, EnemyManager enemyManagerRef, VFXManager vfxManagerRef)
    {    
        enemyManager = enemyManagerRef;
        projectileManager = manager;
        vfxManager = vfxManagerRef;
    }

    public void OnReset()
    {
        currentLifeTime = 0;
        velocityBeforPaused = Vector2.zero;
        isBounced = false;
    }

    private void OnEnable()
    {
        ProjectileManager.OnGameOver += OnGameOver;
        GamePlayManager.OnGamePaused += OnGamePaused;
        GamePlayManager.OnGameContinued += OnGameContinued;
    }

    private void OnDisable()
    {
        ProjectileManager.OnGameOver -= OnGameOver;
        GamePlayManager.OnGamePaused -= OnGamePaused;
        GamePlayManager.OnGameContinued -= OnGameContinued;
    }

    void Update()
    {
        if (!projectileManager || enemyManager.GetGameState() != GameState.Playing)
            return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
            projectileManager.ResetProjectile(this, projectileType);
    }

    public virtual void ShootProjectile(Vector2 pos, Vector2 dir, EnemyType type)
    {
        if(type == EnemyType.None)
        {
            isBounced = true;
        }
        
        transform.position = pos;
        gameObject.SetActive(true);
        projDir = dir;
        EnemyColor = type;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        OnBulletFired.Invoke(transform.position, Quaternion.Euler(0,0, angle), type);
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
        OnBulletDetonated?.Invoke(explosionPos, EnemyColor);
    }
 
    protected void TriggerOnEnemyKilled(EnemyType enemyType)
    {
        OnEnemyKilled?.Invoke(enemyType);
    }
 
    protected void TriggerOnProjectileReflected()
    {
        OnProjectileReflected?.Invoke();
    }

    protected virtual void OnGamePaused() 
    {
        velocityBeforPaused = rigidBody.linearVelocity;
        rigidBody.linearVelocity = Vector2.zero;
    }

    protected virtual void OnGameContinued() 
    {
        rigidBody.linearVelocity = velocityBeforPaused;
    }
}
