using FMODUnity;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter spawnEmitter;

    [SerializeField]
    private StudioEventEmitter deathEmitter;


    public void PlaySpawnSFX() 
    {
        spawnEmitter.Play();
    }

    public void PlayDeathSFX() 
    {
        deathEmitter.Play();
    }
}
