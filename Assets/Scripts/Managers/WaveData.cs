using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public readonly float timeBetweenObjects = 0.75f;

    public uint numberOfSpawnPoints;

    public List<WavePrefab> enemies;
}
