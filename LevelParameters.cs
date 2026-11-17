using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelParameters : MonoBehaviour
{
    public bool WaterLevel;
    public bool BossLevel;
    // Start is called before the first frame update
    void Awake()
    {
        MasterMind.SetWaterLevel(WaterLevel);
        MasterMind.SetBossLevel(BossLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
