using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBatShot : MonoBehaviour
{
    float ProjectileSpeed = 1.8f;
    public GameObject HitBox;
    public LayerMask Enemies;
    public LayerMask Interactables;
    int TimeUp = 0;
    bool Initialized = false;
    Damage impactDmg;
    RaycastHit2D HitCast;
    

    public void GunBatInfo(float Speed, Damage dmg)
    {
        ProjectileSpeed = Speed;
        impactDmg = dmg;
        Initialized = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Initialized)
        {
            transform.position += transform.right * ProjectileSpeed;

            HitCast = Physics2D.BoxCast(HitBox.transform.position, HitBox.transform.localScale, 0, transform.up, 0, Enemies);
            if (HitCast)
            {


                HitCast.transform.GetComponentInParent<IEnemy>().GotHit(impactDmg);
                Destroy(gameObject);
            }
            if (TimeUp > 10)
            {
                Destroy(gameObject);
            }

            TimeUp++;
        }
    }

    
   
    
}
