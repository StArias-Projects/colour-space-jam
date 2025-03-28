using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float maxLifeTime;

    [SerializeField]
    private float attackPower;

    private float currentLifeTime = 0;
    private ProjectileManager projectileManager;
    private Vector2 projDir;
    private EnemyType enemyType;

    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEnemyHit;

    public void SetUp(ProjectileManager manager, EnemyType type)
    {
        enemyType = type;
        projectileManager = manager;
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
    }

    void Update()
    {
        if (!projectileManager)
            return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
            projectileManager.ResetProjectile(this, enemyType);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void ShootProjectile(Vector2 pos, Vector2 dir)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        projDir = dir;
    }
}
