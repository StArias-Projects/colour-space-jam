using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    #region Editor Variables

    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    private EnemyWeaponController weaponController;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private SpriteRenderer shadowSprite;

    [SerializeField]
    [Min(0)]
    private float maxHealth;

    [SerializeField]
    [Min(0)]
    private float maxSpeed;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private LayerMask shieldMask;

    #endregion

    #region Private Variables

    private int poolGroup;
    private int currentDirIndex = -1;

    private float health = 0;
    private float currentSpeed = 0;

    private Vector2 movementDir = Vector2.zero;
    private List<Collider2D> directionPoints = new();
    private EnemyManager enemyManager;

    #endregion

    #region Set Up

    public void SetUpEnemy(EnemyManager manager, ProjectileManager projManager, List<Collider2D> dirPoints, int group, Transform targetTr)
    {
        enemyManager = manager;
        health = maxHealth;
        poolGroup = group;
        currentSpeed = maxSpeed;

        SetUpDirectionPoints(dirPoints);
        weaponController.SetUpWeapon(this, projManager, targetTr);
        sprite.color = enemyManager.GetEnemyColor(enemyType);
    }

    private void SetUpDirectionPoints(List<Collider2D> dirPoints)
    {
        int maxDirPoints = UnityEngine.Random.Range(2, dirPoints.Count);
        directionPoints = dirPoints.OrderBy(x => UnityEngine.Random.value).Take(maxDirPoints).ToList();
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        EnemyManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        EnemyManager.OnGameOver -= OnGameOver;
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = currentSpeed * movementDir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int mask = 1 << collision.gameObject.layer;
        if (currentDirIndex >= 0 && currentDirIndex < directionPoints.Count &&
            (collision.gameObject == directionPoints[currentDirIndex].gameObject
            || (mask & playerMask.value) != 0
            || (mask & shieldMask.value) != 0))
        {
            ChangeMovementDirection();
        }
    }

    #endregion

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public void ChangeMovementDirection()
    {
        currentDirIndex++;
        if (currentDirIndex >= directionPoints.Count)
            currentDirIndex = 0;

        movementDir = (directionPoints[currentDirIndex].transform.position - transform.position).normalized;
    }

    public int GetGroupIndex()
    {
        return poolGroup;
    }

    public bool ReceiveDamage(float damage)
    {
        health -= damage;

        Tween tween = sprite.material.DOFloat(health / maxHealth, "_PercentHealth", .8f).SetEase(Ease.OutBounce);
        shadowSprite.material.DOFloat(health / maxHealth, "_PercentHealth", .8f).SetEase(Ease.OutBounce);

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(EnemyDeath(tween.Duration()));

            return true;
        }

        return false;
    }

    IEnumerator EnemyDeath(float seconds)
    {
        // Trigger Die Animation here
        yield return new WaitForSeconds(seconds);

        enemyManager.ResetEnemy(this);
    }

    public void StartEnemy(Vector2 position)
    {
        currentSpeed = maxSpeed;
        gameObject.SetActive(true);
        health = maxHealth;
        transform.position = position;
        ChangeMovementDirection();
    }

    public void OnGameOver()
    {
        if (!enemyManager)
            return;

        enemyManager.ResetEnemy(this);
    }
}
