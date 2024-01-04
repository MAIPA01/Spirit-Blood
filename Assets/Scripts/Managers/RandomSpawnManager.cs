using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpawnManager : MonoBehaviour
{
    [SerializeField]
    private bool loop = false;

    [ShowIf("loop")]
    [SerializeField]
    private uint duplicatedRoundsFactor = 6;

    [SerializeField]
    private List<RoundData> rounds;

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private TextMeshProUGUI pressButtonTxt;

    /*
    * [SerializeField]
    * private Button startRoundButton;
    */

    [SerializeField]
    private LayerMask objectsLayers;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private uint roundNumber = 0;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private uint waveNumber = 0;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private int enemies = 0;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private bool playNextRound = true;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private bool spawned = false;

    [Foldout("Info")]
    [ReadOnly]
    [SerializeField]
    private bool active = false;

    private void OnValidate()
    {
        if (rounds.Count == 0)
        {
            Debug.LogError("No rounds where specified");
        }
        else
        {
            int i = 0;
            foreach (var item in rounds)
            {
                if (item == null)
                {
                    Debug.LogError(new StringBuilder("Round ").Append(i).Append(" is null. Pls provide some round object").ToString());
                }
                i++;
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawnPoints where specified");
        }

        if (pressButtonTxt == null)
        {
            Debug.LogError("Where is THE Text??");
        }

        /*
        if (startRoundButton == null)
        {

        }
        */
    }

    void Start()
    {
        //startRoundButton.onClick.AddListener(PlayNextRound);
        //startRoundButton.gameObject.SetActive(false);
        active = false;
        pressButtonTxt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (active && Input.GetMouseButton((int)MouseButton.Right))
        {
            PlayNextRound();
        }

        enemies = 0;

        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            if (((1 << item.layer) & objectsLayers.value) != 0)
            {
                enemies++;
            }
        }

        if (!loop)
        {
            if (enemies == 0 && spawned)
            {
                waveNumber++;
                spawned = false;
            }

            if (roundNumber < rounds.Count && waveNumber >= rounds[(int)roundNumber].waves.Count)
            {
                roundNumber++;
                waveNumber = 0;
                playNextRound = false;

                if (roundNumber != rounds.Count)
                {
                    pressButtonTxt.text = "Press RMB to start next round";
                    pressButtonTxt.gameObject.SetActive(true);
                    active = true;
                    //startRoundButton.gameObject.SetActive(true);
                }
            }

            if (roundNumber < rounds.Count && playNextRound)
            {
                if (waveNumber < rounds[(int)roundNumber].waves.Count && !spawned)
                {
                    uint num = rounds[(int)roundNumber].waves[(int)waveNumber].numberOfSpawnPoints;

                    List<Transform> points = new();

                    if (num < spawnPoints.Count)
                    {
                        for (int i = 0; i < num; ++i)
                        {
                            if (points.Count != 0)
                            {
                                List<Transform> temp = new();

                                for (int z = 0; z < spawnPoints.Count; ++z)
                                {
                                    if (!points.Contains(spawnPoints[z]))
                                    {
                                        temp.Add(spawnPoints[z]);
                                    }
                                }

                                points.Add(temp[Random.Range(0, temp.Count)]);

                                temp.Clear();
                            }
                            else
                            {
                                points.Add(spawnPoints[Random.Range(0, spawnPoints.Count)]);
                            }
                        }
                    }
                    else if (num == spawnPoints.Count)
                    {
                        for (int i = 0; i < spawnPoints.Count; ++i)
                        {
                            points.Add(spawnPoints[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < spawnPoints.Count - 1; ++i)
                        {
                            if (points.Count != 0)
                            {
                                List<Transform> temp = new();

                                for (int z = 0; z < spawnPoints.Count; ++z)
                                {
                                    if (!points.Contains(spawnPoints[z]))
                                    {
                                        temp.Add(spawnPoints[z]);
                                    }
                                }

                                points.Add(temp[Random.Range(0, temp.Count)]);

                                temp.Clear();
                            }
                            else
                            {
                                points.Add(spawnPoints[Random.Range(0, spawnPoints.Count)]);
                            }
                        }
                    }

                    WaveData obj = rounds[(int)roundNumber].waves[(int)waveNumber];
                    SpawnEverything(obj, obj.timeBetweenObjects, points.ToArray());
                }
            }
        }
        else
        {
            if (enemies == 0 && spawned)
            {
                waveNumber++;
                spawned = false;
            }

            if (waveNumber >= rounds[(int)(roundNumber % rounds.Count)].waves.Count)
            {
                roundNumber++;
                waveNumber = 0;
                playNextRound = false;
                pressButtonTxt.text = "Press RMB to start next round";
                pressButtonTxt.gameObject.SetActive(true);
                active = true;
                //startRoundButton.gameObject.SetActive(true);
            }

            if (roundNumber % rounds.Count < rounds.Count && playNextRound)
            {
                if (waveNumber < rounds[(int)(roundNumber % rounds.Count)].waves.Count && !spawned)
                {
                    uint num = rounds[(int)(roundNumber % rounds.Count)].waves[(int)waveNumber].numberOfSpawnPoints;

                    List<Transform> points = new();

                    if (num < spawnPoints.Count)
                    {
                        for (int i = 0; i < num; ++i)
                        {
                            if (points.Count != 0)
                            {
                                List<Transform> temp = new();

                                for (int z = 0; z < spawnPoints.Count; ++z)
                                {
                                    if (!points.Contains(spawnPoints[z]))
                                    {
                                        temp.Add(spawnPoints[z]);
                                    }
                                }

                                points.Add(temp[Random.Range(0, temp.Count)]);

                                temp.Clear();
                            }
                            else
                            {
                                points.Add(spawnPoints[Random.Range(0, spawnPoints.Count)]);
                            }
                        }
                    }
                    else if (num == spawnPoints.Count)
                    {
                        for (int i = 0; i < spawnPoints.Count; ++i)
                        {
                            points.Add(spawnPoints[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < spawnPoints.Count - 1; ++i)
                        {
                            if (points.Count != 0)
                            {
                                List<Transform> temp = new();

                                for (int z = 0; z < spawnPoints.Count; ++z)
                                {
                                    if (!points.Contains(spawnPoints[z]))
                                    {
                                        temp.Add(spawnPoints[z]);
                                    }
                                }

                                points.Add(temp[Random.Range(0, temp.Count)]);

                                temp.Clear();
                            }
                            else
                            {
                                points.Add(spawnPoints[Random.Range(0, spawnPoints.Count)]);
                            }
                        }
                    }

                    WaveData obj = rounds[(int)(roundNumber % rounds.Count)].waves[(int)waveNumber];
                    SpawnEverything(obj, obj.timeBetweenObjects, points.ToArray(), (int)(roundNumber / rounds.Count));

                    points.Clear();
                }
            }
        }
    }

    float SpawnEverything(WaveData wave, float offset, Transform[] spawnPoints, int roundsRepetition = 0)
    {
        float time = 0.0f;

        List<GameObject> objects = new();

        int z = 0;

        foreach (WavePrefab prefab in wave.enemies)
        {
            for (int i = 0; i < prefab.count; ++i)
            {
                objects.Add(prefab.prefab);
            }
        }

        int additional = roundsRepetition * (int)duplicatedRoundsFactor;

        for (int i = 0; i < additional; ++i)
        {
            objects.Add(wave.enemies[i % wave.enemies.Count].prefab);
        }

        while (objects.Count > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            int pos = Random.Range(0, objects.Count);

            if (z == 0)
            {
                Instantiate(objects[pos], point.position, Quaternion.identity);
                z = 1;
            }
            else
            {
                StartCoroutine(Spawn(objects[pos], point, time));
            }

            objects.RemoveAt(pos);

            time += offset;
        }

        spawned = true;

        return time;
    }

    IEnumerator Spawn(GameObject prefab, Transform tran, float time)
    {
        yield return new WaitForSeconds(time);
        while (GameTimer.TimeMultiplier == GameTimer.STOPPED)
        {
            continue;
        }
        Instantiate(prefab, tran.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
    }

    void PlayNextRound()
    {
        playNextRound = true;
        active = false;
        pressButtonTxt.gameObject.SetActive(false);
        //startRoundButton.gameObject.SetActive(false);
    }
}
