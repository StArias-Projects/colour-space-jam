using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float maxHealth;

    [SerializeField]
    [Min(0)]
    private float maxSpeed;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private LayerMask playerMask;

    private int poolGroup;
    private int currentDirIndex = -1;

    private float health = 0;
    private float currentSpeed = 0;

    private Vector3 movementDir = Vector3.zero;

    private List<Collider> directionPoints = new List<Collider>();

    private EnemyManager enemyManager;

    public void SetUpEnemy(EnemyManager manager, List<Collider> dirPoints, int group)
    {
        enemyManager = manager;
        health = maxHealth;
        poolGroup = group;
        currentSpeed = maxSpeed;
        SetUpDirectionPoints(dirPoints);
    }

    private void SetUpDirectionPoints(List<Collider> dirPoints)
    {
        int maxDirPoints = UnityEngine.Random.Range(2, dirPoints.Count);
        directionPoints = dirPoints.OrderBy(x => UnityEngine.Random.value).Take(maxDirPoints).ToList();
    }

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
        if (enemyManager.GetGameState() != GamePlayManager.GameState.Playing)
            return;

        rb.linearVelocity = currentSpeed * movementDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentDirIndex >= 0 && currentDirIndex < directionPoints.Count
            && other.gameObject == directionPoints[currentDirIndex].gameObject)
        {
            ChangeMovementDirection();
        }
    }

    #endregion

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
        if (health <= 0)
        {
            enemyManager.ResetEnemy(this);
            return true;
        }

        return false;
    }

    public void StartEnemy(Vector3 position)
    {
        currentSpeed = maxSpeed;
        gameObject.SetActive(true);
        health = maxHealth;
        transform.position = position;
        ChangeMovementDirection();
    }

    public void OnGameOver()
    {
        currentSpeed = 0;
    }
}
