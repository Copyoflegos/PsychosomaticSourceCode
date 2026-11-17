using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeWave : MonoBehaviour
{
    float Speed = 0.4f;

    RaycastHit2D playerCast;

    int timer = 0;

    public GameObject HB;

    private void FixedUpdate()
    {
        transform.position += transform.up * Speed;

        if(timer > 10)
        {
            playerCast = MasterMind.HitBoxToCast(HB, MasterMind.GetPlayerLayer());
            if (playerCast)
            {
                playerCast.transform.GetComponent<PlayerMovement>().GotHit(new Damage(50, true, gameObject, DamageType.Projectile));
            }
        }
        



        timer++;
        if(timer > 250)
        {
            Destroy(gameObject);
        }
    }
}
