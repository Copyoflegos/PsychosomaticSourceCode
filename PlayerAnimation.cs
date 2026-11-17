using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    
    
    AimDirKey CurrentLookDir;
    AimDirKey CurrentMoveDir;

    bool  Walking,  Stapling, StapNotShooting, Enraged, Dashing;
    SpriteRenderer SR; // The SpriteRenderer of this object, mainly for debug purposes right now
    public TherapistSFX TSFX;
    PlayerFloorTracker PFT;

    Animator ANIM;
    
    public GameObject Ripple;
    public GameObject DABOOK;
    public GameObject LightningStrike;
    public SplashParticles SP; // only used during water levels
    public GameObject FloorReflection; // also for water levels

    
    bool WaterLevel;
    
    
    bool EnrageIdleSpawn;

    Vector3 RippleSpawn;
    int currentLayer = 0;

    
    float CurrentNormalizedTime = 0;
    float timeSinceLastChange = 0;
    const float MAXTIMEPAST = 0.1f;
    const float MULTIPLIER = 0;
    AnimatorStateInfo CurrentASI;

    
    
    public Transform[] LightningContacts;
    public GameObject EnrageIdleEffect;
    FloorKey CurrentFloor;
    
    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        ANIM = GetComponent<Animator>();

        PFT = GetComponent<PlayerFloorTracker>();
        
    }
    private void Start()
    {
        WaterLevel = MasterMind.GetWaterLevel();
    }

    private void LateUpdate()
    {
        if (Stapling)
        {
            if (ANIM.GetCurrentAnimatorStateInfo(currentLayer).fullPathHash != CurrentASI.fullPathHash)
            {
                CurrentASI = ANIM.GetCurrentAnimatorStateInfo(currentLayer);

                ANIM.Play(CurrentASI.fullPathHash, -1, CurrentNormalizedTime + (0));
                ANIM.Update(Time.deltaTime);
            }

            CurrentNormalizedTime = ANIM.GetCurrentAnimatorStateInfo(currentLayer).normalizedTime;

            if (timeSinceLastChange > MAXTIMEPAST)
            {
                timeSinceLastChange = MAXTIMEPAST;
            }
        }
    }

    private void Update()
    {
        //SR.sortingOrder = 0 - (int)(transform.position.y / 0.25f);
        ANIM.SetBool("Walking", Walking);

        ANIM.SetInteger("LookDir", (int)CurrentLookDir);
        
        ANIM.SetBool("Stapling", Stapling);

        ANIM.SetBool("Dashing", Dashing);
        

        AnimationSwitch(CurrentMoveDir + 1);

        if (Enraged && !EnrageIdleSpawn)
        {
            StartCoroutine(EnrageIdleSpawnEnum());
        }

        CurrentFloor = PFT.GetCurrentFloor();

        FloorReflection.SetActive(CurrentFloor == FloorKey.Water);

        FloorSwitch(CurrentFloor);

    }

    void FloorSwitch(FloorKey FloorSet) // reminder to add TSFX footsteps change per floor
    {
        if (Enraged)
        {
            FloorSet += 1;
        }
        CurrentFloor = FloorSet;
        SP.AnimationSwitch(CurrentFloor);

    }

    void AnimationSwitch(AimDirKey IndexSet) // Animation Switch layer order is same as aimdir but +1
    {
        currentLayer = (int)IndexSet;
        for(int i = 0; i < ANIM.layerCount; i++)
        {
            ANIM.SetLayerWeight(i, 0);
        }
        ANIM.SetLayerWeight((int)IndexSet, 1);
    }

    public void PlayAnimation(string AnimationName)
    {
        ANIM.Play(AnimationName, -1, 0);

    }

    #region Sets
    
    public bool GetEnraged()
    {
        return Enraged;
    }

    public void SetMoveDir(AimDirKey DIR)
    {
         CurrentMoveDir = DIR;
    }
    public AimDirKey GetMoveDir()
    {
        return CurrentMoveDir;
    }

    public void SetLookDir(AimDirKey DIR)
    {
        CurrentLookDir = DIR;
    }
    public AimDirKey GetLookDir()
    {
        return CurrentLookDir;
    }

    public void SetWalking(bool WalkSet)
    {
        Walking = WalkSet;
    }
    public bool GetWalking()
    {
        return Walking;
    }
    
    public void SetStapling(bool StapSet)
    {
        Stapling = StapSet;
    }

    public void SetDashing(bool DashSet)
    {
        Dashing = DashSet;
    }

    public void SetEnraged(bool EnragedSet)
    {
        Enraged = EnragedSet;
    }
    
    public void PlayStaplingReload()
    {
        ANIM.Play("StaplingReloadStart");
    }
    
    public void SetStapNotShooting(bool Set)
    {
        StapNotShooting = Set;
    }
    
    #endregion

    IEnumerator EnrageIdleSpawnEnum()
    {
        EnrageIdleSpawn = true;
        GameObject HitObject = Instantiate(EnrageIdleEffect, transform.position, Quaternion.identity);
        HitObject.GetComponent<HitEffect>().StartingVariables(gameObject);
        yield return new WaitForSeconds(0.25f);
        EnrageIdleSpawn = false;
    }

    void LaunchBook()
    {
        Instantiate(DABOOK, new Vector3(transform.position.x , transform.position.y + 1, 0), Quaternion.identity).GetComponent<DaBook>().SetDirection(CurrentMoveDir);
    }


    public void WalkContact(int ContactNum) // used for walking particle effects
    {
        
        TSFX.PlayStep();
        switch (CurrentMoveDir)
        {
            case (AimDirKey.Down):
                SP.PlayerSplash("DSplash" + ContactNum);
                break;
            case (AimDirKey.DownLeft):
                SP.PlayerSplash("DLSplash" + ContactNum);
                break;
            case (AimDirKey.DownRight):
                SP.PlayerSplash("DRSplash" + ContactNum);
                break;
            case (AimDirKey.Left):
                SP.PlayerSplash("LSplash" + ContactNum);
                break;
            case (AimDirKey.Right):
                SP.PlayerSplash("RSplash" + ContactNum);
                break;
            case (AimDirKey.Up):
                SP.PlayerSplash("USplash" + ContactNum);
                break;
            case (AimDirKey.UpLeft):
                SP.PlayerSplash("ULSplash" + ContactNum);
                break;
            case (AimDirKey.UpRight):
                SP.PlayerSplash("URSplash" + ContactNum);
                break;
        }


        if(CurrentFloor == FloorKey.Water || CurrentFloor == FloorKey.RageWater)
        {
            RippleSpawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, 0);

            Instantiate(Ripple, RippleSpawn, Quaternion.identity); // fix this in a second
        }
        

    }
    public void LightningContact(int ContactNum) // lightning that strikes when using katana moves
    {
        Vector3 SpawnPos = LightningContacts[ContactNum].position;
        Instantiate(LightningStrike, SpawnPos, Quaternion.identity);
    }
}
