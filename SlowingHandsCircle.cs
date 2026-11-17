using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingHandsCircle : BasicEnemy
{
    public GameObject HB;
    public float SLOWCOUNT;
    public float SLOWAMOUNT;
    RaycastHit2D PlayerCast;
    // Update is called once per frame
    void Update()
    {
        PlayerCast = CastHitCircle(HB);
        if (PlayerCast)
        {
            Player.GetComponent<PlayerMovement>().SlowCountSet(SLOWCOUNT, SLOWAMOUNT);
        }
    }
}
