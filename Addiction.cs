using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;


public class Addiction : MonoBehaviour, IEnemy
{
    public bool DEBUG;
    public bool PLAYER_TP;
    public bool FRAIL;

    public UnityEvent MOVE;

    public enum Phase
    {
        First,
        Second,
        Third,
        Fourth
    }
    public Phase DEBUG_PHASE;
    Phase CurrentPhase = Phase.First;

    // This class is for the boss "addiction"

    // HB : Hit Box
    // SB : Scan Box
    public GameObject debugObject;
    public AddictionLockOn ALO;
    public AddictionAnimator AA;
    public AddictionSFX ASFX;
    public GameObject ASFX3DObject;
    
    AddictionComboManager ACM;
    public KelpSpawner KS;
    bool[] ASBCchecks;
    BossHealthBar BHB;
    PlayerMovement PM;
    
    

    public LayerMask WallMask;
    public LayerMask PlayerMask;
    public LayerMask DestructibleMask;

    public GameObject[] ManicAttackHitbox;
    public Bodybox[] ManicCDBodyBox;

    public GameObject[] FlightAttackHB;
    

    public GameObject[] SwipeIncomingHB;
    public GameObject[] SwipeMidHB;

    public GameObject[] JumpSwipeHB;

    public GameObject[] Swipe2IncomingHB;
    public GameObject[] Swipe2MidHB;
    public GameObject[] Swipe2OutHB;
    public GameObject Swipe2SB;

    public GameObject LaserHB;
    public GameObject LaserSpr;
    public GameObject LaserSB;
    public GameObject LaserStatic;
    GameObject LaserPlayerDir;
    GameObject LaserPointerDir;

    public PolygonCollider2D[] RocketBodyBox;
    public GameObject RocketMainHB;
    public GameObject RocketMainSpr;
    public GameObject RocketSecondarySpr;
    public GameObject RocketSideHB;
    public GameObject RocketSideSpr;
    public GameObject RocketTopHB;
    public GameObject RocketTopSpr;
    public GameObject RocketDiagonalSpr1;
    public GameObject RocketDiagonalHB1;
    public GameObject RocketDiagonalSpr2;
    public GameObject RocketDiagonalHB2;
    public GameObject RocketLandingHB;
    public GameObject RocketStaticVertical1;
    public GameObject RocketStaticHorizontal1;
    public GameObject RocketStaticVertical2;
    public GameObject RocketStaticHorizontal2;

    public GameObject RocketSpin;
    Transform RocketSpinMain;

    public GameObject ThrowableArm;
    public GameObject ThrowableChargeHB;
    public GameObject ThrowableIncSwipeHB;
    public GameObject ThrowableMidSwipeHB;

    public GameObject[] BigLaserHB;

    public GameObject[] PenguinHB;
    

    public GameObject PoleFlyHB;
    public GameObject StripHB;
    

    public GameObject[] LeftSlamHB;
    public GameObject[] RightSlamHB;
    public GameObject[] MidSlamHB;
    public PolygonCollider2D[] HeadSlamBodyBox; // hurtbox being the box in which add can take damage
    

    public GameObject GetOffSpinHB;
    

    public GameObject BridgeStarter;
    public GameObject Phase3Grid;

    public AudioSource MusicP1;
    public AudioSource MusicP2;
    public AudioSource MusicP3;
    public AudioSource MusicP4;

    public GameObject P4Cutscene;

    public GameObject P4FakeTP;

    public GameObject TpSpot;
    //prob need more to order and stuff

    


    

    public Sprite LaserLeft;
    public Sprite LaserFarLeft;
    public Sprite LaserRight;
    public Sprite LaserFarRight;
    public Sprite LaserMiddle;

    public LayerMask NotPlayer;
    public LayerMask FloorMask;
    public LayerMask InteractMask;

    public GameObject BigFallingTV;
    public GameObject FallingTV;
    public GameObject StaticFallingTV;
    public GameObject HandsCircle;
    public GameObject ShockwaveCircle;
    public GameObject SwipeWave;
    public GameObject HitEffectPhase2;
    public GameObject ManicHugeLaser;
    public GameObject ManicLaserDeath;

    public GameObject StaticGround;

    public GameObject ManicFollowLaser;
    GameObject PrevFLaser;
    GameObject CurrentFLaser;

    public GameObject ManicWarmupHB;

    public StaticTrailer ST;

    public GameObject MIDDLE_HOLE;
    

    public Transform[] ColliderPoints; // top right should be 0
    RaycastHit2D LeftCast;
    RaycastHit2D RightCast;
    RaycastHit2D TopCast;
    RaycastHit2D BotCast;
    RaycastHit2D InteractCast;
    bool ManicJumpCast;
    

    float randieNum;
    float SlamRoll; // Rolls a random number for slam chance
    float SwipeRoll; // rolls a random number for blah balh blah
    int LaserRoll; // rolls a ran... o_o
    

    SpriteRenderer SR;

    // Move Check Values
    bool canFlight;
    bool canSwipe;
    bool canSwipe2;
    bool canLaser;
    bool canRocket;
    bool canThrowable;
    bool canPenguin;
    bool canPole;
    bool canSlam;
    bool canGOSpin;
    bool canJumpSwipe;
    bool canLaserShot;

    bool P3ProximityCheck;

    bool IgnoreCollision;

    // Flying Attack Variables
    bool IsFlying;
    bool IsPenguin;
    bool IsCharging;
    bool IsAttacking;
    bool IsLasering;
    bool IsBurstLasering;
    bool IsMainRocket;
    bool IsAltRockets;
    bool IsDiaRockets; // diagonal
    bool IsStripping;
    bool IsPoleFlying;
    bool IsWaiting;
    bool IsFalling;
    bool IsInvulnerable;
    bool IsBatWait;

    bool IsManic;
    bool IsManicSkidding;
    bool IsManicPaused;

    bool ManicTurn; // first frame of turning to keep it from jittering
    bool GrabbedPlayer; // if the player was grabbed in penguin
    bool HandKelped; 

    bool IsParryable = false;
    bool IsFlinchable = false;
    bool IsStunned; // if stunned don't attack or move
    bool WaitForStartUp = true;
    bool randomStart = true;
    bool AnimWaiting = false;

    bool ManicWarming = false;

    bool StapVulnerable = false;

    RaycastHit2D FlyingAttackHB; // HB = hit box
    RaycastHit2D MovingStopHB; // Box that stops the boss from flying through a wall

    Vector3 poleFlightPos;
    Vector3 previousFlightPos;
    Vector3 animFlightPos;
    Vector3 HandKelpPosition;
    Vector3 entryPos;
    
    bool IsComing;

    // Base Movement Variables
    Vector3 MovingDir;
    float MovingSpeed;
    Vector3 KnockBackDir;
    float KnockBackSpeed = 0;
    int KnockBackDur = 0;
    int BleedDur = 0;
    float BleedDmg = 0;

    int ManicJumpCount = 0;
    int ManicJumpCooldown = 0;
    float ManicDmg;

    int StapleCount = 0;
    int StapleDuration = 0;
    const int STAP_DUR_MAX = 600; // every time add gets hit with a staple the duration resets to this amount
    int BurnCount = 0;
    int BurnDuration = 0;
    const int BURN_DUR_MAX = 800;
    const int BURN_COUNT_MAX = 40;

    float LaserZMax = 0;
    float LaserZMin = 0;
    const float LASER_Z_RANGE = 30;

    Vector3[] Phase1LaserPos = new Vector3[10]
    {
       new Vector3(-1,    0.6f),
       new Vector3(1,     0.6f),
       new Vector3(1.8f,  0.973f),
       new Vector3(1.9f,  0.7f),
       new Vector3(1.7f,  0.95f),
       new Vector3(-1.7f, 0.95f),
       new Vector3(-1.9f, 0.7f),
       new Vector3(-1.8f, 0.9f),
       new Vector3(0,     1f),
       new Vector3(0,     0.35f),
    };
    Vector3[] Phase3LaserPos = new Vector3[10]
    {
       new Vector3(-1.70f,  0.60f),
       new Vector3(-2.75f,  1.35f),
       new Vector3(-3.00f,  2.25f),
       new Vector3(-1.85f,  2.85f),
       new Vector3(-0.20f,  3.00f),
       new Vector3(1.80f,  2.75f),
       new Vector3(2.85f,  2.25f),
       new Vector3(2.75f,  1.35f),
       new Vector3(1.70f,  0.60f),
       new Vector3(0.00f,  0.40f),
    };


    bool PoleSpinAudioTimer = true;
    bool Transition = false;
    bool StartUp = false;

    GameObject Player;
    Vector3 PlayerMoveDir;


    Vector2 StartPos;
    const int MAX_HEALTH_P1 = 2000;
    const int MAX_HEALTH_P2 = 4500;
    const int MAX_HEALTH_P3 = 1750;
    const int MAX_HEALTH_P4 = 9999;

    float Health = MAX_HEALTH_P1; // default 2000


    List<int> possibleAttacks = new List<int>();

    
    int randomMoveRoll;
    int TotalMoveCount = 0;
    enum MoveIndex
    {
        Flight,
        Swipe,
        Swipe2,
        Laser,
        Rocket,
        ArmThrow,
        Penguin,
        Pole,
        Slam,
        GOSpin,
        Move,
        FlightMixup // always put this last because it has no scan box
    }
    MoveIndex moveSelector;
    int[] MoveCDList = new int[Enum.GetNames(typeof (MoveIndex)).Length]; // create a move cd list the length of the moveindex
    
    enum FacingDirection
    {
        Down,
        Left,
        Up,
        Right
    }
    FacingDirection FD; // what direction is addiction facing
    
    
    

    public Bodybox[] Boxes;
    public Bodybox[] Phase3Boxes;  

    public GameObject[] ParticlePhase2;
     HitDebris HD;

    public GameObject Eyes;

    public GameObject Phase4ThrowLocation;

    CameraControl CC;

    const int BLEED_DPS = 4;

    const int SLAM_FRAMES = 1;
    const int SLAM_CD = 11;
    const int MSLAM_FRAMES = 1;
    const int MSLAM_CD = 13;

    const int LASER_DURATION = 400;
    const int LASER_MIXUP_DUR = 300;

    const float PENGUIN_DMG = 125;
    const float FLIGHT_DMG = 25;
    const float FLIGHT_DMG_P2 = 50;
    const float SWIPE_DMG_P1 = 25;
    const float SWIPE_DMG_P2 = 50;
    const float TWOSWIPE_DMG = 75;
    const float ROCKETMAIN_DMG = 100;
    const float ROCKETALT_DMG = 75;
    const float THROW_DMG = 50;
    const float THROWCHARGE_DMG = 25; //not used
    const float POLEDANCE_DMG = 75;
    const float POLECHARGE_DMG = 50;
    const float LASER_DMG = 75;
    const float LASER_BURST_DMG = 35;
    const float LASER_DMG_P3 = 50;
    const float BIGLASER_DMG = 50;
    
    const float SMALLSLAM_DMG = 25; //not used
    const float BIGSLAM_DMG_P1 = 50;
    const float BIGSLAM_DMG_P2 = 75;
    const float GOSPIN_DMG = 0;

    const float MANIC_DMG_P3 = 50;
    const float MANIC_DMG_P4 = 25;

    bool PlayerTPCheck = false;


    Vector3 manicDir;
    Vector3 prevDir;
    float ManicSpeed = 0.8f;
    float ManicTurnRate = 200;
    int ManicCount = 0;

    private void Start()
    {
        StartPos = transform.position;
        ACM = GetComponent<AddictionComboManager>();
        
        HD = GetComponent<HitDebris>();
        SR = AA.GetComponent<SpriteRenderer>();
        SR.enabled = false;
        Player = MasterMind.GetPlayer(); // Any MM gets are in start to make sure that all sets have been assigned; but then again, you already knew that, didn't you?
        PlayerMask = MasterMind.GetPlayerLayer();
        LaserPlayerDir = new GameObject();
        LaserPointerDir = new GameObject();
        LaserPlayerDir.transform.position = transform.position;
        
        BHB = GetComponent<BossHealthBar>();
        PM = Player.GetComponent<PlayerMovement>();

        

        
        canFlight = canSwipe = canSwipe2 = canLaser = canRocket = canThrowable = canPenguin = canPole = canSlam = false;
        CC = Camera.main.GetComponent<CameraControl>();

        IsAttacking = false;
        IsInvulnerable = true;

        BHB.SetBossMaxHealth(Health);

        RocketSpinMain = RocketMainSpr.transform.parent;

        if (DEBUG)
        {
            SR.enabled = true;
            BridgeStarter.GetComponent<BridgeFightStart>().StartFight();
            ACM.enabled = false;
            Stunned(1);
        }
        if (PLAYER_TP)
        {
            Player.transform.position = transform.position;
        }

    }
    void SceneStarted()
    {
        
    }
    private void Update()
    {
        MovingStopHB = Physics2D.Linecast(transform.position, transform.position + MovingDir * 1.5f, WallMask); // recently changed to 1.5 from 2.5
        //Debug.DrawLine(transform.position, transform.position + MovingDir * 1.5f);
        BHB.SetBossHealth(Health);

        if (Health <= 0 && CurrentPhase == Phase.First && !Transition)
        {
            IsInvulnerable = true;
            Transition = true;
            StopAllCoroutines();
            ClearAttackVariables();
            KnockBackDur = 0;
            MoveDirection(0);
            StartCoroutine(Phase2Transition());
        }
        else if(Health <= 0 && CurrentPhase == Phase.Second && !Transition)
        {
            IsInvulnerable = true;
            Transition = true;
            StopAllCoroutines();
            ClearAttackVariables();
            KnockBackDur = 0;
            MoveDirection(0);
            StartCoroutine(Phase3Transition());
        }
        else if (Health <= 0 && CurrentPhase == Phase.Third && !Transition)
        {
            IsInvulnerable = true;
            Transition = true;
            StopAllCoroutines();
            ClearAttackVariables();
            KnockBackDur = 0;
            MoveDirection(0);
            StartCoroutine(Phase4Transition()); // was phase 4 transition
        }

        P3ProximityCheck = Vector2.Distance(transform.position, Player.transform.position) < 20f;
        

        if (WaitForStartUp && Vector2.Distance(transform.position, Player.transform.position) < 5 && !StartUp)
        {
            StartUp = true;
            StartCoroutine(StartFight());
        }
        // most of these are just for keeping attack hurtboxes out while he does some move or something

        if (IsFlying)
        {
            if (HitBoxToCast(FlightAttackHB[(int)FD]))
            {
                if(CurrentPhase == Phase.First)
                {
                    PM.GotHit(new Damage(FLIGHT_DMG, gameObject));
                }
                else
                {
                    PM.GotHit(new Damage(FLIGHT_DMG_P2, gameObject));
                }
                
            }
        }

        if (IsPenguin && GrabbedPlayer != true)
        {
            if (HitBoxToCast(PenguinHB[(int)FD]))
            {
                if(PM.LockGrabbed(transform.position - transform.up * 2, gameObject))  
                    GrabbedPlayer = true;
            }

        }

        if (IsCharging)
        {
            if (HitBoxToCast(ThrowableChargeHB))
            {
                PM.GotHit(new Damage(THROWCHARGE_DMG, gameObject));
                GrabbedPlayer = true;
            }
        }

        if (IsLasering)
        {
            if (HitBoxToCast(LaserHB))
            {
                float LaserDmg = LASER_DMG;
                if (CurrentPhase == Phase.Third)
                    LaserDmg = LASER_DMG_P3;
                if(PM.GotHit(new Damage(LaserDmg, 0, true, true, gameObject, Vector2.zero, 0,0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }
            RaycastHit2D TVCast = MasterMind.HitBoxToCast(LaserHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
           
        }
        if (IsBurstLasering)
        {
            if (HitBoxToCast(LaserHB))
            {
                float LaserDmg = LASER_BURST_DMG;
                
                if (PM.GotHit(new Damage(LaserDmg, 0, true, true, gameObject, Vector2.zero, 0, 0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }
            RaycastHit2D TVCast = MasterMind.HitBoxToCast(LaserHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }

        }

        if (IsMainRocket)
        {
            debugObject.transform.position = RocketMainHB.transform.position;
            if (HitBoxToCast(RocketMainHB))
            {
                if(PM.GotHit(new Damage(ROCKETMAIN_DMG, 0, true, true, gameObject, (PM.transform.position - transform.position).normalized, 0.5f, 3, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }
            RaycastHit2D TVCast = MasterMind.HitBoxToCast(RocketMainHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
        }

        if (IsAltRockets)
        {
            if (HitBoxToCast(RocketSideHB))
            {
                if(PM.GotHit(new Damage(ROCKETALT_DMG, 0, true, true, gameObject, Vector2.zero, 0, 0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }

            if (HitBoxToCast(RocketTopHB))
            {
                if (PM.GotHit(new Damage(ROCKETALT_DMG, 0, true, true, gameObject, Vector2.zero, 0, 0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }
            RaycastHit2D TVCast = MasterMind.HitBoxToCast(RocketSideHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
            TVCast = MasterMind.HitBoxToCast(RocketTopHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
        }

        if (IsDiaRockets)
        {
            if (HitBoxToCast(RocketDiagonalHB1))
            {
                if (PM.GotHit(new Damage(ROCKETALT_DMG, 0, true, true, gameObject, Vector2.zero, 0, 0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }

            if (HitBoxToCast(RocketDiagonalHB2))
            {
                if (PM.GotHit(new Damage(ROCKETALT_DMG, 0, true, true, gameObject, Vector2.zero, 0, 0, DamageType.Projectile)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }

            RaycastHit2D TVCast = MasterMind.HitBoxToCast(RocketDiagonalHB1, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
            TVCast = MasterMind.HitBoxToCast(RocketDiagonalHB2, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
        }

        if (IsPoleFlying)
        {
            
            if (HitBoxToCast(PoleFlyHB))
            {
                PM.GotHit(new Damage(POLECHARGE_DMG, gameObject));
            }
        }

        if (IsStripping)
        {
            if (HitBoxToCast(StripHB))
            {
                PM.GotHit(new Damage(POLECHARGE_DMG, gameObject));
            }
            RaycastHit2D TVCast = MasterMind.HitBoxToCast(StripHB, DestructibleMask);
            if (TVCast)
            {
                Destroy(TVCast.transform.gameObject);
            }
        }


        

    }

    private void FixedUpdate() // movement done in fixed because it looks shaky otherwise *blush emoji*
    {
        //max lr is 0.155
        // min ud 0.05
        // max ud 0.135
        float EyeXPos = (Player.transform.position.x - transform.position.x) * 0.05f;
        float EyeYPos = (Player.transform.position.y - transform.position.y) * 0.1f;
        if (Mathf.Abs(EyeXPos) > 0.155f)
        {
            EyeXPos = Mathf.Sign(EyeXPos) * 0.155f;
        }
        if(EyeYPos > 0.135f)
        {
            EyeYPos = 0.135f;
        }
        if (EyeYPos < 0.05f)
        {
            EyeYPos = 0.05f;
        }
        Eyes.transform.localPosition = new Vector3(EyeXPos, EyeYPos);



        BleedDur--;
        if(BleedDur > 0 && BleedDur % BLEED_DPS ==0)
        {
            Health -= BleedDmg;
        }

        
        if(StapleDuration < 0)
        {
            if(StapleCount > 0)
            {
                StapleCount = 0;
                BHB.SetStapleCount(0);
            }
            
        }
        else
        {
            StapleDuration--;
        }

        

        if (IsManic )
        {
            if (!IsManicPaused)
            {
                FloorKey FK = FloorUtilities.GetHighestFloor(Physics2D.LinecastAll(transform.position + manicDir.normalized * 10, transform.position + manicDir.normalized * 12, FloorMask));

                ST.gameObject.transform.up = manicDir;
                if (!IsManicSkidding)
                {
                    manicDir = ((manicDir * ManicTurnRate + (Player.transform.position - transform.position)) / 2).normalized;

                    if ((manicDir - prevDir).magnitude > 0.01f)
                    {
                        //ManicSpeed = 0.3f;
                        ManicTurnRate -= 3;
                        if (ManicTurnRate < 1)
                        {
                            ManicTurnRate = 1;
                        }
                    }
                    else
                    {
                        ManicTurnRate = 400;
                        ManicSpeed += 0.07f; // acceleration (0.07)
                        if (ManicSpeed > 0.8f) // max speed (0.8)
                        {
                            ManicSpeed = 0.8f;
                        }

                    }

                    if (((manicDir - prevDir).magnitude > 0.025f && (manicDir - prevDir).magnitude < 1 && Vector2.Distance(transform.position, Player.transform.position) > 3) ||
                        (FK == FloorKey.Pit && !IgnoreCollision)) //BEGIN TURN
                    {

                        //ManicSpeed = 0.2f;
                        //ManicTurnRate = 200;

                        IsManicSkidding = true;
                    }
                }
                else // WHILE TURNING
                {
                    ManicSpeed -= 0.05f; // skid slow

                    if (ManicSpeed > 0 && !ManicTurn)//ManicSpeed < 0.2f && ManicSpeed > 0.14f)
                    {
                        AA.PlayAnimation("ManicSkidP3");
                        ManicTurn = true;
                    }
                    if (ManicSpeed <= 0)
                    {
                        SetFaceToPlayer();
                        ManicTurn = false;

                        PlayManicRun();

                        IsManicSkidding = false;

                        manicDir = Player.transform.position - transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                        prevDir = manicDir;
                        ManicSpeed = 0.2f;
                        IsManic = true;
                    }
                }

                prevDir = manicDir;

                if (HitBoxToCast(ManicAttackHitbox[(int)FD]))
                {
                    Player.GetComponent<PlayerMovement>().GotHit(new Damage(ManicDmg, gameObject));
                }

                ManicJumpCast = Physics2D.Linecast(transform.position, transform.position + manicDir * 4, WallMask) &&
                    Physics2D.Linecast(transform.position, Player.transform.position, WallMask);

                if (ManicJumpCast)
                {
                    ManicJumpCount++;
                }
                else
                {
                    ManicJumpCount = 0;
                }

                if (ManicJumpCount > 0 && ManicJumpCount <= 10) // was 5
                {
                    if (!IgnoreCollision)
                    {

                        AA.PlayAnimation("ManicJump");
                        IsManicSkidding = false;
                    }
                    IgnoreCollision = true;
                    ST.StaticSet(false);
                    MoveDirection(0);
                }
                else if (ManicJumpCount > 10)
                {
                    SetFaceToPosition(manicDir + transform.position);
                    MoveDirection(manicDir, ManicSpeed / 1.25f);
                    ManicTurnRate = 400;
                    ManicJumpCooldown = 5; // was 7
                }
                else
                {
                    if (ManicJumpCooldown <= 0)
                    {
                        if (IgnoreCollision)
                        {
                            PlayManicRun();
                            ST.StaticSet(true);
                        }

                        IgnoreCollision = false;
                        MoveDirection(manicDir, ManicSpeed);
                    }
                    else
                    {
                        ST.StaticSet(true);
                        ManicJumpCooldown--;
                        MoveDirection(0);
                    }
                }
            }

            
            ManicCount++;

            if(ManicCount % 9 == 0 && !IgnoreCollision)
            {
                if(ManicCount % 2 == 0)
                {
                    ASFX.PlayManicStep1();
                }
                else
                {
                    ASFX.PlayManicStep2();
                }
            }
            if (ManicCount % 30 == 0)
            {
                StartCoroutine(TvDrops(0, 0f));
            }



        }
        

        randieNum = Random.Range(0, 1);
        
       
        if (!IsAttacking && DEBUG && !IsStunned && !IsFalling)
        {
            if (randieNum == 0 && randomStart)
            {
                MOVE.Invoke();
                StartCoroutine(RandomDelay());
                randomStart = false;
            }
        }

        if(TotalMoveCount >= 10000) // int overflow PROTECTION
        {
            TotalMoveCount = 0;
            for(int i = 0; i <= MoveCDList.Length; i++)
            {
                MoveCDList[i] = 0;
            }
        }

        if(CurrentPhase == Phase.Third)
        {
            KnockBackDur = 0;
        }
        
        if(KnockBackDir != Vector3.zero)
        {
            if(KnockBackDur == 0)
            {
                KnockBackDir = Vector3.zero;
            }
            KnockBackDur--;
        }
        Vector3 MovementVector = (MovingDir * MovingSpeed) + (KnockBackDir * KnockBackSpeed);

        RightCast = Physics2D.Linecast(ColliderPoints[2].position, ColliderPoints[3].position, WallMask);
        BotCast = Physics2D.Linecast(ColliderPoints[4].position, ColliderPoints[5].position, WallMask);
        LeftCast = Physics2D.Linecast(ColliderPoints[6].position, ColliderPoints[7].position, WallMask);
        TopCast = Physics2D.Linecast(ColliderPoints[0].position, ColliderPoints[1].position, WallMask);
        
     

        int MovementPerVelocityX = Mathf.Abs((int)(MovementVector.x / 0.001f));  // Each mm is equal to a tenth of a unit and each tenth collision is checked
        int MovementPerVelocityY = Mathf.Abs((int)(MovementVector.y / 0.001f));
        RaycastHit2D hitObject = LeftCast;
        bool hitSomething = false;
        for (int i = 0; i < MovementPerVelocityX; i++) // For the x axis
        {
            LeftCast = Physics2D.Linecast(ColliderPoints[6].position, ColliderPoints[7].position, WallMask);
            RightCast = Physics2D.Linecast(ColliderPoints[2].position, ColliderPoints[3].position, WallMask);
            if ((!(LeftCast && Mathf.Sign(MovementVector.x) < 0) && !(RightCast && Mathf.Sign(MovementVector.x) > 0) ) || IgnoreCollision) // checks if you're moving towards a wall and stops you if you are
            {
                transform.position += new Vector3(0.001f * Mathf.Sign(MovementVector.x), 0);
            }
            
            else
            {
                MovementPerVelocityX = 0;
                if ((IsFlying || IsPoleFlying) && !IsFalling)
                {
                    if (RightCast)
                    {
                        hitObject = RightCast;
                    }
                    if (LeftCast)
                    {
                        hitObject = LeftCast;
                    }
                    hitSomething = true;
                }
            }

        }
        for (int i = 0; i < MovementPerVelocityY; i++) // For the y axis
        {
            TopCast = Physics2D.Linecast(ColliderPoints[0].position, ColliderPoints[1].position, WallMask);
            BotCast = Physics2D.Linecast(ColliderPoints[4].position, ColliderPoints[5].position, WallMask);
            if ((!(TopCast && Mathf.Sign(MovementVector.y) > 0) && !(BotCast && Mathf.Sign(MovementVector.y) < 0) )|| IgnoreCollision)
            {
                transform.position += new Vector3(0, 0.001f * Mathf.Sign(MovementVector.y));
            }
            
            else
            {
                MovementPerVelocityY = 0;
                if ((IsFlying || IsPoleFlying) && !IsFalling)
                {
                    if (TopCast)
                    {
                        hitObject = TopCast;
                    }
                    if (BotCast)
                    {
                        hitObject = BotCast;
                    }
                    hitSomething = true;
                }
            }
        }

        if (hitSomething )
        {
            hitObject = Physics2D.Linecast(transform.position, hitObject.point, WallMask);
           
            FallOff(hitObject.normal);
        }
        
        InteractCast = Physics2D.BoxCast(transform.position, new Vector2(2, 2), 0, transform.up, 0, InteractMask);
        if (IsFlying && InteractCast && InteractCast.transform.GetComponent<HandsCircle>() && !HandKelped) // fly into hand kelp range
        {
            HandKelped = true;
            HandKelpPosition = InteractCast.transform.position;
            entryPos = transform.position;
        }
        if(IsFlying && !InteractCast && HandKelped) // when you fly out then start the pole spin
        {
            PoleSpinSetUp(HandKelpPosition, 2);
            
            HandKelped = false;
            
        }
        

        if (IsStripping)
        {
            transform.position = animFlightPos;
            

            if(Vector3.Distance((transform.position - previousFlightPos).normalized, GetPlayerDir()) < 0.75f)
            {
                IsComing = false;
            }
            else
            {
                IsComing = true;
            }
                    
            previousFlightPos = transform.position;
            
        }
        ST.transform.position = transform.position;
        ASFX3DObject.transform.position = transform.position;

        if (CurrentPhase == Phase.Third && false)
        {
            if (!IsManic)
            {
                StaticGround.GetComponent<SpriteRenderer>().enabled = true;
                RaycastHit2D StaticCast = HitCapsuleToCast(StaticGround);
                if (StaticCast)
                {
                    StaticCast.transform.GetComponent<PlayerMovement>().GotHit(new Damage(15, true, gameObject, DamageType.Zone));
                }
            }
            else
            {
                StaticGround.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public int GetPhase()
    {
        return (int)CurrentPhase;
    }

    void AddAttackPoss(MoveIndex MI, int Chance, int CD)
    {
        for(int i =0; i < Chance; i++)
        {
            possibleAttacks.Add((int)MI);
        }
        MoveCDList[(int)MI] = TotalMoveCount + CD + 1;
    }

    IEnumerator WaitForAttack() // increase min or max to change possible wait times
    {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));
        IsWaiting = false;
    }

    

    void SetFaceToPlayer() // Makes addiction turn to player
    {
        float YDif = Mathf.Abs(transform.position.y - Player.transform.position.y);
        float XDif = Mathf.Abs(transform.position.x - Player.transform.position.x);

        if (transform.position.y > Player.transform.position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Down);
            
        }
        if (transform.position.y < Player.transform.position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Up);
        }
        if (transform.position.x < Player.transform.position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Right);
        }
        if (transform.position.x > Player.transform.position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Left);
        }
    }
    void SetFaceToObject(GameObject Object) // Makes addiction turn to an object
    {
        float YDif = Mathf.Abs(transform.position.y - Object.transform.position.y);
        float XDif = Mathf.Abs(transform.position.x - Object.transform.position.x);

        if (transform.position.y > Object.transform.position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Down);

        }
        if (transform.position.y < Object.transform.position.y && XDif < YDif)
        {
            SetFaceTo(FacingDirection.Up);
        }
        if (transform.position.x < Object.transform.position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Right);
        }
        if (transform.position.x > Object.transform.position.x && YDif < XDif)
        {
            SetFaceTo(FacingDirection.Left);
        }
    }

    void SetFaceToPosition(Vector3 Position) // Makes addiction turn to a position
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

    

    void SetFaceTo(FacingDirection Direction) // Manually make addiction turn
    {
        AA.SetFaceDir((int)Direction);
        FD = Direction;

        // set all boxes to false
        ResetBodyBoxes();
        // set the correct box to true
        if(CurrentPhase <= Phase.Second)
        {
            Boxes[(int)FD].BoxesToggle(true);
        }
        else
        {
            Phase3Boxes[(int)FD].BoxesToggle(true);
        }
    }

    void ResetBodyBoxes()
    {
        Boxes[(int)FacingDirection.Down].BoxesToggle(false);
        Boxes[(int)FacingDirection.Left].BoxesToggle(false);
        Boxes[(int)FacingDirection.Right].BoxesToggle(false);
        Boxes[(int)FacingDirection.Up].BoxesToggle(false);

        HeadSlamBodyBox[(int)FacingDirection.Down].enabled = false;
        HeadSlamBodyBox[(int)FacingDirection.Left].enabled = false;
        HeadSlamBodyBox[(int)FacingDirection.Right].enabled = false;
        HeadSlamBodyBox[(int)FacingDirection.Up].enabled = false;

        RocketBodyBox[0].enabled = false;
        RocketBodyBox[1].enabled = false;

        Phase3Boxes[(int)FacingDirection.Down].BoxesToggle(false);
        Phase3Boxes[(int)FacingDirection.Left].BoxesToggle(false);
        Phase3Boxes[(int)FacingDirection.Right].BoxesToggle(false);
        Phase3Boxes[(int)FacingDirection.Up].BoxesToggle(false);

        ManicCDBodyBox[(int)FacingDirection.Down].BoxesToggle(false);
        ManicCDBodyBox[(int)FacingDirection.Left].BoxesToggle(false);
        ManicCDBodyBox[(int)FacingDirection.Right].BoxesToggle(false);
        ManicCDBodyBox[(int)FacingDirection.Up].BoxesToggle(false);
    }

    void SetPhase3BodyBox()
    {
        //AA.SetFaceDir((int)FacingDirection.Down);
        //FD = FacingDirection.Down;
        //ResetBodyBoxes();
        //Phase3Box.enabled = true;
    }

    Vector2 FaceToDir(FacingDirection Direction)
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

    RaycastHit2D HitBoxToCast(GameObject HitBox) // im sick of writing out huge ugly if statements :\
    {
        
        return Physics2D.BoxCast(
            HitBox.transform.position,
            HitBox.transform.lossyScale,
            HitBox.transform.rotation.eulerAngles.z,
            -HitBox.transform.up, 0, PlayerMask);
    }

    RaycastHit2D HitCircleToCast(GameObject HitBox) // same as above but it casts a circle instead!
    {
        
        return Physics2D.CircleCast(
            HitBox.transform.position, 
            HitBox.transform.lossyScale.x / 2, 
            HitBox.transform.up, 
            0, PlayerMask);
            
    }

    RaycastHit2D HitCapsuleToCast(GameObject HitBox) // same as above but it casts a capsule instead!
    {

        return Physics2D.CapsuleCast(
            HitBox.transform.position, 
            HitBox.transform.lossyScale,
            CapsuleDirection2D.Horizontal,
            0,transform.up,
            0,PlayerMask);

    }

    public bool GetAttacking() // please don't ask me why either of these exist or why im using them 
    {
        return IsAttacking;
    }
    public void SetAttacking(bool value) // seriously please don't ask
    {
        IsAttacking = value;
    }

   

    void MoveDirection(Vector3 dir, float amount) // Use this to tell the boss to move in a direction, amount is units per frame (speed)
    {
        MovingDir = new Vector3(dir.x, dir.y, 0).normalized; 
        MovingSpeed = amount;
    }

    void MoveDirection(float amount) // Overload to just change speed and not direction
    {
        MovingSpeed = amount;
    }
    

    Vector3 GetPlayerDir()
    {
        return (Player.transform.position - transform.position).normalized;
    }

    IEnumerator GetPlayerMoveDir()
    {
        Vector3 temp1 = Player.transform.position;
        yield return new WaitForSeconds(0.05f);
        Vector3 temp2 = Player.transform.position;

        PlayerMoveDir = (temp2 - temp1);
        
    }

    public void GotHit(Damage Damage)
    {
        if (!IsBatWait && !IsInvulnerable)
        {
            float DmgAmount = Damage.DmgAmount;
            int FlinchAmount = Damage.FlinchDuration;
            int StunAmount = Damage.StunDuration;
            int KnockbackAmount = Damage.KBDuration;
            Vector2 KnockbackDir = Damage.KnockBack;
            float KnockbackSpeed = Damage.KBSpeed;
            IsBatWait = true;

            if (FRAIL)
            {
                DmgAmount = 9999;
            }

            if (StapVulnerable && DmgAmount > 2)
            {
                IsBatWait = false;
                return;
            }

           


            if (StunAmount > 0)
            {
                Stunned(StunAmount);
            }

            if(FlinchAmount > 0 && IsFlinchable)
            {
                Stunned(FlinchAmount);
            }

            if(Damage.Parry && IsParryable)
            {
                Stunned(200);
            }

            if(KnockbackDir != Vector2.zero && (IsStunned))
            {
                KnockBackDir = KnockbackDir;
                KnockBackDur = KnockbackAmount;
                KnockBackSpeed = KnockbackSpeed;
            }

            

            if (!IsInvulnerable)
            {

                Health -= DmgAmount;
                AA.TakeDmg(FlinchAmount > 0 && !IsFlinchable);
                ASFX.PlayHurt();
            }
            //StartCoroutine(BatHitWait());
            IsBatWait = false;
        }
        
    }
    
    public void GotBleed(int Duration, float Damage)
    {
        BleedDur = Duration;
        BleedDmg = Damage;

    }

    public void GotStapled(int StapNum)
    {
        StapleDuration = STAP_DUR_MAX;
        StapleCount += StapNum;
        BHB.SetStapleCount(StapleCount);

    }

    public void GotBurned(int BurnNum) // -1 indicates burn POP
    {
        if (BurnNum == -1)
        {
            print("HIT FOR " + BurnCount * 25);
            GotHit(new Damage(BurnCount * 25, 50, gameObject));
            BurnCount = 0;
            BurnNum = 0;
        }
        else
        {
            AA.TakeDmg(false);
        }


        BurnDuration = BURN_DUR_MAX;
        BurnCount += BurnNum;

        if(BurnCount  > BURN_COUNT_MAX)
        {
            BurnCount = BURN_COUNT_MAX;
        }

        
    }

    public void Reset()
    {
        // move back to the starting position
        transform.position = StartPos;

        // turn the brige back on
        BridgeStarter.SetActive(true);
        BridgeStarter.GetComponent<BridgeFightStart>().StopFight();

        // turn off the kelp spawner
        KS.StopSpawning();

        // stop moving and acting
        KnockBackDur = 0;
        MoveDirection(0);
        StopAllCoroutines();
        ClearAttackVariables();
        ResetBodyBoxes();
        ACM.StopFight();

        // reset your values back to base
        CurrentPhase = Phase.First;
        Health = MAX_HEALTH_P1;
        BHB.SetBossMaxHealth(MAX_HEALTH_P1);
        SR.enabled = false;
        IsInvulnerable = true;
        WaitForStartUp = true;
        StartUp = false;
        SetFaceTo(FacingDirection.Down);
        

        if (PLAYER_TP) // if the debug player tp is on then tp the player back again
        {
            Player.transform.position = transform.position;
        }
    }


    //bat hit stops addiction from taking damage faster than every 3 frames
    IEnumerator BatHitWait()
    {

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        IsBatWait = false;
    }
   

    IEnumerator TvDrops(int Length, float SpreadTime) // Always spawns 3 tvs, lengths spawns 3 more per length, Spread increasing time between waves
    {
        
        SpawnTV(Player.transform.position.x, Player.transform.position.y);
        SpawnTV(Player.transform.position.x + Random.Range(4,20), Player.transform.position.y - Random.Range(2, 5));
        SpawnTV(Player.transform.position.x - Random.Range(4, 20), Player.transform.position.y + Random.Range(3, 6));
        yield return new WaitForSeconds(SpreadTime);
        for(int i = 0; i < Length; i++)
        {
            SpawnTV(transform.position.x + Random.Range(0, 20), transform.position.y + Random.Range(0, 20));
            yield return new WaitForSeconds(Random.Range(0, 0.4f));
            SpawnTV(transform.position.x - Random.Range(0, 20), transform.position.y - Random.Range(0, 20));
            yield return new WaitForSeconds(Random.Range(0, 0.4f));
            SpawnTV(transform.position.x + Random.Range(-20, 20),transform.position.y + Random.Range(-20, 20));
            yield return new WaitForSeconds(SpreadTime);
        }
        
        
    }

    void SpawnTV(float EndX, float EndY) // Spawns a TV at the value needed to drop on a specific location, bool confines location to addiction arena
    {
        GameObject TV;
        if (Random.Range(0,4) == 3 && CurrentPhase >= Phase.Second)
        {
            TV = StaticFallingTV;
        }
        else
        {
            TV = FallingTV;
        }
        

        Instantiate(TV, new Vector3(EndX, EndY + 12), Quaternion.identity).
                GetComponent<FallingTV>().SetPoss(EndY);
    }

    void SpawnBigTV(float EndX, float EndY) // Spawns a TV at the value needed to drop on a specific location, bool confines location to addiction arena
    {
        Instantiate(BigFallingTV, new Vector3(EndX, EndY + 12), Quaternion.identity).
                GetComponent<FallingTV>().SetPoss(EndY);
    }

    IEnumerator StartFight()
    {
        ResetBodyBoxes();
        SR.enabled = true;
        AA.PlayAnimation("AddictionBirth1");
        BridgeStarter.GetComponent<BridgeFightStart>().StartFight();
        yield return new WaitForSeconds(4.5f);
        MusicP1.Play();
        ACM.StartFight();
        SetFaceTo(FacingDirection.Down);
        IsInvulnerable = false;
        WaitForStartUp = false;
        
    }

    IEnumerator Phase2Transition() // from phase 1 to phase 2
    {
        ACM.StopFight();
        ResetBodyBoxes();
        SetAttacking(true);
        MoveDirection(transform.up, 0.25f);
        AA.PlayAnimation("Phase1Death");
        IsInvulnerable = true;
        yield return new WaitForSeconds(0.4f);
        MoveDirection(0);

        bool PlayerNear = false;
        while (!PlayerNear)
        {
            PlayerNear = Vector2.Distance(transform.position, Player.transform.position) < 2;
            yield return new WaitForFixedUpdate();
        }
        Player.GetComponent<PlayerMovement>().LockGrabbed(transform.position, gameObject);
        AA.PlayAnimation("Phase1Deathblow");
        
        MusicP1.gameObject.SetActive(false);
        MusicP2.Play();


        yield return new WaitForSeconds(3.792f);
        

        // 3.833

        Player.GetComponent<PlayerMovement>().LetGo(new Vector2(transform.position.x + 5.5f, transform.position.y - 0.4f));
        
        HD.Debris = ParticlePhase2;
        PM.NewPhase();
        
        
        BHB.SetBossMaxHealth(MAX_HEALTH_P2); // default 7000
        Health = MAX_HEALTH_P2;
        CurrentPhase = Phase.Second;
        
        Transition = false;
        IsInvulnerable = false;
        SetFaceTo(FacingDirection.Down);
        KS.StartSpawning();


        yield return StartCoroutine(TwoArmJumpSlamAttackP2());

        ACM.StartFight();
        PlayIdleAnimation();
        SetAttacking(false);
    }

    IEnumerator Phase3Transition() // from phase 2 to phase 3
    {
        SetAttacking(true);
        //AA.PlayAnimation("AddictionDeath");
        IsInvulnerable = true;
        ResetBodyBoxes();
        //HD.Debris = ParticlePhase2;
        PM.NewPhase();
        ACM.StopFight();
        //transform.position = new Vector2(-400, -40);
        //Player.transform.position = transform.position;
        //Phase3Grid.SetActive(true);
        AA.PlayAnimation("Phase1Death");
        IsInvulnerable = true;
        yield return new WaitForSeconds(0.4f);
        MoveDirection(0);

        bool PlayerNear = false;
        while (!PlayerNear)
        {
            PlayerNear = Vector2.Distance(transform.position, Player.transform.position) < 2;
            yield return new WaitForFixedUpdate();
        }
        Player.GetComponent<PlayerMovement>().LockGrabbed(transform.position, gameObject);
        AA.PlayAnimation("Phase2Deathblow");

        MusicP3.Play();
        MusicP2.gameObject.SetActive(false);

        yield return new WaitForSeconds(3.042f);

        Player.GetComponent<PlayerMovement>().LetGo(new Vector2(transform.position.x + 5.5f, transform.position.y - 0.4f));

        BHB.SetBossMaxHealth(MAX_HEALTH_P3);
        Health = MAX_HEALTH_P3;
        CurrentPhase = Phase.Third;
        Transition = false;
        IsInvulnerable = false;
        SetFaceTo(FacingDirection.Down);
        KS.StopSpawning();
        

        
        
        yield return StartCoroutine(ManicCrazeAttackP3());
        ACM.StartFight();
    }

    IEnumerator Phase4Transition() // from phase 3 to phase 4
    {
        ACM.StopFight();
        ResetBodyBoxes();
        SetAttacking(true);
        MoveDirection(transform.up, 0.25f);
        AA.PlayAnimation("AddictionDeath2");
        MusicP3.gameObject.SetActive(false);
        IsInvulnerable = true;
        
        

        yield return new WaitForSeconds(0.4f);
        Instantiate(P4FakeTP, transform.position, Quaternion.identity);
        MoveDirection(0);

        bool PlayerNear = false;
        while (!PlayerNear)
        {
            PlayerNear = Vector2.Distance(transform.position, Player.transform.position) < 2;
            yield return new WaitForFixedUpdate();
        }
        Player.GetComponent<PlayerMovement>().LockGrabbed(transform.position, gameObject);
        GameObject CS = Instantiate(P4Cutscene, transform.position, Quaternion.identity);
        CC.SetFocusObject(CS);
        MusicP4.Play();
        yield return new WaitForSeconds(7f);

        

        CC.ResetFocusObject();

        Destroy(CS);

        Player.GetComponent<PlayerMovement>().LetGo(transform.position);
        Player.GetComponent<PlayerMovement>().SetSoaring(new MoveOrder(Phase4ThrowLocation.transform.position, transform.position, 0.6f));

        HD.Debris = ParticlePhase2;
        PM.NewPhase();


        BHB.SetBossMaxHealth(MAX_HEALTH_P4); 
        Health = MAX_HEALTH_P4;
        CurrentPhase = Phase.Fourth;

        Transition = false;
        IsInvulnerable = false;
        SetFaceTo(FacingDirection.Down);

        transform.position = Phase4ThrowLocation.transform.position;

        yield return new WaitForSeconds(4.25f);
        PlayIdleAnimation();
        yield return new WaitForSeconds(7);

        StartCoroutine(ManicCrazeAttackP4());
    }

    IEnumerator DeathTransition() // from phase 4 to death (right now)
    {
        SetAttacking(true);
        ACM.StopFight();
        AA.PlayAnimation("AddictionDeath2");
        IsInvulnerable = true;
        yield return new WaitForSeconds(0.3f);
        MoveDirection(transform.up, 0.25f);
        yield return new WaitForSeconds(0.32f);
        MoveDirection(0);
        Instantiate(TpSpot, transform.position, Quaternion.identity);
    }

    

    #region Phase1Attacks
    

    public void AddPacePA()
    {
        StartCoroutine(PacePA());
    }
    public IEnumerator PacePA()
    {
        SetAttacking(true);
        

        Vector3 MoveDir = Vector2.zero;
        int RandomNumber = Random.Range(0, 2);
        float PaceSpeed = 0.08f;
        String animation = "PaceCCW";
        switch (FD)
        {
            case FacingDirection.Down:
                if(RandomNumber == 1)
                {
                    MoveDir = transform.right;
                    
                    animation = "PaceCCW";
                }
                else
                {
                    MoveDir = -transform.right;
                    
                    animation = "PaceCW";
                }
                break;
            case FacingDirection.Left:
                if (RandomNumber == 1)
                {
                    MoveDir = transform.up;
                   
                    animation = "PaceCW";
                }
                else
                {
                    MoveDir = -transform.up;
                    
                    animation = "PaceCCW";
                }
                break;
            case FacingDirection.Right:
                if (RandomNumber == 1)
                {
                    MoveDir = transform.up;
                    
                    animation = "PaceCCW";
                }
                else
                {
                    MoveDir = -transform.up;
                    
                    animation = "PaceCW";
                }
                break;
            case FacingDirection.Up:
                if (RandomNumber == 1)
                {
                    MoveDir = transform.right;
                    
                    animation = "PaceCW";
                }
                else
                {
                    MoveDir = -transform.right;
                    
                    animation = "PaceCCW";
                }
                break;
        }
        if(!GetCliff(MoveDir, 5)) // if there is a cliff then end pace
        {
            MoveDirection(MoveDir, PaceSpeed);
            AA.PlayAnimation(animation);
            for(int i =0; i < 2; i++)
            {
                if(i % 2 == 0)
                {
                    ASFX.PlayStep2();
                }
                else
                {
                    ASFX.PlayStep1();
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        PlayIdleAnimation();
        MoveDirection(0);
        SetAttacking(false);
        
    }

    public void AddMovePA()
    {
        StartCoroutine(MovePA());
    }
    public IEnumerator MovePA()
    {
        SetAttacking(true);
        AA.PlayAnimation("DashAct");
        int RandomNumber = Random.Range(1, 3);
        for (int i = 0; i < RandomNumber; i++)
        {
            AA.PlayAnimation("DashAct");
            MoveDirection(GetPlayerDir(), 0.75f);
            yield return new WaitForSeconds(0.15f);
            MoveDirection(0);
            yield return new WaitForSeconds(0.35f);
            PlayIdleAnimation();
            yield return new WaitForSeconds(0.20f);

        }
        PlayIdleAnimation();

        SetAttacking(false);
    }

    public void AddHeadSlamAttackP1()
    {
        StartCoroutine(HeadSlamAttackP1(2));
    }
    public IEnumerator HeadSlamAttackP1(float StallTime)
    {
        SetAttacking(true);
        SetFaceToPlayer();
        ASFX.PlayHeadSlamWind();
        AA.PlayAnimation("HeadSlamWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.65f); // Windup
        AA.PlayAnimation("HeadSlamAct");
        yield return new WaitForSeconds(0.05f); // slight windup on act
        IsFlinchable = true;
        IsParryable = false;
        ASFX.PlaySlam();
        if (HitBoxToCast(MidSlamHB[(int)FD]))
        {
            PM.GotHit(new Damage(BIGSLAM_DMG_P1, true, gameObject));
        }
        yield return new WaitForSeconds(0.05f);
        Vector3 spawnPos = new Vector3(0, -3);
        switch (FD)
        {
            case FacingDirection.Down:
                spawnPos = new Vector3(0, -3) + transform.position;
                break;
            case FacingDirection.Right:
                spawnPos = new Vector3(3, 0) + transform.position;
                break;
            case FacingDirection.Up:
                spawnPos = new Vector3(0, 1) + transform.position;
                break;
            case FacingDirection.Left:
                spawnPos = new Vector3(-3, 0) + transform.position;
                break;
        }
        Instantiate(ShockwaveCircle, spawnPos, Quaternion.identity);

        ResetBodyBoxes();
        HeadSlamBodyBox[(int)FD].enabled = true;

        yield return new WaitForSeconds(StallTime); // 2 default
        AA.PlayAnimation("HeadSlamRecovery");
        yield return new WaitForSeconds(0.33f);

        HeadSlamBodyBox[(int)FD].enabled = false;
        SetFaceTo(FD);
        PlayIdleAnimation();
        
        
        SetAttacking(false);
    }
    public IEnumerator HeadSlamAttackP1()
    {
        yield return StartCoroutine(HeadSlamAttackP1(2));
    }

    public void AddSwipeAttackP1()
    {
        StartCoroutine(SwipeAttackP1(0.375f));
    }
    public IEnumerator SwipeAttackP1(float StallTime) // The attack where he goes fwap! (fix standing still)
    {
        SetAttacking(true);
        SetFaceToPlayer();

        Vector3 MoveDir = Player.transform.position - transform.position;

        if(!GetCliff(MoveDir, 1))
        {
            MoveDirection(MoveDir, 0.5f);
        }

        yield return new WaitForSeconds(0.125f);

        MoveDirection(0);

        SetFaceToPlayer();
        AA.PlayAnimation("SwipeWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.65f); // Windup (changed from 0.45f)
        IsFlinchable = true;
        IsParryable = false;
        bool HitSomething = false;
        Vector3 HitDirection = -transform.up;
        if (HitBoxToCast(SwipeIncomingHB[(int)FD]))
        {
            int kbd = 10;
            
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P1, gameObject, HitDirection, 0.85f, kbd)))
            {
                HitSomething = true;
            }
        }
        // could add delay between inital swipe zone 
        else if (HitBoxToCast(SwipeMidHB[(int)FD]))
        {
            int kbd = 10;

            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P1, gameObject, HitDirection, 0.85f, kbd)))
            {
                HitSomething = true;
            }
        }

        if (HitSomething)
            ASFX.PlaySwipeHit();
        else
            ASFX.PlaySwipeNoHit();

        AA.PlayAnimation("SwipeAct");
        yield return new WaitForSeconds(StallTime); // 0.375 default

        PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator SwipeAttackP1()
    {
        yield return StartCoroutine(SwipeAttackP1(0.375f));
    }

    public void AddJumpSwipeAttackP1()
    {
        StartCoroutine(JumpSwipeAttackP1(0.3f));
    }
    public IEnumerator JumpSwipeAttackP1(float StallTime) // The attack where he goes fwap, dodge!
    {
        SetAttacking(true);
        SetFaceToPlayer();
        Vector3 MoveDir = -FaceToDir(FD);
        if (GetCliff(MoveDir, 6))
        {
            PlayIdleAnimation();
            SetAttacking(false);
            yield break;
        }
        yield return new WaitForSeconds(0.125f);
        AA.PlayAnimation("BackSwipeWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.45f); // Windup
        IsFlinchable = true;
        IsParryable = false;
        
        bool HitSomething = false;
        Vector2 HitDirection = Vector2.zero;

        if (HitBoxToCast(JumpSwipeHB[(int)FD]))
        {
            int kbd = 10;

            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P1, gameObject, HitDirection, 0.85f, kbd)))
            {
                HitSomething = true;
            }
        }


        AA.PlayAnimation("BackSwipeAct");
        if (HitSomething)
            ASFX.PlaySwipeHit();
        else
            ASFX.PlaySwipeNoHit();

        
        
        MoveDirection(MoveDir, 1.45f);
        yield return new WaitForSeconds(0.075f);
        MoveDirection(0);
        yield return new WaitForSeconds(StallTime); // 0.3f default
        PlayIdleAnimation();
        SetAttacking(false);

    }
    public IEnumerator JumpSwipeAttackP1()
    {
        yield return StartCoroutine(JumpSwipeAttackP1(0.3f));
    }

    public void AddRocketShipAttackP1()
    {
        StartCoroutine(RocketShipAttackP1(3));
    }
    public IEnumerator RocketShipAttackP1(float StallTime)
    {
        SetFaceTo(FacingDirection.Down);
        SetAttacking(true);
        ASFX.PlayChargeUp();
        AA.PlayAnimation("RocketWind");
        IsFlinchable = false;

        yield return new WaitForSeconds(0.25f);
        ResetBodyBoxes();
        RocketBodyBox[0].enabled = true;

        yield return new WaitForSeconds(0.75f); // Wind up
        RocketBodyBox[0].enabled = false;

        ASFX.EnableLaser(true);
        CC.Shake(0.05f, 500);

        AA.PlayAnimation("RocketAct");
        IsMainRocket = true;
        IsInvulnerable = true;

        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f); // blast offf!!!11!!1!
        RocketSpin.transform.rotation = Quaternion.Euler(0, 0, 0);
        RocketMainSpr.transform.rotation = Quaternion.Euler(0, 0, 0);
        AA.GetComponent<EnemyOcclusion>().LockOcclusion(12);
        StartCoroutine(TvDrops(5, 0.45f));
        IsAltRockets = true;

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = true;

        for (int i = 0; i < 350; i++)
        {
            float SpeedFactor = 0.010f;
            float MaxSpeed = 1.55f;
            if (SpeedFactor * i >= MaxSpeed)
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + MaxSpeed);

            }
            else
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + SpeedFactor * i);
            }

            RocketSecondarySpr.transform.rotation = Quaternion.Euler(0, 0, RocketSecondarySpr.transform.rotation.eulerAngles.z + MaxSpeed * 0.8f);
            RocketSpinMain.transform.rotation = Quaternion.Euler(0, 0, RocketSpinMain.transform.rotation.eulerAngles.z - MaxSpeed);

            yield return new WaitForSeconds(0.01f);
        }
        // however long this attack lasts

        AA.GetComponent<EnemyOcclusion>().UnlockOcclusion();
        ASFX.EnableLaser(false);
        AA.PlayAnimation("RocketCD");
        IsMainRocket = false;
        IsAltRockets = false;

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.45f);
        ASFX.PlayLand();
        IsFlinchable = true;
        RocketBodyBox[1].enabled = true;

        if (HitBoxToCast(RocketLandingHB)) // landing damage box (probably not worth keeping)
        {
            PM.GotHit(new Damage(10, gameObject));
        }

        CC.Shake(0.4f, 10);

        IsInvulnerable = false;
        yield return new WaitForSeconds(StallTime); // 4 default
        
        AA.PlayAnimation("RocketRecovery");
        yield return new WaitForSeconds(0.4f); // recovery
        SetFaceTo(FD);

        PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator RocketShipAttackP1()
    {
        yield return StartCoroutine(RocketShipAttackP1(3));
    }

    public void AddLaserShootAttackPA()
    {
        StartCoroutine(LaserShootAttackPA(1f));
    }
    public IEnumerator LaserShootAttackPA(float StallTime) //shoots a single laser
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        ASFX.PlayLaserShot();
        yield return new WaitForFixedUpdate();
        ASFX.PlayLaserShot();

        AA.PlayAnimation("LaserCharge1");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.8f); // wind up (1.15) original


        yield return new WaitForSeconds(0.35f);
        yield return StartCoroutine(LaserShoot(0, Phase.First));

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(StallTime); // 1 default
        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserShootAttackPA()
    {
        yield return StartCoroutine(LaserShootAttackPA(1f));
    }

    public void AddFlightMixupP1()
    {
        StartCoroutine(FlightMixupP1(3));
    }
    public IEnumerator FlightMixupP1(float StallTime) // the flight he does in close range
    {
        SetAttacking(true);
        SetFaceToPlayer();

        // Play charge up animation and wait    (wind up)
        AA.PlayAnimation("ChargeWind");
        IsParryable = true;
        IsFlinchable = false;
        Vector2 PreviousDir = Player.transform.position;
        yield return new WaitForSeconds(0.65f);
        AA.PlayAnimation("ChargeTrans");
        IsFlinchable = true;
        IsParryable = false;
        yield return StartCoroutine(FlightMove(StallTime, PreviousDir)); // default 3
    }
    public IEnumerator FlightMixupP1()
    {
        yield return StartCoroutine(FlightMixupP1(3));
    }

    public void AddFlightAttackP1()
    {
        StartCoroutine(FlightAttackP1(0.5f));
    }
    public IEnumerator FlightAttackP1(float StallTime) // The attack where the boss goes like whoosh bang slam! 
    {
        SetAttacking(true);
        SetFaceToPlayer();

        // Play charge up animation and wait    (wind up)
        AA.PlayAnimation("ChargeWind");
        IsParryable = true;
        IsFlinchable = false;
        Vector2 PreviousDir = Player.transform.position;
        yield return new WaitForSeconds(0.65f);

        
        AA.PlayAnimation("ChargeTrans");
        IsFlinchable = true;
        IsParryable = false;
        yield return StartCoroutine(FlightMove(StallTime,PreviousDir)); // 0.5f default
    }
    public IEnumerator FlightAttackP1()
    {
        yield return StartCoroutine(FlightAttackP1(0.5f));
    }
    #endregion

    #region Phase2Attacks

    public void AddGetOffSpinAttackP2()
    {
        StartCoroutine(GetOffSpinAttackP2(0.2f));
    }
    public IEnumerator GetOffSpinAttackP2(float StallTime)
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        
        AA.PlayAnimation("GOSpinWind");
        IsParryable = true;
        IsFlinchable = false;
        ASFX.PlayGoSpin();
        yield return new WaitForSeconds(0.5f);
        ASFX.PlaySpin();

        
        
        AA.PlayAnimation("GOSpinAct");
        IsFlinchable = true;
        IsParryable = false;

        bool HitSomething = false;

        for(int i = 0; i < 50 && !HitSomething; i++)
        {
            if (HitCircleToCast(GetOffSpinHB))
            {
                HitSomething = PM.GotHit(new Damage(GOSPIN_DMG, gameObject, (PM.transform.position - transform.position).normalized, 1.5f, 8));
            }
            yield return null;
        }
        
        yield return StartCoroutine(FlightGoMove(StallTime)); // default 3
    }
    public IEnumerator GetOffSpinAttackP2() // default timing
    {
        yield return StartCoroutine(GetOffSpinAttackP2(0.2f));
    }

    

    public void AddFlightMixupP2()
    {
        StartCoroutine(FlightMixupP2(2));
    }
    public IEnumerator FlightMixupP2(float StallTime) // the flight he does in close range
    {
        SetAttacking(true);
        SetFaceToPlayer();

        // Play charge up animation and wait    (wind up)
        AA.PlayAnimation("ChargeWindP2");
        IsParryable = true;
        IsFlinchable = false;
        Vector2 PreviousDir = Player.transform.position;
        yield return new WaitForSeconds(0.65f);
        AA.PlayAnimation("ChargeTransP2");
        IsFlinchable = true;
        IsParryable = false;
        yield return StartCoroutine(FlightMove(StallTime,PreviousDir)); // 2 default
    }
    public IEnumerator FlightMixupP2()
    {
        yield return StartCoroutine(FlightMixupP2(2));
    }
    public void AddFlightAttackP2()
    {
        StartCoroutine(FlightAttackP2(0.5f));
    }
    public IEnumerator FlightAttackP2(float StallTime) // The attack where the boss goes like whoosh bang slam! 
    {
        SetAttacking(true);
        SetFaceToPlayer();
        
        // Play charge up animation and wait    (wind up)
        AA.PlayAnimation("ChargeWindP2");
        IsParryable = true;
        IsFlinchable = false;
        Vector2 PreviousDir = Player.transform.position;
        yield return new WaitForSeconds(0.65f);
        AA.PlayAnimation("ChargeTransP2");
        IsFlinchable = true;
        IsParryable = false;
        yield return StartCoroutine(FlightMove(StallTime,PreviousDir)); // 0.5 default
    }
    public IEnumerator FlightAttackP2()
    {
        yield return StartCoroutine(FlightAttackP2(0.5f));
    }
    public IEnumerator FlightMove(float CD, Vector2 PrevDir)
    {
        yield return GetPlayerMoveDir();

        debugObject.transform.position = (Player.transform.position + PlayerMoveDir * 5);
        float Dist = Vector2.Distance(transform.position, Player.transform.position);
        
        
        

        // Fly toward point and hit player if he's in the way   (action)
        IsFlying = true; // As long as we don't hit a wall we flying
        Vector3 playerDest = (Player.transform.position + PlayerMoveDir * (Dist / 5));
        Vector3 destDir = (playerDest - (transform.position)).normalized;

        bool FlightSlam = false;
        if (Dist < 1.5f)
        {
            FlightSlam = true;
        }
        switch (FD)
        {
            case FacingDirection.Down:
                if (playerDest.y > transform.position.y)
                    FlightSlam = true;
                break;
            case FacingDirection.Left:
                if (playerDest.x > transform.position.x)
                    FlightSlam = true;
                break;
            case FacingDirection.Right:
                if (playerDest.x < transform.position.x)
                    FlightSlam = true;
                break;
            case FacingDirection.Up:
                if (playerDest.y < transform.position.y)
                    FlightSlam = true;
                break;

        }
        
        if (FlightSlam)
        {
            playerDest = PrevDir;
        }
        

        ASFX.PlayRoar();
        MoveDirection(destDir, 1.2f);

        AA.SetPosition(transform.position);
        AA.SetLock(true);
        
        
        Vector2 destination = (playerDest + destDir * 8);
        SetFaceToPosition(destination);
        switch (CurrentPhase)
        {
            case Phase.First:
                AA.PlayAnimation("ChargeAct");
                break;
            case Phase.Second:
                AA.PlayAnimation("ChargeActP2");
                break;
            default:
                AA.PlayAnimation("ChargeAct");
                break;
        }
        

        int timer = 0;
        while (Vector2.Distance(transform.position, destination) > 2 && timer < 25) // Stall until he hits a wall or reaches destination
        {
            timer++;
            yield return new WaitForFixedUpdate();
        }
        IsFlying = false;
        MoveDirection(0.25f);

        CC.Shake(0.25f, 4);
        // Land and cooldown    (wind down)


        switch (CurrentPhase)
        {
            case Phase.First:
                AA.PlayAnimation("ChargeSlide");
                break;
            case Phase.Second:
                AA.PlayAnimation("ChargeSlideP2");
                break;
            default:
                AA.PlayAnimation("ChargeSlide");
                break;
        }

        
        yield return new WaitForSeconds(0.35f);
        MoveDirection(0f);
        PlayIdleAnimation();
        yield return new WaitForSeconds(CD);
        IsFalling = false;
        

        SetAttacking(false);
    }

    public IEnumerator FlightGoMove(float CD)
    {
        yield return GetPlayerMoveDir();

        debugObject.transform.position = (Player.transform.position + PlayerMoveDir * 5);
        float Dist = Vector2.Distance(transform.position, Player.transform.position);

        // Fly toward point and hit player if he's in the way   (action)
        IsFlying = true; // As long as we don't hit a wall we flying
        Vector3 playerDest = (Player.transform.position + PlayerMoveDir * (Dist / 10));
        Vector3 destDir = (playerDest - (transform.position)).normalized;
        

        ASFX.PlayRoar();
        MoveDirection(destDir, 1.4f);

        AA.SetPosition(transform.position);
        AA.SetLock(true);
        AA.PlayAnimation("SpinFlight");

        Vector2 destination = (playerDest + destDir * 8);
        SetFaceToPosition(destination);
        AA.PlayAnimation("SpinFlight");

        int timer = 0;
        while (Vector2.Distance(transform.position, destination) > 2 && timer < 25) // Stall until he hits a wall or reaches destination
        {
            timer++;
            yield return new WaitForFixedUpdate();
        }
        IsFlying = false;
        MoveDirection(0.25f);

        CC.Shake(0.25f, 4);
        // Land and cooldown    (wind down)

        AA.PlayAnimation("ChargeSlide"); // this should be the "slow down streak"
        yield return new WaitForSeconds(0.35f);
        MoveDirection(0f);
        PlayIdleAnimation();
        yield return new WaitForSeconds(CD);
        IsFalling = false;


        SetAttacking(false);
    }

    public IEnumerator FlightMoveDirection(Vector3 dir)
    {
        SetFaceToPlayer();
        ASFX.PlayRoar();

        // Fly toward point and hit player if he's in the way   (action)
        IsFlying = true; // As long as we don't hit a wall we flying
        
        MoveDirection(dir, 1.2f);

        AA.SetPosition(transform.position);
        AA.SetLock(true);
        AA.PlayAnimation("ChargeAct");

        Vector2 destination = (transform.position + dir * 12);
        SetFaceToPosition(destination);
        AA.PlayAnimation("ChargeAct");

        int timer = 0;
        while (Vector2.Distance(transform.position, destination) > 2 && timer < 25) // Stall until he hits a wall or reaches destination
        {
            timer++;
            yield return new WaitForFixedUpdate();
        }
        IsFlying = false;
        MoveDirection(0.25f);

        CC.Shake(0.25f, 4);
        // Land and cooldown    (wind down)

        AA.PlayAnimation("ChargeSlide"); // this should be the "slow down streak"
        yield return new WaitForSeconds(0.25f);
        MoveDirection(0f);
        yield return new WaitForSeconds(0.5f);
        
    }

    public void AddPenguinAttackP2()
    {
        StartCoroutine(PenguinAttackP2(1.5f));
    }
    public IEnumerator PenguinAttackP2(float StallTime) // The flight but he stops and grabs instead of flying
    {// "Get it?"
        SetFaceToPlayer();
        SetAttacking(true);


        AA.PlayAnimation("PenguinWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.95f);

        if (GetCliff(GetPlayerDir(), 6))
        {
            PlayIdleAnimation();
            SetAttacking(false);
            yield break;
        }

        IsFlinchable = true;
        IsParryable = false;
        SetFaceToPlayer();

        Vector3 PlayerDir = GetPlayerDir();

        Vector3 destinationPos = transform.position + PlayerDir * 8; // go 10 units toward player ~~give or take~~

        IsPenguin = true;
        
        MoveDirection(PlayerDir, 0.5f);
        ASFX.PlayRoar();
        AA.PlayAnimation("PenguinAct");
        int TimeOut = 0;
        while (Vector3.Distance(transform.position, destinationPos) > 0.5f && TimeOut < 25 && !GrabbedPlayer) 
        {
            TimeOut++;
            yield return new WaitForFixedUpdate();
        }

        
        //MoveDirection(0);
        IsPenguin = false;

        if (GrabbedPlayer)
        {
            CC.SetFocusObject(gameObject);
            CC.DampenSet(true);
            PM.transform.position = transform.position;

            Vector2 FinalPos = transform.position + PlayerDir * 12.5f; // 12.5 found by 50 fixedupdates per second over 0.5 second at 0.5 units is 12.5

            RaycastHit2D FloorCheck = Physics2D.Linecast(FinalPos + FaceToDir(FD) * 6, FinalPos + FaceToDir(FD) * 6.1f, FloorMask);

            if (FloorCheck)
            {
                AA.PlayAnimation("PenguinGrab"); // play the grab animation
                yield return new WaitForSeconds(0.25f);
                ASFX.PlayGoSpin();
                CC.Shake(0.15f, 5);
                yield return new WaitForSeconds(0.25f);

                MoveDirection(0);
                CC.Shake(0.25f, 15);
                PM.GotHit(new Damage(PENGUIN_DMG, gameObject)); // deal the grab damage
                ASFX.PlaySlam();

                yield return new WaitForSeconds(1.6f);


                ASFX.EnableLaser(false);

                PM.LetGo(transform.position + (Vector3)FaceToDir(FD) * 6); // put player down and put him a spot
                GrabbedPlayer = false;
                CC.ResetFocusObject();
                CC.DampenSet(false);
                //yield return new WaitForSeconds(StallTime - 0.25f);
                MoveDirection(0);
                AA.PlayAnimation("PenguinRecover");
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                AA.PlayAnimation("PenguinEdge"); // play the grab animation
                yield return new WaitForSeconds(0.25f);
                ASFX.PlayGoSpin();
                CC.Shake(0.15f, 5);
                yield return new WaitForSeconds(0.25f);

                MoveDirection(0);
                CC.Shake(0.25f, 15);

                PM.LetGo(transform.position + (Vector3)FaceToDir(FD) * 6); // put player down and put him a spot
                GrabbedPlayer = false;
                CC.ResetFocusObject();
                CC.DampenSet(false);
                //yield return new WaitForSeconds(StallTime - 0.25f);
                MoveDirection(0);
                AA.PlayAnimation("PenguinRecover");

                yield return new WaitForSeconds(1.6f);


                
            }
            
            
        }
        else
        {
            MoveDirection(0);
            ASFX.PlayLand();
            AA.PlayAnimation("PenguinMiss");
            yield return new WaitForSeconds(1.08f);
            PlayIdleAnimation();
            yield return new WaitForSeconds(StallTime);
        }

        
        
        
        PlayIdleAnimation();

        SetAttacking(false);


    }
    public IEnumerator PenguinAttackP2()
    {
        yield return StartCoroutine(PenguinAttackP2(1.5f));
    }

    public void AddSwipeAttackP2()
    {
        StartCoroutine(SwipeAttackP2(0.25f));
    }
    public IEnumerator SwipeAttackP2(float StallTime) // The attack where he goes fwap!
    {
        SetAttacking(true);
        SetFaceToPlayer();
        Vector3 MoveDir = Player.transform.position - transform.position;
        if(!GetCliff(MoveDir, 1))
        {
            MoveDirection(MoveDir, 0.5f);
        }
        

        yield return new WaitForSeconds(0.125f);

        MoveDirection(0);
        SetFaceToPlayer();
        AA.PlayAnimation("SwipeWindP2");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.6f); // Windup (changed from 0.45f)
        IsFlinchable = true;
        IsParryable = false;
        AA.PlayAnimation("SwipeActP2");

        Vector3 HitDirection = -transform.up;
        bool HitSomething = false;
        if (HitBoxToCast(SwipeIncomingHB[(int)FD]))  
        {
            int kbd = 10;
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if(PM.GotHit(new Damage(SWIPE_DMG_P2, gameObject, HitDirection, 0.85f, kbd)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                HitSomething = true;
            }
        }
        // could add delay between inital swipe zone 
        else if (HitBoxToCast(SwipeMidHB[(int)FD]))  
        {
            int kbd = 10;
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P2, gameObject, HitDirection, 0.85f, kbd)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                HitSomething = true;
            }
        }
        Vector2 SpawnPos = Vector2.zero;
        Vector2 SpawnDir = Vector2.zero;
        switch (FD)
        {
            case FacingDirection.Down:
                SpawnPos = new Vector2(transform.position.x - 0.4f, transform.position.y - 2f);
                SpawnDir = -transform.up;
                break;
            case FacingDirection.Right:
                SpawnPos = new Vector2(transform.position.x + 2f, transform.position.y - 0.1f);
                SpawnDir = transform.right;
                break;
            case FacingDirection.Up:
                SpawnPos = new Vector2(transform.position.x , transform.position.y + 2f);
                SpawnDir = transform.up;
                break;
            case FacingDirection.Left:
                SpawnPos = new Vector2(transform.position.x - 2f, transform.position.y - 0.1f);
                SpawnDir = -transform.right;
                break;
        }
        GameObject SW = Instantiate(SwipeWave, SpawnPos, Quaternion.identity);
        SW.transform.up = SpawnDir;

        if (HitSomething)
            ASFX.PlaySwipeHit();
        else
            ASFX.PlaySwipeNoHit();


        
        yield return new WaitForSeconds(StallTime); // 0.15f default

        
        

        PlayIdleAnimation();
        
        SetAttacking(false);
        
    }
    public IEnumerator SwipeAttackP2()
    {
        yield return StartCoroutine(SwipeAttackP2(0.15f));
    }

    public void AddJumpSwipeAttackP2()
    {
        StartCoroutine(JumpSwipeAttackP2(0.25f));
    }
    public IEnumerator JumpSwipeAttackP2(float StallTime) // The attack where he goes fwap, dodge!
    {
        SetAttacking(true);
        SetFaceToPlayer();

        Vector3 MoveDir = -FaceToDir(FD);
        if (GetCliff(MoveDir, 6))
        {
            PlayIdleAnimation();
            SetAttacking(false);
            yield break;
        }

        yield return new WaitForSeconds(0.125f);
        AA.PlayAnimation("BackSwipeWind2");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.45f); // Windup
        IsFlinchable = true;
        IsParryable = false;
        bool HitSomething = false;
        Vector2 HitDirection = Vector2.zero;
        
        if (HitBoxToCast(JumpSwipeHB[(int)FD]))
        {
            int kbd = 10;
            
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P2, gameObject, HitDirection, 0.85f, kbd)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                HitSomething = true;
            }
        }
        

        AA.PlayAnimation("BackSwipeAct2");

        if (HitSomething)
            ASFX.PlaySwipeHit();
        else
            ASFX.PlaySwipeNoHit();

        MoveDirection(MoveDir, 1.45f);
        yield return new WaitForSeconds(0.075f);
        MoveDirection(0);
        yield return new WaitForSeconds(StallTime); // 0.25f default
        PlayIdleAnimation();
        
        
        SetAttacking(false);

    }
    public IEnumerator JumpSwipeAttackP2()
    {
        yield return StartCoroutine(JumpSwipeAttackP2(0.25f));
    }

    public void AddTwoArmSwipeAttackP2()
    {
        StartCoroutine(TwoArmSwipeAttackP2(0.5f));
    }
    public IEnumerator TwoArmSwipeAttackP2(float StallTime) // The attack where he goes FWAP!
    {
        SetFaceToPlayer();
        SetAttacking(true);
        AA.PlayAnimation("ArmRipP2");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.6f); // Arm Rip
        IsParryable = true;
        
        AA.PlayAnimation("2SwipeWind");
        yield return new WaitForSeconds(0.35f);
        IsFlinchable = true;
        IsParryable = false;
        AA.PlayAnimation("2SwipeAct");
        yield return new WaitForSeconds(0.06f);
        bool HitSomething = false;
        if (HitBoxToCast(Swipe2IncomingHB[(int)FD]))  
        {
            int kbd = 12;
            float kbs = 1;
            switch (FD)
            {
                case FacingDirection.Down:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, transform.right, kbs, kbd));
                    
                    break;
                case FacingDirection.Right:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, kbs, kbd));
                    
                    break;
                case FacingDirection.Up:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.right, kbs, kbd));
                   
                    break;
                case FacingDirection.Left:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, kbs, kbd));
                    
                    break;
            }
            HitSomething = true;
        }

        else if(HitBoxToCast(Swipe2MidHB[(int)FD]))  
        {
            int kbd = 12;
            float kbs = 1;
            switch (FD)
            {
                case FacingDirection.Down:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, transform.right, kbs, kbd));

                    break;
                case FacingDirection.Right:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, kbs, kbd));

                    break;
                case FacingDirection.Up:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.right, kbs, kbd));

                    break;
                case FacingDirection.Left:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, kbs, kbd));

                    break;
            }
            HitSomething = true;
        }

        yield return new WaitForSeconds(0.15f);

        if (HitBoxToCast(Swipe2OutHB[(int)FD]))
        {
            int kbd = 12;
            float kbs = 1;
            switch (FD)
            {
                case FacingDirection.Down:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, transform.right, kbs, kbd));

                    break;
                case FacingDirection.Right:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, kbs, kbd));

                    break;
                case FacingDirection.Up:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.right, kbs, kbd));

                    break;
                case FacingDirection.Left:
                    PM.GotHit(new Damage(TWOSWIPE_DMG, gameObject, -transform.up, 0.85f, kbd));

                    break;
            }
            HitSomething = true;
        }
        if (HitSomething)
            ASFX.PlayTwoSwipeHit();
        else
            ASFX.PlayTwoSwipeNoHit();

        

        if (Vector2.Distance(transform.position, Player.transform.position) < 8)
        {
            yield return new WaitForSeconds(StallTime); // 0.5f default
            yield return StartCoroutine(TwoArmJumpSlamAttackP2(StallTime));
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(ArmThrowAttackP2(StallTime));
        }

        
        
    }
    public IEnumerator TwoArmSwipeAttackP2()
    {
        yield return StartCoroutine(TwoArmSwipeAttackP2(0.5f));
    }

    public void AddTwoArmJumpSlamAttack()
    {
        StartCoroutine(TwoArmJumpSlamAttackP2());
    }
    public IEnumerator TwoArmJumpSlamAttackP2(float StallTime)
    {
        SetFaceToPlayer();
        SetAttacking(true);
        
        IsFlinchable = false;
        AA.PlayAnimation("JumpSlamWind");
        StapVulnerable = true;
        yield return new WaitForSeconds(0.3f);

        Vector3 MoveDir = Player.transform.position - transform.position;
        if(!GetCliff(MoveDir, 1f))
        {
            MoveDirection(MoveDir, 0.9f);

            yield return new WaitForSeconds(0.15f * Vector2.Distance(transform.position, Player.transform.position) / 10);


            MoveDirection(0);
        }
        

        IsFlinchable = true;
        IsParryable = false;
        

        AA.PlayAnimation("JumpSlamAct");
        yield return new WaitForSeconds(0.1f); //cuz headlslam act needs a slight pause

        StapVulnerable = false;
        StartCoroutine(TvDrops(1, 0.05f));
        ASFX.PlaySlam();
        CC.Shake(0.25f, 10);

        if (HitBoxToCast(MidSlamHB[(int)FD]))
        {
            if (PM.GotHit(new Damage(BIGSLAM_DMG_P2, gameObject)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
            }
        }

        

        yield return new WaitForSeconds(StallTime); // 2 default
        AA.PlayAnimation("JumpSlamRecovery");
        yield return new WaitForSeconds(0.33f);

        
        

        PlayIdleAnimation();


        SetAttacking(false);
    }
    public IEnumerator TwoArmJumpSlamAttackP2()
    {
        yield return StartCoroutine(TwoArmJumpSlamAttackP2(1.75f));
    }

    public void AddHeadSlamAttackP2()
    {
        StartCoroutine(HeadSlamAttackP2(3, 2));
    }
    public IEnumerator HeadSlamAttackP2(int Repeat, float StallTime)
    {
        SetAttacking(true);
        SetFaceToPlayer();
        ASFX.PlayHeadSlamWind();
        AA.PlayAnimation("HeadSlamWindP2");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.65f); // Windup
        AA.PlayAnimation("HeadSlamActP2");
        yield return new WaitForSeconds(0.05f); // slight windup on act
        IsFlinchable = true;
        IsParryable = false;
        StartCoroutine(TvDrops(1, 0.05f));
        ASFX.PlaySlam();
        if (HitBoxToCast(MidSlamHB[(int)FD]))
        {
            if (PM.GotHit(new Damage(BIGSLAM_DMG_P2, true, gameObject)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
            }
        }
        yield return new WaitForSeconds(0.05f);
        Vector3 spawnPos = new Vector3(0,-3);
        switch (FD)
        {
            case FacingDirection.Down:
                spawnPos = new Vector3(0, -3) + transform.position;
                break;
            case FacingDirection.Right:
                spawnPos = new Vector3(3, 0) + transform.position;
                break;
            case FacingDirection.Up:
                spawnPos = new Vector3(0, 1) + transform.position;
                break;
            case FacingDirection.Left:
                spawnPos = new Vector3(-3, 0) + transform.position;
                break;
        }
        Instantiate(ShockwaveCircle, spawnPos, Quaternion.identity);

        ResetBodyBoxes();
        HeadSlamBodyBox[(int)FD].enabled = true;
        yield return new WaitForSeconds(0.25f);

        if(Repeat > 1)
        {
            for(int i = 1; i < Repeat; i++)
            {
                AA.PlayAnimation("HeadSlamWindP2");
                IsParryable = true;
                IsFlinchable = false;
                yield return new WaitForSeconds(0.65f); // Windup
                AA.PlayAnimation("HeadSlamActP2");
                yield return new WaitForSeconds(0.05f); // slight windup on act
                IsFlinchable = true;
                IsParryable = false;
                StartCoroutine(TvDrops(1, 0.05f));
                ASFX.PlaySlam();
                if (HitBoxToCast(MidSlamHB[(int)FD]))
                {
                    if (PM.GotHit(new Damage(BIGSLAM_DMG_P2, true, gameObject)))
                    {
                        GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                        HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                    }
                }
                yield return new WaitForSeconds(0.05f);
                spawnPos = new Vector3(0, -3);
                switch (FD)
                {
                    case FacingDirection.Down:
                        spawnPos = new Vector3(0, -3) + transform.position;
                        break;
                    case FacingDirection.Right:
                        spawnPos = new Vector3(3, 0) + transform.position;
                        break;
                    case FacingDirection.Up:
                        spawnPos = new Vector3(0, 1) + transform.position;
                        break;
                    case FacingDirection.Left:
                        spawnPos = new Vector3(-3, 0) + transform.position;
                        break;
                }
                Instantiate(ShockwaveCircle, spawnPos, Quaternion.identity);

                ResetBodyBoxes();
                HeadSlamBodyBox[(int)FD].enabled = true;
                yield return new WaitForSeconds(0.25f);
            }
        }
        yield return new WaitForSeconds(StallTime); // 2 default
        AA.PlayAnimation("HeadSlamRecoveryP2");
        yield return new WaitForSeconds(0.33f);

        HeadSlamBodyBox[(int)FD].enabled = false;
        SetFaceTo(FD);

        PlayIdleAnimation();


        SetAttacking(false);
    }
    public IEnumerator HeadSlamAttackP2()
    {
        yield return StartCoroutine(HeadSlamAttackP2(0, 2));
    }

    bool LaserAim(int type, Phase p)
    {
        float LSRotZ = LaserSpr.transform.rotation.eulerAngles.z;
        int LowerSort = AA.GetComponent<SpriteRenderer>().sortingOrder - 1;
        int HigherSort = LowerSort + 2;
        

        SpriteRenderer LSSR = LaserSpr.GetComponent<SpriteRenderer>();

        

        if (LSRotZ > 315 && LSRotZ < 330) // DownLeft
        {

            LaserAnimationSwitch(1, type, p);

            if(p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[0];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[0];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ > 30 && LSRotZ < 45) // Down Right
        {
            
            LaserAnimationSwitch(9, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[1];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[8];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ >= 45 && LSRotZ < 90) // Right
        {
            LaserAnimationSwitch(8, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[2];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[7];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ >= 90 && LSRotZ < 110) // Hard Right
        {
            LaserAnimationSwitch(7, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[3];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[6];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ >= 110 && LSRotZ < 160) // Up Right
        {
            LaserAnimationSwitch(6, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[4];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[5];
            }
            LSSR.sortingOrder = LowerSort;
            return true;
        }
        else if (LSRotZ > 200 && LSRotZ < 250) // Up Left
        {
            LaserAnimationSwitch(4, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[5];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[3];
            }
            LSSR.sortingOrder = LowerSort;
            return true;
        }
        else if (LSRotZ >= 250 && LSRotZ < 270) // Hard Left
        {
            LaserAnimationSwitch(3, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[6];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[2];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ >= 270 && LSRotZ <= 315) // Left
        {
            LaserAnimationSwitch(2, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[7];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[1];
            }
            LSSR.sortingOrder = HigherSort;
            return true;
        }
        else if (LSRotZ >= 160 && LSRotZ <= 200) // Up 
        {
            LaserAnimationSwitch(5, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[8];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[4];
            }
            LSSR.sortingOrder = LowerSort;
            return true;
        }
        else if (LSRotZ >= 330 || LSRotZ <= 30) // Down 
        {
            LaserAnimationSwitch(0, type, p);

            if (p <= Phase.Second)
            {
                LaserSpr.transform.localPosition = Phase1LaserPos[9];
            }
            else
            {
                LaserSpr.transform.localPosition = Phase3LaserPos[9];
            }
            LSSR.sortingOrder = HigherSort;
            return false;
        }
        else
        {
            Debug.LogError("LASER DIRECTION NOT FOUND");
            return true;
        }
    }
    void LaserAnimationSwitch(int Direction, int Type, Phase P)
    {
        
        switch (P)
        {
            case Phase.First:
                switch (Direction)
                {
                    case 0:
                        AA.PlayAnimation("LaserD");
                        break;
                    case 1:
                        AA.PlayAnimation("LaserDL");
                        break;
                    case 2:
                        AA.PlayAnimation("LaserL");
                        break;
                    case 3:
                        AA.PlayAnimation("LaserHL");
                        break;
                    case 4:
                        AA.PlayAnimation("LaserUL");
                        break;
                    case 5:
                        AA.PlayAnimation("LaserU");
                        break;
                    case 6:
                        AA.PlayAnimation("LaserUR");
                        break;
                    case 7:
                        AA.PlayAnimation("LaserHR");
                        break;
                    case 8:
                        AA.PlayAnimation("LaserR");
                        break;
                    case 9:
                        AA.PlayAnimation("LaserDR");
                        break;
                }
                break;
            case Phase.Second:
                switch (Direction)
                {
                    case 0:
                        AA.PlayAnimation("Laser2D");
                        break;
                    case 1:
                        AA.PlayAnimation("Laser2DL");
                        break;
                    case 2:
                        AA.PlayAnimation("Laser2L");
                        break;
                    case 3:
                        AA.PlayAnimation("Laser2HL");
                        break;
                    case 4:
                        AA.PlayAnimation("Laser2UL");
                        break;
                    case 5:
                        AA.PlayAnimation("Laser2U");
                        break;
                    case 6:
                        AA.PlayAnimation("Laser2UR");
                        break;
                    case 7:
                        AA.PlayAnimation("Laser2HR");
                        break;
                    case 8:
                        AA.PlayAnimation("Laser2R");
                        break;
                    case 9:
                        AA.PlayAnimation("Laser2DR");
                        break;
                }
                break;
            case Phase.Third:
                switch (Direction)
                {
                    case 0:
                        AA.PlayAnimation("LaserD3");
                        break;
                    case 1:
                        AA.PlayAnimation("LaserDL3");
                        break;
                    case 2:
                        AA.PlayAnimation("LaserL3");
                        break;
                    case 3:
                        AA.PlayAnimation("LaserHL3");
                        break;
                    case 4:
                        AA.PlayAnimation("LaserUL3");
                        break;
                    case 5:
                        AA.PlayAnimation("LaserU3");
                        break;
                    case 6:
                        AA.PlayAnimation("LaserUR3");
                        break;
                    case 7:
                        AA.PlayAnimation("LaserHR3");
                        break;
                    case 8:
                        AA.PlayAnimation("LaserR3");
                        break;
                    case 9:
                        AA.PlayAnimation("LaserDR3");
                        break;
                }
                break;
        }
        
    }

    public void AddLaserShootMixupP2()
    {
        StartCoroutine(LaserShootMixupP2(2));
    }
    public IEnumerator LaserShootMixupP2(float StallTime)
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        ASFX.PlayLaserMixup();
        AA.PlayAnimation("MLaserCharge2");
        IsFlinchable = false;
        yield return new WaitForSeconds(1.4f); // wind up
        yield return StartCoroutine(LaserShoot(2, Phase.Second));
        yield return new WaitForSeconds(StallTime); // default 2

        PlayIdleAnimation();
        
        SetAttacking(false);
    }
    public IEnumerator LaserShootMixupP2()
    {
        yield return StartCoroutine(LaserShootMixupP2(2));
    }

    public void AddLaserShootAttackP2()
    {
        StartCoroutine(LaserShootAttackP2(1));
    }
    public IEnumerator LaserShootAttackP2(float StallTime)
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        ASFX.PlayLaserShot();
        yield return new WaitForFixedUpdate();
        ASFX.PlayLaserShot();

        AA.PlayAnimation("NLaserCharge2");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.8f); // wind up (1.15) original

        
        yield return new WaitForSeconds(0.35f);
        yield return StartCoroutine(LaserShoot(0, Phase.Second));

        yield return new WaitForSeconds(0.3f);
        
        yield return new WaitForSeconds(StallTime); // 1 default
        SetFaceToPlayer();
        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserShootAttackP2()
    {
        yield return StartCoroutine(LaserShootAttackP2(1));
    }


    public void AddLaserBurstAttackP2()
    {
        StartCoroutine(LaserBurstAttackP2(3, 1));
    }
    public IEnumerator LaserBurstAttackP2(int Repeat, float StallTime)
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        ASFX.PlayBurstLaserCharge();
        yield return new WaitForFixedUpdate();
        

        AA.PlayAnimation("BLaserCharge2");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.8f); // wind up (1.15) original


        yield return new WaitForSeconds(0.35f);
        yield return StartCoroutine(LaserShoot(1, Phase.Second));

        yield return new WaitForSeconds(0.3f);
        if (Repeat > 0)
        {
            for (int i = 1; i < Repeat; i++)
            {
                yield return StartCoroutine(LaserShoot(1, Phase.Second));
                //AA.PlayAnimation(null);
                yield return new WaitForSeconds(0.3f);
            }
        }
        yield return new WaitForSeconds(StallTime); // 1 default
        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserBurstAttackP2()
    {
        yield return StartCoroutine(LaserBurstAttackP2(0, 1));
    }

    IEnumerator LaserShoot(int Type, Phase p)
    {
        IsFlinchable = true;

        LaserSpr.transform.up = (LaserSpr.transform.position - Player.transform.position).normalized;
        bool AimSwitch = LaserAim(Type, p);
        // record previous rotation, record clamps on rotation using LASER_Z_RANGE constant
        Quaternion prevRot = LaserSpr.transform.rotation;

        float ClampRange = LASER_Z_RANGE;
        if (p == Phase.Third)
            ClampRange += 30;

        LaserZMin = LaserSpr.transform.rotation.eulerAngles.z - ClampRange;
        LaserZMax = LaserSpr.transform.rotation.eulerAngles.z + ClampRange;
        if (LaserZMax > 360) // minus 360 to account for overflow
        {
            LaserZMax -= 360;

        }
        if (LaserZMin < 0) // add 360 to account for overflow
        {
            LaserZMin += 360;
        }

        if (AimSwitch && HitCircleToCast(GetOffSpinHB))
        {
            //PM.GotHit(new Damage(GOSPIN_DMG, gameObject, (PM.transform.position - transform.position).normalized, 1f, 8));
        }

        yield return new WaitForSeconds(0.1f);

        LaserSpr.transform.up = (LaserSpr.transform.position - Player.transform.position).normalized;
        float LaserRotZ = LaserSpr.transform.rotation.eulerAngles.z;

        bool RotationCheck = LaserRotZ > LaserZMin && LaserRotZ < LaserZMax; // check if rotation is within bounds of clamps
        if (LaserZMin > LaserZMax) // if one of the clamps previously overflowed but not the other
            RotationCheck = LaserRotZ > LaserZMin || LaserRotZ < LaserZMax; // then the rotation only needs to fulfill one of the clamps
        if (!RotationCheck) // if the rotation fails the clamp check then revert to previously recorded rotation
        {
            LaserSpr.transform.rotation = prevRot;
        }

        if(Type == 1)
        {
            IsBurstLasering = true;
        }
        else
        {
            IsLasering = true;
        }
        
        LaserSpr.GetComponent<SpriteRenderer>().enabled = true;
        ASFX.EnableLaser(true);
        yield return new WaitForSeconds(0.1f); // frames
        
        LaserSpr.transform.localPosition = new Vector3(0, 0);
        LaserSpr.GetComponent<SpriteRenderer>().enabled = false;
        IsLasering = false;
        IsBurstLasering = false;

        yield return new WaitForSeconds(0.1f); // frames
        ASFX.EnableLaser(false);

        if (CurrentPhase >= Phase.Second)
        {
            //Instantiate(LaserStatic, LaserHB.transform.position, LaserHB.transform.rotation);
        }
        
    }

    public void AddRocketShipAttackP2()
    {
        StartCoroutine(RocketShipAttackP2(2));
    }
    public IEnumerator RocketShipAttackP2(float StallTime)
    {
        SetFaceTo(FacingDirection.Down);
        SetAttacking(true);
        ASFX.PlayChargeUp();
        AA.PlayAnimation("RocketWindP2");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.25f);
        ResetBodyBoxes();
        RocketBodyBox[0].enabled = true;

        yield return new WaitForSeconds(0.75f); // Wind up
        RocketBodyBox[0].enabled = false;


        ASFX.EnableLaser(true);
        CC.Shake(0.05f, 500);
        
        AA.PlayAnimation("RocketActP2");
        IsMainRocket = true;
        IsInvulnerable = true;

        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f); // blast offf!!!11!!1!
        RocketSpin.transform.rotation = Quaternion.Euler(0, 0, 0);
        RocketMainSpr.transform.rotation = Quaternion.Euler(0, 0, 0);
        AA.GetComponent<EnemyOcclusion>().LockOcclusion(12);
        StartCoroutine(TvDrops(5, 0.15f));
        IsAltRockets = true;

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = true;
        
        for (int i = 0; i < 350; i++)
        {
            float SpeedFactor = 0.050f;
            float MaxSpeed = 1.15f; // was 1.55
            if(SpeedFactor * i >= MaxSpeed)
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + MaxSpeed);
                
            }
            else
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + SpeedFactor * i);
            }

            RocketSecondarySpr.transform.rotation = Quaternion.Euler(0, 0, RocketSecondarySpr.transform.rotation.eulerAngles.z + MaxSpeed * 0.8f);
            RocketSpinMain.transform.rotation = Quaternion.Euler(0, 0, RocketSpinMain.transform.rotation.eulerAngles.z - MaxSpeed);

            yield return new WaitForSeconds(0.01f);
        }
        // however long this attack lasts

        AA.GetComponent<EnemyOcclusion>().UnlockOcclusion();
        ASFX.EnableLaser(false);
        AA.PlayAnimation("RocketCDP2");
        IsMainRocket = false;
        IsAltRockets = false;
        
        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = false;
        
        yield return new WaitForSeconds(0.45f); 
        ASFX.PlayLand();
        IsFlinchable = true;
        RocketBodyBox[1].enabled = true;

        CC.Shake(0.4f, 10);
        IsInvulnerable = false;
        yield return new WaitForSeconds(StallTime); // default 2
        AA.PlayAnimation("RocketRecoveryP2");
        yield return new WaitForSeconds(0.4f); // recovery
        SetFaceTo(FD);

        //PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator RocketShipAttackP2()
    {
        yield return StartCoroutine(RocketShipAttackP2(2));
    }

    public void AddArmThrowAttackP2()
    {
        StartCoroutine(ArmThrowAttackP2(1));
    }
    public IEnumerator ArmThrowAttackP2(float StallTime)
    {
        
        SetAttacking(true);
        //AA.PlayAnimation("ArmRip");
        IsFlinchable = false;
        IsParryable = true;
        //yield return new WaitForSeconds(0.6f); // wind up
        AA.PlayAnimation("ThrowWindP2");
        yield return new WaitForSeconds(0.5f);
        SetFaceToPlayer();
        IsParryable = false;
        ASFX.PlayRoar();
        AA.PlayAnimation("ThrowActP2");
        yield return new WaitForSeconds(0.1f);
        IsFlinchable = true;
        GameObject newArm = Instantiate(ThrowableArm, transform.position, Quaternion.identity);
        newArm.transform.up = GetPlayerDir(); // didn't do this in instantiate because im lazy and can't figure out how to convert a vector to quaternion in 2d *shrug*
        newArm.GetComponent<ThrowingArm>().Destination(0.85f, Player.transform.position + GetPlayerDir() * 5); // throws it past the player
        newArm.GetComponent<ThrowingArm>().SetDamage(THROW_DMG, PM);
        SetFaceToPlayer();
        yield return new WaitForSeconds(StallTime); // default 1
        PlayIdleAnimation();

        


        SetAttacking(false);
    }
    public IEnumerator ArmThrowAttackP2()
    {
        yield return StartCoroutine(ArmThrowAttackP2(1));
    }

    //pretty much exclusively for hand kelp now
    public void AddPoleSpinAttackP2() // not called for anything but debug
    {
        PoleSpinSetUp(transform.position, 1.5f);
    }
    public IEnumerator PoleSpinAttackP2(Vector2 Point, float StallTime) // call pole spin set up instead of this
    {
        // this is clearly a very confusing attack routine but, it works by moving addiction using animation key events
        // IsStripping is set to true and so anything that he touches while moving in this ring is hit
        // most of the heavy lifting is done by the animation and the PoleMove method
        
        SetFaceTo(FacingDirection.Down);
        
        IsFlinchable = false;
        transform.position = new Vector3(Point.x, Point.y , 0); // set position to left and down of the hand kelp
        
        IsFlinchable = true;
        AA.SetLock(false);
        
        poleFlightPos = transform.position; 
        AA.SetPosition(poleFlightPos);
        IsStripping = true;


        //based on which direction addiction approaches the kelp he chooses a different starting point for the animation 0/8 = dr, 1/8 down, 2/8 dl, 3/8 left, etc
        float pointX =  Point.x - entryPos.x; // the difference between the x of the point and addiction
        float pointY =  Point.y - entryPos.y;

        String animName;

        if(CurrentPhase == Phase.First)
        {
            animName = "FullSpin";
        }
        else
        {
            animName = "FullSpinP2";
        }

        if (Mathf.Abs(pointX) < 0.1f) // check if the point of contact is along the middle vertical line
        {
            if (pointY > 0) // check if the point of contact is above the horizontal line
            {
                AA.PlayAnimation(animName, 1 / 8f); // down
            }
            else
            {
                AA.PlayAnimation(animName, 5 / 8f); // up
            }
        }
        else if(Mathf.Abs(pointY) < 0.1f) // check if the point of contact is along the middle horizontal line
        {
            if (pointX > 0) // check if the point of contact is to the right of the vertical line
            {
                AA.PlayAnimation(animName, 7 / 8f); // right
            }
            else
            {
                AA.PlayAnimation(animName, 3 / 8f); // left
            }
        }
        else if(pointX > 0) // check if the point of contact is to the right of the vertical line
        {
            if (pointY > 0) // check if the point of contact is above the horizontal line
            {
                AA.PlayAnimation(animName, 6 / 8f); // up right
            }
            else
            {
                AA.PlayAnimation(animName, 0 / 8f); // down right
            }
        }
        else
        {
            if (pointY > 0) // check if the point of contact is above the horizontal line
            {
                AA.PlayAnimation(animName, 4 / 8f); // up left
            }
            else
            {
                AA.PlayAnimation(animName, 2 / 8f); // down left
            }
        }
        

        
        yield return new WaitForSeconds(1f);
        // give him some time to spin

        int i = 0;
        while (IsComing && i < 80) // if the player is lined up properly OR time is up
        {
            i++;
            yield return new WaitForFixedUpdate();
        }
        
        IsStripping = false;
        MoveDirection(GetPlayerDir(), 1.25f);
        IsPoleFlying = true;                
        SetFaceToPlayer();
        AA.PlayAnimation("SpinFlight");
        AA.SetLock(true);
        Vector2 destination = (Player.transform.position + GetPlayerDir() * 10);
        while (IsPoleFlying && Vector2.Distance(transform.position, destination) > 2)
        {
            yield return new WaitForFixedUpdate();
        }
        IsPoleFlying = false;

        MoveDirection(0.25f);
        CC.Shake(0.35f, 5);
        AA.PlayAnimation("ChargeSlide"); 
        yield return new WaitForSeconds(0.6f);

        MoveDirection(0f);
        
        PlayIdleAnimation();
        yield return new WaitForSeconds(StallTime); // default 1.5f

        ACM.StartFight();

        SetAttacking(false);
    }
    

    public void PoleMove(float Degree) // Receives events from the "Full Spin" animation clip, ordered in same way as events (from top to bottom)
    {
        
        float LargeVal = 6;
        float SmallVal = 3;
        switch (Degree)
        {
            case 45:
                if(PoleSpinAudioTimer)
                {
                    ASFX.PlayPoleSpin();
                }
                PoleSpinAudioTimer = false;
                animFlightPos = poleFlightPos + new Vector3(SmallVal, -SmallVal);
                break;
            case 0:
                PoleSpinAudioTimer = true;
                animFlightPos = poleFlightPos + new Vector3(0, -LargeVal);
                break;
            case 315:
                animFlightPos = poleFlightPos + new Vector3(-SmallVal, -SmallVal);
                break;
            case 270:
                animFlightPos = poleFlightPos + new Vector3(-LargeVal, 0);
                break;
            case 225:
                if (PoleSpinAudioTimer)
                {
                    ASFX.PlayPoleSpin();
                }
                PoleSpinAudioTimer = false;
                animFlightPos = poleFlightPos + new Vector3(-SmallVal, SmallVal);
                break;
            case 180:
                PoleSpinAudioTimer = true;
                animFlightPos = poleFlightPos + new Vector3(0, LargeVal);
                break;
            case 135:
                animFlightPos = poleFlightPos + new Vector3(SmallVal, SmallVal);
                break;
            case 90:
                animFlightPos = poleFlightPos + new Vector3(LargeVal, 0);
                break;
        }
    }



    #endregion


    #region Phase3Attacks

    public void AddTVThrowAttackP3()
    {
        StartCoroutine(TVThrowAttackP3());
    }
    public IEnumerator TVThrowAttackP3()
    {
        SetAttacking(true);
        yield return new WaitForSeconds(0.5f);
        SpawnBigTV(Player.transform.position.x, Player.transform.position.y);
        SpawnBigTV(Player.transform.position.x + 5, Player.transform.position.y);
        SpawnBigTV(Player.transform.position.x - 5, Player.transform.position.y);
        SpawnBigTV(Player.transform.position.x, Player.transform.position.y + 5);
        SpawnBigTV(Player.transform.position.x, Player.transform.position.y - 5);
        yield return new WaitForSeconds(0.25f);
        SetAttacking(false);
    }

    public void AddArmThrowAttackP3()
    {
        StartCoroutine(ArmThrowAttackP2(1));
    }
    public IEnumerator ArmThrowAttackP3(float StallTime)
    {

        SetAttacking(true);
        AA.PlayAnimation("ArmRip");
        IsFlinchable = false;
        IsParryable = true;
        yield return new WaitForSeconds(0.6f); // wind up
        AA.PlayAnimation("ThrowWind");
        yield return new WaitForSeconds(0.5f);
        
        IsParryable = false;
        ASFX.PlayRoar();
        AA.PlayAnimation("ThrowAct");
        yield return new WaitForSeconds(0.1f);
        IsFlinchable = true;
        GameObject newArm = Instantiate(ThrowableArm, transform.position, Quaternion.identity);
        newArm.transform.up = GetPlayerDir(); // didn't do this in instantiate because im lazy and can't figure out how to convert a vector to quaternion in 2d *shrug*
        newArm.GetComponent<ThrowingArm>().Destination(0.85f, Player.transform.position + GetPlayerDir() * 5); // throws it past the player
        newArm.GetComponent<ThrowingArm>().SetDamage(THROW_DMG, PM);
        
        yield return new WaitForSeconds(StallTime); // default 1
        PlayIdleAnimation();




        SetAttacking(false);
    }
    public IEnumerator ArmThrowAttackP3()
    {
        yield return StartCoroutine(ArmThrowAttackP2(1));
    }

    public void AddRocketShipAttackP3()
    {
        StartCoroutine(RocketShipAttackP3(2));
    }
    public IEnumerator RocketShipAttackP3(float StallTime)
    {
        
        SetAttacking(true);
        ASFX.PlayChargeUp();
        AA.PlayAnimation("RocketWind");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.25f);
        ResetBodyBoxes();
        RocketBodyBox[0].enabled = true;

        yield return new WaitForSeconds(0.75f); // Wind up
        RocketBodyBox[0].enabled = false;


        ASFX.EnableLaser(true);
        CC.Shake(0.05f, 500);

        AA.PlayAnimation("RocketAct");
        IsMainRocket = true;
        IsInvulnerable = true;

        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f); // blast offf!!!11!!1!
        RocketSpin.transform.rotation = Quaternion.Euler(0, 0, 0);
        RocketMainSpr.transform.rotation = Quaternion.Euler(0, 0, 0);
        AA.GetComponent<EnemyOcclusion>().LockOcclusion(12);
        StartCoroutine(TvDrops(5, 0.15f));
        IsAltRockets = true;
        IsDiaRockets = true;

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = true;
        RocketDiagonalSpr1.GetComponent<SpriteRenderer>().enabled = true;
        RocketDiagonalSpr2.GetComponent<SpriteRenderer>().enabled = true;

        for (int i = 0; i < 350; i++)
        {
            float SpeedFactor = 0.010f;
            float MaxSpeed = 1f;
            if (SpeedFactor * i >= MaxSpeed)
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + MaxSpeed);

            }
            else
            {
                RocketSpin.transform.rotation = Quaternion.Euler(0, 0, RocketSpin.transform.rotation.eulerAngles.z + SpeedFactor * i);
            }

            RocketSecondarySpr.transform.rotation = Quaternion.Euler(0, 0, RocketSecondarySpr.transform.rotation.eulerAngles.z + MaxSpeed * 0.8f);
            RocketSpinMain.transform.rotation = Quaternion.Euler(0, 0, RocketSpinMain.transform.rotation.eulerAngles.z - MaxSpeed);

            yield return new WaitForSeconds(0.01f);
        }
        // however long this attack lasts

        AA.GetComponent<EnemyOcclusion>().UnlockOcclusion();
        ASFX.EnableLaser(false);
        AA.PlayAnimation("RocketCD");
        IsMainRocket = false;
        IsAltRockets = false;
        IsDiaRockets = false;

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketDiagonalSpr1.GetComponent<SpriteRenderer>().enabled = false;
        RocketDiagonalSpr2.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.45f);
        ASFX.PlayLand();
        IsFlinchable = true;
        RocketBodyBox[1].enabled = true;

        CC.Shake(0.4f, 10);
        IsInvulnerable = false;
        yield return new WaitForSeconds(StallTime); // default 2
        AA.PlayAnimation("RocketRecovery");
        yield return new WaitForSeconds(0.4f); // recovery
        SetPhase3BodyBox();

        //PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator RocketShipAttackP3()
    {
        yield return StartCoroutine(RocketShipAttackP3(2));
    }

    public void AddCloseLaserSweepAttackP3()
    {
        StartCoroutine(CloseLaserSweepAttackP3());
    }
    public IEnumerator CloseLaserSweepAttackP3()
    {
        SetAttacking(true);
        
        ASFX.PlayLaserShot();
        AA.PlayAnimation("LaserChargeUp");
        IsFlinchable = false;
        yield return new WaitForSeconds(1.15f); // wind up

        CC.Shake(0.05f, 500);
        IsLasering = true;
        LaserSpr.GetComponent<SpriteRenderer>().enabled = true;
        LaserSpr.transform.position = transform.position;
        LaserSpr.transform.rotation = Quaternion.Euler(0, 0, 90);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 60; i++)
        {
            LaserSpr.transform.rotation = Quaternion.Euler(LaserSpr.transform.rotation.eulerAngles.x, LaserSpr.transform.rotation.eulerAngles.y, LaserSpr.transform.rotation.eulerAngles.z - 6);
            yield return new WaitForFixedUpdate();
        }

        IsFlinchable = true;
        IsLasering = false;
        LaserSpr.GetComponent<SpriteRenderer>().enabled = false;
        SetAttacking(false);
    }

    public void AddFarLaserSweepAttackP3()
    {
        StartCoroutine(FarLaserSweepAttackP3());
    }
    public IEnumerator FarLaserSweepAttackP3()
    {
        SetAttacking(true);
        
        ASFX.PlayLaserShot();
        AA.PlayAnimation("LaserChargeUp");
        IsFlinchable = false;
        yield return new WaitForSeconds(1.15f); // wind up

        CC.Shake(0.05f, 500);
        IsLasering = true;
        LaserSpr.GetComponent<SpriteRenderer>().enabled = true;
        LaserSpr.transform.position = transform.position;
        LaserSpr.transform.up = (LaserSpr.transform.position - Player.transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 360; i++)
        {
            LaserSpr.transform.rotation = Quaternion.Euler(LaserSpr.transform.rotation.eulerAngles.x, LaserSpr.transform.rotation.eulerAngles.y, LaserSpr.transform.rotation.eulerAngles.z - 1);
            yield return new WaitForFixedUpdate();
        }

        IsFlinchable = true;
        IsLasering = false;
        LaserSpr.GetComponent<SpriteRenderer>().enabled = false;
        SetAttacking(false);
    }

    public void AddSwipeAttackP3()
    {
        StartCoroutine(SwipeAttackP3(0.15f));
    }
    public IEnumerator SwipeAttackP3(float StallTime) // The attack where he goes fwap!
    {
        SetAttacking(true);
        
        yield return new WaitForSeconds(0.125f);
        AA.PlayAnimation("SwipeWindP2");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.45f); // Windup
        IsFlinchable = true;
        IsParryable = false;


        Vector3 HitDirection = -transform.up;
        bool HitSomething = false;
        if (HitBoxToCast(SwipeIncomingHB[(int)FD]))
        {
            int kbd = 10;
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P2, gameObject, HitDirection, 0.85f, kbd)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                HitSomething = true;
            }
        }
        // could add delay between inital swipe zone 
        else if (HitBoxToCast(SwipeMidHB[(int)FD]))
        {
            int kbd = 10;
            switch (FD)
            {
                case FacingDirection.Down:
                    HitDirection = transform.right;
                    break;
                case FacingDirection.Right:
                    HitDirection = -transform.up;
                    break;
                case FacingDirection.Up:
                    HitDirection = -transform.right;
                    break;
                case FacingDirection.Left:
                    HitDirection = -transform.up;
                    break;
            }

            if (PM.GotHit(new Damage(SWIPE_DMG_P2, gameObject, HitDirection, 0.85f, kbd)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                HitSomething = true;
            }
        }

        if (HitSomething)
            ASFX.PlaySwipeHit();
        else
            ASFX.PlaySwipeNoHit();


        AA.PlayAnimation("SwipeActP2");
        yield return new WaitForSeconds(StallTime); // 0.15f default




        PlayIdleAnimation();

        SetAttacking(false);

    }
    public IEnumerator SwipeAttackP3()
    {
        yield return StartCoroutine(SwipeAttackP3(0.15f));
    }

    public void AddGetOffSpinAttackP3()
    {
        StartCoroutine(GetOffSpinAttackP3(0.2f));
    }
    public IEnumerator GetOffSpinAttackP3(float StallTime)
    {
        SetAttacking(true);
        
        ASFX.PlayGoSpin();
        AA.PlayAnimation("GOSpinWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.20f); // Windup
        AA.PlayAnimation("GOSpinAct");
        IsFlinchable = true;
        IsParryable = false;

        if (HitCircleToCast(GetOffSpinHB))
        {
            PM.GotHit(new Damage(GOSPIN_DMG, gameObject, (PM.transform.position - transform.position).normalized, 1.5f, 8));
        }


        yield return new WaitForSeconds(StallTime); // 0.2f default

        PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator GetOffSpinAttackP3() // default timing
    {
        yield return StartCoroutine(GetOffSpinAttackP3(0.2f));
    }

    public void AddHeadSlamAttackP3()
    {
        StartCoroutine(HeadSlamAttackP3(2));
    }
    public IEnumerator HeadSlamAttackP3(float StallTime)
    {
        SetAttacking(true);
        
        ASFX.PlayHeadSlamWind();
        AA.PlayAnimation("HeadSlamWindP2");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(0.65f); // Windup
        AA.PlayAnimation("HeadSlamActP2");
        yield return new WaitForSeconds(0.05f); // slight windup on act
        IsFlinchable = true;
        IsParryable = false;
        StartCoroutine(TvDrops(1, 0.05f));
        ASFX.PlaySlam();
        if (HitBoxToCast(MidSlamHB[(int)FD]))
        {
            if (PM.GotHit(new Damage(BIGSLAM_DMG_P2, true, gameObject)))
            {
                GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
            }
        }
        yield return new WaitForSeconds(0.05f);
        Vector3 spawnPos = new Vector3(0, -3);
        switch (FD)
        {
            case FacingDirection.Down:
                spawnPos = new Vector3(0, -3) + transform.position;
                break;
            case FacingDirection.Right:
                spawnPos = new Vector3(3, 0) + transform.position;
                break;
            case FacingDirection.Up:
                spawnPos = new Vector3(0, 1) + transform.position;
                break;
            case FacingDirection.Left:
                spawnPos = new Vector3(-3, 0) + transform.position;
                break;
        }
        Instantiate(ShockwaveCircle, spawnPos, Quaternion.identity);

        ResetBodyBoxes();
        HeadSlamBodyBox[(int)FD].enabled = true;

        yield return new WaitForSeconds(StallTime); // 2 default
        AA.PlayAnimation("HeadSlamRecoveryP2");
        yield return new WaitForSeconds(0.33f);

        HeadSlamBodyBox[(int)FD].enabled = false;
        SetPhase3BodyBox();

        PlayIdleAnimation();


        SetAttacking(false);
    }
    public IEnumerator HeadSlamAttackP3()
    {
        yield return StartCoroutine(HeadSlamAttackP3(2));
    }

    public void AddBigLaserAttackP3()
    {
        StartCoroutine(BigLaserAttackP3(2));
    }
    public IEnumerator BigLaserAttackP3(float StallTime)
    {
        SetAttacking(true);
        SetFaceToPlayer();

        yield return new WaitForSeconds(0.125f);

        if (GetCliff(-FaceToDir(FD), 30))
        {
            PlayIdleAnimation();
            
            SetAttacking(false);
            yield break;
        }
        ASFX.PlayGigaLaserCharge();
        AA.PlayAnimation("GLaserWind"); // change to big laser
        IsParryable = true;
        IsFlinchable = false;

        

        yield return new WaitForSeconds(2.15f); // Windup (3.25 / 2)
        IsFlinchable = true;
        IsParryable = false;

        AA.PlayAnimation("GLaserStillIdle"); // change to big laser
        ASFX.EnableGigaLaser(true);


        BigLaserHB[(int)FD].GetComponent<SpriteRenderer>().enabled = true;
        for(int i = 0; i < 100; i++)
        {
            if (HitBoxToCast(BigLaserHB[(int)FD]))
            {
                if (PM.GotHit(new Damage(BIGLASER_DMG, gameObject)))
                {
                    GameObject HitObject = Instantiate(HitEffectPhase2, Player.transform.position, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(Player.transform.gameObject);
                }
            }

            if (i == 15)
            {
                MoveDirection(-FaceToDir(FD), 0.35f);
                AA.PlayAnimation("GLaserMoveIdle");
                
            }
            yield return new WaitForFixedUpdate();
        }
        ASFX.EnableGigaLaser(false);
        MoveDirection(0);
        BigLaserHB[(int)FD].GetComponent<SpriteRenderer>().enabled = false;
        AA.PlayAnimation("GLaserCD");

        yield return new WaitForSeconds(StallTime - 0.6f); // 1 default

        AA.PlayAnimation("ManicRecover");

        yield return new WaitForSeconds(0.7f);

        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator BigLaserAttackP3()
    {
        yield return StartCoroutine(BigLaserAttackP3(1));
    }

    public void AddManicCrazeAttackP3()
    {
        StartCoroutine(ManicCrazeAttackP3(8f));
    }
    public IEnumerator ManicCrazeAttackP3(float StallTime)
    {
        SetFaceToPlayer();
        SetAttacking(true);

        
        AA.PlayAnimation("ManicFullWindUp");
        IsParryable = true;
        IsFlinchable = false;
        ManicCount = 0;
        yield return new WaitForSeconds(0.15f);
        ASFX.PlaySlam();
        yield return new WaitForSeconds(1.44f - 0.15f);
        ASFX.PlaySlam();
        yield return new WaitForSeconds(1.84f - 1.44f);
        ASFX.PlaySlam();
        yield return new WaitForSeconds((3.60f - 1.84f) / 10);
        for (int i = 0; i < 9; i++)
        {
            ASFX.PlaySlam();
            if(i == 4)
            {
                ASFX.PlayManicLaserCharge();
            }
            yield return new WaitForSeconds((3.60f - 1.84f) / 10);
        }
        //yield return new WaitForSeconds(3.60f - 1.84f);
        // instantiate laser
        ASFX.EnableManicLaser(true);
        GameObject MANICLASER = Instantiate(ManicHugeLaser, transform.position, Quaternion.identity);
        StartCoroutine(TvDrops(16, 0.5f));
        
        yield return new WaitForSeconds(0.55f);
        AA.PlayAnimation("ManicWalkOut");
        
        yield return new WaitForSeconds(0.42f);
        
        
        transform.position = transform.position - transform.up * 2;

        

        SetFaceToPlayer();
        AA.PlayAnimation("ManicRun");
        IsFlinchable = true;
        IsParryable = false;
        IsInvulnerable = false;
        
        manicDir = Player.transform.position - transform.position;
        prevDir = manicDir;
        ManicSpeed = 0.8f;
        IsManic = true;
        IsManicSkidding = false;
        ManicDmg = MANIC_DMG_P3;
        ST.StaticSet(true);
        ST.SetStaticTime(500);
        ST.gameObject.transform.up = manicDir;

        yield return new WaitForSeconds(12f);

        Destroy(CurrentFLaser);
        Destroy(PrevFLaser);

        ASFX.EnableManicLaser(false);
        GameObject LASERDEATH = Instantiate(ManicLaserDeath, MANICLASER.transform.position, Quaternion.identity);
        Destroy(MANICLASER);

        Vector3 fallDir = manicDir.normalized;
        ASFX.PlayRoar();


        IsManic = false;
        IsInvulnerable = false;
        SetFaceToPosition(transform.position + fallDir);

        ResetBodyBoxes();

        ManicCDBodyBox[(int)FD].BoxesToggle(true);

        AA.PlayAnimation("ManicCD");
        MoveDirection(fallDir, 0.35f);
        ST.StaticSet(false);
        //ST.DeleteStatic();
        yield return new WaitForSeconds(0.5f);
        MoveDirection(0.25f);
        yield return new WaitForSeconds(0.5f);
        MoveDirection(0.1f);
        yield return new WaitForSeconds(0.5f);

        MoveDirection(0);

        Destroy(LASERDEATH);
        
        yield return new WaitForSeconds(StallTime - 0.6f); // 4 default

        AA.PlayAnimation("ManicRecover");

        yield return new WaitForSeconds(0.7f);

        SetFaceTo(FD);
        PlayIdleAnimation();
        SetAttacking(false);
    }
    public IEnumerator ManicCrazeAttackP3()
    {
        yield return StartCoroutine(ManicCrazeAttackP3(1f));
    }

    public void AddLaserShootMixupP3()
    {
        StartCoroutine(LaserShootMixupP3(2));
    }
    public IEnumerator LaserShootMixupP3(float StallTime)
    {
        SetAttacking(true);

        Vector3 FlightDir = transform.position - Player.transform.position;

        SetFaceToPosition(transform.position + FlightDir);

       
        
        IsFlinchable = false;
        yield return new WaitForSeconds(0.65f);

        AA.PlayAnimation("P3Jump");
        MoveDirection(FlightDir, 0.7f);

        yield return new WaitForSeconds(0.75f);

        MoveDirection(0);

        SetFaceTo(FacingDirection.Down);
        ASFX.PlayLaserMixup();
        AA.PlayAnimation("MLaserCharge3");
        IsFlinchable = false;
        yield return new WaitForSeconds(1.4f); // wind up
        yield return StartCoroutine(LaserShoot(2, Phase.Third));
        yield return new WaitForSeconds(StallTime); // default 2

        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserShootMixupP3()
    {
        yield return StartCoroutine(LaserShootMixupP3(2));
    }

    public void AddLaserShootAttackP3()
    {
        StartCoroutine(LaserShootAttackP3(1));
    }
    public IEnumerator LaserShootAttackP3(float StallTime)
    {
        SetAttacking(true);

        Vector3 FlightDir = Player.transform.position - transform.position;

        SetFaceToPosition(transform.position + FlightDir);

        

        IsFlinchable = false;

        

        AA.PlayAnimation("P3Jump");
        MoveDirection(FlightDir, 0.9f);

        yield return new WaitForSeconds(0.5f);

        MoveDirection(0);

        SetFaceTo(FacingDirection.Down);
        ASFX.PlayLaserShot();
        yield return new WaitForFixedUpdate();
        ASFX.PlayLaserShot();

        AA.PlayAnimation("NLaserCharge3");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.8f); // wind up (1.15) original


        yield return new WaitForSeconds(0.35f);
        yield return StartCoroutine(LaserShoot(0, Phase.Third));

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(StallTime); // 1 default
        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserShootAttackP3()
    {
        yield return StartCoroutine(LaserShootAttackP3(1));
    }


    public void AddLaserBurstAttackP3()
    {
        StartCoroutine(LaserBurstAttackP3(4, 1));
    }
    public IEnumerator LaserBurstAttackP3(int Repeat, float StallTime)
    {
        SetAttacking(true);
        SetFaceTo(FacingDirection.Down);
        ASFX.PlayBurstLaserCharge();
        yield return new WaitForFixedUpdate();
        

        AA.PlayAnimation("BLaserCharge3");
        IsFlinchable = false;
        yield return new WaitForSeconds(0.8f); // wind up (1.15) original


        yield return new WaitForSeconds(0.35f);
        yield return StartCoroutine(LaserShoot(1, Phase.Third));

        yield return new WaitForSeconds(0.3f);
        if (Repeat > 0)
        {
            for (int i = 1; i < Repeat; i++)
            {
                yield return StartCoroutine(LaserShoot(1, Phase.Third));
                yield return new WaitForSeconds(0.3f);
                
            }
        }
        yield return new WaitForSeconds(StallTime); // 1 default
        PlayIdleAnimation();

        SetAttacking(false);
    }
    public IEnumerator LaserBurstAttackP3()
    {
        yield return StartCoroutine(LaserBurstAttackP3(4, 1));
    }

    #endregion

    public void AddManicCrazeAttackP4()
    {
        StartCoroutine(ManicCrazeAttackP4(0.5f));
    }
    public IEnumerator ManicCrazeAttackP4(float StallTime)
    {
        SetFaceToPlayer();
        SetAttacking(true);
        CC.Shake(0.1f, 10000000000);
        ManicCount = 0;
        AA.PlayAnimation("PenguinWind");
        IsParryable = true;
        IsFlinchable = false;
        yield return new WaitForSeconds(2f);

        SetFaceToPlayer();
        AA.PlayAnimation("ManicRunP4");
        IsFlinchable = true;
        IsParryable = false;
        IsInvulnerable = false;

        ManicDmg = MANIC_DMG_P4;
        manicDir = Player.transform.position - transform.position;
        prevDir = manicDir;
        ManicSpeed = 0.8f;
        IsManic = true;
        IsManicSkidding = false;
        ST.StaticSet(true);
        ST.SetStaticTime(500);
        ST.gameObject.transform.up = manicDir;

        //yield return new WaitForSeconds(8f);
        //ST.StaticSet(false);
        //ST.DeleteStatic();
        //IsManic = false;
        //IsInvulnerable = false;
        //MoveDirection(0);
        //PlayIdleAnimation();
        //yield return new WaitForSeconds(StallTime); // 1.5 default

        //SetAttacking(false);
    }
    public IEnumerator ManicCrazeAttackP4()
    {
        yield return StartCoroutine(ManicCrazeAttackP4(1f));
    }

    public void ManicPause()
    {
        MoveDirection(0);
        IsManicPaused = true;
    }
    public void ManicUnpause(Vector2 NewPos)
    {
        transform.position = NewPos;
        IsManicPaused = false;
    }
    public void ManicUnpause()
    {
        
        IsManicPaused = false;
    }

    void PlayManicRun()
    {
        if (CurrentPhase == Phase.Third)
            AA.PlayAnimation("ManicRun");
        else
            AA.PlayAnimation("ManicRunP4");
    }

    bool GetCliff(Vector3 Dir, float Speed)
    {
        bool ReturnABool = Physics2D.Linecast(transform.position, transform.position + Dir * (Speed), WallMask);

        return ReturnABool;
    }

    void PlayIdleAnimation()
    {
        switch (CurrentPhase)
        {
            case Phase.First:
                AA.PlayAnimation("Idle");
                break;
            case Phase.Second:
                AA.PlayAnimation("Idle2");
                break;
            default:
                AA.PlayAnimation("Idle3");
                break;
        }
    }

    void LetGoGrab()
    {
        if (GrabbedPlayer)
        {
            ASFX.EnableLaser(false);
            PM.LetGo(transform.position); // put player down and put him a spot
            GrabbedPlayer = false;
            PlayIdleAnimation();
            StopAllCoroutines();
            ClearAttackVariables();
        }
    }

    void PoleSpinSetUp(Vector2 Point, float StallTime)
    {
        ACM.StopFight();
        MoveDirection(0);
        ClearAttackVariables();
        StopAllCoroutines();
        IsAttacking = true;

        StartCoroutine(PoleSpinAttackP2(Point, StallTime));
    }

    void FallOff(Vector3 Direction)
    {
        if (Direction.x < 0)
        {
            SetFaceTo(FacingDirection.Right);
        }
        else if (Direction.x > 0)
        {
            SetFaceTo(FacingDirection.Left);
        }
        else if (Direction.y < 0)
        {
            SetFaceTo(FacingDirection.Up);
        }
        else if (Direction.y > 0)
        {
            SetFaceTo(FacingDirection.Down);
        }
        ACM.StopFight();
        ClearAttackVariables();
        StopAllCoroutines();
        IsFalling = true;

        StartCoroutine(FallOffCliff(Direction));
    }

    IEnumerator FallOffCliff(Vector3 Direction)
    {
        MoveDirection(0);

        bool DriftCCW = false;
        Vector2 point1 = new Vector2(transform.position.x - Direction.y * 10, transform.position.y - Direction.x * 10);
        
        RaycastHit2D FloorCast = Physics2D.Linecast(point1, point1, FloorMask);
        

        if (FloorCast)
        {
            DriftCCW = false;
        }
        else
        {
            DriftCCW = true;
        }

        String PhaseSuffix = "";
        switch (CurrentPhase)
        {
            case Phase.First:
                PhaseSuffix = "";
                break;
            case Phase.Second:
                PhaseSuffix = "P2";
                break;
            default:
                PhaseSuffix = "";
                break;
        }
        
        if((DriftCCW && (FD == FacingDirection.Down || FD == FacingDirection.Up)) || (!DriftCCW && !(FD == FacingDirection.Down || FD == FacingDirection.Up)))
        {
            AA.PlayAnimation("DriftCCW" + PhaseSuffix);
        }
        else
        {
            AA.PlayAnimation("DriftCW" + PhaseSuffix);
        }

        //yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.5f);
        AA.SetLock(false);
        float SpawnOffset = 5.5f;
        float SpawnOffset2 = 0.5f;
        
       
        switch (FD)
        {
            case FacingDirection.Down:
                if (DriftCCW)
                    transform.position = new Vector2(transform.position.x + SpawnOffset, transform.position.y + SpawnOffset2);
                else
                    transform.position = new Vector2(transform.position.x - SpawnOffset, transform.position.y + SpawnOffset2);
                break;
            case FacingDirection.Left:
                if (!DriftCCW)
                    transform.position = new Vector2(transform.position.x + SpawnOffset2, transform.position.y - SpawnOffset);
                else
                    transform.position = new Vector2(transform.position.x + SpawnOffset2, transform.position.y + SpawnOffset);
                break;
            case FacingDirection.Up:
                if (DriftCCW)
                    transform.position = new Vector2(transform.position.x - SpawnOffset, transform.position.y - SpawnOffset2);
                else
                    transform.position = new Vector2(transform.position.x + SpawnOffset, transform.position.y - SpawnOffset2);
                break;
            case FacingDirection.Right:
                if (!DriftCCW)
                    transform.position = new Vector2(transform.position.x - SpawnOffset2, transform.position.y + SpawnOffset);
                else
                    transform.position = new Vector2(transform.position.x - SpawnOffset2, transform.position.y - SpawnOffset);
                break;
        }
        //yield return new WaitForSeconds(0.2f);
        SetAttacking(true);
        
        float RandomConeRange = Random.Range(0, 0.5f);
        int RandomConeMultiplier = Random.Range(0, 2);
        if(RandomConeMultiplier == 0)
        {
            RandomConeMultiplier = -1;
        }
        if(Direction.x == 1)
        {
            Direction.x -= RandomConeRange;
            Direction.y = RandomConeRange * RandomConeMultiplier;
        }
        else if(Direction.x == -1)
        {
            Direction.x += RandomConeRange;
            Direction.y = RandomConeRange * RandomConeMultiplier;
        }
        else if (Direction.y == -1)
        {
            Direction.x = RandomConeRange * RandomConeMultiplier;
            Direction.y += RandomConeRange ;
        }
        else if (Direction.y == 1)
        {
            Direction.x = RandomConeRange * RandomConeMultiplier;
            Direction.y -= RandomConeRange;
        }
        
        yield return StartCoroutine(FlightMoveDirection(Direction));

        IsFalling = false;
        PlayIdleAnimation();

        SetAttacking(false);

        ACM.StartFight();
    }

    void ClearAttackVariables() // turns off sound effects, bools, and other misc. features of every attack
    {
        ASFX.EnableLaser(false);
        SetAttacking(false);
        CC.StopShaking();
        AA.SetLock(true);
        if (GrabbedPlayer)
        {
            PM.LetGo();
        }
        

        RocketSideSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketTopSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketMainSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketSecondarySpr.GetComponent<SpriteRenderer>().enabled = false;
        LaserSpr.GetComponent<SpriteRenderer>().enabled = false;
        RocketDiagonalSpr1.GetComponent<SpriteRenderer>().enabled = false;
        RocketDiagonalSpr2.GetComponent<SpriteRenderer>().enabled = false;
        RocketStaticHorizontal1.SetActive(false);
        RocketStaticVertical1.SetActive(false);
        RocketStaticHorizontal2.SetActive(false);
        RocketStaticVertical2.SetActive(false);
        ST.StaticSet(false);

        //WaitForStartUp = false;
        IsFlying = false;
        IsPenguin = false;
        IsCharging = false;
        IsLasering = false;
        IsMainRocket = false;
        IsAltRockets = false;
        IsDiaRockets = false;
        IsStripping = false;
        IsPoleFlying = false;
        IsWaiting = false;
        GrabbedPlayer = false;
        IsManic = false;

        randomStart = true;
        IsInvulnerable = false;
        IsBatWait = false;
        IsParryable = false;
        IsFlinchable = true;
        IsFalling = false;
        IsStunned = false;
        IgnoreCollision = false;
        StapVulnerable = false;
    }

    void Stunned(int StunDur) // fix sprite thing
    {
        if(IsFlinchable == false) // if a windup is interuptted by a true stun then its duration is doubled
        {
            AA.VulnerableStun();
            StunDur *= 2;
        }

        if (CurrentPhase != Phase.Fourth && !IsManic)
        {
            StopAllCoroutines();
            StartCoroutine(StunnedEnum(StunDur));
            ACM.Stunned();
        }
        
    }
    IEnumerator StunnedEnum(int StunDur)
    {
        MoveDirection(0);
        //SetFaceToPlayer();
        switch (CurrentPhase)
        {
            case Phase.First:
                AA.PlayAnimation("Stunned");
                break;
            case Phase.Second:
                AA.PlayAnimation("StunnedP2");
                break;
            default:
                AA.PlayAnimation("Stunned");
                break;
        }
        
        ClearAttackVariables();
        IsStunned = true;
        for(int i = 0; i < StunDur; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        ACM.StartFight();
        PlayIdleAnimation();
        IsStunned = false;
    }

    public bool GetFlinchable()
    {
        return IsFlinchable;
    }

    public bool GetStunned()
    {
        return IsStunned;
    }

    public bool GetProximityCheck()
    {
        return P3ProximityCheck;
    }

    public void WaitForAnimationEvent()
    {
        AnimWaiting = false;
    }

    IEnumerator RandomDelay()
    {
        yield return new WaitForSeconds(Random.Range(1, 4));
        randomStart = true;
    }
}
