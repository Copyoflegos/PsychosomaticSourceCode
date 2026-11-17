using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IEnemy
{
    //This class is the class for basic enemy behavior that other enemies will inherit from
    CameraControl CC;
    public LayerMask NotPlayer;
    public LayerMask PlayerMask;

    protected Animator ANIM;
    protected AudioSource AS;

    protected GameObject Player;

    int stunTime = 0;
    protected bool IsStunned;

    Vector2 KnockBackDir, MoveDir;
    int KnockBackDur, MoveDur;
    float KnockBackSpeed, MoveSpeed;

    RaycastHit2D RightCast, LeftCast, TopCast, BotCast;
    public Transform[] ColliderPoints;
    Vector2 TransformAdd = Vector2.zero;
    public float Health = 99999;
    public bool IgnoreCollision;

    public void Start()
    {
        CC = Camera.main.GetComponent<CameraControl>();
        AS = GetComponent<AudioSource>();
        ANIM = GetComponent<Animator>();
        Player = MasterMind.GetPlayer();
        
    }
    public virtual void FixedUpdate()
    {
        if(stunTime > 0)
        {
            stunTime--;
            ANIM.SetBool("Stunned", true);
            IsStunned = true;
        }
        else
        {
            ANIM.SetBool("Stunned", false);
            IsStunned = false;
        }


        if(KnockBackDur > 0)
        {
            TransformAdd += KnockBackDir * KnockBackSpeed;
            KnockBackDur--;
        }
        if(MoveDur > 0)
        {
            TransformAdd += MoveDir * MoveSpeed;
            MoveDur--;
        }

        if (!IgnoreCollision)
        {
            RightCast = Physics2D.Linecast(ColliderPoints[0].position, ColliderPoints[1].position, NotPlayer);
            BotCast = Physics2D.Linecast(ColliderPoints[1].position, ColliderPoints[2].position, NotPlayer);
            LeftCast = Physics2D.Linecast(ColliderPoints[2].position, ColliderPoints[3].position, NotPlayer);
            TopCast = Physics2D.Linecast(ColliderPoints[3].position, ColliderPoints[0].position, NotPlayer);

            float PushValue = 0f;

            if (TopCast && TransformAdd.y > 0)
            {
                TransformAdd = new Vector2(TransformAdd.x, -PushValue);
                Collided();
            }
            if (BotCast && TransformAdd.y < 0)
            {
                TransformAdd = new Vector2(TransformAdd.x, PushValue);
                Collided();
            }
            if (LeftCast && TransformAdd.x < 0)
            {
                TransformAdd = new Vector2(PushValue, TransformAdd.y);
                Collided();
            }
            if (RightCast && TransformAdd.x > 0)
            {
                TransformAdd = new Vector2(-PushValue, TransformAdd.y);
                Collided();
            }
        }
        

        
       

        if (TransformAdd != Vector2.zero)
        {
            ANIM.SetBool("Walking", true);
        }
        else
        {
            ANIM.SetBool("Walking", false);
        }

        transform.position += (Vector3)TransformAdd;
        TransformAdd = Vector2.zero;

        if(Health <= 0)
        {
            Die();
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    public virtual void Die()
    {
        print("DEATH BY BASE CLASS");
    }

    public void MoveDirection(Vector2 Dir, float Speed, int Dur)
    {
        MoveDir = Dir;
        MoveSpeed = Speed;
        MoveDur = Dur;
    }

    public RaycastHit2D CastHitCircle(GameObject HitBox, LayerMask MaskToCast)
    {
        return Physics2D.CircleCast(HitBox.transform.position, HitBox.transform.lossyScale.x / 2, transform.up, 0, MaskToCast);
    }
    public RaycastHit2D CastHitCircle(GameObject HitBox)
    {
        return Physics2D.CircleCast(HitBox.transform.position, HitBox.transform.lossyScale.x / 2, transform.up, 0, PlayerMask);
    }
    public RaycastHit2D CastHitBox(GameObject HitBox, LayerMask MaskToCast)
    {
        return Physics2D.BoxCast(
                HitBox.transform.position,
                HitBox.transform.lossyScale,
                HitBox.transform.rotation.eulerAngles.z,
                -HitBox.transform.up, 0, MaskToCast);
    }
    public RaycastHit2D CastHitBox(GameObject HitBox)
    {
        return Physics2D.BoxCast(
                HitBox.transform.position,
                HitBox.transform.lossyScale,
                HitBox.transform.rotation.eulerAngles.z,
                -HitBox.transform.up, 0, PlayerMask);
    }
    public RaycastHit2D[] CastHitBoxAll(GameObject HitBox, LayerMask MaskToCast)
    {
        return Physics2D.BoxCastAll(
                HitBox.transform.position,
                HitBox.transform.lossyScale,
                HitBox.transform.rotation.eulerAngles.z,
                -HitBox.transform.up, 0, MaskToCast);
    }

    public virtual void GotHit(Damage dmg)
    {
        //AS.Play();
        if (dmg.DmgAmount > 3)
            CC.Shake(0.25f, 4);
        else
            CC.Shake(0.01f, 1);

        Health -= dmg.DmgAmount;

        if(dmg.StunDuration > 0)
        {
            stunTime = dmg.StunDuration;
        }
        if(dmg.FlinchDuration > 0)
        {
            stunTime = dmg.FlinchDuration;
        }

        if (dmg.KnockBack != Vector2.zero)
        {
            KnockBackDir = dmg.KnockBack;
            KnockBackDur = dmg.KBDuration;
            KnockBackSpeed = dmg.KBSpeed;
        }
    }


    public bool GetFlinchable()
    {
        return true;
    }

    public bool GetStunned()
    {
        return IsStunned;
    }

    public void GotBleed(int dur, float dmg)
    {

    }
    public void GotStapled(int num)
    {

    }
    public void GotBurned(int num)
    {

    }
    public void Reset()
    {
        
    }

    protected virtual void SideSwitch() // makes the enemy face towards the player
    {
        float playerDifX = Mathf.Abs(transform.position.x - Player.transform.position.x);
        float playerDifY = Mathf.Abs(transform.position.y - Player.transform.position.y);
        if (playerDifX > playerDifY) // if more horizontal difference then default to facing L or R
        {
            if (transform.position.x - Player.transform.position.x < 0) // facing Right
            {
                AnimationSwitch(4);
            }
            else // facing left
            {
                AnimationSwitch(2);
            }
        }
        else
        {
            if (transform.position.y - Player.transform.position.y < 0) // facing up
            {
                AnimationSwitch(3);
            }
            else // facing down
            {
                AnimationSwitch(1);
            }

        }
    }

    protected  string GetFaceDir()
    {
        float playerDifX = Mathf.Abs(transform.position.x - Player.transform.position.x);
        float playerDifY = Mathf.Abs(transform.position.y - Player.transform.position.y);
        if (playerDifX > playerDifY) // if more horizontal difference then default to facing L or R
        {
            if (transform.position.x - Player.transform.position.x < 0) // facing Right
            {
                return "RIGHT";
            }
            else // facing left
            {
                return "LEFT";
            }
        }
        else
        {
            if (transform.position.y - Player.transform.position.y < 0) // facing up
            {
                return "UP";
            }
            else // facing down
            {
                return "DOWN";
            }

        }
    }

    protected virtual void Collided()
    {
        return;
    }

    protected void AnimationSwitch(int IndexSet)
    {
        for (int i = 0; i < ANIM.layerCount; i++)
        {
            ANIM.SetLayerWeight(i, 0);
        }
        ANIM.SetLayerWeight(IndexSet, 1);
    }
}
