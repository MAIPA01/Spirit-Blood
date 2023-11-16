using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class SpawnPrefab
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private int count = 0;

    public int GetCount() {  return count; }
    public GameObject GetPrefab() {  return prefab; }
}

[System.Serializable]
public class SpawnPoint
{
    [SerializeField] private Transform transform = null;
    [SerializeField] private List<SpawnPrefab> prefabList;

    public List<SpawnPrefab> GetSpawnPrefabs() { return prefabList; }
    public Transform GetTransform() { return transform; }
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] 
    private bool loop = false;
    [SerializeField]
    [ShowIf("loop")]
    private float timeBetweenWaves = 5.0f;
    [SerializeField]
    private float timeBetweenObjects = 1.0f;
    [SerializeField] 
    private List<SpawnPoint> spawnList = new();

    private float timeBefore = 0.0f;

    void Awake()
    {
        timeBefore = timeBetweenWaves + SpawnEverything(timeBefore);
    }

    private void Update()
    {
        if (loop)
        {
            timeBefore = timeBetweenWaves + SpawnEverything(timeBefore);
        }
    }

    float SpawnEverything(float addT)
    {
        float lastTime = 0.0f;
        foreach (SpawnPoint point in spawnList)
        {
            float time = addT;

            foreach (SpawnPrefab prefab in point.GetSpawnPrefabs())
            {
                for (int i = 0; i < prefab.GetCount(); ++i, time += timeBetweenObjects)
                {
                    StartCoroutine(Spawn(prefab.GetPrefab(), point.GetTransform(), time));
                }
            }

            if (lastTime < time)
            {
                lastTime = time;
            }
        }

        return lastTime;
    }

    IEnumerator Spawn(GameObject prefab, Transform tran, float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(prefab, tran.position, Quaternion.identity);
        yield return new WaitForSeconds(2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        foreach (SpawnPoint point in spawnList)
        {
            Gizmos.DrawSphere(point.GetTransform().position, 0.25f);
        }
    }
}
