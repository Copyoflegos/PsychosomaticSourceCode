using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KelpSpawner : MonoBehaviour
{
    public bool Started;
    bool Delay;
    float Rate = 0.5f; // lower is faster
    public GameObject Kelp;
    public Transform[] SpawnLimits = new Transform[2];
    public KelpTree[] SpawnParents;

    // Update is called once per frame
    void Update()
    {
        if (Started && !Delay)
        {
            StartCoroutine(ParentSpawnDelay());
        }
    }
    IEnumerator SpawnDelay()
    {
        Delay = true;
        yield return new WaitForSeconds(Rate);
        float RandomX = Random.Range(SpawnLimits[0].position.x, SpawnLimits[1].position.x);
        float RandomY = Random.Range(SpawnLimits[0].position.y, SpawnLimits[1].position.y);
        Instantiate(Kelp, new Vector3(RandomX, RandomY), Quaternion.identity);
        Delay = false;
    }
    IEnumerator ParentSpawnDelay()
    {
        Delay = true;
        yield return new WaitForSeconds(Rate);
        for (int i = 0; i < SpawnParents.Length; i++)
        {
            if (SpawnParents[i].RoomForChildren())
            {
                SpawnParents[i].AddChild(Kelp);
                yield return new WaitForSeconds(0.05f); // a delay so they don't all spawn in at once
            }
        }
        Delay = false;
    }
    public void StartSpawning()
    {
        Started = true;
    }
    public void StartSpawning(float rate)
    {
        Started = true;
        Rate = rate;
    }
    public void StopSpawning()
    {
        Started = false;
    }
}
