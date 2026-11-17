using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunExplosion : MonoBehaviour
{
    int CountDown = 50;
    

    // Update is called once per frame
    void Update()
    {
        CountDown--;
        if(CountDown < 0)
        {
            Destroy(gameObject);
        }
    }
}
