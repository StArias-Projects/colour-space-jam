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


    [SerializeField]
    private float rotateTowardsTargetSpeed = 0;

    #endregion

    #region Private Variables

    private int poolGroup;
    private int currentDirIndex = -1;
    private Transform target;
    private float health = 0;
    private float currentSpeed = 0;
    private bool isDead = false;

    private Vector2 movementDir = Vector2.zero;
    private List<Collider2D> directionPoints = new();
    private EnemyManager enemyManager;

    #endregion

    #region Set Up

    public void SetUpEnemy(EnemyManager manager, ProjectileManager projManager, List<Collider2D> dirPoints, int group, Transform targetTr, EnemyType enemyColor)
    {
        enemyManager = manager;
        health = maxHealth;
        poolGroup = group;
        currentSpeed = maxSpeed;
        enemyType = enemyColor;
        target = targetTr;
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
        if (!weaponController.IsShooting)
        {
            rigidBody.AddForce( currentSpeed * movementDir);
            RotateTowardsTarget(1);
        }
        else
        {
            RotateTowardsTarget(.5f);
        }
    }

    private void RotateTowardsTarget(float rotateSpeedMultiplier =1f)
    {
        if (rotateTowardsTargetSpeed == 0) return;
        Vector2 directionToPlayer = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.fixedDeltaTime * rotateTowardsTargetSpeed * rotateSpeedMultiplier);
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

        Tween tween = sprite.material.DOFloat(health / maxHealth, "_PercentHealth", .5f).SetEase(Ease.OutSine);
        shadowSprite.material.DOFloat(health / maxHealth, "_PercentHealth", .5f).SetEase(Ease.OutSine);

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(EnemyDeath(.5f));

            return true;
        }

        return false;
    }

    IEnumerator EnemyDeath(float seconds)
    {
        isDead = true;
        sprite.material.DOFloat(-1, "_PercentHealth", .5f).SetEase(Ease.OutSine);
        shadowSprite.material.DOFloat(-1, "_PercentHealth", .5f).SetEase(Ease.OutSine);

        yield return new WaitForSeconds(seconds);

        enemyManager.ResetEnemy(this);
    }

    public void StartEnemy(Vector2 position)
    {
        currentSpeed = maxSpeed;
        gameObject.SetActive(true);
        isDead = false;
        health = maxHealth;
        transform.position = position;
        ChangeMovementDirection();

        sprite.DOFade(0, 0).OnComplete(()=> sprite.DOFade(1, 1));
    }

    public void OnGameOver()
    {
        if (!enemyManager)
            return;

        enemyManager.ResetEnemy(this);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
