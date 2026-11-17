using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTV : MonoBehaviour
{
    float EndingYPos = -25;
    float ActivateYPos = -25;

    float YPosition;
    public GameObject HitBox;
    public GameObject Shadow;
    LayerMask PlayerLayer;
    PlayerMovement PM;
    SpriteRenderer SR;
    GameObject myShadow;
    protected bool stopHurt;
    float TimeAlive = 0;
    AudioSource AS;
    Animator anim;

    public float YActiveOffset = 2;
    public float Speed = 0.125f;
    public float YShadowOffset = 0.5f;
    public float Damage = 25;

    public bool StaticTV;
    public GameObject StaticHB;

    BoxCollider2D BC;


    public virtual void Start()
    {
        PlayerLayer = MasterMind.GetPlayerLayer();
        SR = GetComponent<SpriteRenderer>();
        PM = MasterMind.GetPlayer().GetComponent<PlayerMovement>();
        AS = GetComponent<AudioSource>();
        BC = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        if(Random.value > 0.5f)
        {
            SR.flipX = true;
        }
        else
        {
            SR.flipX = false;
        }

        if (StaticTV)
        {
            SR.color = Color.red;
        }
    }

    public virtual void Update()
    {
        YPosition = transform.position.y;

        if(YPosition <= ActivateYPos && !stopHurt)
        {
            if(Physics2D.BoxCast(HitBox.transform.position, HitBox.transform.localScale, 0, transform.up, 0, PlayerLayer))
            {
                PM.GotHit(new Damage(25, gameObject));
            }
        }
        
        TimeAlive += Time.deltaTime;
        if(TimeAlive >= 25)
        {
            StartCoroutine(Die());
            TimeAlive = 0;
        }

        if (StaticTV && stopHurt)
        {
            StaticHB.GetComponent<SpriteRenderer>().enabled = true;
            if(MasterMind.HitBoxToCast(StaticHB, PlayerLayer))
            {
                PM.GotHit(new Damage(2, true, gameObject, DamageType.Zone));
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (!stopHurt)
        {
            transform.position -= transform.up * Speed;
        }
        if (YPosition <= EndingYPos && !stopHurt)
        {
            Destroy(myShadow);
            stopHurt = true;
            SR.sortingOrder = -1;
            AS.Play();
            anim.Play("FallingTVLand");
            BC.enabled = true;
        }
    }

    protected IEnumerator Die()
    {
        anim.Play("FallingTVDie");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public void SetPoss(float EndPos)
    {
        EndingYPos = EndPos;
        ActivateYPos = EndPos + YActiveOffset;
        myShadow = Instantiate(Shadow, new Vector2(transform.position.x, EndPos - YShadowOffset), Quaternion.identity);
    }
}
