using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TvCrabProjectile : MonoBehaviour
{

    const float SPEED = 0.25f;
    public GameObject HITBOX;

    RaycastHit2D HitCast;
    public LayerMask PlayerMask;
    Vector2 Direction;
    Animator ANIM;

    int timer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        ANIM = GetComponent<Animator>();
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (Vector3)Direction * SPEED;

        HitCast = Physics2D.BoxCast(transform.position, HITBOX.transform.localScale, 0, transform.up, 0, PlayerMask);
        if (HitCast)
        {
            if(HitCast.transform.GetComponent<PlayerMovement>().GotHit(new Damage(25, gameObject)))
            {
                Destroy(gameObject);
            }
            
        }
        timer++;
        if(timer > 300)
        {
            Destroy(gameObject);
        }
    }

    public void ProjectileInfo(Vector2 dir)
    {
        Direction = dir.normalized;

        float X = Mathf.Abs(Direction.x);
        float Y = Mathf.Abs(Direction.y);
        if (X > Y) // if more horizontal difference then default to facing L or R
        {
            if (Direction.x > 0) // facing Right
            {
                ANIM.Play("CrabProjectileR");
            }
            else // facing left
            {
                ANIM.Play("CrabProjectileL");
            }
        }
        else
        {
            if (Direction.y > 0) // facing up
            {
                ANIM.Play("CrabProjectileU");
            }
            else // facing down
            {
                ANIM.Play("CrabProjectileD");
            }

        }
    }
}
