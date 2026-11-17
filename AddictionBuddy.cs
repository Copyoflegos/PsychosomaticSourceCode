using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionBuddy : MonoBehaviour
{
    PlayerMovement PM;
    GameObject Player;
    Vector3 MovementOffset;
    Vector3 FloatOffset;
    AimDirKey CurrentDir;
    public GameObject ChargePunchHB;
    SpriteRenderer ChargePunchSR;
    public GameObject SmallPunchHB;
    SpriteRenderer SmallPunchSR;
    public GameObject BarrageHB;
    SpriteRenderer BarrageSR;
    SpriteRenderer SR;
    public GameObject RotChild;

    bool FreeMove;
    RaycastHit2D HitCast;
    public LayerMask EnemyLayer;

    public Sprite[] DirectionSprites;
    bool FloatDownUp = true;
    int FloatFrames = 0;
    float FloatFloat =0;

    // Start is called before the first frame update
    void Start()
    {
        Player = MasterMind.GetPlayer();
        PM = Player.GetComponent<PlayerMovement>();

        ChargePunchSR = ChargePunchHB.GetComponent<SpriteRenderer>();
        SmallPunchSR = SmallPunchHB.GetComponent<SpriteRenderer>();
        BarrageSR = BarrageHB.GetComponent<SpriteRenderer>();

        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SR.sprite = DirectionSprites[(int)CurrentDir];

        
        
        MovementOffset = 0.5f * -ADUtil.GetAimDirVector(CurrentDir);
        MovementOffset.y += 0.5f;

        RotChild.transform.up = -ADUtil.GetAimDirVector(CurrentDir);

        if(CurrentDir == AimDirKey.Down || CurrentDir == AimDirKey.DownLeft || CurrentDir == AimDirKey.DownRight)
        {
            SR.sortingOrder = 3;
        }
        else
        {
            SR.sortingOrder = 6;
        }
    }

    private void FixedUpdate()
    {
        // toggle jitter jump for a jumpy motion instead of smooth
        bool JitterJump = true;
        if(FloatDownUp && FloatFrames > 0)
        {
            FloatFloat -= 0.01f;
            if (JitterJump)
            {
                FloatFloat = -0.05f;
            }
        }
        else if(!FloatDownUp && FloatFrames > 0)
        {
            FloatFloat += 0.01f;
            if (JitterJump)
            {
                FloatFloat = 0.05f;
            }
        }
        else
        {
            FloatDownUp = !FloatDownUp;
            FloatFrames = 20;
            if (JitterJump)
            {
                FloatFloat = 0;
                FloatFrames = 10;
            }
            
        }
        FloatFrames--;

        FloatOffset = new Vector3(0, FloatFloat, 0);

        if (FreeMove == false)
        {
            CurrentDir = PM.GetAimDir();
            transform.position = Player.transform.position + MovementOffset + FloatOffset;
        }
    }

    public void QuickHit()
    {
        
        CurrentDir = PM.GetAimDir();
        RotChild.transform.up = -ADUtil.GetAimDirVector(CurrentDir);
        StartCoroutine(SpriteFlash(SmallPunchSR));
        FreeMove = true;
        HitCast = MasterMind.HitBoxToCast(SmallPunchHB, EnemyLayer);
        if (HitCast)
        {
            HitCast.transform.SendMessage("GotHit", 20);
        }
    }
    public void SuccessiveQuickHit()
    {
        StartCoroutine(SpriteFlash(SmallPunchSR));
        FreeMove = true;
        HitCast = MasterMind.HitBoxToCast(SmallPunchHB, EnemyLayer);
        if (HitCast)
        {
            HitCast.transform.SendMessage("GotHit", 20);
        }
    }

    public void BuddyChargeHit(float Amount)
    {
        
        
        StartCoroutine(SpriteFlash(ChargePunchSR));
        FreeMove = true;
        StartCoroutine(FreeMoveCancelWait());

        HitCast = MasterMind.HitBoxToCast(ChargePunchHB, EnemyLayer);
        if (HitCast)
        {
            HitCast.transform.SendMessage("GotHit", Amount * 0.75f);
        }
    }
    public void BarrageHit()
    {
        
        
        StartCoroutine(BarrageFlash());
        FreeMove = true;
        StartCoroutine(FreeMoveCancelWait());

        HitCast = MasterMind.HitBoxToCast(BarrageHB, EnemyLayer);
        if (HitCast)
        {
            HitCast.transform.SendMessage("GotHit", 150);
        }
    }

    IEnumerator FreeMoveCancelWait()
    {
        yield return new WaitForSeconds(2);
        CancelFreeMove();
    }

    IEnumerator BarrageFlash()
    {
        BarrageSR.enabled = true;
        yield return new WaitForSeconds(1.45f);
        BarrageSR.enabled = false;
    }

    IEnumerator SpriteFlash(SpriteRenderer SR)
    {
        SR.enabled = true;
        yield return new WaitForSeconds(0.25f);
        SR.enabled = false;
    }

    public void CancelFreeMove()
    {
        CurrentDir = PM.GetAimDir();
        FreeMove = false;
    }
}
