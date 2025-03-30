using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class SquareProjectileController : ProjectileController
{
    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float speedAfterBounced = 12;

    public override void ShootProjectile(Vector2 pos, Vector2 dir, EnemyType type)
    {
        base.ShootProjectile(pos, dir, type);
        sprite.color = enemyManager.GetEnemyColor(type);
    }


    private void FixedUpdate()
    {
        rigidBody.linearVelocity = projDir * speed;

        float angle = Mathf.Atan2(projDir.y, projDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Shield shieldHit))
        {
            if (shieldHit.color == enemyColor)
            {
                projDir = shieldHit.transform.up;
                speed = speedAfterBounced;
                isBounced = true;

                ProjectileBounceEffect();
                TriggerOnProjectileReflected();
            }
        }
        else if (isBounced && collision.TryGetComponent(out EnemyController enemy))
        {
            TriggerOnBulletDetonated(transform.position);

            if (enemy.ReceiveDamage(attackPower))
            {
                EnemyType enemyType = enemy.GetEnemyType();
                TriggerOnEnemyKilled(enemyType);
            }
            projectileManager.ResetProjectile(this, projectileType);
        }
        else if (!isBounced && collision.TryGetComponent(out PlayerManager player))
        {
            TriggerOnBulletDetonated(transform.position);
            TriggerOnPlayerHit(attackPower);
            projectileManager.ResetProjectile(this, projectileType);
            player.ReceiveDamage(attackPower);

        }
    }

    private void ProjectileBounceEffect()
    {
        //do bullet vfx and maybe change to white bullet to indicate it can hit any enemy color now
        sprite.color = Color.white;
    }
}
