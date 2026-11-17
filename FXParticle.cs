using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXParticle : MonoBehaviour
{
    int timer = 0;
    public int MaxTime = 25;
    private void FixedUpdate()
    {
        timer++;
        if(timer > MaxTime)
        {
            Destroy(gameObject);
        }
    }
}
