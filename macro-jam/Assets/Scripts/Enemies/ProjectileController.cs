using System;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float speedAfterBounced = 12;

    [SerializeField]
    private float maxLifeTime;

    [SerializeField]
    private float attackPower;

    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private LayerMask shieldMask;

    [SerializeField]
    private LayerMask enemyMask;

    private float currentLifeTime = 0;
    private ProjectileManager projectileManager;
    private Vector2 projDir;
    public EnemyType EnemyType { get; private set; }
    private bool isBounced = false;

    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEnemyHit;
    public static event Action<ProjectileController> OnBulletDetonated;

    public void SetUp(ProjectileManager manager, EnemyType type)
    {
        EnemyType = type;
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

    private void OnGameOver()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
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
        if(collision.TryGetComponent(out Shield shieldHit))
        {
            if(shieldHit.color == EnemyType)
            {
                projDir = shieldHit.transform.up;
                speed = speedAfterBounced;
                isBounced = true;
                //do bullet vfx and maybe change to white bullet to indicate it can hit any enemy color now
            }
        }
        else if(isBounced && collision.TryGetComponent(out EnemyController enemy))
        {
            OnBulletDetonated?.Invoke(this);
            enemy.ReceiveDamage(attackPower);
            projectileManager.ResetProjectile(this, EnemyType);
            
        }




        int mask = 1 << collision.gameObject.layer;

        if ((mask & playerMask.value) != 0)
        {
            OnBulletDetonated?.Invoke(this);
            OnPlayerHit?.Invoke(attackPower);
            projectileManager.ResetProjectile(this, EnemyType);
        }
    }

    public void ShootProjectile(Vector2 pos, Vector2 dir)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        projDir = dir;
    }
}
