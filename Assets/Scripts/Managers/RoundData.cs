using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Spawn Manager/Round Data")]
public class RoundData : ScriptableObject
{
    public List<WaveData> waves;
}