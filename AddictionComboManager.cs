using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AddictionComboManager : MonoBehaviour
{
    public bool DEBUG;
    public Phase DEBUG_PHASE;

    Addiction Add;
    GenericScanBoxController GSBC;

    bool Started = false;
    bool Stopped = false;
    bool Comboing = false;
    bool[] GSBCchecks;

    public bool canFlight;
    public bool canSwipe;
    public bool canSwipe2;
    public bool canLaser;
    public bool canRocket;
    public bool canThrowable;
    public bool canPenguin;
    public bool canPole;
    public bool canSlam;
    public bool canGOSpin;
    public bool canJumpSwipe;
    public bool canLaserShot;

    bool P3ProximityCheck;

    int WalkCount = 0;
    const int MAX_WALKS = 3;
    int randomMoveRoll;
    List<int> possibleAttacks = new List<int>();

    public enum Phase
    {
        First,
        Second,
        Third,
        Fourth
    }
    Phase CurrentPhase;

    enum MoveIndex
    {   
        // Phase 1
        R1,
        L1,
        S1,
        JH1,
        F1,
        W1,
        //I1,

        // Phase 2
        P2,
        G2,
        T2,
        LB2,
        LM2,
        LN2,
        JHN2,
        JHB2,
        R2,
        S2,
        
        F2,

        // Phase 3
        L3,
        BL3,
        ML3,
        HL3, // Huge laser
        M3,

        M4,

        Error
    }
    MoveIndex moveSelector;
    int[] MoveCDList = new int[Enum.GetNames(typeof(MoveIndex)).Length]; // create a move cd list the length of the moveindex
    int[] MoveREPEATList = new int[Enum.GetNames(typeof(MoveIndex)).Length]; // create a move cd list the length of the moveindex

    
    readonly Dictionary<MoveIndex, int[]> MoveDict = new Dictionary<MoveIndex, int[]>() // this is a type-safe dictionary that contains each moves CHANCE and CD (in that order) and REPEATS
    {
        {MoveIndex.S1,     new int[3]{2, 0, 3}}, // default 2
        {MoveIndex.L1,     new int[3]{4, 1, 1}},
        {MoveIndex.JH1,    new int[3]{2, 1, 1}},
        {MoveIndex.R1,     new int[3]{1, 8, 1}},
        {MoveIndex.F1,     new int[3]{2, 0, 2}},
        {MoveIndex.W1,     new int[3]{2, 0, 3}},

        {MoveIndex.P2,     new int[3]{1, 9, 1}},
        {MoveIndex.G2,     new int[3]{1, 0, 2}},
        {MoveIndex.T2,     new int[3]{2, 3, 1}},
        {MoveIndex.LB2,    new int[3]{1, 6, 1}},
        {MoveIndex.LM2,    new int[3]{1, 6, 1}},
        {MoveIndex.LN2,    new int[3]{2, 6, 1}},
        {MoveIndex.JHN2,   new int[3]{3, 3, 1}},
        {MoveIndex.JHB2,   new int[3]{2, 3, 1}},
        {MoveIndex.R2,     new int[3]{1, 8, 1}},
        {MoveIndex.S2,     new int[3]{2, 0, 2}},
        {MoveIndex.F2,     new int[3]{1, 0, 3}},
        

        {MoveIndex.L3,     new int[3]{3, 0, 2}},
        {MoveIndex.ML3,    new int[3]{2, 1, 1}},
        {MoveIndex.BL3,    new int[3]{2, 1, 1}},
        {MoveIndex.HL3,    new int[3]{12, 4, 1}},
        {MoveIndex.M3,     new int[3]{1, 4, 1}},

        {MoveIndex.M4,  new int[3]{1, 0, 10} }

    };

    int TotalMoveCount = 0;

    private void Awake()
    {
        Add = GetComponent<Addiction>();
        GSBC = GetComponent<GenericScanBoxController>();
    }
    

    // Update is called once per frame
    void Update()
    {
        GSBCchecks = GSBC.Checks;
        if (DEBUG)
        {
            CurrentPhase = DEBUG_PHASE;
        }
        else
        {
            CurrentPhase = (Phase)Add.GetPhase();
        }

        

        P3ProximityCheck = Add.GetProximityCheck();

        if (!Add.GetAttacking() && !Comboing && !Add.GetStunned() && Started && !Stopped)
        {
            GSBCchecks = GSBC.Checks;

            canFlight = GSBCchecks[0];
            canSwipe = GSBCchecks[1];
            canSwipe2 = GSBCchecks[2];
            canLaser = GSBCchecks[3];
            canRocket = GSBCchecks[4];
            canThrowable = GSBCchecks[5];
            canPenguin = GSBCchecks[6];
            canPole = GSBCchecks[7];
            canSlam = GSBCchecks[8];
            canGOSpin = GSBCchecks[9];

            

            possibleAttacks.Clear();

            if (CurrentPhase == Phase.First)
            {
                if (!canLaser)
                {
                    AddAttackPoss(MoveIndex.L1);
                }
                if (true)
                {
                    AddAttackPoss(MoveIndex.R1);
                }
                if (canSwipe)
                {
                    AddAttackPoss(MoveIndex.S1);
                }
                if (canSlam)
                {
                    AddAttackPoss(MoveIndex.JH1);
                }
                if (true)
                {
                    AddAttackPoss(MoveIndex.F1);
                }
                if (true)
                {
                    AddAttackPoss(MoveIndex.W1);
                }

            }
            
            else if(CurrentPhase == Phase.Second)
            {
                
                if (canPenguin)
                {
                    AddAttackPoss(MoveIndex.P2);
                }
                if (true)
                {
                    AddAttackPoss(MoveIndex.G2);
                }
                if (canSwipe2)
                {
                    AddAttackPoss(MoveIndex.T2);
                }
                if (!canLaser)
                {
                    AddAttackPoss(MoveIndex.LB2);
                }
                if (!canLaser)
                {
                    AddAttackPoss(MoveIndex.LM2);
                }
                if (!canLaser)
                {
                    AddAttackPoss(MoveIndex.LN2);
                }
                if (canSlam)
                {
                    AddAttackPoss(MoveIndex.JHN2);
                }
                if (canSlam)
                {
                    AddAttackPoss(MoveIndex.JHB2);
                }
                if (true)
                {
                    AddAttackPoss(MoveIndex.R2);
                }
                if (canSwipe)
                {
                    AddAttackPoss(MoveIndex.S2);
                }
                
                if (true)
                {
                    AddAttackPoss(MoveIndex.F2);
                }
                
            }

            else if (CurrentPhase == Phase.Third)
            {
                if (canSwipe)
                {
                    AddAttackPoss(MoveIndex.HL3);
                }
                
                AddAttackPoss(MoveIndex.M3);
                AddAttackPoss(MoveIndex.BL3);
                AddAttackPoss(MoveIndex.ML3);
                AddAttackPoss(MoveIndex.L3);
            }
            else if(CurrentPhase == Phase.Fourth)
            {
                AddAttackPoss(MoveIndex.M4);
            }

            if (possibleAttacks.Count > 0)
            {
                randomMoveRoll = Random.Range(0, possibleAttacks.Count);

                moveSelector = (MoveIndex)possibleAttacks[randomMoveRoll];

                MoveCDList[(int)moveSelector] = TotalMoveCount + MoveDict[moveSelector][1] + 1;

                for(int i = 0; i < MoveREPEATList.Length; i++) // cleanses all non-repeats
                {
                    if(i == (int)moveSelector)
                    {
                        MoveREPEATList[i]++;
                    }
                    else
                    {
                        MoveREPEATList[i] = 0;
                    }
                }
            }
            else
            {
                moveSelector = MoveIndex.Error;
                
                Debug.LogError("NO POSSIBLE MOVES");
            }


            TotalMoveCount++;
            if(CurrentPhase == Phase.First)
            {
                switch (moveSelector)
                {
                    case MoveIndex.R1:
                        StartCoroutine(RComboP1());
                        break;
                    case MoveIndex.L1:
                        StartCoroutine(LComboP1());
                        break;
                    case MoveIndex.S1:
                        StartCoroutine(SComboP1());
                        break;
                    case MoveIndex.JH1:
                        StartCoroutine(JHComboP1());
                        break;
                    case MoveIndex.F1:
                        StartCoroutine(FComboP1());
                        break;
                    case MoveIndex.W1:
                        StartCoroutine(WComboP1());
                        break;

                }
            }
            else if(CurrentPhase == Phase.Second)
            {
                switch (moveSelector)
                {
                    case MoveIndex.P2:
                        StartCoroutine(PComboP2());
                        break;
                    case MoveIndex.G2:
                        StartCoroutine(GComboP2());
                        break;
                    case MoveIndex.T2:
                        StartCoroutine(TComboP2());
                        break;
                    case MoveIndex.LB2:
                        StartCoroutine(LBComboP2());
                        break;
                    case MoveIndex.LM2:
                        StartCoroutine(LMComboP2());
                        break;
                    case MoveIndex.LN2:
                        StartCoroutine(LNComboP2());
                        break;
                    case MoveIndex.JHN2:
                        StartCoroutine(JHNComboP2());
                        break;
                    case MoveIndex.JHB2:
                        StartCoroutine(JHBComboP2());
                        break;
                    case MoveIndex.R2:
                        StartCoroutine(RComboP2());
                        break;
                    case MoveIndex.S2:
                        StartCoroutine(SComboP2());
                        break;
                    
                    case MoveIndex.F2:
                        StartCoroutine(FComboP2());
                        break;
                    
                }
            }
            else if(CurrentPhase == Phase.Third)
            {
                switch (moveSelector)
                {
                    case MoveIndex.M3:
                        StartCoroutine(MComboP3());
                        break;
                    case MoveIndex.HL3:
                        StartCoroutine(HLComboP3());
                        break;
                    case MoveIndex.BL3:
                        StartCoroutine(BLComboP3());
                        break;
                    case MoveIndex.ML3:
                        StartCoroutine(MLComboP3());
                        break;
                    case MoveIndex.L3:
                        StartCoroutine(LComboP3());
                        break;
                   
                    
                }
            }

            //print(moveSelector);
        }
    }

    void AddAttackPoss(MoveIndex MI)
    {
        if(TotalMoveCount >= MoveCDList[(int)MI] && MoveREPEATList[(int)MI] < MoveDict[MI][2])
        {
            int Chance = MoveDict[MI][0];

            for (int i = 0; i < Chance; i++)
            {
                possibleAttacks.Add((int)MI);
            }
        }
    }

    public void StartFight()
    {
        Started = true;
        Stopped = false;
    }
    public void StopFight()
    {
        Stunned();
        Stopped = true;
    }

    public void Stunned()
    {
        StopAllCoroutines();
        Comboing = false;
    }

    IEnumerator WalkCombo()
    {
        Comboing = true;
        while(!canSwipe && WalkCount < MAX_WALKS)
        {
            WalkCount++;
            yield return StartCoroutine(Add.MovePA());
        }
        Comboing = false;
    }

    #region Phase1Combos

    IEnumerator JHComboP1() // Flight GetOffSpin Flight HeadSlam Combo
    {
        Comboing = true;
        yield return StartCoroutine(Add.JumpSwipeAttackP1(0));
        yield return StartCoroutine(Add.HeadSlamAttackP1());
        Comboing = false;
    }

    IEnumerator FComboP1()
    {
        Comboing = true;
        yield return StartCoroutine(Add.FlightAttackP1(2));
        Comboing = false;
    }

    IEnumerator LComboP1()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserShootAttackPA());
        Comboing = false;
    }

    IEnumerator RComboP1()
    {
        Comboing = true;
        yield return StartCoroutine(Add.RocketShipAttackP1());
        Comboing = false;
    }

    IEnumerator SComboP1()
    {
        Comboing = true;
        yield return StartCoroutine(Add.SwipeAttackP1());
        Comboing = false;
    }

    IEnumerator WComboP1()
    {
        Comboing = true;
        yield return StartCoroutine(Add.PacePA());
        Comboing = false;
    }
    
    #endregion

    #region Phase2Combos

    IEnumerator PComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.PenguinAttackP2());
        Comboing = false;
    }

    IEnumerator GComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.GetOffSpinAttackP2());
        
        Comboing = false;
    }

    IEnumerator TComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.TwoArmSwipeAttackP2());
        
        Comboing = false;
    }

    IEnumerator LBComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserBurstAttackP2(3, 2));
        Comboing = false;
    }

    IEnumerator LMComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserShootMixupP2());
        Comboing = false;
    }

    IEnumerator LNComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserShootAttackP2(1));
        Comboing = false;
    }

    IEnumerator JHNComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.JumpSwipeAttackP2());
        yield return StartCoroutine(Add.HeadSlamAttackP2());

        Comboing = false;
    }

    IEnumerator JHBComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.JumpSwipeAttackP2());
        yield return StartCoroutine(Add.HeadSlamAttackP2(3, 2));

        Comboing = false;
    }

    IEnumerator RComboP2()
    {
        Comboing = true;
        
        yield return StartCoroutine(Add.RocketShipAttackP2());
        Comboing = false;
    }

    IEnumerator SComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.SwipeAttackP2());
        
        Comboing = false;
    }

    

    IEnumerator FComboP2()
    {
        Comboing = true;
        yield return StartCoroutine(Add.FlightAttackP2());
        
        Comboing = false;
    }

    
    #endregion

    #region Phase3Combos

    IEnumerator G3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.GetOffSpinAttackP3());
        Comboing = false;
    }
    IEnumerator cL3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.CloseLaserSweepAttackP3());
        Comboing = false;
    }
    IEnumerator S3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.SwipeAttackP3());
        Comboing = false;
    }
    IEnumerator P3Combo()
    {
        Comboing = true;
        yield return null;
        //yield return StartCoroutine(Add.GetOffSpinAttackP3());
        Comboing = false;
    }
    IEnumerator H3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.HeadSlamAttackP3());
        Comboing = false;
    }
    IEnumerator B3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.TVThrowAttackP3());
        Comboing = false;
    }
    IEnumerator fL3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.FarLaserSweepAttackP3());
        Comboing = false;
    }
    IEnumerator T3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.ArmThrowAttackP3());
        Comboing = false;
    }
    IEnumerator R3Combo()
    {
        Comboing = true;
        yield return StartCoroutine(Add.RocketShipAttackP3());
        Comboing = false;
    }
    IEnumerator W3Combo()
    {
        Comboing = true;
        yield return null;
        //yield return StartCoroutine(Add.GetOffSpinAttackP3());
        Comboing = false;
    }

    IEnumerator MComboP3()
    {
        Comboing = true;
        yield return StartCoroutine(Add.ManicCrazeAttackP3());
        Comboing = false;
    }
    IEnumerator LComboP3()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserShootAttackP3(0.5f));
        Comboing = false;
    }
    IEnumerator HLComboP3()
    {
        Comboing = true;
        yield return StartCoroutine(Add.BigLaserAttackP3());
        Comboing = false;
    }
    IEnumerator BLComboP3()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserBurstAttackP3(4, 1f));
        Comboing = false;
    }
    IEnumerator MLComboP3()
    {
        Comboing = true;
        yield return StartCoroutine(Add.LaserShootMixupP3(0.5f));
        Comboing = false;
    }


    #endregion


}

