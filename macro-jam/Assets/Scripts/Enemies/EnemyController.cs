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
    private EnemySFX sfxController;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private SpriteRenderer shadowSprite;

    [SerializeField]
    [Min(0)]
    private float maxHealth;

    [SerializeField]
    [Min(0)]
    private float maxMovementForce;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private LayerMask shieldMask;

    [SerializeField]
    private LayerMask boundaryMask;


    [SerializeField]
    private float rotateTowardsTargetSpeed = 0;

    #endregion

    #region Private Variables

    private int poolGroup;
    private int currentDirIndex = -1;
    private Transform target;
    private float health = 0;
    private float movementForce = 0;
    private bool isDead = false;

    private Vector2 movementDir = Vector2.zero;
    private List<Collider2D> directionPoints = new();
    private EnemyManager enemyManager;
    private Vector2 velocityBeforePause = Vector2.zero;

    #endregion

    #region Set Up

    public void SetUpEnemy(EnemyManager manager, ProjectileManager projManager, List<Collider2D> dirPoints, int group, Transform targetTr, EnemyType enemyColor)
    {
        enemyManager = manager;
        health = maxHealth;
        poolGroup = group;
        movementForce = maxMovementForce;
        enemyType = enemyColor;
        target = targetTr;
        SetUpDirectionPoints(dirPoints);
        weaponController.SetUpWeapon(this, projManager, sfxController, targetTr);
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
        GamePlayManager.OnGamePaused += OnGamePaused;
        GamePlayManager.OnGameContinued -= OnGameContinued;
    }

    private void OnDisable()
    {
        EnemyManager.OnGameOver -= OnGameOver;
        GamePlayManager.OnGamePaused -= OnGamePaused;
        GamePlayManager.OnGameContinued -= OnGameContinued;
    }

    private void FixedUpdate()
    {
        if (!enemyManager || enemyManager.GetGameState() != GameState.Playing)
            return;

        if (!weaponController.IsShooting)
        {
            //rigidBody.linearVelocity = currentSpeed * movementDir;
            rigidBody.AddForce(movementForce * movementDir);
            RotateTowardsTarget(1);
        }
        else
        {
            RotateTowardsTarget(.5f);
        }
    }

    private void RotateTowardsTarget(float rotateSpeedMultiplier = 1f)
    {
        if (rotateTowardsTargetSpeed == 0) return;
        Vector2 directionToPlayer = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.fixedDeltaTime * rotateTowardsTargetSpeed * rotateSpeedMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int mask = 1 << collision.gameObject.layer;
        bool isCorrectIndex = currentDirIndex >= 0 && currentDirIndex < directionPoints.Count;
        bool isCorrectMask = collision.gameObject == directionPoints[currentDirIndex].gameObject || (mask & playerMask.value) != 0
                                || (mask & shieldMask.value) != 0 || (mask & boundaryMask.value) != 0;
        if (isCorrectIndex && isCorrectMask)
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
        rigidBody.linearVelocity = Vector3.zero;

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
            sfxController.PlayDeathSFX();
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
        movementForce = maxMovementForce;
        gameObject.SetActive(true);
        isDead = false;
        health = maxHealth;
        transform.position = position;
        ChangeMovementDirection();

        sprite.DOFade(0, 0).OnComplete(() => sprite.DOFade(1, 1));
        sfxController.PlaySpawnSFX();
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

    private void OnGamePaused()
    {
        velocityBeforePause = rigidBody.linearVelocity;
        rigidBody.linearVelocity = Vector2.zero;
    }

    private void OnGameContinued()
    {
        rigidBody.linearVelocity = velocityBeforePause;
    }
}
