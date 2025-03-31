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

    private void Start()
    {
        StartCoroutine(SpawningCoroutine());
    }


    IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(startSpawningDelay);
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(secondsPerSpawnMin, secondsPerSpawnMax));
            Pickup spawnedPickup = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Count)], transform);
            spawnedPickup.SetUp(projectileManager);
            spawnedPickup.transform.position = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Count)].position;
        }

    }
}
