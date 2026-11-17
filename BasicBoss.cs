using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class BasicBoss : MonoBehaviour, IEnemy
{
    //This class is the class for basic boss behavior that other bosses will inherit from
    CameraControl CC;
    public LayerMask NotPlayer;
    public LayerMask PlayerMask;

    public Animator ANIM;
    public AudioSource AS;

    protected GameObject Player;
    protected PlayerMovement PM;

    int stunTime = 0;
    protected bool IsStunned;
    protected bool IsAttacking;
    protected bool IsFlinchable;
    protected bool IsParryable;
    protected bool StaplerVulnerable;
    protected bool HasCollided;

    Vector2 KnockBackDir, MoveDir;
    int KnockBackDur, MoveDur;
    float KnockBackSpeed, MoveSpeed;

    RaycastHit2D RightCast, LeftCast, TopCast, BotCast;
    public Transform[] ColliderPoints; // 0-1 top, 2-3 right, 4-5 bot, 6-7 left
    Vector2 TransformAdd = Vector2.zero;


    protected float Health;
    public float[] PhaseHealths;
    public bool IgnoreCollision;


    // Move variables

    protected enum FacingDirection
    {
        Down,
        Left,
        Up,
        Right
    }
    protected FacingDirection FD; // what direction is addiction facing

    protected List<int> possibleAttacks = new List<int>();

    protected int TotalMoveCount = 0; // goes up everytime a move happens
    private int randomMoveRoll;

    public virtual void Start()
    {
        CC = Camera.main.GetComponent<CameraControl>();
        Player = MasterMind.GetPlayer();
        PM = Player.GetComponent<PlayerMovement>();
        Health = PhaseHealths[0];

        
    }

    public virtual void FixedUpdate()
    {
        if (KnockBackDur > 0)
        {
            TransformAdd += KnockBackDir * KnockBackSpeed;
            KnockBackDur--;
        }
        if (MoveDur > 0)
        {
            
            TransformAdd += MoveDir * MoveSpeed;
            MoveDur--;

        }

        if (!IgnoreCollision)
        {
            int MovementPerVelocityX = Mathf.Abs((int)(TransformAdd.x / 0.001f));  // Each mm is equal to a tenth of a unit and each tenth collision is checked
            int MovementPerVelocityY = Mathf.Abs((int)(TransformAdd.y / 0.001f));
            RaycastHit2D hitObject = LeftCast;
            
            for (int i = 0; i < MovementPerVelocityX; i++) // For the x axis
            {
                LeftCast = Physics2D.Linecast(ColliderPoints[6].position, ColliderPoints[7].position, NotPlayer);
                RightCast = Physics2D.Linecast(ColliderPoints[2].position, ColliderPoints[3].position, NotPlayer);
                if (!(LeftCast && Mathf.Sign(TransformAdd.x) < 0) && !(RightCast && Mathf.Sign(TransformAdd.x) > 0)) // checks if you're moving towards a wall and stops you if you are
                {
                    transform.position += new Vector3(0.001f * Mathf.Sign(TransformAdd.x), 0);
                    
                }
                else
                {
                    MovementPerVelocityX = 0;
                    Collided();
                }

            }
            for (int i = 0; i < MovementPerVelocityY; i++) // For the y axis
            {
                TopCast = Physics2D.Linecast(ColliderPoints[0].position, ColliderPoints[1].position, NotPlayer);
                BotCast = Physics2D.Linecast(ColliderPoints[4].position, ColliderPoints[5].position, NotPlayer);
                if (!(TopCast && Mathf.Sign(TransformAdd.y) > 0) && !(BotCast && Mathf.Sign(TransformAdd.y) < 0))
                {
                    transform.position += new Vector3(0, 0.001f * Mathf.Sign(TransformAdd.y));
                    
                }
                else
                {
                    MovementPerVelocityY = 0;
                    Collided();
                }
            }
        }

        
        TransformAdd = Vector2.zero;

        if (Health <= 0)
        {
            NextPhase();
        }
    }

    public virtual void NextPhase()
    {
        Debug.LogError("NEXT PHASE NOT IMPLEMENTED TO DERIVED CLASS");
    }

    protected void AddAttackPoss(int MI, int[,] MoveDict, int[] MoveCDList, int[] MoveREPEATList)
    {
        if (TotalMoveCount >= MoveCDList[MI] && MoveREPEATList[MI] < MoveDict[MI,2])
        {
            int Chance = MoveDict[MI,0];

            for (int i = 0; i < Chance; i++)
            {
                possibleAttacks.Add(MI);
            }
        }
    }

    protected int RollForAttack(int[,] MoveDict, int[] MoveCDList, int[] MoveREPEATList)
    {
        if (possibleAttacks.Count > 0)
        {
            randomMoveRoll = Random.Range(0, possibleAttacks.Count);

            int moveSelector = possibleAttacks[randomMoveRoll];

            MoveCDList[moveSelector] = TotalMoveCount + MoveDict[moveSelector,1] + 1;

            for (int i = 0; i < MoveREPEATList.Length; i++) // cleanses all non-repeats
            {
                if (i == moveSelector)
                {
                    MoveREPEATList[i]++;
                }
                else
                {
                    MoveREPEATList[i] = 0;
                }
            }

            possibleAttacks.Clear();

            return moveSelector;
        }
        else
        {
            //Debug.LogError("NO POSSIBLE MOVES");
            return -1;
        }
    }

    protected virtual void Collided()
    {
        
        HasCollided = true;
        return;
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
    public RaycastHit2D[] CastHitBoxAll(GameObject HitBox)
    {
        return Physics2D.BoxCastAll(
                HitBox.transform.position,
                HitBox.transform.lossyScale,
                HitBox.transform.rotation.eulerAngles.z,
                -HitBox.transform.up, 0, PlayerMask);
    }

    public void MoveDirection(Vector2 Dir, float Speed, int Dur)
    {
        HasCollided = false;
        MoveDir = Dir.normalized;
        MoveSpeed = Speed;
        MoveDur = Dur;

    }
    public void MoveDirection(Vector2 Dir, float Speed)
    {
        
        MoveDirection(Dir, Speed, 99999);
    }

    protected virtual void SetFaceToPlayer() // makes the enemy face towards the player
    {
        float playerDifX = Mathf.Abs(transform.position.x - Player.transform.position.x);
        float playerDifY = Mathf.Abs(transform.position.y - Player.transform.position.y);
        if (playerDifX > playerDifY) // if more horizontal difference then default to facing L or R
        {
            if (transform.position.x - Player.transform.position.x < 0) // facing Right
            {
                SetFaceTo(FacingDirection.Right);
            }
            else // facing left
            {
                SetFaceTo(FacingDirection.Left);
            }
        }
        else
        {
            if (transform.position.y - Player.transform.position.y < 0) // facing up
            {
                SetFaceTo(FacingDirection.Up);
            }
            else // facing down
            {
                SetFaceTo(FacingDirection.Down);
            }

        }
    }

    protected virtual void SetFaceToObject(GameObject Object) // makes the enemy face towards the player
    {
        float objectDifX = Mathf.Abs(transform.position.x - Object.transform.position.x);
        float objectDifY = Mathf.Abs(transform.position.y - Object.transform.position.y);
        if (objectDifX > objectDifY) // if more horizontal difference then default to facing L or R
        {
            if (transform.position.x - Object.transform.position.x < 0) // facing Right
            {
                SetFaceTo(FacingDirection.Right);
            }
            else // facing left
            {
                SetFaceTo(FacingDirection.Left);
            }
        }
        else
        {
            if (transform.position.y - Object.transform.position.y < 0) // facing up
            {
                SetFaceTo(FacingDirection.Up);
            }
            else // facing down
            {
                SetFaceTo(FacingDirection.Down);
            }

        }
    }

    public virtual void SetFaceToPosition(Vector3 Position) // Makes addiction turn to a position
    {
        float YDif = Mathf.Abs(transform.position.y - Position.y);
        float XDif = Mathf.Abs(transform.position.x - Position.x);

        if (transform.position.y > Position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Down);

        }
        if (transform.position.y < Position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Up);
        }
        if (transform.position.x < Position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Right);
        }
        if (transform.position.x > Position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Left);
        }
    }

    protected virtual void SetFaceTo(FacingDirection Direction)
    {
        AnimationSwitch((int)Direction + 1);
        FD = Direction;
    }

    protected Vector2 FaceToDir(FacingDirection Direction)
    {
        switch (Direction)
        {
            case FacingDirection.Down:
                return -transform.up;
            case FacingDirection.Up:
                return transform.up;
            case FacingDirection.Left:
                return -transform.right;
            case FacingDirection.Right:
                return transform.right;
            default:
                return Vector2.zero;
        }
    }

   


    public virtual void GotHit(Damage dmg)
    {
        //AS.Play();
        if (dmg.DmgAmount > 3)
            CC.Shake(0.25f, 4);
        else
            CC.Shake(0.01f, 1);

        Health -= dmg.DmgAmount;

        if (dmg.StunDuration > 0)
        {
            stunTime = dmg.StunDuration;
        }
        if (dmg.FlinchDuration > 0)
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
        return IsFlinchable;
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

    protected void AnimationSwitch(int IndexSet)
    {
        for (int i = 0; i < ANIM.layerCount; i++)
        {
            ANIM.SetLayerWeight(i, 0);
        }
        ANIM.SetLayerWeight(IndexSet, 1);
    }
    public virtual void PlayIdleAnimation()
    {
        Debug.LogError("PLAY IDLE ANIMATION NOT IMPLEMENTED IN CLASS");
    }


    protected IEnumerator WaitForFrames(int Frames) // probably won't use this seconds is more conveniant with the animations, even if player uses frames
    {
        for(int i = 0; i < Frames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }
}
