using UnityEngine;
using DG.Tweening;

public class RayProjectile : ProjectileController
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    private float distance;

    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private LayerMask shieldMask;

    public override void ShootProjectile(Vector2 pos, Vector2 dir, EnemyType type)
    {
        base.ShootProjectile(pos, dir, type);
        var color = enemyManager.GetEnemyColor(type);

        Sequence sequence = DOTween.Sequence().SetUpdate(false);
        sequence.Append(DOTween.To(() => lineRenderer.startColor, (x) => lineRenderer.startColor = x, color, .1f).SetUpdate(false));
        sequence.Append(DOTween.To(() => lineRenderer.endColor, (x) => lineRenderer.endColor = x, color, .1f).SetUpdate(false));
        Color fadedColor = color;
        fadedColor.a = 0;
        sequence.Append(DOTween.To(() => lineRenderer.startColor, (x) => lineRenderer.startColor = x, fadedColor, maxLifeTime / 2f).SetUpdate(false));
        sequence.Append(DOTween.To(() => lineRenderer.endColor, (x) => lineRenderer.endColor = x, fadedColor, maxLifeTime / 2f).SetUpdate(false));

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, pos + (dir * distance));
        var mask = playerMask | shieldMask;
        RaycastHit2D raycastHit = Physics2D.Raycast(pos, dir, distance, mask, -1);

        if (raycastHit && raycastHit.collider.TryGetComponent(out Shield shield) && shield.color == EnemyColor)
        {
            Vector3 shieldPos = shield.transform.position;
            TriggerOnBulletDetonated(shield.transform.position);
            shield.OnHit();
            TriggerOnProjectileReflected();

            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(1, shieldPos);
            lineRenderer.SetPosition(2, shieldPos + shield.transform.up * 100);

            mask = LayerMask.GetMask("Enemy", "Default");
            raycastHit = Physics2D.Raycast(shieldPos, shield.transform.up, 100, mask, -1);
            if (raycastHit && raycastHit.collider.TryGetComponent(out EnemyController enemy))
            {
                enemy.ReceiveDamage(attackPower);
                TriggerOnBulletDetonated(raycastHit.point);
                lineRenderer.SetPosition(2, enemy.transform.position);
            }
            else if (raycastHit && raycastHit.collider.TryGetComponent(out FractureCrystal crystal))
            {
                crystal.OnHit(true);
                TriggerOnBulletDetonated(raycastHit.point);
                lineRenderer.SetPosition(2, crystal.transform.position);
            }

            return;
        }

        if (raycastHit) {
            mask = playerMask;
            RaycastHit2D newrayCastHit = Physics2D.Raycast(raycastHit.transform.position, dir, 20, mask, -1);
            if (newrayCastHit && newrayCastHit.collider.TryGetComponent(out PlayerManager player))
            {
                lineRenderer.SetPosition(1, player.transform.position);
                player.ReceiveDamage(attackPower);
                TriggerOnBulletDetonated(raycastHit.point);
            }
        }
    }
}
