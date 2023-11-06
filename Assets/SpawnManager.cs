using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private List<SpawnPrefab> prefabList = new();

    public List<SpawnPrefab> GetSpawnPrefabs() { return prefabList; }
    public Transform GetTransform() { return transform; }
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> spawnList = new();

    void Awake()
    {
        foreach (SpawnPoint point in spawnList)
        {
            int time = 0;

            foreach (SpawnPrefab prefab in point.GetSpawnPrefabs())
            {
                for (int i = 0; i < prefab.GetCount(); ++i, ++time)
                {
                    StartCoroutine(Spawn(prefab.GetPrefab(), point.GetTransform(), time));
                }
            }
        }
    }

    IEnumerator Spawn(GameObject prefab, Transform tran, int time)
    {
        yield return new WaitForSeconds(time);
        GameObject obj = Instantiate(prefab, tran.position, Quaternion.identity);
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
