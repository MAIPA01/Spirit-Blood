using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[System.Serializable]
public class WaveObject
{
    [SerializeField] private List<uint> spawnPointsIndexes;
    [SerializeField] private List<WaveSpawnPrefab> prefabs;
    [SerializeField] private uint repeat = 1;

    public List<uint> GetSpawnPointsIndexes() { return spawnPointsIndexes; }
    public List<WaveSpawnPrefab> GetWaveSpawnPrefabs() { return prefabs; }

    public void FinishOneCycle() { repeat--; }

    public uint GetRepeat() { return repeat; }
}

[System.Serializable]
public class WaveSpawnPrefab
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private uint count = 0;

    public uint GetCount() { return count; }
    public GameObject GetPrefab() { return prefab; }
}

public class WaveSpawnManager : MonoBehaviour
{
    [SerializeField] private float timeBetweenWaves = 5.0f;
    [SerializeField] private float timeBetweenObjects = 1.0f;
    [SerializeField] private TextMeshProUGUI countdownTxt;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<WaveObject> waves;

    private float timeCount = 0.0f;
    private float oneWaveTime = 0.0f;

    private uint aWave = 0;

    private void OnValidate()
    {
        if (waves.Count == 0)
        {
            Debug.LogError("~~~~Provide some Waves~~~~");
        }
    }

    void Start()
    {
        if (waves.Count > 0)
        {
            for (uint i = aWave; i < waves.Count; ++i)
            {
                if (waves[(int)i].GetRepeat() > 0)
                {
                    aWave = i;
                    oneWaveTime = SpawnEverything(waves[(int)i]);
                    waves[(int)i].FinishOneCycle();
                    break;
                }
            }
        }

        timeCount = timeBetweenWaves;
        countdownTxt.text = "";
    }

    void Update()
    {
        if (waves.Count > 0)
        {
            if (oneWaveTime > 0.0f)
            {
                oneWaveTime -= Time.deltaTime;
            }
            else
            {
                if (timeCount > 0.0f)
                {
                    timeCount -= Time.deltaTime;
                    countdownTxt.text = new StringBuilder("Next Wave In: ").Append(timeCount.ToString("0")).Append("s").ToString();
                }
                else
                {
                    countdownTxt.text = "";
                    for (uint i = aWave; i < waves.Count; ++i)
                    {
                        if (waves[(int)i].GetRepeat() > 0)
                        {
                            aWave = i;
                            oneWaveTime = SpawnEverything(waves[(int)i]);
                            waves[(int)i].FinishOneCycle();
                            break;
                        }
                    }
                    timeCount = timeBetweenWaves;
                }
            }
        }
    }

    float SpawnEverything(WaveObject wave)
    {
        float time = 0.0f;

        foreach (WaveSpawnPrefab prefab in wave.GetWaveSpawnPrefabs())
        {
            for (int i = 0; i < prefab.GetCount(); ++i, time += timeBetweenObjects)
            {
                int n = (int)wave.GetSpawnPointsIndexes()[Random.Range(0, wave.GetSpawnPointsIndexes().Count)];
                if (n < spawnPoints.Count)
                {
                    Transform t = spawnPoints[n];
                    StartCoroutine(Spawn(prefab.GetPrefab(), t, time));
                }
                else
                {
                    Debug.LogWarning(n + " point was not defined!");
                }
            }
        }

        return time;
    }

    IEnumerator Spawn(GameObject prefab, Transform tran, float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(prefab, tran.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        foreach (Transform point in spawnPoints)
        {
            Gizmos.DrawSphere(point.position, 0.25f);
        }
    }
}
