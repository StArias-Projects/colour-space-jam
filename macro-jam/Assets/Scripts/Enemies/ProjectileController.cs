using System;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float speedAfterBounced = 12;

    [SerializeField]
    private float maxLifeTime;

    [SerializeField]
    private float attackPower;

    private float currentLifeTime = 0;
    private ProjectileManager projectileManager;
    private Vector2 projDir;
    public EnemyType EnemyType { get; private set; }
    private bool isBounced = false;

    public static event Action<float> OnPlayerHit;
    public static event Action<ProjectileController> OnBulletDetonated;
    public static event Action<EnemyType> OnEnemyKilled;
    public static event Action OnProjectileReflected;

    public void SetUp(ProjectileManager manager, EnemyType type, EnemyManager enemyManager)
    {
        EnemyType = type;
        projectileManager = manager;
        sprite.color = enemyManager.GetEnemyColor(type);
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

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = projDir * speed;

        float angle = Mathf.Atan2(projDir.y, projDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        if (!projectileManager)
            return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
            projectileManager.ResetProjectile(this, EnemyType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Shield shieldHit))
        {
            if (shieldHit.color == EnemyType)
            {
                projDir = shieldHit.transform.up;
                speed = speedAfterBounced;
                isBounced = true;

                ProjectileBounceEffect();
                OnProjectileReflected?.Invoke();
            }
        }
        else if (isBounced && collision.TryGetComponent(out EnemyController enemy))
        {
            OnBulletDetonated?.Invoke(this);

            if (enemy.ReceiveDamage(attackPower))
            {
                EnemyType enemyType = enemy.GetEnemyType();
                OnEnemyKilled?.Invoke(enemyType);
            }
            projectileManager.ResetProjectile(this, EnemyType);
        }
        else if (!isBounced && collision.transform.parent && collision.transform.parent.TryGetComponent(out PlayerManager player))
        {
            OnBulletDetonated?.Invoke(this);
            OnPlayerHit?.Invoke(attackPower);
            projectileManager.ResetProjectile(this, EnemyType);
        }
    }

    private void ProjectileBounceEffect()
    {
        //do bullet vfx and maybe change to white bullet to indicate it can hit any enemy color now
        sprite.color = Color.white;
    }

    public void ShootProjectile(Vector2 pos, Vector2 dir)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        projDir = dir;
    }

    private void OnGameOver()
    {
        if (!projectileManager)
            return;

        projectileManager.ResetProjectile(this, EnemyType);
    }
}
