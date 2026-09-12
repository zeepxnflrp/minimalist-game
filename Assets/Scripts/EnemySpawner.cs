using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [Header("Spawn Area")]
    public float spawnX = 10f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Waves")]
    public int startingEnemies = 1;
    public int maxEnemiesPerWave = 12;

    public float timeBetweenWaves = 2f;
    public float timeBetweenEnemies = 0.3f;

    private int currentWaveSize;

    void Start()
    {
        currentWaveSize = startingEnemies;

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsPlaying);
            SpawnWave();

            yield return new WaitForSeconds(timeBetweenWaves);

            currentWaveSize =
                Mathf.Min(currentWaveSize + 1, maxEnemiesPerWave);
        }
    }

    void SpawnWave()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < currentWaveSize; i++)
        {
            yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsPlaying);
            float randomY = Random.Range(minY, maxY);

            Vector3 spawnPosition = new Vector3(
                spawnX,
                randomY,
                0
            );

            Instantiate(
                enemyPrefab,
                spawnPosition,
                enemyPrefab.transform.rotation
            );

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }
}