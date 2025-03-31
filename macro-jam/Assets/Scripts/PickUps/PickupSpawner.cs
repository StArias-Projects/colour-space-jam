using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    float startSpawningDelay = 10;

    [SerializeField]
    float secondsPerSpawnMin = 3;

    [SerializeField]
    float secondsPerSpawnMax = 10;

    [SerializeField]
    List<Pickup> pickupPrefabs;

    [SerializeField]
    List<Transform> possibleSpawnPoints;

    [SerializeField] ProjectileManager projectileManager;

    private bool isSpawning = false;

    private void Start()
    {
        isSpawning = true;
        StartCoroutine(SpawningCoroutine());
    }

    private void OnEnable()
    {
        GamePlayManager.OnGameOver += Spawning;
        GamePlayManager.OnGamePaused += Spawning;
        GamePlayManager.OnGameContinued += NoSpawning;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= Spawning;
        GamePlayManager.OnGamePaused -= Spawning;
        GamePlayManager.OnGameContinued -= NoSpawning;
    }
    private void Spawning()
    {
        isSpawning = true;
    }

    private void NoSpawning()
    {
        isSpawning = false;
    }


    IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(startSpawningDelay);

        while (enabled)
        {
            yield return new WaitUntil(() => isSpawning);
            yield return new WaitForSeconds(Random.Range(secondsPerSpawnMin, secondsPerSpawnMax));

            Pickup spawnedPickup = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Count)], transform);
            spawnedPickup.SetUp(projectileManager);
            spawnedPickup.transform.position = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Count)].position;
        }
    }
}
