using UnityEngine;
using FMODUnity;

public class ProjectileSFXController : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter shotEmitter;

    [SerializeField]
    private StudioEventEmitter projectileDestroyed;

    public void PlayShotSFX()
    {
        shotEmitter.Play();
    }

    public void PlayProjectileDestroyed()
    {
        if (!projectileDestroyed)
            return;

        projectileDestroyed.Play();
    }
}
