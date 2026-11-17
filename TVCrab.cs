using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVCrab : BasicEnemy
{
    public float SHOOTRANGE = 8;
    public float FREEZERANGE = 1.5f;
    public float WAKEUPRANGE = 10;
    public float GAPRANGE = 20; // if too far away walk in
    public float WALKSPEED = 0.03f;
    public float RUNSPEED = 0.15f;

    bool Sleeping = true;
    bool IdleWalkLocked = false;
    bool ShootingLocked = false;

    public GameObject ProjectilePrefab;
    public GameObject  LaserHB, LaserGround, LaserSPR;
    [Space(10)]
    public GameObject HeadPrefab, BodyPrefab;
    public float HeadSpeed, BodySpeed;
    public int HeadDur, BodyDur;

    RaycastHit2D LaserCast;
    
    float playerDifX, playerDifY;
   
    [Space(10)]
    public bool BABY_MODE;
    public bool MANIC_MODE;

    private void Awake()
    {
        if (MANIC_MODE)
        {
            Health = 200;
        }
        if (BABY_MODE)
        {
            Health = 0.25f;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Sleeping && Vector2.Distance(transform.position, Player.transform.position) < WAKEUPRANGE && !Physics2D.Linecast(transform.position, Player.transform.position, NotPlayer))
        {
            Sleeping = false;
        }

        SideSwitch();
    }

    public override void FixedUpdate()
    {
        if (!ShootingLocked && Sleeping && !IdleWalkLocked)
        {
            int randomRoll = Random.Range(0, 8);
            switch (randomRoll)
            {
                case 0:
                    MoveDirection(new Vector2( 0,  1), WALKSPEED, 999); // U
                    break;
                case 1:
                    MoveDirection(new Vector2( 0, -1), WALKSPEED, 999); // D
                    break;
                case 2:
                    MoveDirection(new Vector2(-1,  0), WALKSPEED, 999); // L
                    break;
                case 3:
                    MoveDirection(new Vector2( 1,  0), WALKSPEED, 999); // R
                    break;
                case 4:
                    MoveDirection(new Vector2( 1,  1), WALKSPEED, 999); // UR
                    break;
                case 5:
                    MoveDirection(new Vector2( 1, -1), WALKSPEED, 999); // DR
                    break;
                case 6:
                    MoveDirection(new Vector2(-1, -1), WALKSPEED, 999); // DL
                    break;
                case 7:
                    MoveDirection(new Vector2(-1,  1), WALKSPEED, 999); // UL
                    break;
            }
            IdleWalkLocked = true;
            StartCoroutine(IdleWalkTimer());
        }

        float PlayerDist = Vector2.Distance(transform.position, Player.transform.position);

        if (!ShootingLocked && !Sleeping && PlayerDist > FREEZERANGE && PlayerDist < SHOOTRANGE )
        {
            ANIM.SetBool("Scared", false);
            MoveDirection((transform.position - Player.transform.position).normalized, RUNSPEED, 10000);
        }
        else if (!ShootingLocked && !Sleeping && Vector2.Distance(transform.position, Player.transform.position) < FREEZERANGE)
        {
            ANIM.SetBool("Scared", true);
            MoveDirection(Vector2.zero, 0, 0);
        }
        else if (!ShootingLocked && !Sleeping && !BABY_MODE && PlayerDist < GAPRANGE)
        {
            ANIM.SetBool("Scared", false);
            ShootingLocked = true;
            if (MANIC_MODE)
            {
                StartCoroutine(ManicShoot());
            }
            else
            {
                StartCoroutine(Shoot());
            }
            
        }
        else if(!ShootingLocked && !Sleeping)
        {
            ANIM.SetBool("Scared", false);
            MoveDirection((Player.transform.position - transform.position).normalized, RUNSPEED, 10000);
        }

        base.FixedUpdate();
    }
    IEnumerator Shoot()
    {
        MoveDirection(Vector2.zero, 0, 0);
        ANIM.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.75f);

        GameObject Bullet = Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(Player.transform.position - transform.position));
        Bullet.GetComponent<TvCrabProjectile>().ProjectileInfo(Player.transform.position - transform.position);
        yield return new WaitForSeconds(0.25f);

        ANIM.SetBool("Shoot", false);
        ShootingLocked = false;
    }

    IEnumerator ManicShoot()
    {
        MoveDirection(Vector2.zero, 0, 0);
        ANIM.SetBool("Shoot", true);
        Vector2 ShootDir = Player.transform.position - transform.position;
        LaserGround.transform.up = -ShootDir;
        LaserGround.SetActive(true);
        yield return new WaitForSeconds(0.75f);

        //LaserHB.transform.up = -ShootDir;
        LaserSPR.transform.up = -ShootDir;
        LaserSPR.SetActive(true);
        LaserCast = CastHitBox(LaserHB);
        if (LaserCast)
        {
            Player.GetComponent<PlayerMovement>().GotHit(new Damage(50, gameObject));
        }
        yield return new WaitForFixedUpdate();
        LaserGround.SetActive(false);
        yield return new WaitForFixedUpdate();
        LaserSPR.SetActive(false);
        yield return new WaitForSeconds(0.25f);

        ANIM.SetBool("Shoot", false);
        ShootingLocked = false;
    }

    IEnumerator IdleWalkTimer()
    {
        yield return new WaitForSeconds(0.35f);
        MoveDirection(Vector2.zero, 0, 0);
        yield return new WaitForSeconds(0.25f);
        IdleWalkLocked = false;
    }

    public override void Die()
    {
        Instantiate(HeadPrefab, transform.position, Quaternion.identity). GetComponent<Bodypart>().BodyPartInfo(HeadSpeed, HeadDur, -(Player.transform.position - transform.position).normalized, GetFaceDir());
        Instantiate(BodyPrefab, transform.position, Quaternion.identity).GetComponent<Bodypart>().BodyPartInfo(BodySpeed, BodyDur, -(Player.transform.position - transform.position).normalized, GetFaceDir());
    }

    public override void GotHit(Damage dmg)
    {
        
        if (dmg.DmgAmount < 3)
        {
            dmg.DmgAmount = 0.25f;
        }
        

        base.GotHit(dmg);

    }
    
    
}
