using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class funnyfloor : MonoBehaviour
{
    SpriteRenderer SR;

    float HorMultiple =1, VerMultiple=-1;
    int RandomChance;

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RandomChance = Random.Range(1, 10000);
        if(RandomChance >= 9990)
        {
            if(RandomChance % 2 == 0)
            {
                HorMultiple = 1;
                VerMultiple = -1;
            }
            
            else
            {
                HorMultiple = -1;
                VerMultiple = 1;
            }
        }

        if(SR.size.x < 50)
        {
            SR.size = new Vector2(50, SR.size.y);
        }
        if (SR.size.y < 50)
        {
            SR.size = new Vector2(SR.size.x, 50);
        }
        SR.size = new Vector2(SR.size.x + 0.01f * HorMultiple, SR.size.y + 0.01f * VerMultiple);
    }
}
