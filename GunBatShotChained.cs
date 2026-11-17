using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBatShotChained : MonoBehaviour
{
    const float SPEED = 1.8f;
    public GameObject HitBox;
    public GameObject SlamHitBox;
    public GameObject ChainHitBox;
    public LayerMask Enemies;
    public LayerMask Interactables;

    Animator ANIM;

    bool Started;
    bool Animated;
    bool Destroyed;

    RaycastHit2D HitCast;
    RaycastHit2D[] SlamCast;
    GameObject previousHit;
    GameObject player;
    float chainLength;
    Damage impactDmg;

    private void Start()
    {
        ANIM = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Started && !Destroyed)
        {
            if(Vector2.Distance(player.transform.position, transform.position) < chainLength - 1)
            {
                transform.position += transform.right * SPEED;

                HitCast = Physics2D.BoxCast(HitBox.transform.position, HitBox.transform.localScale, 0, transform.up, 0, Enemies);
                if (HitCast && previousHit != HitCast.transform.gameObject)
                {
                    previousHit = HitCast.transform.gameObject;
                    
                    HitCast.transform.GetComponentInParent<IEnemy>().GotHit(impactDmg);
                }
            }
            else if(!Animated)
            {
                Animated = true;
                ANIM.Play("GunBatSlamWindUp");
            }
        }
    }

    public void ChainInfo(GameObject Player, float ChainLength, Damage ImpactDmg)
    {
        player = Player;
        chainLength = ChainLength;
        Started = true;
        impactDmg = ImpactDmg;
    }

    public void Slam(Damage dmg)
    {
        ANIM.Play("GunBatSlam");
        

        SlamCast = Physics2D.BoxCastAll(SlamHitBox.transform.position, SlamHitBox.transform.localScale, 0, transform.up, 0, Enemies);
        for (int i= 0; i < SlamCast.Length; i++)
        {
            SlamCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(dmg.DmgAmount, dmg.StunDuration, dmg.Source));
        }
        StartCoroutine(PullDelay(dmg));
    }

    public void Explode()
    {

        //player.GetComponent<PlayerMovement>().StartGunbatSpinning(transform.position, (int)Vector2.Distance(transform.position, player.transform.position));
        StartCoroutine(DestroyDelay());
    }

    IEnumerator PullDelay(Damage dmg)
    {
        yield return new WaitForSeconds(0.5f);
        SlamCast = Physics2D.BoxCastAll(SlamHitBox.transform.position, SlamHitBox.transform.localScale, 0, transform.up, 0, Enemies);
        for (int i = 0; i < SlamCast.Length; i++)
        {
            SlamCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(0, 0, dmg.Source, (player.transform.position - transform.position), 0.25f, 3));
        }
        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        Destroyed = true;
        yield break;
    }
    
}
