using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class WaveManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private uint round; // która runda
    [SerializeField] private uint wave; // iloœæ fal danej rundy
    [SerializeField] private uint stage; // który stage
    [SerializeField] private uint maxEnemy;
    [SerializeField] private uint currEnemy; // iloœæ przeciwników

    [Header("Wave Settings")]
    // liniowy przyrost fal na rundê
    [SerializeField] private float waveLinearA = 1f / 5f;
    [SerializeField] private float waveLinearB = 2f;

    [Header("Enemy Settings")]
    // liniowy przyrost przeciwników na rundê
    [SerializeField] private float enemyLinearA = 2f;
    [SerializeField] private float enemyLinearB = 18f;

    [Header("Collections")]
    [SerializeField] private List<GameObject> enemysPrefabs = new();
    [SerializeField] private List<Transform> spawnPoints = new();

    [Header("Next Round Wait Time")]
    // czas oczekiwania na nastêpn¹ rundê
    [SerializeField] private float nextRoundWaitTime = 2f;
    private float nextRoundTime = 0f;

    [Header("Next Wave Wait Time")]
    // czas oczekiwania na nastêpn¹ falê
    [SerializeField] private float nextWaveMaxWaitTime = 10f;
    [SerializeField] private float nextWaveMinWaitTime = 5f;
    private float nextWaveTime = 0f;

    [Header("Next Stage Wait Time")]
    // czas oczekiwania na nastêpny etap
    [SerializeField] private float nextStageMaxWaitTime = 3f;
    [SerializeField] private float nextStageMinWaitTime = 1f;
    private float nextStageTime = 0f;

    private void OnValidate()
    {
        if (nextWaveMaxWaitTime < nextWaveMinWaitTime)
        {
            nextWaveMaxWaitTime = nextWaveMinWaitTime;
        }
        if (nextWaveMinWaitTime > nextWaveMaxWaitTime)
        {
            nextWaveMinWaitTime = nextWaveMaxWaitTime;
        }

        if (nextStageMaxWaitTime < nextStageMinWaitTime)
        {
            nextStageMaxWaitTime = nextStageMinWaitTime;
        }
        if (nextStageMinWaitTime > nextStageMaxWaitTime)
        {
            nextStageMinWaitTime = nextStageMaxWaitTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currEnemy == 0 && wave == 0)
        {
            nextRoundTime -= Time.deltaTime;
            if (nextRoundTime <= 0f)
            {
                nextRoundTime = nextRoundWaitTime;

                // Nowa runda
                NewRound();
            }
        }
        else
        {
            RoundLoop();
        }
    }

    void NewRound()
    {
        stage = 0;
        nextStageTime = 0f;
        nextWaveTime = 0f;

        int wavei = Mathf.RoundToInt(waveLinearA * round + waveLinearB);
        wave = wavei < 0 ? 0 : (uint)wavei;

        int enemyi = Mathf.RoundToInt((enemyLinearA * round + enemyLinearB) / wave);
        maxEnemy = enemyi < 0 ? 0 : (uint)enemyi;

        round++;
    }

    void RoundLoop()
    {
        if (wave > 0 || currEnemy > 0)
        {
            if (currEnemy > 0)
            {
                nextStageTime -= Time.deltaTime;
                if (nextStageTime <= 0f)
                {
                    nextStageTime = Random.Range(nextStageMinWaitTime, nextStageMaxWaitTime);
                    SpawnEnemys();
                    stage++;
                }
            }
            else
            {
                wave -= 1;
                nextWaveTime -= Time.deltaTime;
                if (nextWaveTime <= 0f)
                {
                    nextWaveTime = Random.Range(nextWaveMinWaitTime, nextWaveMaxWaitTime);
                    currEnemy = maxEnemy;
                }
            }
        }
    }

    void SpawnEnemys()
    {
        uint min = (uint)Mathf.Min(2, (int)currEnemy);
        uint max = (uint)Mathf.Min(5, (int)currEnemy);
        uint enemysToSpawn = (uint)Random.Range((int)min, (int)(max + 1));

        currEnemy -= enemysToSpawn;

        for (uint i = 0; i < enemysToSpawn; i++)
        {
            int enemyTypeId = Random.Range(0, enemysPrefabs.Count);
            int spawPointId = Random.Range(0, spawnPoints.Count);

            Instantiate(enemysPrefabs[enemyTypeId], spawnPoints[spawPointId].position, Quaternion.identity);
        }
    }
}
