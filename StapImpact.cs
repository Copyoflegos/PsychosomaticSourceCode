using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StapImpact : MonoBehaviour
{
    int dieFrame = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dieFrame == 5)
        {
            Destroy(gameObject);
        }
        dieFrame++;
    }
}
