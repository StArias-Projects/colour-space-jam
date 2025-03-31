using UnityEngine;

public class FractureCrystal : Pickup
{
    

    [SerializeField]
    float numberOfFractures = 5;

    ProjectileType projectileToFire = ProjectileType.Small;
  
    public void OnHit(bool byRay)
    {
        if (triggered) return;
        if (byRay)
        {
            numberOfFractures *= 2;
        }
        Trigger(null);
    }

    protected override void Trigger(PlayerManager player)
    {
        base.Trigger(player);
     
        float radiansPerIteration = Mathf.PI *2 / numberOfFractures;
        for (int i = 0; i < numberOfFractures; i++)
        {
            ProjectileController proj = projectileManager.GetProjectile(projectileToFire);
            Vector2 firingDirection;
            firingDirection.x = Mathf.Cos(i * radiansPerIteration);
            firingDirection.y = Mathf.Sin(i * radiansPerIteration);
            print(firingDirection);
            proj.ShootProjectile(transform.position, firingDirection, EnemyType.None);

        }
        
    }
}
