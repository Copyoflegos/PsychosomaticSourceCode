using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBLIGHT : MonoBehaviour
{
    SpriteRenderer SR;

    Color32 BR = new Color(1, 0, 0);
    Color32 BB = new Color(0, 0, 1);
    Color32 BG = new Color(0, 1, 0);

    int FrameCount = 0;
    int FRAMEMAX = 75;
    int ColorCount = 0;
    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(FrameCount >= FRAMEMAX)
        {
            switch (ColorCount)
            {
                case 0:
                    SR.color = BR;
                    break;
                case 1:
                    SR.color = BB;
                    break;
                case 2:
                    SR.color = BG;
                    ColorCount = -1;
                    break;
            }
            ColorCount++;
            FrameCount = 0;
        }
        FrameCount++;
    }
}
