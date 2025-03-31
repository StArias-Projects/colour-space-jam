using UnityEngine;

public class SquareProjectileController : ProjectileController
{
    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float speedAfterBounced = 12;

    [SerializeField]
    private LayerMask boundaryMask;

    public override void ShootProjectile(Vector2 pos, Vector2 dir, EnemyType type)
    {
        base.ShootProjectile(pos, dir, type);
        sprite.color = enemyManager.GetEnemyColor(type);
    }

    private void FixedUpdate()
    {
        if (enemyManager.GetGameState() != GameState.Playing)
            return;

        rigidBody.linearVelocity = projDir * speed;

        float angle = Mathf.Atan2(projDir.y, projDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int mask = 1 << collision.gameObject.layer;
        if((mask & boundaryMask) != 0) 
        {
            TriggerOnBulletDetonated(transform.position);
            projectileManager.ResetProjectile(this, projectileType);
        }
        else if (collision.TryGetComponent(out Shield shieldHit) && shieldHit.color == EnemyColor)
        {
            shieldHit.OnHit();
            projDir = shieldHit.transform.up;
            speed = speedAfterBounced;
            isBounced = true;

            ProjectileBounceEffect();
            shieldHit.PlayShieldBulletReflected();
            TriggerOnProjectileReflected();

            float angle = Mathf.Atan2(shieldHit.transform.up.y, shieldHit.transform.up.x) * Mathf.Rad2Deg;

            vfxManager.PlayFiringVFX(transform.position, Quaternion.Euler(0, 0, angle), EnemyColor);
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
        else if (!isBounced && collision.TryGetComponent(out PlayerManager player) && EnemyColor != EnemyType.None)
        {
            TriggerOnBulletDetonated(transform.position);
            TriggerOnPlayerHit(attackPower);
            projectileManager.ResetProjectile(this, projectileType);
        }
        else if (isBounced && collision.TryGetComponent(out FractureCrystal crystal))
        {
            crystal.OnHit(false);
        }
    }

    private void ProjectileBounceEffect()
    {
        //do bullet vfx and maybe change to white bullet to indicate it can hit any enemy color now
        sprite.color = Color.white;
    }
}
