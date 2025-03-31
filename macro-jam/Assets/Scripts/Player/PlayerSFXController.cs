using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    [SerializeField]
    private List<StudioEventEmitter> bulletReflectedSFX;
    [SerializeField]
    private StudioEventEmitter movementSFX;
    [SerializeField]
    private StudioEventEmitter playerDeathSFX;
    [SerializeField]
    private StudioEventEmitter swapShieldSFX;
    [SerializeField]
    private StudioEventEmitter wallBounceSFX; 
    [SerializeField]
    private StudioEventEmitter spawnPlayerSFX;
    [SerializeField]
    private StudioEventEmitter heartBeatSFX;

    private bool isHeartBeatPlaying = false;
    private Coroutine movementFadeIn;
    private Coroutine movementFadeOut;

    private void OnEnable()
    {
        GamePlayManager.OnGameOver += StopMovementSFX;
        GamePlayManager.OnGamePaused += StopMovementSFX;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= StopMovementSFX;
        GamePlayManager.OnGamePaused -= StopMovementSFX;
    }

    public void PlayBulletReflectedSFX(int shieldIndex) 
    {
        bulletReflectedSFX[shieldIndex].Play();
    }

    public void PlayMovementSFX() 
    {
        if (movementFadeIn != null)
            StopCoroutine(movementFadeIn);
        if (movementFadeOut != null)
            StopCoroutine(movementFadeOut);

        movementFadeIn = StartCoroutine(AudioManager.MusicFadeIn(movementSFX, false, 0.5f));
    }

    public void StopMovementSFX() 
    {
        if (movementFadeIn != null)
            StopCoroutine(movementFadeIn);
        if (movementFadeOut != null)
            StopCoroutine(movementFadeOut);

        movementFadeOut = StartCoroutine(AudioManager.MusicFadeOut(movementSFX, false, 0.5f));
    }

    public void PlayPlayerDeathSFX() 
    {
        movementSFX.Stop();
        playerDeathSFX.Play();
    }

    public void PlaySwapShieldSFX() 
    {
        swapShieldSFX.Play();
    }

    public void PlayWallBounce() 
    {
        wallBounceSFX.Play();
    }

    public void PlaySpawnPlayer() 
    {
        spawnPlayerSFX.Play();
    }

    public void PlayHeartBeat() 
    {
        if (isHeartBeatPlaying)
            return;

        isHeartBeatPlaying = true;
        heartBeatSFX.Play();
    }

    public void StopHeartBeat() 
    {
        if (!isHeartBeatPlaying)
            return;

        isHeartBeatPlaying = false;
        heartBeatSFX.Stop();
    }
}
