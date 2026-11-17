using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionSpot : MonoBehaviour
{
    LayerMask PlayerOnly;

    float ConfuseTime = 2;

    float RayCastX = 0; // Remember, these are set to object's scale, so set publically if scale needs to be different later.
    float RayCastY = 0;

    private void Awake()
    {
        PlayerOnly = LayerMask.GetMask("Player");
        RayCastX = transform.localScale.x;
        RayCastY = transform.localScale.y; 
    }


    void Update()
    {
        RaycastHit2D SlamHit = Physics2D.BoxCast(transform.position, new Vector2(RayCastX, RayCastY), 0, transform.up, 0, PlayerOnly);
        if (SlamHit)
        {
            SlamHit.transform.SendMessage("ConfuseCountSet", ConfuseTime);
        }
    }
}
