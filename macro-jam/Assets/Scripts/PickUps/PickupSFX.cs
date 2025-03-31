using FMODUnity;
using UnityEngine;

public class PickupSFX : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter pickUpTriggered;

    [SerializeField]
    private StudioEventEmitter pickUpSpawn;

    [SerializeField]
    private StudioEventEmitter pickUpAvailable;

    [SerializeField]
    private StudioEventEmitter pickUpExpired;

    private void OnEnable()
    {
        GamePlayManager.OnGameOver += StopSFX;
        GamePlayManager.OnGamePaused += StopSFX;
        GamePlayManager.OnGameContinued += PlayPickUpAvailable;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= StopSFX;
        GamePlayManager.OnGamePaused -= StopSFX;
        GamePlayManager.OnGameContinued -= PlayPickUpAvailable;
    }

    public void StopSFX()
    {
        pickUpAvailable.Stop();
    }

    public void PlayPickUpTriggered()
    {
        pickUpAvailable.Stop();
        pickUpTriggered.Play();
    }

    public void PlayPickUpAvailable()
    {
        pickUpAvailable.Play();
    }

    public void PlayPickUpSpawn()
    {
        pickUpSpawn.Play();
    }

    public void PlayPickUpExpired()
    {
        pickUpAvailable.Stop();
        pickUpExpired.Play();
    }
}
