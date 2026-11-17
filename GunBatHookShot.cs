using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBatHookShot : MonoBehaviour
{
    float ProjectileSpeed = 0.85f; // was 1.8
    public GameObject HitBox;
    
    public LayerMask Interactables;
    int TimeUp = 0;
    bool Initialized = false;
    Damage impactDmg;
    RaycastHit2D HitCast;

    GameObject Player;

    public void GunBatInfo(GameObject player)
    {
        Player = player;
        Initialized = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Initialized)
        {
            transform.position += transform.up * ProjectileSpeed;
        }

        HitCast = Physics2D.BoxCast(HitBox.transform.position, HitBox.transform.localScale, 0, transform.up, 0, Interactables);

        if (HitCast)
        {
            Player.GetComponent<PlayerMovement>().HookShotGrab(transform.position);
            
            Destroy(gameObject);
        }

        TimeUp++;
        if(TimeUp > 10)
        {
            Destroy(gameObject);
        }
    }
}
