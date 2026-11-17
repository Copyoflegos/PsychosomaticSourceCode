using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //This script makes the player move, duh 
    // and makes them attack, apparently...
    // ... and everything else that isn't animation

    #region GlobalFunctionVariables
    public bool IsNotFighting = false; // This badboy stops attacks 
    public bool UseLockPrototype = false; // are we using lock on
    public bool PopInHealth; // Is health overtime?
    static bool GUNBATFREEAIM = true;// Sets whether or not we using full 360 degree aim or 8 directional
    #endregion

    #region LockOnConstants
    const float LOCK_ON_DIF = 0.75f; // How close to the x or y value for an object to be consider straight for the lock on
    const float LOCK_ON_MULT = 0.55f; // As the player goes further from lock on target how much wider should DIF become?
    // it sounds confusing but both of the lock on variables work together to make a cone, the DIF creates the initial width and the MULT creates the spread over distance
    const float MAX_LOCK_DIST = 15; // How far can an enemy be locked on from
    const float SWITCH_LOCK_DIF = 400f; // big number, it is the weight of the value for each lock on switch
    #endregion

    #region LockOnInstanceVariables
    GameObject LockOnDummy; // Whatever object we are currently locked on to
    public Transform LockOnParent; // Holds all enemy objects to parse through when looking for a lock on target 
    bool IsLockedOn = false;
    bool InputLockOnWait = true;
    public GameObject LockOnIndicator;
    #endregion

    #region InputConstants
    const float L_HOR_THRESH = 0.3f; // Threshold for Left stick Horizontal Movement
    const float L_VER_THRESH = 0.3f;
    const float R_HOR_THRESH = 0.5f;
    const float R_VER_THRESH = 0.5f;
    #endregion

    #region InputVariables
    bool LLeft, LRight, LUp, LDown; // Each bool corresponds with an input being held down
    bool LLeftDown, LRightDown, LUpDown, LDownDown;
    bool RLeft, RRight, RUp, RDown;
    bool KeyBoardMouse = false; // is the player using keyboard and mouse?
    bool AttackInput, AttackInputDown, DashInput, DashInputDown, LockOnInput,  RunDrawInput, RunDrawInputDown, RunDrawInputUp, PauseInputDown, EnrageInput, TauntInput, KickAttackInput, KickAttackInputDown;
    bool AAInput, AAInputDown, GunBatInput, GunBatInputDown, GunBatInputHold, BaubleInputDown, BaubleInput, ModeSwitchDown, ModeSwitchUp, CloneInput, CloneInputDown;
    bool JumpInput, JumpInputDown;
    Vector2 RightStickVal = new Vector2(0, 0);
    Vector2 LeftStickVal = new Vector2(0, 0);
    #endregion

    
    public GameObject PauseBG;

    #region PhysicsInstanceVariables
    float VelocityX = 0; // Velocity on the horizontal axis decided by directions held
    float VelocityY = 0; // Velocity on the vertical axis

    float ActionSpeed = 1; // Every velocity is multiplied by this (Used for slow on attack)
    
    Vector3 DashVelocity;
    Vector3 FirstDashDir;

    Vector3 ActionMoveDir;
    float ActionMoveSpeed;

    //SKATER BOY
    Vector2 SkatePastMoveDir; // the way the player was moving in the past
    Vector2 SkateCurrentMoveDir; // the way the player is moving currently
    Vector2 SkateFutureMoveDir; // the way the player wants to move
    float CurrentSkateSpeed = 0;
    float TurnCoefficient = 0;

    Vector2 KnockbackVector; // Which way should the player be knocked back
    float KnockbackDuration; // And for how long
    float KnockbackSpeed;

    Vector2 WindDirection;
    int WindDuration;
    float WindSpeed;

    Vector2 ForceDirection;
    int ForceDuration;
    float ForceSpeed;

    float FreezeDuration = 0;
    float ConfusedDuration = 0; // When this is higher than 0, the players controls are reversed!
    float SlowDuration = 0;

    float SlowSpeed = 1;
    
    float StunDuration = 0;

    

    bool DashInputHold = false;
    bool AttackInputHold = false;
    bool JumpInputHold = false;
    bool KickAttackInputHold = false;
    bool LStickHold = false;
    #endregion

    #region InstanceStateBools
    bool Enraged = false;
    bool Paused = false;
    bool InKatana = false;
    bool IsGrabbedLocked = false;
    bool IsGrabbedUnlocked = false;
    bool NoOverride = false;
    bool IsStapling = false;

    bool HitInvul = true; // is therapist immune to damage within a short period after being hit?
    bool StingInvul = true;
    bool DashInvul = false; // is therapist immune to damage while dashing?
    bool RollInvul = false; // immune to sting damage while rolling
    bool JumpInvul = false;
    
    bool AlternateStaple = false; // has the therapist already shot the first staple

    bool GunBatCircleLock = false;

    

    

    bool VisibilityLocked = false;
    bool Invulnerable = false; // is therapist immune to damage?
    bool Interrupted = false; // if the player's current attack has been interrupted;

    bool InfiniteRage = false; // debug currently
    bool HoldRight = false; // debug
    bool InfiniteHealth = false; // debug
    #endregion

    #region SpeedConstants
    const float PLAYER_SPEED = 1.25f; // The natural movement multiplier of the player
    const float CON_PLAYER_SPEED = 1.45f;
    const float MOVE_SPEED = 0.125f; // The amount(in units) the player moves per frame before the multiplier
    const float RUNDRAW_SPEED = 0.45f;
    const float RAGING_SPEED = 0; // raging speed not enrage speed
    const float GUNBAT_SPEED = 0f;
    const float GUNBAT_SHOT_SPEED = 1.8f;
    const float GUNBAT_CIRCLE_ENLARGE_SPEED = 0.3f;
    const float GUNBAT_CIRCLE_ENLARGE_RESTRAINT = 0; // The amount that gunbat duration "restrains" the circle growth speed
    const float DASHMUL_1 = 2.15f; // Dash speed, basically
    const float DASHMUL_2 = 2.1f; //Second dash speed, it's different now >:)
    const float DASHMUL_R = 2f; // A MAD DASH SPEED FAM PUMP UP THE JAMS (Enraged dash)
    const float JUMPMUL = 1.85f;
    const float JUMPMUL_H = 2.05f;
    const float JUMPMUL_E = 2.2f;

    const float SKATE_SPEED1 = 1.25f;
    const float SKATE_SPEED2 = 2.05f;
    const float SKATE_SPEED3 = 3.75f;
    const float SKATE_TURNCOEF1 = 10; // turn coefficient
    const float SKATE_TURNCOEF2 = 20;
    const float SKATE_TURNCOEF3 = 25;

    const float SKATE_BRAKE_SPEEDL = 0.01f; // SPEED LOSS means slowing down
    const float SKATE_BRAKE_TURNL = 0.05f; // TURN LOSS means more control over the turn
    const float SKATE_GAS_SPEEDG = 0.55f; // SPEED GAIN means speeding up
    const float SKATE_GAS_TURNG = 0.50f; // TURN GAIN means less control over the turn
    const float SKATE_NEUT_SPEEDL = 0.01f;
    const float SKATE_NEUT_TURNL = 0.03f;
    const float SKATE_NEUT_TURNG = 0.03f;

    const float SHOTGUN_SLOW = 0.15f;
    const float SHOTGUNKNOCKBACK = 0.35f; // How fast the shotgun knocks you back

    const float THRUSTPUSH = 0.85f; // starts fast
    const float THRUSTPUSH2 = 0.5f;
    const float THRUSTPUSH3 = 0.2f; // then slows down

    const float FLIPKNOCKBACK = 0.9f;

    const float KICK_PUSH = 0.7f;
    const float KICK_PUSH2 = 0.1f;
    const float KICK_PUSH3 = 0f;

    const float DANDY_PUSH = 0.95f;
    const float DANDY_PUSH2 = 0.2f;

    const float SLIDE_KICK_PUSH = 0.8f;
    const float SLIDE_KICK_PUSH2 = 0.1f;

    const float SLASH_PUSH = 0.5f;

    const float STAP_SPIN_PUSH = 0.5f;

    const float KATANA_THRUST_PUSH = 1.5f;
    const float KATANA_THRUST_PUSH2 = 0.1f;

    const float ATTACKSLOW = 0.3f; // How slow you are while attacking
    const float SLAMSLOW = 0;
    const float SPINSLOW = 0.6f;
    const float FRENZYSLOW = 0f;

    const float WHIPCHARGESLOW_1 = 0.25f;
    const float WHIPATTACKSLOW_1 = 0;
    const float WHIPCHARGESLOW_2 = 0.15f;
    const float WHIPATTACKSLOW_2 = 0;
    const float WHIPCHARGESLOW_3 = 0.15f;
    const float WHIPATTACKSLOW_3 = 0;

    const float FLIPSLOW = 0;
    const float KICKSLOW = 0;
    const float LIGHTKICK_SLOW = 0.35f;
    const float SLASHKICKSLOW = 0;
    const float ATTACKSLOW_R = 0.25f; // was 0.55f
    const float UPPERCUTSLOW = 0.25f;
    const float CHARGEPUNCH_SLOW = 0;
    const float BATTLESTANCESLOW = 0;

    const float STAPLING_SLOW = 0.75f;
    const float STAPLING_RELOADSLOW = 0;
    const float STAPLERBLAST_SLOW = 0.25f;
    const float STAPLERBLAST_KNOCKBACK = 0.65f;

    const float JUMPBURST_SPEED = 1.8f;

    const float KATANA_SPEED = 0.75f;
    const float KATANA_DASHMUL1 = 3.9f; // seems fast but it is to compensate for natural katana speed
    const float KATANA_DASHMUL2 = 0;
    const float KATANA_SPIN_SLOW = 0.2f;
    const float KATANA_EXIT_SPEED = 0;
    
    #endregion

    // The TIMING one
    // FIXED UPDATE FRAMES: 10.6666 = 100 ms animation frame
    // Animation frames in unity are 40 ms long in aseprite assuming 24 samples a second
    #region FrameCountConstants 
    //GUNBAT CONSTANTS

    const int GUNBAT_DURATION = 50; // how long does it take to charge gunbat
    const int GUNBAT_RECOVERY = 22; // how long after firing does the player stay slowed
    const int GUNBAT_CHAIN_RECOVERY = 20; // how long do you have to slam gbc

    const int GBCDELAY = 10; // GBC = gunbat chain Delay = how many frames is the windup
    const int GBCFRAMES = 35; 

    const float GUNBAT_MAX_CIRCLE_RANGE = 16;
    const float GUNBAT_CIRCLE_RANGE1 = 8;
    const float GUNBAT_CIRCLE_RANGE2 = 12;
    const int GUNBAT_CIRCLE_THRESH1 = 0;
    const int GUNBAT_CIRCLE_THRESH2 = 15;
    const int GUNBAT_CIRCLE_THRESH3 = 30;

    //STAPLE CONSTANTS

    const int STAPLE_DURATION = 170; // 200 default
    const int STAPLE_STARTUP = 150; // when does stapling actually start (counting down from stap duration)
    const int STAPLE_DPS = 2;
    const int STAPLE_MAXAMMO = 68; // if the staplers have been fully charged
    
    const int STAPLE_QUALITY_THRESH = 15; // Reload cooldown
    const int RELOAD_FRAMES = 40;
    const int RELOAD_FILL_FRAME = 15; // on what frame do you get ammo back exactly

    const int STAPLERBLAST_DELAY = 0;
    const int STAPLERBLAST_FRAMES1 = 6;
    const int STAPLERBLAST_FRAMES2 = 8;
    const int STAPLERBLAST_CD = 12;

    const int STAPLING_DASHFRAMES = 17;
    const int STAPLING_DASHCD = 50;

    // ENRAGE CONSTANTS
    const int ENRAGE_STARTUP = 25; // how many frames does enrage transformation last for

    const int MAX_RAGE = 1000; // Maximum rage (500)
    const int MIN_RAGE = 10; // Minimum hits to enter rage (70)

    const int RAGE_COST = 50; // How much rage it costs to enter enrage
    const int RAGE_BUILD = 32; // How much rage you get per hit (unused)

    const int RAGE_HEALTH_P = 10; // health for a punch (was 5)
    const int RAGE_HEALTH_HK = 20; // health for a kick
    const int RAGE_HEALTH_LK = 10;
    const int RAGE_HEALTH_FK = 5;
    const int RAGE_HEALTH_GK = 30;

    const int RAGE_BUILD_P = 7;
    const int RAGE_BUILD_HK = 10;
    const int RAGE_BUILD_LK = 10;
    const int RAGE_BUILD_FK = 25;
    const int RAGE_BUILD_GK = 25;

    // FOCUS/CONCENTRATION CONSTANTS

    const int MAX_CONC = 25;


    const int RAGE_HURT = 250; // how much rage you lose when you get hit

    const float RAGE_DEGRADE = 0.60f;

    const int FLAME_THRESH_1 = 24; // how much health for each speed threshhold
    const int FLAME_THRESH_2 = 49;
    
    

    const int RAGING_DURATION = 5;

    // RUNDRAW CONSTANTS

    const int MAX_RUNDRAW_DUR = 90; // How long it takes to heal when reading

    // DASH (and jump) CONSTANTS

    const int DASHFRAMES_1 = 15; // How long the dash lasts
    const int DASHFRAMES_2 = 15; // How long the second dash lasts
    const int DASHFRAMES_R = 18; // How long enraged dash lasts (was 26)
    const int DASHFRAMES_RS = 15;
    const int JUMPFRAMES = 24;
    const int JUMPFRAMES_S = 18; // spot jump frames
    const int JUMPFRAMES_H = 35; // hookshot jump frames
    const int JUMPFRAMES_E = 20;
    const int JUMPFRAMES_ES = 12;

    const int TPDELAY = 5;
    const int TPFRAMES = 8;
    const int TPCD = 12;

    const int COMBO_RESET_RS = 50;

    const int JUMPTHRESH_H = 15;
    const int JUMPTHRESH_S = 5;
    const int JUMPTHRESH = 7;
    const int JUMPTHRESH_E = 5;
    const int JUMPTHRESH_ES = 3;
    const int DASHTHRESH_2 = 2; // Within what time frame do you have to hold button to roll after sliding
    const int ROLLTHRESH = 5; // within what time frame can you combo in roll

    const int DASHCD_1 = 35; // How long from pressing dash can dash again
    const int DASHCD_2 = 20; // How long from holding dash can dash again (additive to the first cd)
    const int DASHCD_R = 28; // Enraged dash cd
    const int DASHCD_RS = 26;
    const int DASH_JUMPCD = 20;
    const int JUMPCD = 42;
    const int JUMPSCD = 30;
    const int JUMP_DASHCD = 30;
    const int JUMPCD_E = 30;
    const int JUMP_DASHCD_E = 25;
    const int JUMPCD_ES = 25;
    const int JUMP_DASHCD_ES = 20;

    // FLAMETHROWER CONSTANTS

    const int FLAMETHROWER_TRANSDELAY = 35;
    const int FLAMETHROWER_WAVEDELAY = 5; // How many frames to wait in between each flame thrower projectile instantiation
    const int FLAMETHROWER_WAVE1 = 5; // for base
    const int FLAMETHROWER_WAVE2 = 10;
    const int FLAMETHROWER_WAVE3 = 15;
    const int FLAMETHROWER_WAVE4 = 30; // for blue

    const int FLAMECHOKE_TRANSDELAY = 25;
    const int FLAMECHOKE_CD = 75;
    const int FLAMECHOKE_DURATION = 5;
    
    // SHOTGUN CONSTANTS

    const int SHOTGUNFRAMES = 6; // How long shotgun lasts
    const int SHOTGUNDELAY = 2;
    const int SHOTGUNCOOLDOWN = 15;

    // TAUNT CONSTANTS

    const int TAUNT_DURATION = 150;

    // WHIP 1 CONSTANTS

    const int WHIPFRAMES_1 = 8;
    const int WHIPDELAY_1 = 10;
    const int WHIPWIND_1 = 2;
    const int ATTACKCD_WHIP_1 = 25;
    const int DASHCD_WHIP_1 = 5;

    // WHIP 2 CONSTANTS

    const int WHIPFRAMES_2 = 10;
    const int WHIPDELAY_2 = 5;
    const int WHIPWIND_2 = 2;
    const int ATTACKCD_WHIP_2 = 25;
    const int DASHCD_WHIP_2 = 25;

    // WHIP 3 CONSTANTS

    const int WHIPFRAMES_3 = 10;
    const int WHIPDELAY_3 = 7;
    const int WHIPWIND_3 = 4;
    const int ATTACKCD_WHIP_3 = 40;
    const int DASHCD_WHIP_3 = 30;


    // ATTACK CONSTANTS

    const int ATTACKFRAMES = 10; // How many frames to hit for 
    const int ATTACKDELAY = 0; // Wait before hitting
    const int ATTACKWIND = 2; // how many it waits after
    const int ATTACKCD_ATTACK = 22; // How long until you can hit again
    const int DASHCD_A = 5; // How long from Attacking can dash again

    // SLAM CONSTANTS

    const int SLAMFRAMES = 25;
    const int SLAMDELAY = 10;
    const int ATTACKCD_SLAM = SLAMFRAMES + SLAMDELAY;
    const int DASHCD_SLAM = SLAMFRAMES + SLAMDELAY;
    
    // THRUST CONSTANTS

    const int THRUSTFRAMES = 20;
    const int THRUST_THRESH1 = 13;
    const int THRUST_THRESH2 = 17;
    const int THRUSTDELAY = 0;
    const int THRUSTCD = 5;
    const int ATTACKCD_THRUST = 34;
    const int DASHCD_THRUST = 40;

    // SPIN CONSTANTS

    const int SPINFRAMES1 = 15;
    const int SPINFRAMES2 = 10;
    const int SPINDELAY = 2;
    const int ATTACKCD_SPIN = 35;
    const int ATTACKCD_STAPLER_SPIN = 30;
    const int DASHCD_SPIN = 20;

    // FRENZY CONSTANTS

    const int FRENZYFRAMES1 = 5; // kind of like the cd before you can 
    const int FRENZYDELAY1 = 15;
    const int FRENZYFRAMES2 = 5;
    
    const int ATTACKCD_FRENZY = 25;
    const int DASHCD_FRENZY = 20;

    // FLIP CONSTANTS

    const int FLIPFRAMES = 6;
    const int FLIPDELAY = 0;
    const int FLIPCD = 16;
    const int FLIP_ATTACKCD = FLIPCD + FLIPDELAY + FLIPFRAMES + 2;
    const int DASHCD_FLIP = FLIPCD + FLIPDELAY + FLIPFRAMES + 4;

    const int COMBO_RESET_FLIP = 30;
    const int BURN_F = 2;

    // BATTLESTANCE CONSTANTS

    const int BATTLESTANCE_ENTER = 0; // was 6
    const int BATTLESTANCE_MAX = 13;
    const int BATTLESTANCE_SLAM2 = 46;
    const int BATTLESTANCE_SLAM3 = 86;

    //  RAGE ATTACK CONSTANTS

    const int R_ATTACKDELAY = 0;
    const int R_ATTACKFRAMES = 10;
    const int R_ATTACKCD = 12;
    const int R_ATTACKCD2 = 15; // was 15

    

    const int DASHCD_RA = 15; // How long from Rage Attacking canDash

    const int BURN_R = 1;

    // UPPERCUT CONSTANTS

    const int UPPERCUTDELAY = 0;
    const int UPPERCUTFRAMES = 30;
    const int ATTACKCD_UPPERCUT = 10;
    const int DASHCD_UPPERCUT = 20;

    // KICK CONSTANTS

    const int KICKDELAY = 50;
    const int KICK_LEADUP = 22;
    const int KICKFRAMES = 14;
    const int KICK_ATTACKCD = KICK_LEADUP + KICKFRAMES + 2;
    const int DASHCD_KICK = KICK_LEADUP + KICKFRAMES + 2;

    const int KICK_THRESH0 = 6;
    const int KICK_THRESH1 = 12;
    const int KICK_THRESH2 = 16;

    const int COMBO_RESET_KICK = 65; // was 30
    const int BURN_HK = 3;

    // LIGHT KICK CONSTANTS

    const int LIGHTKICK_DELAY = 22;
    const int LIGHTKICK_FRAMES = 6;
    const int LIGHTKICK_ATTACKCD = LIGHTKICK_DELAY + LIGHTKICK_FRAMES + 2;
    const int DASHCD_LIGHTKICK = LIGHTKICK_DELAY + LIGHTKICK_FRAMES + 2;

    const int COMBO_RESET_LKICK = 50;
    const int BURN_LK = 2;

    // DANDY PUNCH CONSTANTS

    const int DANDY_DELAY = 0;
    const int DANDY_FRAMES = 20;
    const int DANDY_FRAMES2 = 30;
    
    const int DANDY_CD = 2;
    const int DANDY_ATTACKCD = 30;
    const int DANDY_THRESH1 = 3; // at what point does the dandy punch slow down
    const int DASHCD_DANDY = 30;

    // SLIDE KICK CONSTANTS

    const int SLIDE_KICK_DELAY = 0;
    const int SLIDE_KICK_FRAMES = 12;
    const int SLIDE_KICK_THRESH1 = 2;
    const int SLIDE_KICK_CD = 0;
    const int DASHCD_SLIDE_KICK = 12;
    const int SLIDE_KICK_ATTACKCD = 12;

    // CHARGE STANCE CONSTANTS

    const int CHARGE_UPPER_THRESH = 52; // at what point does uppercut become charge punch
    const int CHARGE_BIG_THRESH = 90; // at what point does the charge punch hit harder
    const int CHARGE_MIN_THRESH = 18;
    // both above used for dandy punch distance as well

    // BIG PUNCH CONSTANTS

    const int BIGPUNCHDELAY = 0;
    const int BIGPUNCHFRAMES = 35;
    const int DASHCD_BIGPUNCH = 20;
    const int BIGPUNCH_ATTACKCD = 25;
    


    // SLASH KICK CONSTANTS

    const int SLASHKICKDELAY = 22;
    const int SLASHKICKFRAMES = 12;
    const int SLASHKICK_THRESH = 10;
    const int SLASHKICK_ATTACKCD = SLASHKICKDELAY + SLASHKICKFRAMES + 4;
    const int DASHCD_SLASHKICK = SLASHKICKDELAY + SLASHKICKFRAMES + 4;

    const int BURN_GK = 4;

    // PARRY CONSTANTS

    const int PARRY_FRAMES = 7; // potentially up to 5 for 1 frame of animation
    const int PARRY_CD = 25; 
    const int PARRY_PARRYCD = 60;
    const int PARRY_ATTACKCD = 40;
    const int PARRY_DASHCD = 50;
    const int PARRY_WINDOW = 35;
    const int PARRY_DURATION_MAX = 6; // similar to parry frames
    const int PARRY_HIT_CD = 30; // how long after taking damage can you parry again

    // KATANA CONSTANTS

    const int KATANA_TRANS_FRAMES = 40;
    const int KATANA_EXIT_FRAMES = 23;
    const int KATANA_QUALITYMAX = 5; // how high does quality go
    const int KATANA_QUALITYTHRESH = 5; // how many hits it takes to go up a tier in bat form

    // KATANA ATTACK CONSTANTS

    const int KATANA_ATTACK_DELAY = 45;
    const int KATANA_ATTACK_ATTACKCD = 50;
    const int KATANA_ATTACK_PARRYCD = 40;
    const int KATANA_ATTACK_DASHCD = 40;
    const int KATANA_ATTACK_FRAMES = 15;

    // KATANA THRUST CONSTANTS

    const int KATANA_THRUST_THRESH1 = 5;
    const int KATANA_THRUST_FRAMES = 8;
    const int KATANA_THRUST_ATTACKCD = 50;
    const int KATANA_THRUST_DASHCD = 40;
    const int KATANA_THRUST_DELAY = 0;
    const int KATANA_THRUST_CD = 40;
    const int KATANA_THRUST_CD2 = 10;
    const int KATANA_THRUST_MAXAMMO = 3;

    // KATANA DASH CONSTANTS

    const int KATANA_DASH_FRAMES = 45;
    const int KATANA_DASH_THRESH = 25; // at what point in the roll do you lose your invulnerability and stop moving
    const int KATANA_DASH_CD = 55;

    // KATANA SPIN CONSTANTS

    const int KATANA_SPIN_FRAMES = 20;
    const int KATANA_SPIN_ATTACKCD = 30;
    const int KATANA_SPIN_PARRYCD = 30;
    const int KATANA_SPIN_DASHCD = 30;

    // EYE BAUBLE CONSTANTS

    const int EYE_CD_REDUCE_ATTACK = 7; // how much is the normal attack cooldown reduced by eye bauble
    const int EYE_DELAY_REDUCE_KATANA = 10; // how much is katana delay shaved off by eye bauble
    const int EYE_DELAY_REDUCE_STAPLERS = 1; //etc

    // KNIFE BAUBLE CONSTANTS

    const int KNIFE_BLEED_DMG = 1;
    const int KNIFE_BLEED_DPS = 5;
    const int KNIFE_BLEED_DURADD = 100;
    const int KNIFE_SWING_DMG = 1;
    const int KNIFE_SWING_DUR = 35;

    // GRAPPLE SPIN CONSTANTS

    const int TOTAL_SPIN_ROT = 2; // how many circles in a spin 

    // FALLING CONSTANTS

    const int FALLING_FRAMES = 30;

    #endregion

    #region DamageConstant
    const float STAPLER_DAMAGE = 13;
    const float STAPLERBLAST_DMG = 50;

    const float KATANA_DMG = 425; // with any quality points
    const float KATANA_DMG_MIN = 25; // with no quality points
    const float KATANA_THRUST_DMG = 200;
    const float KATANA_THRUST_DMG_MIN = 25; // no quality
    const float KATANA_SPIN_DMG_MIN = 25;
    const float KATANA_SPIN_DMG_MAX = 250;

    //these should be constants but the need for gameObject doesn't let me
    Damage ATTACK_DMG;
    Damage SLAM_DMG;
    Damage SLAM_DMG2;
    Damage SLAM_DMG3;
    Damage THRUST_DMG;
    Damage SPIN_DMG;
    Damage CON_SPIN_DMG;
    Damage FRENZY_DMG;

    const float MIN_WHIPDMG_1 = 50;
    const float MAX_WHIPDMG_1 = 150;

    const float MIN_WHIPDMG_2 = 50;
    const float MAX_WHIPDMG_2 = 50;

    const float MIN_WHIPDMG_3 = 100;
    const float MAX_WHIPDMG_3 = 200;



    Damage GUNBATSLAM_DMG;
    
    Damage GUNBATIMPACT_DMG;
    Damage GUNBATSHOT_DMG;

    Damage DARKCLONE_DMG;
    Damage DARKCLONE_DMG2;

    Damage R_ATTACK_DMG;
    Damage R_ATTACK_DMG2;
    Damage SHOTGUN_DMG;
    Damage SHOTGUNTIP_DMG;

    Damage STAP_SHOT1_DMG;
    Damage STAP_SHOT2_DMG;
    Damage STAP_SHOT3_DMG;

    Damage SLIDE_KICK_DMG;

    const int UPPERCUT_DMG = 300;
    Damage KICK_DMG;
    Damage SLASHKICK_DMG; // heavy kick
    Damage FLIP_DMG;
    Damage LIGHTKICK_DMG;
    Damage BATTING_DMG;
    const int DANDY_DMG = 1;
    const int DANDYSLAM_DMG = 200;
    const int BIGPUNCH_DMG = 100;
    const int BIGPUNCH_DMG2 = 600;
    
    #endregion


    #region HitCDConstants
    const int RUNDRAW_CD = 12;
    const int GUNBAT_CD = 10;
    const int SHOTGUN_CD = 10;
    const int TPTV_CD = 5; // Cd for addiction accessory
    const int ADDBUDDY_CD = 6;
    const int RELOAD_CD = 15;
    const int RAGE_HEAL_CD = 1;
    #endregion

    #region HealthConstants
    const int MAXHEALTH = 175; // was 175
    const int HEALTHYTHRESH = 150; // Minimum health to be within a health range
    const int OKAYTHRESH = 125;
    const int HURTTHRESH = 50;
    const int DYINGTHRESH = 0;
    const int HEAL_AMOUNT = 115;
    const float HEALTH_DEATH_THRESH = 65; // How low you have to be to die, for death protection
    #endregion
   

    float Health; // Health o_o

    #region EnumDeclarations
    AimDirKey AimDir = 0;
    AimDirKey WalkDir = 0;

    public enum PlayerDoingState // What the micro state the player is currently doing (ie: Attacking, Using Gunbat, Rolling, Reading)
    {
        NoFight, // Only active when outside of a brain
        Normal,
        Shooting,

        NormalAttack,
        SpinAttack,
        
        WhipCharging1,
        WhipAttacking1,

        Dashing,
        Rolling,
        Jumping,
        EnrageJump,

        Taunting,

        BattleStance,
        Thrusting,
        Slamming,

        
        
        Parrying,
        ParryStance,

        Skating,

        SentFlying,
        Soaring,
        Falling,
        Grabbed,

        StapleShooting,
        StapleDashing,
        Reloading,

        EnrageAttacking1,
        EnrageAttacking2,
        EnrageShotgun,
        EnrageCartwheel,
        EnrageSpotdodge,
        DandyPunching,
        Uppercutting,
        Charging,
        BigPunching,

        HeavyKicking,
        SlashKicking,
        FlipKicking,
        LightKicking,
        SlideKicking,
        KickStance,

        BattingStance,

        Raging,
        Transforming,

        KatanaRoll

    }
    public PlayerDoingState PDS;

    readonly PlayerDoingState[] StaplerWhitelist = new PlayerDoingState[1]
    {
        PlayerDoingState.StapleShooting
    };
    readonly PlayerDoingState[] ReloadWhitelist = new PlayerDoingState[2]
    {
        PlayerDoingState.Normal,
        PlayerDoingState.StapleShooting
    };
    readonly PlayerDoingState[] DashWhitelist = new PlayerDoingState[4]
    {
        PlayerDoingState.Normal,
        PlayerDoingState.NormalAttack,
        PlayerDoingState.BattleStance,
        PlayerDoingState.WhipCharging1
    };
    readonly PlayerDoingState[] JumpWhitelist = new PlayerDoingState[4]
    {
        PlayerDoingState.Normal,
        PlayerDoingState.NormalAttack,
        PlayerDoingState.BattleStance,
        PlayerDoingState.StapleShooting
    };
    readonly PlayerDoingState[] EnrageTransformBlacklist = new PlayerDoingState[1]{
        PlayerDoingState.Grabbed
    };
    readonly PlayerDoingState[] FallingBlacklist = new PlayerDoingState[4]
    {
        PlayerDoingState.Soaring,
        PlayerDoingState.Jumping,
        PlayerDoingState.EnrageJump,
        PlayerDoingState.Grabbed
    };
    readonly PlayerDoingState[] GrabBlacklist = new PlayerDoingState[1] 
    {
        PlayerDoingState.Grabbed
    };
    readonly PlayerDoingState[] ParryWhitelist = new PlayerDoingState[1]
    {
        PlayerDoingState.Parrying
    };

    FloorKey FK;

    #endregion

    //IE: durations and other simple countdowns
    #region InstanceCounts
    int AttackCooldown = 0;
    int DashDuration1 = 0;
    int DashDuration2 = 0;
    int DashCooldown = 0;
    int JumpCooldown = 0;
    
    
    
    
    

    int StaplingAlternateCount = 0;
    int StaplingDuration = -1;
    int StaplingAmmo = STAPLE_MAXAMMO;
    int FocusStapleCount = 0;
    
    
    int StaplingDashDuration = 0;

    int KatanaQuality = 0;
    int KatanaQualityCount = 0;
    int KatanaParryCoolDown = 0;
    int KatanaDashDuration = 0;
    int KatanaParryWindow = 0;
    //int ParryDuration = 0;
    int KatanaThrustAmmo = 0;

    bool IsParrying = false;
    float ParryDamage = 0;
    int ParryStun = 0;

    
    
    int RAttackAlternate = 1;
    
    
    int RAttackCooldown = 0;
    
    
    int RDashCooldown = 0;
    public int RageExtraLifeCount = 1;

    int BurstCount = 1;
    int BurstTimer = 0; // How long the player has to burst before the current health threshold is lost
    int BurstHitCount = 0; // How many hits are in the current combo
    float BurstHealthThreshold = 0; // What amount of health is the player's health set to when bursting
    const float BURST_HEALTH_LOSS = 0.25f;

    int BattleStanceCount = 0;

    int ChargePunchCount = 0;

    
    int GunBatMoveCD = 0;
    float HookShotTimeCD = 0;
    int ShotgunMoveCD = 0;
    
    int ReloadMoveCD = 0;

    int HitCounter = 0;
    int PreviousHitCounter = 0;
    int ConcentrationCounter = 0;

    float RageCounter = 0;

    int AttackComboCount = 0;
    int AttackComboTime = 0;

    int WhipComboCount = 0;
    int WhipComboTime = 0;

    int KickComboCount = 0;
    int KickComboTime = 0;

    int ShotgunComboCount = 0;
    //int ShotgunComboTime = 0;

    int TpComboCount = 0;
    int TpComboTime = 0;
    
    int BleedKnifeCount = 0;

    int RocketSkateCount = 0;
    
    #endregion

    public RageBar RB; // Rage Bar 
    public HealthBar HB; // Health Bar
    public HealthBar FB; // Focus Bar
    public HealthBar BB; // Burst Bar
    public StaplerBar SB;

    #region Hurt&Aimboxes
    public GameObject VisualAim; // This gameobject is lined up with whatever direction therapist is facing, is used for lining up attacks
    public GameObject GunbatVAim; // Like Visual Aim but full 360

    public GameObject LaserAim;
    public GameObject LaserAim2;
    public GameObject GunbatRangeCircle;
    SpriteRenderer GunbatRangeSR;
    GameObject GBC;

    public GameObject StapBlast, StapImpact;
    public SpriteRenderer StapleBlast1, StapleBlast2;
    public SpriteRenderer StapleBlast3, StapleBlast4;
    public GameObject RStapLinecast1, RStapLinecast2, LStapLinecast1, LStapLinecast2;
    public GameObject RStapLinecast3, LStapLinecast3;
    GameObject StapImpInst1;
    public GameObject StaplerBlastHurtBox;

    public GameObject AttackHurtBox; // A visual indicator for the attack hurtbox, it's dimensions are used in the boxcast for attack hurtbox
    public GameObject SlamHurtBox;
    public GameObject SlamHurtBox2;
    public GameObject SlamHurtBox3;
    public GameObject ThrustHurtBox;
    public GameObject FrenzyHurtBox;
    public GameObject WhipHurtBox;
    public GameObject[] WhipHurtBoxes2;
    public GameObject WhipHurtBox3;

    public GameObject UpperCutHurtBox;
    public GameObject RAttackHurtBox;
    public GameObject KickHurtBox;
    public GameObject SlashKickHurtBox;
    public GameObject FlipHurtBox;
    public GameObject LightKickHurtBox;
    public GameObject DandyHurtBox;
    public GameObject DandySlamHurtBox;
    public GameObject ChargePunchHurtBox;
    public GameObject SlideKickHurtBox;

    public GameObject KatanaHurtBox;
    public GameObject KatanaHurtBox2;
    public GameObject KatanaThrustHurtBox;

    public GameObject ShotgunHurtBoxBase;
    public GameObject ShotgunHurtBoxTip;
    public GameObject[] ShotgunProjectileBases;

    public GameObject FlameThrowerChokeHB;
    public GameObject FlameThrowerChokeVisual;

    //Makes the laser look like it points from the gunbat tip
    static readonly Vector3[] LASER_POINT_POSITIONS = { new Vector3(0.15f,0.55f), new Vector3(0.5f,0.75f),
        new Vector3(0.85f,0.45f), new Vector3(0.5f,0.15f), new Vector3(-0.05f,0),
        new Vector3(-0.55f,0.1f), new Vector3(-0.85f, 0.45f), new Vector3(-0.5f,0.775f) }; // goes in order of aimdir up clockwise  

    public Vector3[] SPIN_POSITIONS;
    #endregion


    public Transform[] StapleBlastPoints; // For the very cool stapler blast system
    public Transform[] WalkBoxPoints;  // For the very cool collision system
    RaycastHit2D WLeftHit, WRightHit, WUpHit, WDownHit;
    List<IEnemy> KatanaThrustHits = new List<IEnemy>(); // which enemies have already been counted for the thrust reset
    List<GameObject> LightningBalls = new List<GameObject>(); // a container for the lightning balls that have been released by the thrusts

    #region LayerMasks
    public LayerMask WalkMask;
    public LayerMask EnemyMask;
    public LayerMask InteractMask;
    public LayerMask FloorMask;
    
    #endregion

    #region Prefabs
    public GameObject GunBatShot;
    public GameObject GunBatShotChained;
    public GameObject GunBatHookShot;

    public GameObject ShotgunProjectile;
    public GameObject ShotgunExplosion;

    public GameObject TPTVPrefab;
    public GameObject TPTVHitbox;

    public GameObject AddBuddyPrefab;

    public GameObject FlameThrowerProjectile1; // base
    public GameObject FlameThrowerProjectile2;
    public GameObject FlameThrowerProjectile3;
    public GameObject FlameThrowerProjectile4; // blue

    public GameObject FlameOrb;

    public GameObject GroundSpiker;

    public GameObject MortarProjectile;

    public GameObject HitEffect;
    public GameObject HitEffectThrust;
    public GameObject HitEffectSlam;
    public GameObject HitEffectBlood;
    public GameObject HitEffectEnrageHit;

    public GameObject AttackFocusPos; // Focus Position
    public GameObject Whip1FocusPos;
    public GameObject Whip3FocusPos;
    public GameObject SlamFocusPos;

    public GameObject AttackFImage; // Focus Image
    public GameObject SpinFImage;
    public GameObject SlamFImage;
    public GameObject ThrustFImage;
    public GameObject Whip1FImage;
    public GameObject Whip2FImageDiag; // Diagonals
    public GameObject Whip2FImageStrt; // Straights
    public GameObject Whip3FImage;
    public GameObject StaplerFImage;

    public GameObject EnrageHKickProj;
    

    public GameObject GrabHand;

    public GameObject LightningBall; // thrust lightning ball

    public GameObject Staple;

    public GameObject DarkClone;

    GameObject TPTVObject1; // instance
    GameObject TPTVObject2; // instance
    GameObject GrabParent; // instance

    GameObject ConcSpinChild;

    AddictionBuddy AddBuddy; // instance
    #endregion

    #region FaceVariables
    bool StaticFace; // instance
    Sprite CurrentOuchSprite; // instance

    public Image HealthImage;
    public Image HealthImageContainer;

    public Sprite HealthySprite;
    public Sprite OkaySprite;
    public Sprite HurtSprite;
    public Sprite DyingSprite;

    public Sprite HealthyOSprite;
    public Sprite OkayOSprite;
    public Sprite HurtOSprite;
    public Sprite DyingOSprite;
    public Sprite EnragedSprite;

    public Color EnrageColorBase;
    public Color EnrageColorOne;
    public Color EnrageColorTwo;
    public Color EnrageColorThree;
    #endregion

    RaycastHit2D[] AttackCast;
    RaycastHit2D[] ShotgunCast;

    #region ComponentVariables
    PlayerAnimation PA; // Controls the players animations and sprites

    SpriteRenderer SR; // just here so I can change colors 

    public TherapistSFX TSFX;

    PlayerFloorTracker PFT;

    PlayerInput PI;

    CameraControl CC;

    TherLockOn TLO;
    #endregion

    public GameObject HealthGraphObject;

    public DarkClone DarkTherapist;
    IEnemy DarkTherTarget; // Everytime u hit an enemy it becomes DT's target
    GameObject DTTargetObject;
    int DTFlinchTimer = 0;
    int DTAttackTimer = 0;
    int DTZoneTimer = 0;

    Coroutine CurrentAction;



    private void Awake()
    {
        

        ATTACK_DMG = new Damage(50, gameObject);
        
        SLAM_DMG = new Damage(200, gameObject, 50); // flinch 
        SLAM_DMG2 = new Damage(200, gameObject, 75); // flinch 
        SLAM_DMG3 = new Damage(300, gameObject, 100); // flinch 
        THRUST_DMG = new Damage(15, gameObject);
        SPIN_DMG = new Damage(100, gameObject);
        CON_SPIN_DMG = new Damage(150, gameObject);
        FRENZY_DMG = new Damage(150, gameObject);

        

        R_ATTACK_DMG = new Damage(40, gameObject);
        R_ATTACK_DMG2 = new Damage(60, gameObject);

        LIGHTKICK_DMG = new Damage(25, gameObject, LIGHTKICK_FRAMES);
        SLASHKICK_DMG = new Damage(25, gameObject, SLASHKICKFRAMES + SLIDE_KICK_FRAMES);
        KICK_DMG =      new Damage(25, gameObject, (40 * 2) + KICKFRAMES + SLIDE_KICK_FRAMES);

        BATTING_DMG =   new Damage(250, (120), gameObject);


        FLIP_DMG = new Damage(25, 15, gameObject);
        

        SHOTGUN_DMG = new Damage(25, gameObject); // Point blank
        SHOTGUNTIP_DMG = new Damage(25, gameObject);

        GUNBATSLAM_DMG = new Damage(15, 100, gameObject);
        GUNBATIMPACT_DMG = new Damage(10, gameObject);
        GUNBATSHOT_DMG = new Damage(100, 25, gameObject);

        SLIDE_KICK_DMG = new Damage(10, gameObject);

        DARKCLONE_DMG = new Damage(50, gameObject);
        DARKCLONE_DMG2 = new Damage(100, 50, gameObject);

        STAP_SHOT1_DMG = new Damage(1.5f, gameObject);
        STAP_SHOT2_DMG = new Damage(1.5f, gameObject);
        STAP_SHOT3_DMG = new Damage(0.25f, gameObject);

        #region DebugSets
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        
        LockOnInput = false;
        #endregion

        #region ComponentGets
        PI = GetComponent<PlayerInput>();
        SR = GetComponent<SpriteRenderer>();
        PA = GetComponent<PlayerAnimation>();
        CC = Camera.main.GetComponent<CameraControl>();
        TLO = LockOnIndicator.GetComponent<TherLockOn>();
        PFT = GetComponent<PlayerFloorTracker>();
        #endregion

        #region StartingValues
        PDS = PlayerDoingState.Normal;
        

        VisualAim.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f); //Set a default aim direction

        MasterMind.SetPlayer(gameObject); // Tell the mastermind who the player is
        MasterMind.SetBaseTime(1);
        

        Health = MAXHEALTH;
        ActionSpeed = PLAYER_SPEED;
        
        RageExtraLifeCount = 1;

        GunbatRangeSR = GunbatRangeCircle.GetComponent<SpriteRenderer>();
        #endregion

        // this will spawn the buddy that is equipped (sword will be floating sword etc.)
        

        
    }


    // Inputs taken within update to ensure responsiveness
    private void Update()
    {
        // EDITOR COMMANDS (DEBUG)
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                OverrideAction(StartEnrage());
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                RageExtraLifeCount++;
                BurstCount = 1;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Health += 75;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (Time.timeScale == 0.2f)
                {
                    Time.timeScale = 1;
                    MasterMind.SetBaseTime(1f);
                }
                else if (Time.timeScale == 1)
                {
                    Time.timeScale = 0.2f;
                    MasterMind.SetBaseTime(0.2f);
                }
            }
            if (Input.GetKeyDown(KeyCode.J) && AllowAction())
            {
                PA.PlayAnimation("EnrageDodge");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                ConcentrationCounter = MAX_CONC;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                HoldRight = !HoldRight;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                InfiniteHealth = !InfiniteHealth;
            }
            if (Input.GetKeyDown(KeyCode.B)) 
            {

            }
            if (InfiniteRage)
            {
                RageCounter = MAX_RAGE;
            }
            if (HoldRight)
            {
                LRight = true;
            }
        }


        KeyBoardMouse = PI.user.controlScheme.ToString() == "Keyboard&Mouse(<Keyboard>,<Mouse>)";

        if(KatanaQualityCount >= KATANA_QUALITYTHRESH)
        {
            KatanaQualityCount = 0;
            KatanaQuality++;
            
            if(KatanaQuality >= KATANA_QUALITYMAX)
            {
                KatanaQuality = KATANA_QUALITYMAX;
                TSFX.PlayQuality3();
            }
            else
            {
                TSFX.PlayQuality1();
            }
            
        }

        if (IsNotFighting) // this will break everything
            PDS = PlayerDoingState.NoFight;

        #region Enrage
        
        if (RageExtraLifeCount >= 1 && BurstCount <= 0 && Health <= MAXHEALTH * 0.4f && AllowActionBlacklist(EnrageTransformBlacklist)) // if you die but have an extra life then revive with enrage
        {

            OverrideAction(StartEnrage());
        }
        


        if (RageCounter <= 0 && Enraged) // if you run out of time in enrage then end it
        {
            //EndEnrage();
        }

        if(Health >= MAXHEALTH && Enraged) // if you max out ur health in enrage then end it
        {
            //EndEnrage();
            //EnragePop();
        }

        
        #endregion

        if (PauseInputDown && Time.timeScale != 0)
        {
            Time.timeScale = 0;
            PauseInputDown = false;
            Paused = true;
            PauseBG.SetActive(true);
            
        }

        if (PauseInputDown && Time.timeScale == 0 && Paused)
        {
            UnPause();
        }

        if(PreviousHitCounter != HitCounter)
        {
            if(HitCounter == GunBatMoveCD || HitCounter == ReloadMoveCD)
            {
                TSFX.PlayAbilityReady();
            }
            PreviousHitCounter = HitCounter;
        }


        #region LockOnIfs
        if (LockOnInput && !IsLockedOn) 
        {
            SetLockOnDummy(GetClosestLockOn());
            LockOnInput = false;
        }
        else if (LockOnInput && IsLockedOn)
        {
            LockOff();
        }


        if (RLeft && IsLockedOn && InputLockOnWait)// if the rstick is flicked left then find an enemy leftward to lock to
        {
            SetLockOnDummy(GetLeftLockOn());
            InputLockOnWait = false;
        }
        else if (RRight && IsLockedOn && InputLockOnWait)
        {
            SetLockOnDummy(GetRightLockOn());
            InputLockOnWait = false;
        }
        else if (RDown && IsLockedOn && InputLockOnWait) 
        {
            SetLockOnDummy(GetDownLockOn());
            InputLockOnWait = false;
        }
        else if (RUp && IsLockedOn && InputLockOnWait)
        {
            SetLockOnDummy(GetUpLockOn());
            InputLockOnWait = false;
        }

        if(!RUp && !RDown && !RLeft && !RRight)
        {
            InputLockOnWait = true;
        }

        if (IsLockedOn)
        {
            if (LockOnIndicator != null && LockOnDummy != null)
            {
                LockOnIndicator.transform.position = LockOnDummy.transform.position;
            }
            else
            {
                LockOff();
            }
        }
        else
        {
            LockOnIndicator.transform.position = new Vector3(MasterMind.LockOnValue, MasterMind.LockOnValue);

        }
        #endregion

        

        
        // Depending on R stick orientation set visual indicator to different sides and orientation is saved into AimDir
        if (PDS == PlayerDoingState.Normal || PDS == PlayerDoingState.StapleShooting || PDS == PlayerDoingState.WhipAttacking1) // Don't change the aim dir while attacking
        {
            if (KeyBoardMouse)
            {
                AimDir = GetLockOnDir(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            SetAimDirectionToLook(); // Sets aim dir based on the direction of the RIGHT stick
            
            if (UseLockPrototype && IsLockedOn)
            {
                AimDir = GetLockOnDir(LockOnDummy);
            }

            LockOnToEnemy();

            SetVAimToAimDir();


            SetFreeAimToLook(); 

            if (KeyBoardMouse)
            {
                SetFreeAimToMouse();
            }
        }





        if (EnrageInput && BurstCount > 0 && AllowAction())
        {
            
            StartAction(Burst());
            
        }


        #region StaplerIfs


        if ( AAInput && AllowAction() && !Enraged) // START STAPLING
        {
            StartAction(StapleShoot());
        }

        
        //if (!Enraged && AllowAction() && RunDrawInputDown) // RELOAD
        //{
        //    if (HitCounter >= ReloadMoveCD)
        //    {
        //        StartAction(Reload());
        //    }
        //}

        if(!Enraged && RunDrawInput && AllowAction())
        {
            RunDrawInputDown = false;
            StartAction(EnterParryStance());
            //RunDrawInputUp = true;
        }

        if (Enraged && RunDrawInput && AllowAction())
        {
            RunDrawInputDown = false;
            StartAction(EnrageParry());
            //RunDrawInputUp = true;
        }
        //if(!RunDrawInput && RunDrawInputUp && !Enraged && AllowAction())
        //{
        //    StartAction(Parry());
        //    RunDrawInputUp = false;
        //    RunDrawInput = false;
        //}


        #endregion

        if (InKatana)
        {
            KatanaQuality = 2;
            
            if (GunBatInputDown && KatanaParryCoolDown <= 0 && PDS == PlayerDoingState.Normal)
            {
                //StartCoroutine(KatanaParry());
            }

            if (AttackInputDown && AttackCooldown <= 0 && KatanaParryWindow <= 0 && PDS == PlayerDoingState.Normal)
            {
                AttackInputDown = false;
                AttackInputHold = true;
                //StartCoroutine(KatanaAttackSwing());
            }

            if (AttackInputDown && KatanaParryWindow > 0)
            {
                AttackInputDown = false;
                
                //StartCoroutine(KatanaAttackShort());
                KatanaParryWindow = 0;
            }
            if ((LLeft || LRight || LUp || LDown) && KatanaParryWindow > 0  && !LStickHold)
            {
                AttackInputDown = false;
                KatanaThrustHits.Clear();
                KatanaThrustAmmo = KATANA_THRUST_MAXAMMO;
                //StartCoroutine(KatanaThrust());
                KatanaParryWindow = 0;

            }

            
        }

        #region GunBatIfs
        

        if (!GunBatInput)
        {
            GunBatInputHold = false;
        }

        if (GunBatInputDown && !Enraged && PDS == PlayerDoingState.Normal && AttackCooldown <= 0 && AllowAction()) // WHIP
        {
            GunBatInputHold = true;
            GunBatInputDown = false;

            StartAction(WhipAttack1());

            if (LLeft || LRight || LUp || LDown) // for thrust
            {
                LStickHold = true;
            }


        }

        //shotgun
        if (AAInput && (Enraged) && (PDS == PlayerDoingState.Normal || PDS == PlayerDoingState.EnrageAttacking1)  && AllowAction())
        {
            
             StartAction(ShotgunBlast());
            
        }
        #endregion

        #region AttackIfs
        // If attack input then send a boxcast to find all enemies and then tell them they got hit
        if (AttackInputDown && PDS == PlayerDoingState.Normal && !Enraged && AttackCooldown <= 0 && AllowAction()) // normal attack
        {
            AttackInputDown = false; // Attack input down does NOT mean the first frame that the player presses the attack button
            
             StartAction(Attack());
            
            AttackInputHold = true;


            if(LLeft || LRight || LUp || LDown) // for thrust
            {
                LStickHold = true;
            }
        }
        

        if (AttackInputDown && PDS == PlayerDoingState.StapleShooting && AttackCooldown <= 0 && false) // staple spin
        {
            AttackInputDown = false; // Attack input down does NOT mean the first frame that the player presses the attack button

            //StartAction(StapleSpinAttackSwing());

            AttackInputHold = true;


            if (LLeft || LRight || LUp || LDown) // for thrust
            {
                LStickHold = true;
            }
        }

        if (AttackInputDown && PDS == PlayerDoingState.Normal && Enraged && RAttackCooldown <= 0 && AllowAction()) // enrage attack 
        {
            AttackInputHold = true;
            AttackInputDown = false;

            
            bool LstickInput = LLeft || LRight || LUp || LDown;



            
            if (KickComboCount == 0 || true)
            {
                StartAction(EnrageAttack());
            }
            else
            {
                StartAction(EnterChargeStance());
                if (LLeft || LRight || LUp || LDown) // for thrust
                {
                    LStickHold = true;
                }
            }

        }

        if(GunBatInputDown && PDS == PlayerDoingState.Normal && Enraged && RAttackCooldown <= 0 && AllowAction())
        {
            GunBatInputHold = true;
            GunBatInputDown = false;

            if(KickComboCount == 0 || true)
            {
                StartAction(EnterKickStance());
            }
            else
            {
                StartAction(FlipAttackSwing());
            }
        }

        if(!(LLeft || LRight || LUp || LDown))
        {
            LStickHold = false;
        }

        
        

        

        

        if (!AttackInput)
        {
            AttackInputHold = false;

            
        }

        if (!KickAttackInput)
        {
            KickAttackInputHold = false;
        }

        #endregion

        #region DashIfs

        // Stapler Dash 
        if (DashInputDown && DashCooldown <= 0 && AllowAction(StaplerWhitelist))
        {
            StartAction(StaplerDash());
        }

        // Normal Dash
        if (DashInputDown && !Enraged && DashCooldown <= 0 && AllowAction(DashWhitelist))
        {
            StartAction(Dash());
        }

        if (JumpInputDown && !Enraged && JumpCooldown <= 0 && (LLeft || LRight || LUp || LDown) && AllowAction(JumpWhitelist))
        {
            StartAction(Jump());
        }
        if (JumpInputDown && !Enraged && JumpCooldown <= 0 && !(LLeft || LRight || LUp || LDown) && AllowAction(JumpWhitelist))
        {
            StartAction(SpotJump());
        }

        if (JumpInputDown && Enraged && JumpCooldown <= 0 && (LLeft || LRight || LUp || LDown) && AllowAction(JumpWhitelist))
        {
            StartAction(EnrageJump());
        }
        if (JumpInputDown && Enraged && JumpCooldown <= 0 && !(LLeft || LRight || LUp || LDown) && AllowAction(JumpWhitelist))
        {
            StartAction(EnrageSpotJump());
        }




        if (DashInputDown && InKatana && DashCooldown <= 0 && AllowAction())
        {
            StartAction(KatanaRoll());
        }

        // Cartwheel 
        if (DashInputDown && Enraged && RDashCooldown <= 0 && (LLeft || LRight || LUp || LDown) && AllowAction()) // Cartwheel
        {
            StartAction(Cartwheel());
        } 
        // Spot Dodge 
        else if(DashInputDown && Enraged && RDashCooldown <= 0 && !(LLeft || LRight || LUp || LDown) && AllowAction()) // Spot dodge
        {
            StartAction(SpotDodge());
        }
        

        if (!DashInput)
        {
            DashInputHold = false;
        }
        if (!JumpInput)
        {
            JumpInputHold = false;
        }

        #endregion

        

        

        if (TauntInput && PDS == PlayerDoingState.Normal)
        {
            TauntInput = false;
            
            //StartCoroutine(Taunt());
        }




        
        //Health Sprite Ifs
        if (!StaticFace)
        {
            SetCurrentOuchFace();
        }

        //HealthImageContainer.color = new Color(1, Health / 300, Health / 300);
        
        //Gameover
        if (Health <= 0 )
        {

            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            //transform.position = MasterMind.GetCurrentSI().GetStartingPosition();
            //NewPhase();

            //MasterMind.GetCurrentSI().RestartScene();
            
        }

        

        #region ReadingIfs
        /*
        if (RunDrawInput && !RunDrawing && RunDrawMoveCD <= HitCounter && PBS == PlayerBeingState.Normal && PDS == PlayerDoingState.Normal) // Start Rundraw
        {
            RunDrawing = true;
            GlobalVelMul = RUNDRAW_SPEED;
            RunDrawHit = false;
            RunDrawDuration = 1;
            PDS = PlayerDoingState.Reading;
        }

        

        if (RunDrawDuration >= MAX_RUNDRAW_DUR && PopInHealth) // Pop heal block
        {
            TSFX.PlayHeal();
            Health += HEAL_AMOUNT;
            if (Health >= MAXHEALTH)
                Health = MAXHEALTH;
            PDS = PlayerDoingState.Normal;
            RunDrawDuration = 0;
           
        }

        if (RunDrawing && ((PDS != PlayerDoingState.Reading && PBS != PlayerBeingState.Paused) || RunDrawHit || RunDrawDuration == 0 || PBS == PlayerBeingState.Grabbed))// End Rundraw
        {
            if (RunDrawHit)
            {
                PDS = PlayerDoingState.Normal;
            }

            RunDrawing = false;
            GlobalVelMul = PLAYER_SPEED;
            RunDrawMoveCD = HitCounter + RUNDRAW_CD;
            RunDrawHit = false;
            RunDrawDuration = 0;
            
        }
        */
        #endregion

        if (Health > MAXHEALTH)
        {
            Health = MAXHEALTH;
        }
        if(ConcentrationCounter > MAX_CONC)
        {
            ConcentrationCounter = MAX_CONC;
        }
        if(ConcentrationCounter < 0)
        {
            ConcentrationCounter = 0;
        }
        if(BurstCount <= 0)
        {
            BurstHealthThreshold = 0;
        }
        if(Health >= BurstHealthThreshold)
        {
            BurstTimer = 0;
            BurstHealthThreshold = 0;
        }

        if(HealthGraphObject)
            HealthGraphObject.transform.position = new Vector3(HealthGraphObject.transform.position.x + 0.005f, Health - 250);

        #region PASets
        // All my beautiful little PA sets
        PA.SetStapling(PDS == PlayerDoingState.StapleShooting || PDS == PlayerDoingState.StapleDashing);
        PA.SetDashing(PDS == PlayerDoingState.StapleDashing); // only for stapler not dashing in general
        PA.SetLookDir(AimDir);
        
        #endregion

        
        RB.SetRage((int)RageCounter, MAX_RAGE);
        HB.SetHealth((int)Health, MAXHEALTH);
        FB.SetHealth((int)ConcentrationCounter, MAX_CONC);
        BB.SetHealth((int)BurstHealthThreshold, MAXHEALTH);
       

        if (Enraged)
        {
            HealthImage.sprite = EnragedSprite;
        }

        



        LLeftDown = false;
        LRightDown = false;
        LUpDown = false;
        LDownDown = false;
    }


    //Physics calculation done within fixed update to ensure even movement
    private void FixedUpdate()
    {
        #region CD_DurationTickers
        AttackCooldown--;
        RAttackCooldown--;
        DashCooldown--;
        JumpCooldown--;
        RDashCooldown--;

        WhipComboTime--;
        KickComboTime--;
        AttackComboTime--;
        TpComboTime--;

        KatanaParryCoolDown--;
        KatanaParryWindow--;
        
        StunDuration--;
        

        HookShotTimeCD -= Time.deltaTime;

        BurstTimer--;

        DTFlinchTimer--;
        DTAttackTimer--;
        DTZoneTimer--;

        if (BurstTimer <= 0)
        {
            BurstHealthThreshold -= BURST_HEALTH_LOSS;
            
        }
        if(BurstHealthThreshold <= 0)
        {
            BurstHitCount = 0;
        }

        if (BleedKnifeCount > 0)
            BleedKnifeCount--;

        if (StaplingDuration > 0 && StaplingDuration != STAPLE_STARTUP)
            StaplingDuration--;
        
        if(WhipComboTime <= 0)
        {
            WhipComboCount = 0;
        }
        if(KickComboTime <= 0)
        {
            KickComboCount = 0;
        }
        if(AttackComboTime <= 0)
        {
            AttackComboCount = 0;
        }
        if(TpComboTime <= 0)
        {
            TpComboCount = 0;
        }

        if(DarkTherTarget != null)
        {
            if (Enraged && DTFlinchTimer < 0 && DarkTherTarget.GetFlinchable() && Vector2.Distance(transform.position, DTTargetObject.transform.position) < 7) // 
            {
                DarkTherapist.gameObject.SetActive(true);
                DTFlinchTimer = 800;
                DTAttackTimer += 50;
                Vector3 NewPos = DTTargetObject.transform.position + ( DTTargetObject.transform.position - transform.position).normalized * 2;
                DarkTherapist.InitializeSwing(NewPos, (DTTargetObject.transform.position - NewPos).normalized);
            }
            if (Enraged && DTAttackTimer < 0 && Vector2.Distance(transform.position, DTTargetObject.transform.position) < 7)
            {
                DarkTherapist.gameObject.SetActive(true);
                DTAttackTimer = 200;
                DTFlinchTimer += 0;
                Vector3 NewPos = DTTargetObject.transform.position + (DTTargetObject.transform.position - transform.position).normalized * 1;
                DarkTherapist.InitializePunch(NewPos, ( DTTargetObject.transform.position - NewPos).normalized);
            }
            if (!DarkTherapist.gameObject.activeInHierarchy && Enraged && DTZoneTimer < 0)
            {
                DarkTherapist.gameObject.SetActive(true);
                DTZoneTimer = 1200;

                DTFlinchTimer += 100;
                DTAttackTimer += 100;

                float RandX = Random.Range(0, 7);
                float RandY = Random.Range(0, 7);
                Vector3 NewPos = transform.position + new Vector3(RandX, RandY);
                DarkTherapist.InitializeZone(NewPos);
            }
        }
        
        

        if(PDS == PlayerDoingState.BattleStance)
        {
            BattleStanceCount++;
        }
        else
        {
            BattleStanceCount = 0;
        }

        

        if(BattleStanceCount == BATTLESTANCE_MAX)
        {
            TSFX.PlayLockOn();
        }
        if(BattleStanceCount == BATTLESTANCE_SLAM2)
        {
            //TSFX.PlayLockOn();
        }
        if (BattleStanceCount == BATTLESTANCE_SLAM3)
        {
            //TSFX.PlayLockOn();
        }
        

        
        if(BleedKnifeCount % KNIFE_BLEED_DPS == 0 && BleedKnifeCount > 0)
        {
            if(Health - KNIFE_BLEED_DMG > 0) // can't kill you
            {
                Health -= KNIFE_BLEED_DMG;
            }
        }

        

        if(PlayerDoingState.Skating == PDS)
        {
            RocketSkateCount++;
        }

        #endregion


        if (IsStapling)
        {

            //StapleBlast1.transform.position = StapleBlastPoints[(int)AimDir * 2].position;
            //StapleBlast1.transform.rotation = StapleBlastPoints[(int)AimDir * 2 + 1].rotation;
            //StapleBlast2.transform.position = StapleBlastPoints[(int)AimDir * 2].position;
            //StapleBlast2.transform.rotation = StapleBlastPoints[(int)AimDir * 2 + 1].rotation;

            if (StaplingAlternateCount == 0)
            {
                if (StaplingAmmo > 30)
                    TSFX.PlayStaple();
                else if (StaplingAmmo > 0)
                    TSFX.PlayStapleLow();
                SetWalkDirectionToMovement();

                if (AlternateStaple) // Left Hit
                {
                    StapleBlast1.enabled = true; StapleBlast2.enabled = false;

                    CC.Shake(0.05f, 1);
                    RaycastHit2D StapleCast = Physics2D.Linecast(LStapLinecast1.transform.position, LStapLinecast2.transform.position, EnemyMask);
                    if (StapleCast)
                    {
                        StapleCast.transform.GetComponentInParent<IEnemy>().GotHit(STAP_SHOT1_DMG);
                        Instantiate(StapImpact, StapleCast.point, LStapLinecast1.transform.rotation * Quaternion.Euler(0, 0, 90));
                    }
                    if(ConcentrationCounter >= MAX_CONC)
                    {
                        StapleBlast3.enabled = true; StapleBlast4.enabled = false;
                        RaycastHit2D StapleCast2 = Physics2D.Linecast(LStapLinecast1.transform.position, LStapLinecast3.transform.position, EnemyMask);
                        if (StapleCast2)
                        {
                            StapleCast2.transform.GetComponentInParent<IEnemy>().GotHit(STAP_SHOT3_DMG);
                            Instantiate(StapImpact, StapleCast2.point, LStapLinecast1.transform.rotation * Quaternion.Euler(0, 0, 90));
                        }
                    }

                    AlternateStaple = false;
                }
                else // Right Hit
                {
                    StapleBlast2.enabled = true; StapleBlast1.enabled = false;

                    RaycastHit2D StapleCast = Physics2D.Linecast(RStapLinecast1.transform.position, RStapLinecast2.transform.position, EnemyMask);
                    if (StapleCast)
                    {
                        StapleCast.transform.GetComponentInParent<IEnemy>().GotHit(STAP_SHOT2_DMG);
                        Instantiate(StapImpact, StapleCast.point, RStapLinecast1.transform.rotation * Quaternion.Euler(0, 0, 90));
                    }
                    if (ConcentrationCounter >= MAX_CONC)
                    {
                        StapleBlast4.enabled = true; StapleBlast3.enabled = false;
                        RaycastHit2D StapleCast2 = Physics2D.Linecast(RStapLinecast1.transform.position, RStapLinecast3.transform.position, EnemyMask);
                        if (StapleCast2)
                        {
                            StapleCast2.transform.GetComponentInParent<IEnemy>().GotHit(STAP_SHOT3_DMG);
                            Instantiate(StapImpact, StapleCast2.point, RStapLinecast1.transform.rotation * Quaternion.Euler(0, 0, 90));
                        }
                    }
                    AlternateStaple = true;
                }
                SB.SetStapleCount(StaplingAmmo);
                StaplingAlternateCount = STAPLE_DPS;
                //FocusStapleCount++; For focus projectile variation

                if(FocusStapleCount > 18)
                {
                    FocusStapleCount = 0;
                    if(ConcentrationCounter >= MAX_CONC)
                    {
                        MoveOrder[] MO = new MoveOrder[1]
                        {
                            new MoveOrder(VisualAim.transform.up, 0.25f, 25)
                        };
                        Instantiate(StaplerFImage, transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().InitializeImage(0, 25, 0, new Damage(1, gameObject), MO);
                    }
                }

            }
            else
            {
                StapleBlast1.enabled = false;
                StapleBlast2.enabled = false;
                StapleBlast3.enabled = false;
                StapleBlast4.enabled = false;
            }

            StaplingAlternateCount--;
        }


        if (ConfusedDuration > 0)
        {
            ConfusedDuration--;
            //GlobalVelMul = -1;
        }
        
        

        Vector3 TransformAdd = ReturnVelocity();

        TransformAdd *= ActionSpeed;

        

        VelocityX = TransformAdd.x;
        VelocityY = TransformAdd.y;

        if(PDS == PlayerDoingState.Skating) // THIS FINNA BE A SKATER BOY KINDA SITCH
        {
            if(!LeftStickVal.Equals( new Vector2(0, 0)))
            {
                SetFreeAimToMovement();
            }
            
            if (GunBatInput) // if you hold down gunbat you brake
            {
                if(CurrentSkateSpeed > SKATE_SPEED1) // holding brake will slow you and give you a better turn radius to a point
                {
                    CurrentSkateSpeed -= SKATE_BRAKE_SPEEDL;
                }
                if(TurnCoefficient > SKATE_TURNCOEF1)
                {
                    TurnCoefficient -= SKATE_BRAKE_TURNL;
                }
                
            }
            else if (AttackInputDown) // holding attack puts on the gas
            {
                AttackInputDown = false;
                if (CurrentSkateSpeed < SKATE_SPEED3) // holding gas will speed you up and give you a worse turn radius to a point
                {
                    CurrentSkateSpeed += SKATE_GAS_SPEEDG;
                }
                if (TurnCoefficient < SKATE_TURNCOEF3)
                {
                    TurnCoefficient += SKATE_GAS_TURNG;
                }
            }
            else 
            {
                if (CurrentSkateSpeed > SKATE_SPEED2) // doing neither gas nor brake will lose you speed to a point and try to equal out your turn radius
                {
                    CurrentSkateSpeed -= SKATE_NEUT_SPEEDL;
                }
                if(TurnCoefficient < SKATE_TURNCOEF2)
                {
                    TurnCoefficient = SKATE_TURNCOEF2;
                }
            }


            if (RocketSkateCount == 1)
            {
                CC.SetSpeedTo(0.1f);
                CurrentSkateSpeed = SKATE_SPEED1;
                TurnCoefficient = SKATE_TURNCOEF2;

                SkateCurrentMoveDir = GunbatVAim.transform.up;
                SkatePastMoveDir = GunbatVAim.transform.up;
                SkateFutureMoveDir = GunbatVAim.transform.up;
            }
            

            SkateFutureMoveDir = GunbatVAim.transform.up;
            SkateCurrentMoveDir = (SkateFutureMoveDir + SkatePastMoveDir * TurnCoefficient).normalized;
            
            SkatePastMoveDir = SkateCurrentMoveDir;
            
            

            TransformAdd = SkateCurrentMoveDir * CurrentSkateSpeed * MOVE_SPEED;
        }

        

        #region StaplerShots
        
        
        #endregion

        if(ActionMoveDir != Vector3.zero && ActionMoveSpeed != 0)
        {
            TransformAdd = ActionMoveDir * ActionMoveSpeed;
        }

        if (PDS == PlayerDoingState.Normal && KnockbackDuration <= 0) // changes therapist's direction to his movement
        {
            SetWalkDirectionToMovement();
        }

        

        if (SlowDuration > 0)
        {
            SlowDuration--;
            TransformAdd /= SlowSpeed;
        }

        if(WindDuration > 0 ) // potentially allow the wind to push the player even during moments of no movement?
        {
            TransformAdd += (Vector3)WindDirection * WindSpeed;
            WindDuration--;
        }

        

        if (FreezeDuration > 0)
        {
            TransformAdd = Vector2.zero;
            FreezeDuration--;
        }

        if (ForceDuration > 0)
        {
            if (Mathf.Abs(ForceDirection.x) > 0)
            {
                TransformAdd.x = 0;
            }
            if (Mathf.Abs(ForceDirection.y) > 0)
            {
                TransformAdd.y = 0;
            }
            TransformAdd += (Vector3)ForceDirection * ForceSpeed;

            ForceDuration--;
        }

        #region CollisionSystem

        bool IgnoreCollision = false;
        if(PDS == PlayerDoingState.Soaring)
        {
            IgnoreCollision = true;
        }

        //print(TransformAdd);


        // All of this is the collisions system, checks every frame if the linecasts are intersecting a wall and stops the player if they are
        if(Mathf.Abs(TransformAdd.x) > 30) // keeps the max transform add to 30 or below because 32.768 causes int overflow when divided by 0.001
        {
            TransformAdd.x = Mathf.Sign(TransformAdd.x) * 30;
        }
        if (Mathf.Abs(TransformAdd.y) > 30)
        {
            TransformAdd.x = Mathf.Sign(TransformAdd.y) * 30;
        }
        int MovementPerVelocityX = Mathf.Abs((int)(TransformAdd.x / 0.001f));  // Each mm is equal to a tenth of a unit and each tenth collision is checked
        int MovementPerVelocityY = Mathf.Abs((int)(TransformAdd.y / 0.001f));

        for(int i = 0; i < MovementPerVelocityX; i++) // For the x axis
        {
            WLeftHit = Physics2D.Linecast(WalkBoxPoints[6].position, WalkBoxPoints[7].position, WalkMask);
            WRightHit = Physics2D.Linecast(WalkBoxPoints[2].position, WalkBoxPoints[3].position, WalkMask);
            if ((!(WLeftHit && Mathf.Sign(TransformAdd.x) < 0) && !(WRightHit && Mathf.Sign(TransformAdd.x) > 0) ) || IgnoreCollision) // My little monstrous if statement, it checks if you're moving towards a wall and stops you if you are
            {
                transform.position += new Vector3(0.001f * Mathf.Sign(TransformAdd.x), 0);
            }
            else
            {
                //print(WLeftHit.normal);
                //print(WRightHit.normal);
                MovementPerVelocityX = 0;
                KnockbackVector.x = 0;
            }
            
        }
        for (int i = 0; i < MovementPerVelocityY; i++) // For the y axis
        {
            WUpHit = Physics2D.Linecast(WalkBoxPoints[0].position, WalkBoxPoints[1].position, WalkMask);
            WDownHit = Physics2D.Linecast(WalkBoxPoints[4].position, WalkBoxPoints[5].position, WalkMask);
            if ((!(WUpHit && Mathf.Sign(TransformAdd.y) > 0) && !(WDownHit && Mathf.Sign(TransformAdd.y) < 0)) || IgnoreCollision)
            {
                transform.position += new Vector3(0, 0.001f * Mathf.Sign(TransformAdd.y));
            }
            else
            {
                //print(WUpHit.normal);
                //print(WDownHit.normal);
                MovementPerVelocityY = 0;
                KnockbackVector.y = 0;
                
            }
        }
        #endregion

        transform.position = new Vector3((float)System.Math.Round(transform.position.x, 2), (float)System.Math.Round(transform.position.y, 2), transform.position.z);
        // this line keeps the player rounded off at certain decimals to avoid weird "glitchy" coordinates where you can see in between tiled objects


        if (KnockbackVector.x == 0 && KnockbackVector.y == 0 && PDS == PlayerDoingState.SentFlying)
        {

            KnockbackDuration = 0;

            Interrupted = false;
            

            ActionSpeed = PLAYER_SPEED;
            PDS = PlayerDoingState.Normal;
            
            
        }

        //Animator Walking Set
        if ((Mathf.Abs(TransformAdd.x) > 0 || Mathf.Abs(TransformAdd.y) > 0))
        {
            PA.SetWalking(true);
        }
        else
        {
            PA.SetWalking(false);
        }




        FK = PFT.GetCurrentFloor();
        

        if ((FK == FloorKey.Pit) && AllowActionBlacklist(FallingBlacklist))
        {
            OverrideAction(Falling());
        }

        

        if (transform.position.z != 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //The two velocities are set to 0 so the player doesn't slide forever
        //VelocityX = 0;
        //VelocityY = 0;
    }

    void SetVAimToAimDir()
    {
        switch (AimDir) // Sets attack direction to line up with aim direction
        {
            case AimDirKey.Up:
                VisualAim.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case AimDirKey.UpRight:
                VisualAim.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 315);
                break;
            case AimDirKey.Right:
                VisualAim.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case AimDirKey.DownRight:
                VisualAim.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 225);
                break;
            case AimDirKey.Down:
                VisualAim.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case AimDirKey.DownLeft:
                VisualAim.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 135);
                break;
            case AimDirKey.Left:
                VisualAim.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case AimDirKey.UpLeft:
                VisualAim.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f);
                VisualAim.transform.rotation = Quaternion.Euler(0, 0, 45);
                break;
        }
    }

    void LockOnToEnemy()
    {
        if (!(RRight || RLeft || RUp || RDown) && !KeyBoardMouse) // if not receiving input from right stick then set aim dir to closest enemy (if one is within range)
        {
            RaycastHit2D[] ProximityCheck;
            ProximityCheck = Physics2D.CircleCastAll(transform.position, 7, transform.up, 0, EnemyMask);

            GameObject TheFound = gameObject;
            float SmallestDist = 99;
            for (int i = 0; i < ProximityCheck.Length; i++)
            {
                if (Vector2.Distance(ProximityCheck[i].transform.position, transform.position) < SmallestDist)
                {
                    SmallestDist = Vector2.Distance(ProximityCheck[i].transform.position, transform.position);
                    TheFound = ProximityCheck[i].transform.gameObject;
                }
            }
            if (SmallestDist != 99)
            {
                AimDir = GetLockOnDir(TheFound);
            }
        }
    }

    public void SetKnockback(Vector2 KBV, int Duration, float Speed) // Duration is in frames FIX THIS and the enrage section
    {
        OverrideAction(SentFlying(KBV, Duration, Speed));
    }
    
    public void SetWind(Vector2 WBV, int Duration, float Speed) // 0.25f beats out walking but not dashing
    {

        WindDirection = WBV.normalized;
        WindDuration = Duration;
        WindSpeed = Speed;
    }

    public void SetForce(Vector2 FBV, int Duration, float Speed) // same as wind but it override any movement against it
    {

        ForceDirection = FBV.normalized;
        ForceDuration = Duration;
        ForceSpeed = Speed;
    }

    public void SetSoaring(MoveOrder MO) // basically knockback with different animations and can pass through colliders
    {
        OverrideAction(SentSoaring(MO));
    }
    public void SetSoaring(MoveOrder[] MO) // basically knockback with different animations and can pass through colliders
    {
        OverrideAction(SentSoaringComplex(MO));
    }

    Vector2 ReturnVelocity()
    {
        // Movement for each side
        float tempX = 0;
        float tempY = 0;
        if (LLeft)
            tempX -= MOVE_SPEED;
        if (LRight)
            tempX += MOVE_SPEED;
        if (LUp)
            tempY += MOVE_SPEED;
        if (LDown)
            tempY -= MOVE_SPEED;

        if (Mathf.Abs(tempX) == Mathf.Abs(tempY)) // If the player moves diagonally then they move slower
        {
            tempX *= 0.7f;
            tempY *= 0.7f;
        }

        return new Vector2(tempX, tempY);
    }
    Vector2 ReturnVelocity(AimDirKey direction) // Overload for an aimdir
    {
        // Movement for each side
        float tempX = 0;
        float tempY = 0;
        switch (direction)
        {
            case AimDirKey.Down:
                tempY -= MOVE_SPEED;
                break;
            case AimDirKey.Left:
                tempX -= MOVE_SPEED;
                break;
            case AimDirKey.Right:
                tempX += MOVE_SPEED;
                break;
            case AimDirKey.Up:
                tempY += MOVE_SPEED;
                break;
            case AimDirKey.DownRight:
                tempX += MOVE_SPEED;
                tempY -= MOVE_SPEED;
                break;
            case AimDirKey.DownLeft:
                tempX -= MOVE_SPEED;
                tempY -= MOVE_SPEED;
                break;
            case AimDirKey.UpRight:
                tempX += MOVE_SPEED;
                tempY += MOVE_SPEED;
                break;
            case AimDirKey.UpLeft:
                tempX -= MOVE_SPEED;
                tempY += MOVE_SPEED;
                break;
        }

        if (Mathf.Abs(tempX) == Mathf.Abs(tempY)) // If the player moves diagonally then they move slower
        {
            tempX *= 0.7f;
            tempY *= 0.7f;
        }

        return new Vector2(tempX, tempY);
    }

    void EndEnrage()
    {
        RageCounter = 0;
        Invulnerable = false;
        Enraged = false;
        if(PDS == PlayerDoingState.Normal)
            PA.PlayAnimation("Idle");
    }
    void EnragePop()
    {
        
        RaycastHit2D[] EnrageCircleCastLarge = Physics2D.CircleCastAll(transform.position, 10, transform.up, 0, EnemyMask);

        for (int i = 0; i < EnrageCircleCastLarge.Length; i++) // doesn't currently do anything to the guy you hit because of hit invuln
        {
            EnrageCircleCastLarge[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(0, gameObject, transform.position - EnrageCircleCastLarge[i].transform.position, 1f, 3));
        }
    }

    IEnumerator StartEnrage()
    {
        NoOverride = true;
        PA.PlayAnimation("EnrageStart");
        Health = MAXHEALTH ;
        Enraged = true;
        //RageCounter = MAX_RAGE;
        RageExtraLifeCount--;
        PDS = PlayerDoingState.Transforming;
        ActionSpeed = 0;
        Invulnerable = true;
        WalkDir = AimDirKey.Down;


        yield return StartCoroutine(EnrageTransSoundDelay());

        
        for (int i = 0; i < ENRAGE_STARTUP; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        NoOverride = false;
        PA.PlayAnimation("EnrageIdle");
        Invulnerable = false;
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
    }

    IEnumerator EnrageTransSoundDelay()
    {
        yield return new WaitForSeconds(0.25f);
        RaycastHit2D[] EnrageCircleCastLarge = Physics2D.CircleCastAll(transform.position, 15, transform.up, 0, EnemyMask);
        RaycastHit2D[] EnrageCircleCastSmall = Physics2D.CircleCastAll(transform.position, 3, transform.up, 0, EnemyMask);
        for (int i = 0; i < EnrageCircleCastLarge.Length; i++)
        {
            EnrageCircleCastLarge[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(0, 80, gameObject));
        }
        for (int i = 0; i < EnrageCircleCastSmall.Length; i++)
        {
            EnrageCircleCastSmall[i].transform.GetComponentInParent<IEnemy>().GotHit(
                new Damage(0, gameObject, (EnrageCircleCastSmall[i].transform.position - transform.position).normalized, 0.25f, 10));
        }
        TSFX.PlayEnrage();
    }


    #region SetDirTo
    void SetAimDirToVector(Vector2 Dir)
    {
        //Dir = Dir.normalized;

        int Count = 1; // Count is added or multiplied based on which conditions are met
        
        if(Mathf.Abs(Dir.x) >  0.0001f ) 
        {
            if(Dir.x > 0f) // Right facing
            {
                Count = 2;
            }
            else // Left Facing
            {
                Count = -1;
            }
        }
        if(Mathf.Abs(Dir.y) > 0.0001f)
        {
            if(Dir.y > 0f) // Up facing
            {
                Count *= 3;
            }
            else // Down Facing
            {
                Count *= 4;
            }
        }
        
        switch (Count)
        {
            case -1:
                AimDir = AimDirKey.Left;
                break;
            case 1: // Default
                AimDir = AimDirKey.Down;
                break;
            case 2:
                AimDir = AimDirKey.Right;
                break;
            case 3:
                AimDir = AimDirKey.Up;
                break;
            case 4:
                AimDir = AimDirKey.Down;
                break;
            case 6:
                AimDir = AimDirKey.UpRight;
                break;
            case -3:
                AimDir = AimDirKey.UpLeft;
                break;
            case -4:
                AimDir = AimDirKey.DownLeft;
                break;
            case 8:
                AimDir = AimDirKey.DownRight;
                break;
        }
        
    }
    void SetFreeAimToLook()
    {
        if (Mathf.Abs(RightStickVal.x) + Mathf.Abs(RightStickVal.y) >= 1)
        {
            GunbatVAim.transform.localPosition = RightStickVal;
        }
        else
        {
            GunbatVAim.transform.position = VisualAim.transform.position;
        }
        GunbatVAim.transform.up = GunbatVAim.transform.position - transform.position;
        LaserAim2.transform.up = -GunbatVAim.transform.up;
    }
    void SetFreeAimToMovement()
    {
        GunbatVAim.transform.localPosition = LeftStickVal;
        
        GunbatVAim.transform.up = GunbatVAim.transform.position - transform.position;
        LaserAim2.transform.up = -GunbatVAim.transform.up;
    }
    void SetFreeAimToMouse()
    {
        Vector3 mosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mosPos.z = 0;
        GunbatVAim.transform.localPosition = (mosPos - transform.position).normalized;
        GunbatVAim.transform.up = GunbatVAim.transform.position - transform.position;
        LaserAim2.transform.up = -GunbatVAim.transform.up;
    }
    
    void SetAimDirectionToMovement() // Set aimdir based on left stick USED FOR THRUST :)
    {
        if (LDown) AimDir = AimDirKey.Down;
        if (LUp) AimDir = AimDirKey.Up;
        if (LLeft) AimDir = AimDirKey.Left;
        if (LLeft && LDown) AimDir = AimDirKey.DownLeft;
        if (LLeft && LUp) AimDir = AimDirKey.UpLeft;
        if (LRight) AimDir = AimDirKey.Right;
        if (LRight && LDown) AimDir = AimDirKey.DownRight;
        if (LRight && LUp) AimDir = AimDirKey.UpRight;
    }
    void SetWalkDirectionToMovement() // Sets the walkdir based on the left stick
    {
        if (LDown) WalkDir = AimDirKey.Down;
        if (LUp) WalkDir = AimDirKey.Up;
        if (LLeft) WalkDir = AimDirKey.Left;
        if (LLeft && LDown) WalkDir = AimDirKey.DownLeft;
        if (LLeft && LUp) WalkDir = AimDirKey.UpLeft;
        if (LRight) WalkDir = AimDirKey.Right;
        if (LRight && LDown) WalkDir = AimDirKey.DownRight;
        if (LRight && LUp) WalkDir = AimDirKey.UpRight;


        PA.SetMoveDir(WalkDir);
    }

    void SetWalkDirectionToVector(Vector2 dir) // sets walkdir based on a facing direction (used in roll direction calculation)
    {
        if(dir.x ==0 && dir.y == 0)
        {
            return;
        }
        else if(dir.x == 0)
        {
            if(dir.y > 0)
            {
                WalkDir = AimDirKey.Up;
            }
            else
            {
                WalkDir = AimDirKey.Down;
            }
        }
        else if(dir.y == 0)
        {
            if(dir.x > 0)
            {
                WalkDir = AimDirKey.Right;
            }
            else
            {
                WalkDir = AimDirKey.Left;
            }
        }
        else if(dir.x == dir.y)
        {
            if(dir.x < 0)
            {
                WalkDir = AimDirKey.DownLeft;
            }
            else
            {
                WalkDir = AimDirKey.UpRight;
            }
        }
        else if(dir.x > dir.y)
        {
            WalkDir = AimDirKey.DownRight;
        }
        else
        {
            WalkDir = AimDirKey.UpLeft;
        }

        PA.SetMoveDir(WalkDir);
    }

    void SetAimDirectionToLook() // Sets the animators DIR to rightstick
    {
        if (RDown) AimDir = AimDirKey.Down;
        if (RUp) AimDir = AimDirKey.Up;
        if (RLeft) AimDir = AimDirKey.Left;
        if (RLeft && RDown) AimDir = AimDirKey.DownLeft;
        if (RLeft && RUp) AimDir = AimDirKey.UpLeft;
        if (RRight) AimDir = AimDirKey.Right;
        if (RRight && RDown) AimDir = AimDirKey.DownRight;
        if (RRight && RUp) AimDir = AimDirKey.UpRight;
    }
    void SetWalkDirectionToLook() // Sets the walk DIR to rightstick (unused)
    {
        if (RDown) WalkDir = AimDirKey.Down;
        if (RUp) WalkDir = AimDirKey.Up;
        if (RLeft) WalkDir = AimDirKey.Left;
        if (RLeft && RDown) WalkDir = AimDirKey.DownLeft;
        if (RLeft && RUp) WalkDir = AimDirKey.UpLeft;
        if (RRight) WalkDir = AimDirKey.Right;
        if (RRight && RDown) WalkDir = AimDirKey.DownRight;
        if (RRight && RUp) WalkDir = AimDirKey.UpRight;

        PA.SetMoveDir(WalkDir);
    }
    void SetWalkDirToAimDir() // these both do what they say on the tin
    {
        WalkDir = AimDir;
    }
    void SetMoveDirToWalkDir()
    {
        AimDir = WalkDir;
    }
    #endregion

    #region LockOn

    void LockOff()
    {
        UnlockDummy();
        LockOnInput = false;
    }

    void SetLockOnDummy(GameObject Lock) // Sets lock on dummy to whatever object is given
    {
        TLO.NewLockOn();
        TSFX.PlayLockOn();
        LockOnDummy = Lock;
        IsLockedOn = true;

        if(LockOnDummy == null)
        {
            UnlockDummy();
        }
    }
    void UnlockDummy() // Unlocks dummy
    {
        LockOnDummy = null;
        IsLockedOn = false;
    }

    AimDirKey GetLockOnDir(GameObject LockOnObj)// returns the direction that the lock on makes the player face
    {
        Vector2 LockPos = LockOnObj.transform.position;
        Vector2 SelfPos = transform.position;

        float XDif = Mathf.Abs(LockPos.x - SelfPos.x); // The difference betweeen player's and lock-on's X position
        float YDif = Mathf.Abs(LockPos.y - SelfPos.y); // same for Y

        bool XBool =  XDif < LOCK_ON_DIF * YDif * LOCK_ON_MULT; // This effectively makes the zones where the player switches directions into a cone rather than a square for horizontal movement
        bool YBool =  YDif < LOCK_ON_DIF * XDif * LOCK_ON_MULT; // same but for vertical

        if (XBool && LockPos.y > SelfPos.y) return AimDirKey.Up; 
        else if (LockPos.x > SelfPos.x && YBool) return AimDirKey.Right;
        else if (XBool && LockPos.y < SelfPos.y) return AimDirKey.Down; 
        else if (LockPos.x < SelfPos.x && YBool) return AimDirKey.Left; 

        else if (LockPos.x > SelfPos.x && LockPos.y > SelfPos.y) return AimDirKey.UpRight; 
        else if (LockPos.x > SelfPos.x && LockPos.y < SelfPos.y) return AimDirKey.DownRight; 
        else if (LockPos.x < SelfPos.x && LockPos.y < SelfPos.y) return AimDirKey.DownLeft; 
        else if (LockPos.x < SelfPos.x && LockPos.y > SelfPos.y) return AimDirKey.UpLeft; 

        else return 0; // default
            
    }
    AimDirKey GetLockOnDir(Vector2 LockOnPos)// returns the direction that the lock on makes the player face
    {
        Vector2 LockPos = LockOnPos;
        Vector2 SelfPos = transform.position;

        float XDif = Mathf.Abs(LockPos.x - SelfPos.x); // The difference betweeen player's and lock-on's X position
        float YDif = Mathf.Abs(LockPos.y - SelfPos.y); // same for Y

        bool XBool = XDif < LOCK_ON_DIF * YDif * LOCK_ON_MULT; // This effectively makes the zones where the player switches directions into a cone rather than a square for horizontal movement
        bool YBool = YDif < LOCK_ON_DIF * XDif * LOCK_ON_MULT; // same but for vertical

        if (XBool && LockPos.y > SelfPos.y) return AimDirKey.Up;
        else if (LockPos.x > SelfPos.x && YBool) return AimDirKey.Right;
        else if (XBool && LockPos.y < SelfPos.y) return AimDirKey.Down;
        else if (LockPos.x < SelfPos.x && YBool) return AimDirKey.Left;

        else if (LockPos.x > SelfPos.x && LockPos.y > SelfPos.y) return AimDirKey.UpRight;
        else if (LockPos.x > SelfPos.x && LockPos.y < SelfPos.y) return AimDirKey.DownRight;
        else if (LockPos.x < SelfPos.x && LockPos.y < SelfPos.y) return AimDirKey.DownLeft;
        else if (LockPos.x < SelfPos.x && LockPos.y > SelfPos.y) return AimDirKey.UpLeft;

        else return 0; // default

    }

    GameObject GetClosestLockOn()
    {
        GameObject CurrentClosest = null;

        for(int i = 0; i < LockOnParent.childCount; i++)
        {
            Transform CurrentChild = LockOnParent.GetChild(i);

            if (Vector2.Distance(CurrentChild.position, transform.position) < MAX_LOCK_DIST)
            {
                if (!CurrentClosest)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
                else if(Vector2.Distance(CurrentClosest.transform.position, transform.position) > Vector2.Distance(CurrentChild.position, transform.position))
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
            }
            
        }

        return CurrentClosest;
    }

    GameObject GetDownLockOn()
    {
        if (LockOnDummy == null)
        {
            return null;
        }

        
        GameObject CurrentClosest = null;
        for (int i = 0; i < LockOnParent.childCount; i++) // every enemy is looped through
        {
            Transform CurrentChild = LockOnParent.GetChild(i);

            if (CurrentChild.position.y < LockOnDummy.transform.position.y && LockOnDummy.transform.position != CurrentChild.position) // if the enemy is below the target
            {
                print("Phase1 of: " + CurrentChild);
                float XDif = Mathf.Abs(CurrentChild.position.x - LockOnDummy.transform.position.x);
                float YDif = Mathf.Abs(CurrentChild.position.y - LockOnDummy.transform.position.y);

                print("Phase2 of: " + CurrentChild);
                if (CurrentClosest == null) // if no other enemy has been selected yet
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
                else if (YDif + XDif * SWITCH_LOCK_DIF < Mathf.Abs(CurrentClosest.transform.position.y - LockOnDummy.transform.position.y) + Mathf.Abs(CurrentClosest.transform.position.x - LockOnDummy.transform.position.x) * SWITCH_LOCK_DIF)
                { // Weighs the difference of Y difference and X difference of the enemy vs the last enemy saved
                    // what is not so obvious is that the switch lock dif is 400 which basically makes ydif never metter unless it's a complete tie(both the enemy and saved enemy have same x value)
                    // also which ever one is weighed more heavily is based on wether it is a vertical or horizontal switch
                    CurrentClosest = CurrentChild.gameObject;
                }

            }

        }

        if(CurrentClosest == null)
        {
            return LockOnDummy;
        }

        return CurrentClosest;
    }

    GameObject GetUpLockOn()
    {
        if (LockOnDummy == null)
        {
            return null;
        }


        GameObject CurrentClosest = null;
        for (int i = 0; i < LockOnParent.childCount; i++)
        {
            Transform CurrentChild = LockOnParent.GetChild(i);

            if (CurrentChild.position.y > LockOnDummy.transform.position.y && LockOnDummy.transform.position != CurrentChild.position)
            {
                
                float XDif = Mathf.Abs(CurrentChild.position.x - LockOnDummy.transform.position.x);
                float YDif = Mathf.Abs(CurrentChild.position.y - LockOnDummy.transform.position.y);


                if (CurrentClosest == null)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
                else if (YDif + XDif * SWITCH_LOCK_DIF < Mathf.Abs(CurrentClosest.transform.position.y - LockOnDummy.transform.position.y) + Mathf.Abs(CurrentClosest.transform.position.x - LockOnDummy.transform.position.x) * SWITCH_LOCK_DIF)
                {

                    CurrentClosest = CurrentChild.gameObject;
                }

            }
        }

        if (CurrentClosest == null)
        {
            return LockOnDummy;
        }

        return CurrentClosest;
    }

    GameObject GetLeftLockOn()
    {
        if (LockOnDummy == null)
        {
            return null;
        }


        GameObject CurrentClosest = null;
        for (int i = 0; i < LockOnParent.childCount; i++)
        {
            Transform CurrentChild = LockOnParent.GetChild(i);

            if (CurrentChild.position.x < LockOnDummy.transform.position.x && LockOnDummy.transform.position != CurrentChild.position)
            {
                
                float XDif = Mathf.Abs(CurrentChild.position.x - LockOnDummy.transform.position.x);
                float YDif = Mathf.Abs(CurrentChild.position.y - LockOnDummy.transform.position.y);

                if (CurrentClosest == null)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
                else if (XDif + YDif * SWITCH_LOCK_DIF < Mathf.Abs(CurrentClosest.transform.position.x - LockOnDummy.transform.position.x) + Mathf.Abs(CurrentClosest.transform.position.y - LockOnDummy.transform.position.y) * SWITCH_LOCK_DIF)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
            }

        }

        if (CurrentClosest == null)
        {
            return LockOnDummy;
        }

        return CurrentClosest;
    }

    GameObject GetRightLockOn()
    {
        if (LockOnDummy == null)
        {
            return null;
        }


        GameObject CurrentClosest = null;
        for (int i = 0; i < LockOnParent.childCount; i++)
        {
            Transform CurrentChild = LockOnParent.GetChild(i);

            if (CurrentChild.position.x > LockOnDummy.transform.position.x && LockOnDummy.transform.position != CurrentChild.position)
            {
                
                float XDif = Mathf.Abs(CurrentChild.position.x - LockOnDummy.transform.position.x);
                float YDif = Mathf.Abs(CurrentChild.position.y - LockOnDummy.transform.position.y);

                
                if (CurrentClosest == null)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }
                else if (XDif + YDif * SWITCH_LOCK_DIF < Mathf.Abs(CurrentClosest.transform.position.x - LockOnDummy.transform.position.x) + Mathf.Abs(CurrentClosest.transform.position.y - LockOnDummy.transform.position.y) * SWITCH_LOCK_DIF)
                {
                    CurrentClosest = CurrentChild.gameObject;
                }

            }

        }

        if (CurrentClosest == null)
        {
            return LockOnDummy;
        }

        return CurrentClosest;
    }
    #endregion

    #region Debuff
    public void FreezeCountSet(float SetTo)
    {
        FreezeDuration = SetTo;
    }
    public void SlowCountSet(float Duration, float Speed)
    {
        SlowDuration = Duration;
        SlowSpeed = Speed;
    }
    public void ConfuseCountSet(float SetTo) // Set the players confuse duration to a number (number of seconds)
    {
        ConfusedDuration = SetTo;
    }
    #endregion

    public bool GotHit(Damage Damage) 
    {
        if (IsGrabbedUnlocked && HitInvul /*&& !Damage.GrabDamage*/)
        {
            //return false;
            GrabParent.SendMessage("LetGoGrab");
        }

        if (IsParrying)
        {
            print("PARRIED");
            //KatanaParryWindow = PARRY_WINDOW;

            if (AllowAction(ParryWhitelist))
            {
                StartAction(ParrySuccess());
                if(Damage.DmgType == DamageType.Normal)
                {
                    if(ConcentrationCounter >= MAX_CONC)
                    {
                        Damage.Source.GetComponentInParent<IEnemy>().GotHit(new Damage(ParryDamage + 100, ParryStun, gameObject));
                    }
                    else
                    {
                        Damage.Source.GetComponentInParent<IEnemy>().GotHit(new Damage(ParryDamage, ParryStun, gameObject));
                    }
                }
                    
            }
            return false;
            
        }

        bool GrabDamageImmune = false;
        if (IsGrabbedLocked)
        {
            if (!GrabParent.Equals(Damage.Source)) // you cannot be hurt by something that is not your grab parent
            {
                GrabDamageImmune = true;
            }
        }

        bool GettingHit = true;

        if (Damage.Low && JumpInvul) // if it's a low and ur jumping then dodge
        {
            GettingHit = false;
            
        }

        else if (Damage.High && DashInvul)// if it's NOT a low and ur dashing then dodge*
        {
            GettingHit = false;
        } 
            

        if (PDS == PlayerDoingState.Soaring) // invincible during stop signs flings
            GettingHit = false;

        if (HitInvul && GettingHit && !Invulnerable && !IsParrying && !GrabDamageImmune) // if not invulnerable or parrying or lock grabbed
        {
            float Dmg = Damage.DmgAmount;
            bool InvulType = false;

            if(Dmg > 0)
            {
                ConcentrationCounter -= MAX_CONC / 2;
            }
            
            if(Dmg > 0)
            {
                if(BurstHitCount == 0)
                {
                    BurstHealthThreshold = Health;
                    BurstHitCount++;
                    BurstTimer = 75;
                }
                else if(BurstHitCount == 1)
                {
                    BurstHitCount++;
                    BurstTimer = 65;
                }
                else if(BurstHitCount == 2)
                {
                    BurstHitCount++;
                    BurstTimer = 55;
                }
                else
                {
                    BurstHitCount++;
                    BurstTimer = 45;
                }
            }

            if(Dmg > MAXHEALTH * 0.3f)
            {
                if(RageExtraLifeCount >= 1 && BurstCount <= 0 && AllowActionBlacklist(EnrageTransformBlacklist))
                {
                    Dmg = 0;
                    OverrideAction(StartEnrage());
                }
            }
            
            if(Dmg == 0)
            {
                InvulType = true;
            }

            CC.Shake(0.5f, 3);

            KatanaParryCoolDown = PARRY_HIT_CD;

            if (InfiniteHealth) // debug infinite health means you take no damage
            {
                Dmg = 0;
            }

            if ((Health > HEALTH_DEATH_THRESH) && Health - Dmg <= 0) // if above health threshold OR enraged then you cannot die
            {
                Dmg = 0;
                if(Health > 0)
                    Health = 1;
            }
            
            if(Dmg > 0 && GetVisibility()) // if player is visible then shoot out blood
            {
                GameObject HitObject = Instantiate(HitEffectBlood, transform.position, Quaternion.identity);
                HitObject.transform.up =  Damage.Source.transform.position - transform.position;
                HitObject.GetComponent<HitEffect>().StartingVariables(gameObject);
            }

            Health -= Dmg; // the damage

            SetCurrentOuchFace();
            StartCoroutine(ChangeFace(CurrentOuchSprite, 0.5f));

            
            HitInvul = false;

            TSFX.PlayHurt();
            if (InvulType)
            {
                Time.timeScale = 0;
                StartCoroutine(HitInvulWait2());
            }
            else
            {
                Time.timeScale = 0;
                StartCoroutine(HitInvulWait());
            }

            if (!IsGrabbedLocked)
            {
                if ((Dmg != 0 || InfiniteHealth) && Damage.KnockBack == Vector2.zero && DashDuration2 <= 0 && KatanaDashDuration <= 0) // basic knockback for all damage
                    SetKnockback((transform.position - Damage.Source.transform.position).normalized, 3, 0.75f);
                if (!DashInvul && (Damage.KnockBack != Vector2.zero) && Damage.StunDuration <= 0) // special knockback for certain attacks
                {
                    SetKnockback(Damage.KnockBack, Damage.KBDuration, Damage.KBSpeed);
                }
            }
            

            if (Damage.StunDuration > 0)
            {
                print("STUN IS NOT IMPLEMENTED FOR THERAPIST");
                //Stunned(Damage.StunDuration);
            }

            if (Damage.FlinchDuration > 0)
            {
                print("FLINCH IS NOT IMPLEMENTED FOR THERAPIST");
            }
            if (Damage.Parry)
            {
                print("PARRY IS NOT IMPLEMENTED FOR THERAPIST");
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GotGrabHit(Damage Damage)
    {
        Health -= Damage.DmgAmount;
        return false;
    }

    public bool GotStung(Damage Damage)
    {
        if (!RollInvul && !Invulnerable && StingInvul)
        {
            float Dmg = Damage.DmgAmount;
            if (Health - Dmg <= 0) // sting damage can't kill you
            {
                Dmg = 0;
                if(Health > 0)
                    Health = 1;
            }

            if (InfiniteHealth) // debug infinite health means you take no damage
            {
                Dmg = 0;
            }
            CC.Shake(0.02f, 3);
            Health -= Dmg;

            StingInvul = false;
            StartCoroutine(StingInvulWait());

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LockGrabbed(Vector3 Position, GameObject Grabber) // if player is grabbed hide his sprite and make him unable to move
    {
        if (!Invulnerable && !IsGrabbedLocked && AllowActionBlacklist(GrabBlacklist))
        {
            
            GrabParent = Grabber;

            transform.position = new Vector3(Position.x, Position.y, 0);
            StunDuration = -1;

            OverrideAction(GrabLocked());
            
            return true;
        }
        return false;
    }
    public bool UnlockGrabbed(Vector3 Position, GameObject Grabber) // if player is grabbed hide his sprite and make him unable to move
    {
        if (!Invulnerable && !IsGrabbedUnlocked && !IsGrabbedLocked && AllowActionBlacklist(GrabBlacklist))
        {
            GrabParent = Grabber;

            transform.position = new Vector3(Position.x, Position.y, 0);
            StunDuration = -1;

            OverrideAction(GrabUnlocked());

            return true;
        }
        return false;
    }
    public void LetGo(Vector2 newPos) // when player is let go show his sprite and allow him to move again
    {
        transform.position = new Vector3(newPos.x, newPos.y, 0);

        LetGo();
    }
    public void LetGo() // when player is let go show his sprite and allow him to move again
    {
        IsGrabbedLocked = false;
        IsGrabbedUnlocked = false;
    }


    #region UIGets
    public int GetMaxRage()
    {
        return MAX_RAGE;
    } 
    public int GetRageCount()
    {
        return (int)RageCounter;
    }
    public int GetMaxHealth()
    {
        return MAXHEALTH;
    }
    public int GetHealthCount()
    {
        return (int)Health;
    }
    public int GetLives()
    {
        return RageExtraLifeCount;
    }
    
    public bool GetEnraged()
    {
        return Enraged;
    }
    
    public int GetRagingCD()
    {
        return RAGE_HEAL_CD;
    }
    public int GetHitCount()
    {
        return HitCounter;
    }
    public int GetShotgunMoveCD()
    {
        return ShotgunMoveCD;
    }
    public int GetShotgunCD()
    {
        return SHOTGUN_CD;
    }
    public int GetGunbatMoveCD()
    {
        return GunBatMoveCD;
    }
    public int GetGunbatCD()
    {
        return GUNBAT_CD;
    }
    public int GetReloadMoveCD()
    {
        return ReloadMoveCD;
    }
    public int GetReadingMoveCD()
    {
        return 0;
    }
   
    #endregion

    void ToggleVisibility(bool enabled)
    {
        if (!VisibilityLocked)
        {
            SR.enabled = enabled;
        }
    }
    bool GetVisibility()
    {
        return SR.enabled;
    }

    void SetCurrentOuchFace()
    {
        
        if (Health >= HEALTHYTHRESH)
        {
            HealthImage.sprite = HealthySprite;
            CurrentOuchSprite = HealthyOSprite;
        }
        else if (Health >= OKAYTHRESH)
        {
            HealthImage.sprite = OkaySprite;
            CurrentOuchSprite = OkayOSprite;
        }
        else if (Health >= HURTTHRESH)
        {
            HealthImage.sprite = HurtSprite;
            CurrentOuchSprite = HurtOSprite;
        }
        else
        {
            HealthImage.sprite = DyingSprite;
            CurrentOuchSprite = DyingOSprite;
        }
        
    }

    public void PickUpFlameOrb()
    {
        RageCounter += 40;
    }

    public void UnPause()
    {
        Time.timeScale = MasterMind.GetBaseTime();
        PauseInputDown = false;
        Paused = false;
        PauseBG.SetActive(false);
    }

    public void RefreshCooldowns()
    {
        HitCounter += 50;
    }

    public void RefreshLives()
    {
        RageExtraLifeCount = 1;
    }

    public void RefreshBurst()
    {
        BurstCount = 1;
    }

    public void RefreshHealth()
    {
        Health = MAXHEALTH;
        TSFX.PlayBigHeal();
    }

    public void RefreshAmmo()
    {
        StaplingAmmo = STAPLE_MAXAMMO;
        SB.Reload();
        SB.SetStapleCount(StaplingAmmo);
    }

    public void ClearConcentration()
    {
        ConcentrationCounter = 0;
    }


    public void NewPhase()
    {
        ConcentrationCounter += MAX_CONC / 2;
        RefreshHealth();
        RefreshBurst();
        EndEnrage();
        //RefreshLives();
        //RefreshAmmo();
    }

    

    IEnumerator StingInvulWait()
    {
        yield return new WaitForFixedUpdate();
        StingInvul = true;
    }

    IEnumerator HitInvulWait()
    {
        if (GetVisibility())
        {
            StartCoroutine(HitInvulBlink());
        }
        
        Time.timeScale = MasterMind.GetBaseTime();
        yield return new WaitForSecondsRealtime(0.55f);
        HitInvul = true;
        
    }

    IEnumerator HitInvulBlink()
    {
        ToggleVisibility(false);
        yield return new WaitForSecondsRealtime(0.15f);
        ToggleVisibility(true);
        yield return new WaitForSecondsRealtime(0.1f);
        ToggleVisibility(false);
        yield return new WaitForSecondsRealtime(0.1f);
        ToggleVisibility(true);
        yield return new WaitForSecondsRealtime(0.2f);
    }

    IEnumerator HitInvulWait2() // if hit by a 0 dmg source then invulnerability is for a single frame
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = MasterMind.GetBaseTime();
        yield return new WaitForEndOfFrame();
        HitInvul = true;

    }
    IEnumerator HitInvulGrabbedWait()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = MasterMind.GetBaseTime();
        yield return new WaitForSecondsRealtime(0.4f);
        HitInvul = true;
    }

    IEnumerator ChangeFace(Sprite FaceToChange, float Time)
    {
        StaticFace = true;
        HealthImage.sprite = FaceToChange;
        yield return new WaitForSeconds(Time);
        StaticFace = false;
    }

    bool AllowActionBlacklist(PlayerDoingState[] BlackList) // for actions that can override everything but certain moves
    {
        
        bool Allowed = true;
        for (int i = 0; i < BlackList.Length; i++)
        {
            if (BlackList[i] == PDS)
            {
                Allowed = false;
            }
        }
        if (Allowed && !Paused && !NoOverride)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    bool AllowAction(PlayerDoingState[] WhiteList) // for actions that can override a specific number of moves
    {
        bool Allowed = false;
        for(int i = 0; i < WhiteList.Length; i++)
        {
            if(WhiteList[i] == PDS)
            {
                Allowed = true;
            }
        }
        if(Allowed && !Paused && !NoOverride)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    bool AllowAction() // Every action calls this to check if it has the go ahead
    {
        if (!Paused && PDS == PlayerDoingState.Normal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Coroutine OverrideAction(IEnumerator Coroutine)
    {
        if (!NoOverride)
        {
            if (CurrentAction != null)
                StopCoroutine(CurrentAction);

            return StartAction(Coroutine);
        }
        else
        {
            return null;
        }
    }

    private Coroutine StartAction(IEnumerator Coroutine) // resets all instance variables so that if an action gets overridden it does not permanently leave a variable as the wrong value
    {
        
        if (CurrentAction != null)
            StopCoroutine(CurrentAction);

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        DashInvul = false;
        RollInvul = false;
        JumpInvul = false;
        IsParrying = false;
        ActionMoveDir = Vector2.zero;
        ActionMoveSpeed = 0;
        BattleStanceCount = 0;
        NoOverride = false;
        StapleBlast1.enabled = false;
        StapleBlast2.enabled = false;
        StapleBlast3.enabled = false;
        StapleBlast4.enabled = false;
        IsStapling = false;
        VisibilityLocked = false;
        ToggleVisibility(true);
        IsGrabbedLocked = false;
        IsGrabbedUnlocked = false;
        SR.color = Color.white;

        return CurrentAction = StartCoroutine(Coroutine);
    }

    #region Action Coroutines

    IEnumerator GrabLocked()
    {
        PDS = PlayerDoingState.Grabbed;

        ActionSpeed = 0;

        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;

        ToggleVisibility(false);
        VisibilityLocked = true;

        IsGrabbedLocked = true;

        while (IsGrabbedLocked)
        {
            yield return new WaitForFixedUpdate();
        }

        VisibilityLocked = false;
        ToggleVisibility(true);
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        PA.PlayAnimation("Idle");
    }

    IEnumerator GrabUnlocked()
    {
        PDS = PlayerDoingState.Grabbed;

        ActionSpeed = 0;

        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;

        ToggleVisibility(false);
        VisibilityLocked = true;

        IsGrabbedUnlocked = true;

        while (IsGrabbedUnlocked)
        {
            yield return new WaitForFixedUpdate();
        }

        VisibilityLocked = false;
        ToggleVisibility(true);
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        PA.PlayAnimation("Idle");
    }

    IEnumerator Falling()
    {
        NoOverride = true;
        
        PA.PlayAnimation("Falling");
        PDS = PlayerDoingState.Falling;
        ActionSpeed = 0;
        

        for(int i = 0; i < FALLING_FRAMES && !(EnrageInput && BurstCount > 0); i++) // if the conditions for bursting are true then stop falling and save urself
        {
            yield return new WaitForFixedUpdate();
        }

        if(EnrageInput && BurstCount > 0)
        {
            NoOverride = false;
            StartAction(BurstJump());
            yield break;
        }

        Health = 0;
    }

    IEnumerator BurstJump()
    {
        PDS = PlayerDoingState.Jumping;
        PA.PlayAnimation("Jumping");
        SR.sortingOrder = 999;
        ActionSpeed = JUMPBURST_SPEED;

        for(int i = 0; i < 25; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        PDS = PlayerDoingState.Normal;
        if (Enraged)
        {
            PA.PlayAnimation("EnrageIdle");
        }
        else
        {
            PA.PlayAnimation("Idle");
        }
        ActionSpeed = PLAYER_SPEED;
        SR.sortingOrder = 5;
    }

    IEnumerator SentFlying(Vector2 KBV, int Duration, float Speed)
    {
        PA.PlayAnimation("Sent Flying Idle");
        SetWalkDirectionToVector(-(Vector3)KBV);
        PDS = PlayerDoingState.SentFlying;
        ActionSpeed = 0;
        
        ActionMoveDir = KBV;
        ActionMoveSpeed = Speed;
        
        for(int i = 0; i < Duration; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        PA.PlayAnimation("Sent Flying End");
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;
    }

    IEnumerator SentSoaring(MoveOrder MO)
    {
        PA.PlayAnimation("Sent Flying Idle");
        SetWalkDirectionToVector(-(Vector3)MO.Direction);
        PDS = PlayerDoingState.Soaring;
        ActionSpeed = 0;

        ActionMoveDir = new Vector3(MO.Direction.x, MO.Direction.y, 0);
        ActionMoveSpeed = MO.Speed;

        for (int i = 0; i < MO.Duration; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        PA.PlayAnimation("Sent Flying End");
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;
    }

    IEnumerator SentSoaringComplex(MoveOrder[] MO) // can change direction in the flight
    {
        PA.PlayAnimation("Sent Flying Idle");
        SetWalkDirectionToVector(-(Vector3)MO[0].Direction);
        PDS = PlayerDoingState.Soaring;
        ActionSpeed = 0;
        
        for(int x = 0; x < MO.Length; x++)
        {
            ActionMoveDir = new Vector3(MO[x].Direction.x, MO[x].Direction.y, 0);
            ActionMoveSpeed = MO[x].Speed;
            for (int i = 0; i < MO[x].Duration; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        

        PA.PlayAnimation("Sent Flying End");
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;
    }

    IEnumerator SpotJump()
    {
        PA.PlayAnimation("Jumping");
        TSFX.PlayDash();
        PDS = PlayerDoingState.Jumping;

        JumpCooldown = JUMPSCD;
        DashCooldown = JUMP_DASHCD;
        ActionSpeed = PLAYER_SPEED;

        JumpInputDown = false;
        JumpInputHold = true;
        JumpInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = 0;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        bool Cancelled = false;
        for (int i = JUMPFRAMES_S; i > 0; i--)
        {
            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < JUMPTHRESH_S && !Cancelled)
            {
                JumpInvul = false;
                ActionMoveSpeed = 0;

                

                if (JumpInputHold)
                {
                    JumpInvul = false;
                    StartAction(Roll());
                    Cancelled = true;
                }
            }

            yield return new WaitForFixedUpdate();
        }


        if (!Cancelled)
        {
            JumpInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;

            PA.PlayAnimation("Idle");
            ActionMoveSpeed = 0;
        }
        // END
    }

    IEnumerator Jump()
    {
        SR.sortingOrder = 9999;
        PA.PlayAnimation("Jumping");
        TSFX.PlayDash();
        PDS = PlayerDoingState.Jumping;

        JumpCooldown = JUMPCD;
        DashCooldown = JUMP_DASHCD;
        ActionSpeed = PLAYER_SPEED;

        JumpInputDown = false;
        JumpInputHold = true;
        JumpInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = JUMPMUL;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        bool Cancelled = false;
        for (int i = JUMPFRAMES; i > 0; i--)
        {
            
            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < JUMPTHRESH && !Cancelled)
            {
                ActionMoveSpeed = 0;
                JumpInvul = false;

                

                if (JumpInputHold)
                {
                    JumpInvul = false;
                    StartAction(Roll());
                    Cancelled = true;
                }
            }
            
            yield return new WaitForFixedUpdate();
        }
        SR.sortingOrder = 5;

        if (!Cancelled)
        {
            JumpInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;

            PA.PlayAnimation("Idle");
            ActionMoveSpeed = 0;
        }
        // END
    }

    IEnumerator EnrageJump()
    {
        SR.sortingOrder = 9999;
        PA.PlayAnimation("EnrageJump");
        TSFX.PlayDash();
        PDS = PlayerDoingState.EnrageJump;

        JumpCooldown = JUMPCD_E;
        DashCooldown = JUMP_DASHCD_E;
        ActionSpeed = PLAYER_SPEED;

        JumpInputDown = false;
        JumpInputHold = true;
        JumpInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = JUMPMUL_E;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        bool Cancelled = false;
        for (int i = JUMPFRAMES_E; i > 0; i--)
        {

            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < JUMPTHRESH_E && !Cancelled)
            {
                ActionMoveSpeed = 0;
                JumpInvul = false;

                

                
            }

            yield return new WaitForFixedUpdate();
        }
        SR.sortingOrder = 5;

        if (!Cancelled)
        {
            JumpInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;

            PA.PlayAnimation("EnrageIdle");
            ActionMoveSpeed = 0;
        }
        // END
    }

    IEnumerator EnrageSpotJump()
    {
        CC.ResetFocusObject();
        CC.DampenSet(false);

        PA.PlayAnimation("EnrageJump");
        TSFX.PlayDash();
        PDS = PlayerDoingState.Jumping;

        JumpCooldown = JUMPCD_ES;
        DashCooldown = JUMP_DASHCD_ES;
        ActionSpeed = PLAYER_SPEED;

        JumpInputDown = false;
        JumpInputHold = true;
        JumpInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = 0;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        bool Cancelled = false;
        for (int i = JUMPFRAMES_ES; i > 0; i--)
        {
            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < JUMPTHRESH_ES && !Cancelled)
            {
                JumpInvul = false;
                ActionMoveSpeed = 0;

                

                
            }

            yield return new WaitForFixedUpdate();
        }


        if (!Cancelled)
        {
            JumpInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;

            PA.PlayAnimation("EnrageIdle");
            ActionMoveSpeed = 0;
        }
        // END
    }

    IEnumerator HookshotJump()
    {
        
        PA.PlayAnimation("HookJumping");
        TSFX.PlayDash();
        PDS = PlayerDoingState.Jumping;

        DashCooldown = JUMPCD;
        JumpCooldown = JUMPCD;
        ActionSpeed = 0;

        GunBatInputDown = false;
        JumpInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = JUMPMUL_H;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        
        for (int i = JUMPFRAMES_H; i > 0; i--)
        {

            if (i < 25)
            {
                ActionMoveSpeed = JUMPMUL_H / 1.5f;
            }

            if (i < 20)
            {
                ActionMoveSpeed = JUMPMUL_H / 2;
            }

            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < JUMPTHRESH_H)
            {
                ActionMoveSpeed = 0;

                while (GunBatInput)
                {
                    
                    yield return new WaitForFixedUpdate();
                }

                
                JumpInvul = false;

                
            }

            yield return new WaitForFixedUpdate();
        }

        

        JumpInvul = false;
        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;

        PA.PlayAnimation("Idle");
        ActionMoveSpeed = 0;
    }

    IEnumerator Dash()
    {
        PA.PlayAnimation("Dashing");
        TSFX.PlayDash();
        PDS = PlayerDoingState.Dashing;

        DashCooldown = DASHCD_1;
        JumpCooldown = DASH_JUMPCD;
        ActionSpeed = PLAYER_SPEED;

        DashInputDown = false;
        DashInputHold = true;
        DashInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = DASHMUL_1;

        SetWalkDirectionToVector(ActionMoveDir);
        FirstDashDir = new Vector2(VelocityX, VelocityY);

        bool Cancelled = false;
        for (int i = DASHFRAMES_1; i > 0; i--)
        {
            //If last couple frames of dash and still holding button then dash in new direction faster for a couple of frames
            if (i < DASHTHRESH_2 && DashInputHold && !Cancelled)
            {

                DashInvul = false;
                StartAction(Roll());
                Cancelled = true;

            }
            else if (i < DASHTHRESH_2 && AttackInputHold && !Cancelled)
            {
                DashInvul = false;
                StartAction(ThrustAttack());
                Cancelled = true;
            }
            yield return new WaitForFixedUpdate();
        }


        if (!Cancelled)
        {
            DashInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;
            
            PA.PlayAnimation("Idle");
            ActionMoveSpeed = 0;
        }
        // END
    }
    IEnumerator Roll()
    {
        TSFX.PlayRoll();

        PDS = PlayerDoingState.Rolling;


        RollInvul = true;

        ActionSpeed = PLAYER_SPEED;

        PA.PlayAnimation("Rolling");

        DashDuration2 = DASHFRAMES_2;
        DashCooldown += DASHCD_2;
        JumpCooldown += DASH_JUMPCD;

        Vector2 temp = ReturnVelocity();
        if ((temp.x == 0 && temp.y == 0))
        {
            ActionMoveDir = new Vector3(FirstDashDir.x , FirstDashDir.y );
        }
        else
        {
            ActionMoveDir = new Vector3(temp.x, temp.y);
        }
        ActionMoveSpeed = DASHMUL_2;
        SetWalkDirectionToVector(ActionMoveDir); // Sets direction to direction we moving

        bool Cancelled = false;
        for (int i = DASHFRAMES_2; i > 0; i--)
        {
            if (i > DASHFRAMES_2 - 9)
            {
                if (AttackInput)
                {
                    AttackInputHold = true;
                }

            }

            if (i < ROLLTHRESH && AttackInputHold && !Cancelled)
            {
                Cancelled = true;
                RollInvul = false;

                AttackInputDown = false;
                StartAction(SlamAttack());
            }
            yield return new WaitForFixedUpdate();
        }

        if (!Cancelled)
        {
            RollInvul = false;
            PDS = PlayerDoingState.Normal;
            ActionMoveDir = Vector2.zero;
            ActionSpeed = PLAYER_SPEED;
            
            PA.PlayAnimation("Idle");
            ActionMoveSpeed = 0;
        }


    }

    IEnumerator StaplerDash()
    {
        TSFX.PlayDash();

        DashInputDown = false;
        DashCooldown = STAPLING_DASHCD;
        

        DashInputHold = true;
        ActionSpeed = PLAYER_SPEED;

        Vector3 MoveDir;
        if(VelocityX == 0 && VelocityY == 0)
        {
            MoveDir = ReturnVelocity(WalkDir);

            MoveDir *= ActionSpeed;

            VelocityX = MoveDir.x;
            VelocityY = MoveDir.y;
        }
        


        PDS = PlayerDoingState.StapleDashing;

        DashInvul = true;
        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = DASHMUL_1;
        SetWalkDirectionToVector(ActionMoveDir);
        IsStapling = true;


        for (int i = STAPLING_DASHFRAMES; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        StapleBlast1.enabled = false;
        StapleBlast2.enabled = false;
        StapleBlast3.enabled = false;
        StapleBlast4.enabled = false;

        IsStapling = false;
        DashInvul = false;
        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;
        
        PA.PlayAnimation("Idle");
        ActionMoveSpeed = 0;
    }

    IEnumerator SpotDodge()
    {
        PA.PlayAnimation("EnrageSpotDodge");
        TSFX.PlayDash();

        

        
        RDashCooldown = DASHCD_RS;
        
        ActionSpeed = PLAYER_SPEED;

        DashInputDown = false;

        PDS = PlayerDoingState.EnrageSpotdodge;
        DashInvul = true;
        

        SetWalkDirectionToMovement();

        for (int i = DASHFRAMES_RS; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }


        DashInvul = false;
        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;
        
        PA.PlayAnimation("EnrageIdle");
        ActionMoveSpeed = 0;
    }
    IEnumerator Cartwheel()
    {
        PA.PlayAnimation("EnrageDodge");
        TSFX.PlayDash();
        PDS = PlayerDoingState.EnrageCartwheel;
        RAttackAlternate = 1;
        RAttackCooldown = 0;


       

        ActionSpeed = PLAYER_SPEED;

        DashInputDown = false;
        
        DashInvul = true;

        SetWalkDirectionToMovement(); // Sets aim direction based on LEFT stick

        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);

        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        ActionMoveSpeed = DASHMUL_R;

        SetWalkDirectionToVector(ActionMoveDir);

        

        for (int i = DASHFRAMES_R; i > 0 || DashInput ; i--)
        {
            

            yield return new WaitForFixedUpdate();
            RDashCooldown++;
        }

        RDashCooldown += 2;
        RollInvul = false;
        DashInvul = false;


        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;

        PA.PlayAnimation("EnrageIdle");
        ActionMoveSpeed = 0;



    }

    IEnumerator Teleport()
    {
        
        TSFX.PlayDash();
        PDS = PlayerDoingState.EnrageCartwheel;

        ActionSpeed = 0;

        DashInputDown = false;
        
        DashInvul = true;


        SetAimDirectionToMovement();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        PA.PlayAnimation("EnrageDodge");

        Vector3 TpDir = VisualAim.transform.up;

        CC.SetStatic(true);

        for (int i = TPDELAY; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        transform.position = transform.position + (TpDir * 3);
        TpComboCount++;
        TpComboTime = 25;
        DashInvul = false;
        
        for (int i = TPFRAMES; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }
        CC.SetStatic(false);
        if (TpComboCount == 3)
        {
            for (int i = TPCD; i > 0; i--)
            {
                yield return new WaitForFixedUpdate();
            }
            TpComboCount = 0;
        }

        
        

        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;

        PA.PlayAnimation("EnrageIdle");
        ActionMoveSpeed = 0;

        
        

    }

    IEnumerator KatanaRoll()
    {
        TSFX.PlayDash();
        KatanaDashDuration = KATANA_DASH_FRAMES;
        DashInputDown = false;
        DashCooldown = KATANA_DASH_CD;
        ActionSpeed = KATANA_SPEED;



        Vector3 MoveDir;
        if (VelocityX == 0 && VelocityY == 0) // if standing still then calculate velocity based on previous walkdir
        {
            MoveDir = ReturnVelocity(WalkDir);

            MoveDir *= ActionSpeed;

            VelocityX = MoveDir.x;
            VelocityY = MoveDir.y;
        }


        PDS = PlayerDoingState.KatanaRoll;
        // This section starts the first dash 

        ActionMoveDir = new Vector3(VelocityX * KATANA_DASHMUL1, VelocityY * KATANA_DASHMUL1);


        SetWalkDirectionToVector(DashVelocity);
        PA.PlayAnimation("KatanaRoll");

        for (int i = DASHFRAMES_R; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }


        RollInvul = false;
        DashInvul = false;
        PDS = PlayerDoingState.Normal;
        ActionMoveDir = Vector2.zero;
        ActionSpeed = PLAYER_SPEED;
        
        PA.PlayAnimation("KatanaIdle");

    }

    IEnumerator StapleShoot()
    {
        PDS = PlayerDoingState.StapleShooting;
        ActionSpeed = STAPLING_SLOW;
        StaplingAlternateCount = STAPLE_DPS;

        IsStapling = true;
        while (AAInput)
        {
            yield return new WaitForFixedUpdate();
        }
        IsStapling = false;
        StapleBlast1.enabled = false;
        StapleBlast2.enabled = false;
        StapleBlast3.enabled = false;
        StapleBlast4.enabled = false;
        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        PA.SetStapNotShooting(false);
        PA.PlayAnimation("Idle");
    }

    IEnumerator Attack()
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.NormalAttack;
        TSFX.PlayAttack();

        ActionSpeed = ATTACKSLOW;
        AttackCooldown = ATTACKCD_ATTACK;
        DashCooldown = DASHCD_A;
        JumpCooldown = DASHCD_A;

        if (AttackComboCount == 0)
        {
            PA.PlayAnimation("Attacking");
            AttackComboCount++;
            AttackComboTime = 50;
        }
        else
        {
            PA.PlayAnimation("FlipAttacking");
           
            AttackComboCount = 0;
            
        }
        

        for (int x = 0; x < ATTACKDELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        if(ConcentrationCounter >= MAX_CONC)
        {
            FocusImage AtkImage;
            AtkImage = Instantiate(AttackFImage, AttackFocusPos.transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>();

            MoveOrder[] MO = new MoveOrder[1]
            {
                new MoveOrder(VisualAim.transform.up, 0.65f, 5)
            };

            AtkImage.InitializeImage(0, 5, 0, new Damage(3f, gameObject), MO);
        }

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(AttackHurtBox.transform.position, AttackHurtBox.transform.localScale, AttackHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(ATTACK_DMG));
            GameObject HitObject = Instantiate(HitEffect, AttackCast[i].point, Quaternion.identity);
            HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            ConcentrationCounter += 2;
        }

        // if holding or not pressing then do not cancel wind down
        for (int x = 0; x <= ATTACKFRAMES && (!AttackInputDown || AttackInputHold); x++)
        {
            yield return new WaitForFixedUpdate();
        }


        if (AttackInputHold)
        {
            StartAction(EnterBattleStance());
        }
        else if (AttackInputDown)
        {
            AttackInputDown = false;
            AttackInputHold = true;

            StartAction(SpinAttackSwing());
        }
        else
        {
            PDS = PlayerDoingState.Normal;
            ActionSpeed = PLAYER_SPEED;
            SetWalkDirToAimDir();
            for (int x = 0; x <= ATTACKWIND; x++)
            {
                yield return new WaitForFixedUpdate();
            }
            PA.PlayAnimation("Idle");
        }

    }

    IEnumerator WhipAttack1()
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.WhipCharging1;
        

        
        AttackCooldown = ATTACKCD_WHIP_1;
        DashCooldown = DASHCD_WHIP_1;
        JumpCooldown = DASHCD_WHIP_1;

        //SetWalkDirectionToMovement();
        Vector3 MoveDir;
        MoveDir = ReturnVelocity(WalkDir);
        MoveDir *= ActionSpeed;

        VelocityX = MoveDir.x;
        VelocityY = MoveDir.y;

        ActionMoveDir = new Vector3(VelocityX, VelocityY);
        
        
        ActionMoveSpeed = 3f;
        ActionSpeed = 0;


        int ChargeTime = 0;
        PA.PlayAnimation("WhipCharge");

        float CurrentDamage = MIN_WHIPDMG_1;

        for (int x = 0; x < WHIPDELAY_1; x++)
        {
            ChargeTime++;

            if (ChargeTime > 35 && ChargeTime < 40)
            {
                SR.color = Color.red;
            }
            else if (ChargeTime >= 40)
            {
                SR.color = Color.blue;
            }
            else
            {
                SR.color = Color.white;
            }

            CurrentDamage += (MAX_WHIPDMG_1 - MIN_WHIPDMG_1) / 35;

            yield return new WaitForFixedUpdate();
        }

        if (KeyBoardMouse)
        {
            AimDir = GetLockOnDir(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        

        SetAimDirectionToLook(); // Sets aim dir based on the direction of the RIGHT stick

        LockOnToEnemy();

        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);
        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;
        ActionSpeed = WHIPATTACKSLOW_1;

        if (CurrentDamage > MAX_WHIPDMG_1)
            CurrentDamage = MAX_WHIPDMG_1;

        if (ChargeTime > 40)
            CurrentDamage -= 15;

        SR.color = Color.white;
        PDS = PlayerDoingState.WhipAttacking1;

        ActionSpeed = WHIPATTACKSLOW_1;
        TSFX.PlayAttack();

        PA.PlayAnimation("Whip1");

        if (ConcentrationCounter >= MAX_CONC)
        {
            //Instantiate(Whip1FImage, Whip1FocusPos.transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().InitializeImage();
            //Instantiate(Whip1FImage, Whip1FocusPos.transform.position + Whip1FocusPos.transform.right * 2, VisualAim.transform.rotation).GetComponent<FocusImage>().InitializeImage();
            //Instantiate(Whip1FImage, Whip1FocusPos.transform.position - Whip1FocusPos.transform.right * 2, VisualAim.transform.rotation).GetComponent<FocusImage>().InitializeImage();
        }

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(WhipHurtBox.transform.position, WhipHurtBox.transform.localScale, WhipHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        RaycastHit2D IntCast = Physics2D.BoxCast(WhipHurtBox.transform.position, WhipHurtBox.transform.localScale, WhipHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, InteractMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(MIN_WHIPDMG_1, gameObject));

            GameObject HitObject = Instantiate(HitEffect, AttackCast[i].point, Quaternion.identity);
            HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            ConcentrationCounter++;
        }
        if (IntCast)
        {
            HookShotGrab(IntCast.point);
        }
        else
        {
            WhipComboTime = WHIPFRAMES_1 + 35;
            WhipComboCount = 1;
        }
        
        
        
        for (int x = 0; x <= WHIPFRAMES_1 ; x++) // && (!AttackInputDown || AttackInputHold)
        {
            yield return new WaitForFixedUpdate();
        }

        if (GunBatInputHold)
        {
            StartAction(EnterBattleStance());
            yield break;
        }

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        SetWalkDirToAimDir();
        
        PA.PlayAnimation("Idle");

    }

    IEnumerator WhipAttack2()
    {

        PDS = PlayerDoingState.WhipCharging1;
        


        AttackCooldown = ATTACKCD_WHIP_2;
        DashCooldown = DASHCD_WHIP_2;
        JumpCooldown = DASHCD_WHIP_2;
        ActionSpeed = WHIPCHARGESLOW_2;

        PA.PlayAnimation("WhipCharge");


        float CurrentDamage = MIN_WHIPDMG_2;
        int ChargeTime = 0;

        for (int x = 0; x < WHIPDELAY_2; x++) // || GunBatInputHold
        {
            if (GunBatInputHold)
                ChargeTime++;

            if (ChargeTime > 15 && ChargeTime < 20)
            {
                SR.color = Color.red;
            }
            else if (ChargeTime > 20)
            {
                SR.color = Color.blue;
            }
            else
            {
                SR.color = Color.white;
            }

            CurrentDamage += (MAX_WHIPDMG_2 - MIN_WHIPDMG_2) / 15;

            yield return new WaitForFixedUpdate();
        }

        if (CurrentDamage > MAX_WHIPDMG_2)
            CurrentDamage = MAX_WHIPDMG_2;

        if (ChargeTime > 20)
            CurrentDamage -= 15;

        TSFX.PlaySpin();
        PA.PlayAnimation("Whip2");

        SR.color = Color.white;
        PDS = PlayerDoingState.WhipAttacking1;

        ActionSpeed = WHIPATTACKSLOW_2;

        bool HitSomething = false;
        
        RaycastHit2D[][] AttackCasts = new RaycastHit2D[WhipHurtBoxes2.Length][];

        for (int i = 0; i < WhipHurtBoxes2.Length; i++)
        {
            GameObject CurrentHB = WhipHurtBoxes2[i];
            AttackCasts[i] = Physics2D.BoxCastAll(CurrentHB.transform.position, CurrentHB.transform.localScale, CurrentHB.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);

            if (ConcentrationCounter >= MAX_CONC)
            {
                if(i % 2 == 0) // evens diagonal
                    Instantiate(Whip2FImageDiag, CurrentHB.transform.position, CurrentHB.transform.rotation).GetComponent<FocusImage>().InitializeImage(0.4f + (0.025f * i));
                if (i % 2 != 0) // odds straight
                    Instantiate(Whip2FImageStrt, CurrentHB.transform.position, CurrentHB.transform.rotation).GetComponent<FocusImage>().InitializeImage(0.4f + (0.025f * i));
            }
        }


        for (int i = 0; i < AttackCasts.Length; i++)
        {
            for(int x = 0; x < AttackCasts[i].Length; x++)
            {
                AttackCasts[i][x].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(CurrentDamage, gameObject));


                HitSomething = true;
            }
            
        }
        if (HitSomething)
        {
            HitCounter += 1;
            ConcentrationCounter++;

        }

        WhipComboTime = WHIPFRAMES_2 + 35;
        //WhipComboCount = 2;

        for (int x = 0; x <= WHIPFRAMES_2; x++)
        {
            yield return new WaitForFixedUpdate();
        }




        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        PA.PlayAnimation("Idle");

    }

    IEnumerator WhipAttack3()
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.WhipCharging1;
        

        ActionSpeed = WHIPCHARGESLOW_3;
        AttackCooldown = ATTACKCD_WHIP_3;
        DashCooldown = DASHCD_WHIP_3;
        JumpCooldown = DASHCD_WHIP_3;

        int ChargeTime = 0;
        PA.PlayAnimation("WhipCharge");

        float CurrentDamage = MIN_WHIPDMG_3;

        for (int x = 0; x < WHIPDELAY_3; x++)
        {
            ChargeTime++;

            if (ChargeTime > 50 && ChargeTime < 60)
            {
                SR.color = Color.red;
            }
            else if (ChargeTime >= 60)
            {
                SR.color = Color.blue;
            }
            else
            {
                SR.color = Color.white;
            }

            CurrentDamage += (MAX_WHIPDMG_3 - MIN_WHIPDMG_3) / 50;

            yield return new WaitForFixedUpdate();
        }

        if (CurrentDamage > MAX_WHIPDMG_3)
            CurrentDamage = MAX_WHIPDMG_3;

        if (ChargeTime > 60)
            CurrentDamage -= 15;


        PA.PlayAnimation("Whip3");
        TSFX.PlaySlam();

        SR.color = Color.white;
        PDS = PlayerDoingState.WhipAttacking1;

        ActionSpeed = WHIPATTACKSLOW_3;

        if (ConcentrationCounter >= MAX_CONC)
        {
            Instantiate(Whip3FImage, Whip3FocusPos.transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().InitializeImage();
        }

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(WhipHurtBox3.transform.position, WhipHurtBox3.transform.localScale, WhipHurtBox3.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(MIN_WHIPDMG_3, gameObject));

            GameObject HitObject = Instantiate(HitEffect, AttackCast[i].point, Quaternion.identity);
            HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            ConcentrationCounter++;
        }

        
        WhipComboCount = 0;

        // if holding or not pressing then do not cancel wind down
        for (int x = 0; x <= WHIPFRAMES_3; x++) // && (!AttackInputDown || AttackInputHold)
        {
            yield return new WaitForFixedUpdate();
        }

       

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        SetWalkDirToAimDir();
        
        PA.PlayAnimation("Idle");

    }

    IEnumerator EnrageParry()
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.Parrying;
        TSFX.PlayAttack();

        ActionSpeed = 0;
        AttackCooldown = 20;
        DashCooldown = 25;
        JumpCooldown = 25;

        PA.PlayAnimation("KatanaParry");

        IsParrying = true;

        ParryDamage = 25;
        ParryStun = 15;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        IsParrying = false;

        for (int x = 0; x <= 10; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        SetWalkDirToAimDir();

        PA.PlayAnimation("EnrageIdle");
    }

    IEnumerator Parry(int Duration)
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.Parrying;
        TSFX.PlayAttack();

        ActionSpeed = 0;
        AttackCooldown = 20;
        DashCooldown = 25;
        JumpCooldown = 25;

        PA.PlayAnimation("KatanaParry");

        IsParrying = true;

        if (Duration > 90)
        {
            ParryDamage = 150;
            ParryStun = 50;
        }

        else if (Duration > 40)
        {
            ParryDamage = 100;
            ParryStun = 35;
        }
        else
        {
            ParryDamage = 0;
            ParryStun = 25;
        }

        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        IsParrying = false;

        for (int x = 0; x <= 10; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        SetWalkDirToAimDir();
        
        PA.PlayAnimation("Idle");
    }

    IEnumerator ParrySuccess()
    {
        PA.SetMoveDir(AimDir);
        PDS = PlayerDoingState.Parrying;
        TSFX.PlayAttack();
        PA.PlayAnimation("ParryHit"); // change to parry success

        ConcentrationCounter = MAX_CONC;

        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        PDS = PlayerDoingState.Normal;
        ActionSpeed = PLAYER_SPEED;
        SetWalkDirToAimDir();

        PA.PlayAnimation("Idle"); 
    }

    //IEnumerator GunbatAttack() // same as normal attack swing with different particle, and has more dmg + knockback, maybe more range?
    //{
    //    PDS = PlayerDoingState.AttackCancelable;
    //    TSFX.PlayAttack();

    //    PA.SetGunbatSwing(true);
    //    CC.Shake(0.3f, 5);

    //    // normal gunbat things

    //    PA.SetGunCharge(false);
    //    GunbatRangeCircle.transform.localScale = new Vector3(3, 3);
    //    GunbatRangeSR.enabled = false;
    //    GunBatCircleLock = false;
    //    LaserAim2.SetActive(false);


    //    GlobalVelMul = ATTACKSLOW;
    //    AttackCooldown = ATTACKCD_ATTACK;
    //    DashCooldown = DASHCD_A;


    //    PA.SetMoveDir(AimDir);


    //    for (int x = 0; x < ATTACKDELAY; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    bool HitSomething = false;

    //    AttackCast = Physics2D.BoxCastAll(AttackHurtBox.transform.position, AttackHurtBox.transform.localScale, AttackHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //    for (int i = 0; i < AttackCast.Length; i++)
    //    {
    //        TSFX.PlayShotgun();

    //        Damage GUNBAT_ATTACK_DMG = new Damage(75, gameObject, (AttackCast[i].transform.position - transform.position).normalized, 1f, 3);
    //        AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(GUNBAT_ATTACK_DMG));



    //        GameObject HitObject = Instantiate(HitEffect, AttackCast[i].point, Quaternion.identity);
    //        HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
    //        HitSomething = true;
    //    }
    //    if (HitSomething)
    //    {
    //        HitCounter++;

    //        GunBatMoveCD = HitCounter + GUNBAT_CD; // only put gunbat on cd if u actually hit something with this
    //    }

    //    // if holding or not pressing then do not cancel wind down
    //    for (int x = 0; x <= ATTACKFRAMES && (!AttackInputDown || AttackInputHold) && PDS == PlayerDoingState.AttackCancelable; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    PA.SetGunbatSwing(false);

    //    if (AttackInputHold && PDS == PlayerDoingState.AttackCancelable)
    //    {

    //        PDS = PlayerDoingState.BattleStance;
    //        GlobalVelMul = BATTLESTANCESLOW;
    //    }
    //    else if (AttackInputDown)
    //    {
    //        AttackInputDown = false;
    //        AttackInputHold = true;
    //        PA.SetSpinning(true);
    //        StartCoroutine(SpinAttackSwing());
    //    }
    //    else if (PDS == PlayerDoingState.AttackCancelable)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = PLAYER_SPEED;
    //        SetWalkDirToAimDir();
    //    }
    //}

    IEnumerator SlamAttack()
    {
        PDS = PlayerDoingState.Slamming;

        ActionSpeed = SLAMSLOW; // How much slower will the player be while attacking
        AttackCooldown = ATTACKCD_SLAM;
        DashCooldown = DASHCD_SLAM;
        JumpCooldown = DASHCD_SLAM;

        PA.SetMoveDir(AimDir);
        PA.PlayAnimation("Slamming");

        Damage dmg;
        GameObject HB;

        dmg = SLAM_DMG;
        HB = SlamHurtBox;

        for (int x = 0; x < SLAMDELAY && !AttackInputDown; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        

        if(ConcentrationCounter >= MAX_CONC)
        {
            Instantiate(SlamFImage, SlamFocusPos.transform.position, Quaternion.identity).GetComponent<FocusImage>().InitializeImage();
        }

        TSFX.PlaySlam();

        CC.Shake(0.25f, SLAMFRAMES);

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(HB.transform.position, HB.transform.localScale, HB.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(dmg));

            GameObject HitObject = Instantiate(HitEffectSlam, AttackCast[i].transform.position, Quaternion.identity);
            HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);

            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter += 5;
        }

        for (int x = 0; x <= SLAMFRAMES && !Interrupted; x++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        SetWalkDirToAimDir();

        PA.PlayAnimation("Idle");
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
    }

    IEnumerator ThrustAttack()
    {
        if (AttackInput)
        {
            AttackInputHold = true;
        }

        PDS = PlayerDoingState.Thrusting;
        TSFX.PlayThrust();
        PA.PlayAnimation("Thrusting");
        

        AttackCooldown = ATTACKCD_THRUST;
        DashCooldown = DASHCD_THRUST;
        JumpCooldown = DASHCD_THRUST;

        ActionSpeed = 0;

        SetAimDirectionToMovement();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        ActionMoveDir = VisualAim.transform.up;
        ActionMoveSpeed = THRUSTPUSH;


        for (int x = 0; x < THRUSTDELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        int CutFrames = 0;

        if (ConcentrationCounter >= MAX_CONC)
        {
            MoveOrder[] MO = new MoveOrder[3]
            {
                new MoveOrder(ActionMoveDir, THRUSTPUSH, THRUST_THRESH1),
                new MoveOrder(ActionMoveDir, THRUSTPUSH2, THRUST_THRESH2 - THRUST_THRESH1),
                new MoveOrder(ActionMoveDir, THRUSTPUSH3, THRUSTFRAMES - THRUST_THRESH2)
            };
            Instantiate(ThrustFImage, transform.position, VisualAim.transform.rotation).
                GetComponent<FocusImage>().InitializeImage(0.25f, THRUSTFRAMES, 0, new Damage(THRUST_DMG.DmgAmount / 3, gameObject, ActionMoveDir, 0.15f, 4), MO);
        }

        for (int x = 0; x <= THRUSTFRAMES; x++)
        {
            if (x % 3 == 0)
            {
                AttackCast = Physics2D.BoxCastAll(ThrustHurtBox.transform.position, ThrustHurtBox.transform.localScale, ThrustHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
                for (int i = 0; i < AttackCast.Length; i++)
                {
                    AttackCast[i].transform.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(THRUST_DMG));

                    GameObject HitObject = Instantiate(HitEffectThrust, AttackCast[i].point, Quaternion.identity);
                    HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);

                    if (x < THRUST_THRESH1) // if you hit something then shorten the thrust and jump to the mid speed
                    {
                        CutFrames = THRUST_THRESH1 - x;
                        x = THRUST_THRESH1;
                    }
                }
            }

            if (x < THRUST_THRESH1)
            {
                ActionMoveSpeed = THRUSTPUSH;
            }
            else if (x < THRUST_THRESH2)
            {
                ActionMoveSpeed = THRUSTPUSH2;
            }
            else
            {
                ActionMoveSpeed = THRUSTPUSH3;
            }

            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        AttackCooldown = ATTACKCD_THRUST - (THRUSTDELAY + THRUSTFRAMES); // makes attack cooldown equal to the amount of frames that have supposedly past between the first attack cd adjustment and now
        // this means that if less frames pass then planed then the attack cd is lower than it would be which means that ending the thrust early (by thrusting into an enemy) will make the attack cd shorter

        

        for (int x = 0; x < THRUSTCD; x++)
        {
            yield return new WaitForFixedUpdate();
        }


        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;
        ActionSpeed = PLAYER_SPEED;

        SetWalkDirToAimDir();

        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("Idle");

    }

    IEnumerator SpinAttackSwing()
    {

        PDS = PlayerDoingState.SpinAttack;
        TSFX.PlaySpin();


        AttackCooldown = ATTACKCD_SPIN;
        DashCooldown = DASHCD_SPIN;
        JumpCooldown = DASHCD_SPIN;
        ActionSpeed = SPINSLOW;



        PA.PlayAnimation("Spinning");

        for (int x = 0; x < SPINDELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        if (ConcentrationCounter >= MAX_CONC)
        {
            if(ConcSpinChild != null) // you can only have one out at a time
            {
                Destroy(ConcSpinChild);
            }
            ConcSpinChild = Instantiate(SpinFImage, transform.position, Quaternion.identity);
            ConcSpinChild.GetComponent<FocusImage>().InitializeImage();
        }

        bool HitSomething = false;
        AttackCast = Physics2D.CircleCastAll(transform.position, 2, transform.up, 0, EnemyMask); // A circle with a radius of 2
        for (int i = 0; i < AttackCast.Length; i++)
        {
            if (ConcentrationCounter > MAX_CONC / 2)
            {
                AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(CON_SPIN_DMG));
            }
            else
            {
                AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(SPIN_DMG));
            }
            
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter += 1;
            ConcentrationCounter += 4;

        }
        for (int x = 0; x <= SPINFRAMES1 && !Interrupted; x++)
        {
            yield return new WaitForFixedUpdate();
        }




        if (AttackInputHold)
        {
            StartAction(EnterBattleStance());
        }

        else 
        {
            PDS = PlayerDoingState.Normal;
            ActionSpeed = PLAYER_SPEED;
            PA.PlayAnimation("Idle");
        }

    }

    IEnumerator EnterParryStance()
    {
        ActionSpeed = 0;
        PDS = PlayerDoingState.ParryStance;
        PA.PlayAnimation("KatanaIdle");
        int HoldDuration = 0;
        bool Hold = true;

        while (Hold)
        {
            if(HoldDuration == 11)
            {
                TSFX.PlayLockOn();
            }
            if(HoldDuration == 41)
            {
                TSFX.PlayLockOn();
            }
            if(HoldDuration == 91)
            {
                TSFX.PlayLockOn();
            }

            if (!RunDrawInput)
            {
                if(HoldDuration > 10)
                {
                    StartAction(Parry(HoldDuration));

                    yield break;
                }
                else
                {
                    Hold = false;
                }
            }

            

            HoldDuration++;
            yield return new WaitForFixedUpdate();
        }

        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("Idle");
    }

    IEnumerator Burst()
    {
        BurstCount--;
        ActionSpeed = 0;
        PA.PlayAnimation("EnrageStart");
        NoOverride = true;
        Invulnerable = true;
        PDS = PlayerDoingState.Transforming;
        TSFX.PlayEnrage();
        BurstHitCount = 0;
        BurstTimer = 0;
        
        if (BurstHealthThreshold > 0) // if the burst health threshold is set
        {
            
            
            Health = BurstHealthThreshold;
            BurstHealthThreshold = 0;
        }
        

        RaycastHit2D[] EnrageCircleCastLarge = Physics2D.CircleCastAll(transform.position, 15, transform.up, 0, EnemyMask);
        
        for (int i = 0; i < EnrageCircleCastLarge.Length; i++)
        {
            EnrageCircleCastLarge[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(0, 80, gameObject));
        }

        for(int i = 0; i < 8; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        TSFX.PlayBigHeal();
        NoOverride = false;
        Invulnerable = false;
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("Idle");
    }

    IEnumerator EnterBattleStance()
    {
        ActionSpeed = 0;
        PDS = PlayerDoingState.BattleStance;
        PA.PlayAnimation("BattleStanceEntry");
        bool Hold = true;
        BattleStanceCount = 0;

        bool AttackVariation = false;
        if (GunBatInputHold)
        {
            AttackVariation = true;
        }
        

        while (Hold)
        {

            // THRUST
            if (BattleStanceCount > BATTLESTANCE_ENTER && BattleStanceCount < BATTLESTANCE_MAX && (LLeft || LRight || LUp || LDown) && !LStickHold)
            {
                BattleStanceCount = 0;

                StartAction(ThrustAttack());
                yield break;
            }

            if ((!AttackInput && !AttackVariation) || (!GunBatInput && AttackVariation))
            {
                // SLAM
                if (BattleStanceCount > BATTLESTANCE_MAX)
                {
                    StartAction(SlamAttack());
                    BattleStanceCount = 0;
                    yield break;
                }
                // EXIT BATTLE STANCE
                else
                {
                    BattleStanceCount = 0;
                    Hold = false;
                }
            }

            //BattleStanceCount++;
            yield return new WaitForFixedUpdate();
        }


        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("Idle");
        
        BattleStanceCount = 0;

    }

    // CONTAINS MANY COMMENTED OUT COROUTINES
    //IEnumerator StapleSpinAttackSwing()
    //{

    //    PDS = PlayerDoingState.Attack;
    //    if (PBS == PlayerBeingState.Stapling)
    //    {
    //        PBS = PlayerBeingState.Normal;
    //    }
    //    TSFX.PlaySpin();

    //    SetAimDirectionToMovement();
    //    SetVAimToAimDir();
    //    PA.SetMoveDir(AimDir);

    //    AttackCooldown = ATTACKCD_STAPLER_SPIN;
    //    DashCooldown = DASHCD_SPIN;
    //    GlobalVelMul = SPINSLOW;



    //    PA.SetStapleSpinning(true);

    //    bool StapleBuff = false;

    //    if (StaplingAmmo > 15)
    //    {
    //        StaplingAmmo -= 15;
    //        if (StaplingAmmo < 0)
    //        {
    //            StaplingAmmo = 0;
    //        }
    //        SB.SetStapleCount(StaplingAmmo);
    //        StapleBuff = true;
    //        StapleSpinPushforward = true;
    //    }


    //    for (int x = 0; x < SPINDELAY; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = PLAYER_SPEED;
    //        }
    //        PA.SetStapleSpinning(false);
    //        yield break;
    //    }



    //    for (int x = 0; x < 5 && StapleBuff; x++)
    //    {
    //        TSFX.PlayStaple();
    //        AttackCast = Physics2D.CircleCastAll(transform.position, 2, transform.up, 0, EnemyMask); // A circle with a radius of 2
    //        for (int i = 0; i < AttackCast.Length; i++)
    //        {
    //            //AttackCast[i].transform.GetComponentInParent<IEnemy>().GotStapled(2);
    //        }


    //        yield return new WaitForFixedUpdate();
    //    }

    //    AttackCast = Physics2D.CircleCastAll(transform.position, 2, transform.up, 0, EnemyMask);

    //    for (int i = 0; i < AttackCast.Length; i++)
    //    {
    //        AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(SPIN_DMG));
    //    }


    //    StapleSpinPushforward = false;

    //    if (StapleBuff)
    //    {
    //        for (int x = 0; x <= SPINFRAMES2 && !Interrupted; x++)
    //        {
    //            yield return new WaitForFixedUpdate();
    //        }
    //    }
    //    else
    //    {
    //        for (int x = 0; x <= SPINFRAMES1 && !Interrupted; x++)
    //        {
    //            yield return new WaitForFixedUpdate();
    //        }
    //    }

    //    PA.SetStapleSpinning(false);

    //    if (AttackInputHold && PDS == PlayerDoingState.Attack)
    //    {
    //        PDS = PlayerDoingState.BattleStance;
    //        GlobalVelMul = BATTLESTANCESLOW;
    //    }

    //    else if (PDS == PlayerDoingState.Attack)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = PLAYER_SPEED;

    //    }

    //}

    //IEnumerator FrenzyAttackSwing()
    //{
    //    PDS = PlayerDoingState.Attack;
    //    TSFX.PlayAttack();

    //    GlobalVelMul = FRENZYSLOW; // How much slower will the player be while attacking
    //    AttackCooldown = ATTACKCD_FRENZY;
    //    DashCooldown = DASHCD_FRENZY;

    //    PA.SetMoveDir(AimDir);
    //    //PA.SetAttacking(true);

    //    for (int x = 0; x < FRENZYDELAY1; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    bool HitSomething = false;

    //    AttackCast = Physics2D.BoxCastAll(FrenzyHurtBox.transform.position, FrenzyHurtBox.transform.localScale, FrenzyHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //    for (int i = 0; i < AttackCast.Length; i++)
    //    {
    //        AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(FRENZY_DMG));



    //        GameObject HitObject = Instantiate(HitEffect, AttackCast[i].point, Quaternion.identity);
    //        HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
    //        HitSomething = true;
    //    }
    //    if (HitSomething)
    //    {
    //        HitCounter++;



    //    }

    //    for (int z = 0; z <= FRENZYFRAMES1 && !Interrupted; z++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    // if holding or not pressing then do not cancel wind down
    //    for (int x = 0; x <= FRENZYFRAMES2 && (AttackInputHold) && !Interrupted; x++)
    //    {
    //        PA.SetMoveDir(AimDir);
    //        yield return new WaitForFixedUpdate();
    //    }


    //    PA.SetAttacking(false); // should be like finish fury swipe to be ready to officially end it our transition into another one or battlestance

    //    if (AttackInputHold && PDS == PlayerDoingState.Attack)
    //    {
    //        PDS = PlayerDoingState.BattleStance;
    //        GlobalVelMul = BATTLESTANCESLOW;
    //    }

    //    else if (PDS == PlayerDoingState.Attack)
    //    {


    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = PLAYER_SPEED;
    //        SetWalkDirToAimDir();
    //    }
    //}

    IEnumerator DandyPunchAttackSwing(int Level)
    {
        PDS = PlayerDoingState.DandyPunching;
        TSFX.PlayAttack();
        PA.PlayAnimation("DandyPunch");


        
        

        AttackCooldown = DANDY_ATTACKCD;
        DashCooldown = DASHCD_DANDY;
        JumpCooldown = DASHCD_DANDY;

        ActionSpeed = 0;

        
        PA.SetMoveDir(AimDir);

        SetAimDirectionToMovement();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        ActionMoveSpeed = DANDY_PUSH;
        Vector2 PunchDir = VisualAim.transform.up;
        ActionMoveDir = PunchDir;

        for (int x = 0; x < DANDY_DELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }



        int Frames = 0;
        int CD = 0;

        if(Level == 0)
        {
            Frames = DANDY_FRAMES;
            CD = 40 - DANDY_FRAMES;
        }
        else if(Level == 1)
        {
            Frames = DANDY_FRAMES2;
            CD = 40 - DANDY_FRAMES2;
        }
        

        for (int x = Frames; x > 0; x--)
        {

            AttackCast = Physics2D.BoxCastAll(DandyHurtBox.transform.position, DandyHurtBox.transform.localScale, DandyHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
            for (int i = 0; i < AttackCast.Length; i++)
            {
                AttackCast[i].transform.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(DANDY_DMG, gameObject, PunchDir, ActionMoveSpeed, 1));
                if(Level == 1 && x > 1)
                {
                    x = 1;
                }
            }

            if (x > DANDY_THRESH1)
            {
                ActionMoveSpeed = DANDY_PUSH;
            }
            else
            {
                ActionMoveSpeed = DANDY_PUSH2;
            }

            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        if(Level == 1)
        {
            AttackCast = Physics2D.BoxCastAll(DandySlamHurtBox.transform.position, DandySlamHurtBox.transform.localScale, DandySlamHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
            for (int i = 0; i < AttackCast.Length; i++)
            {
                AttackCast[i].transform.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(DANDYSLAM_DMG, gameObject, PunchDir, 0.8f, 7));

            }
        }

        ActionMoveSpeed = 0;
        ActionMoveDir = Vector2.zero;
        

        for (int x = 0; x < CD; x++)
        {
            yield return new WaitForFixedUpdate();
        }


        ActionSpeed = PLAYER_SPEED;

        SetWalkDirToAimDir();

        PDS = PlayerDoingState.Normal;

        PA.PlayAnimation("EnrageIdle");
    }

    IEnumerator SlideKickAttackSwing()
    {
        PDS = PlayerDoingState.SlideKicking;
        TSFX.PlayAttack();
        

        PA.PlayAnimation("SlideKick");


        AttackCooldown = SLIDE_KICK_ATTACKCD;
        DashCooldown = DASHCD_SLIDE_KICK;
        JumpCooldown = DASHCD_SLIDE_KICK;

        ActionSpeed = 0;


        PA.SetMoveDir(AimDir);

        SetAimDirectionToMovement();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        ActionMoveSpeed = SLIDE_KICK_PUSH;
        ActionMoveDir = VisualAim.transform.up;

        for (int x = 0; x < SLIDE_KICK_DELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }



        
        

        for (int x = SLIDE_KICK_FRAMES; x > 0; x--)
        {

            AttackCast = Physics2D.BoxCastAll(SlideKickHurtBox.transform.position, SlideKickHurtBox.transform.localScale, SlideKickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
            for (int i = 0; i < AttackCast.Length; i++)
            {
                AttackCast[i].transform.transform.GetComponentInParent<IEnemy>().GotHit(SLIDE_KICK_DMG);

                if(x > SLIDE_KICK_THRESH1)
                    x = SLIDE_KICK_THRESH1;

            }

            if (x > SLIDE_KICK_THRESH1)
            {
                ActionMoveSpeed = SLIDE_KICK_PUSH;
            }
            else
            {
                ActionMoveSpeed = SLIDE_KICK_PUSH2;
            }

            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        

        ActionMoveSpeed = 0;
        ActionMoveDir = Vector2.zero;


        for (int x = 0; x < SLIDE_KICK_CD; x++)
        {
            yield return new WaitForFixedUpdate();
        }


        ActionSpeed = PLAYER_SPEED;

        SetWalkDirToAimDir();

        PDS = PlayerDoingState.Normal;

        PA.PlayAnimation("EnrageIdle");
    }

    IEnumerator EnrageAttack()
    {
        if (RAttackAlternate == 1)
        {
            PDS = PlayerDoingState.EnrageAttacking1;
            
        }
        if (RAttackAlternate == 2)
        {
            PDS = PlayerDoingState.EnrageAttacking2;
        }
        
        ActionSpeed = ATTACKSLOW_R; // How much slower will the player be while attacking
        PA.SetMoveDir(AimDir);
        
        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(RAttackHurtBox.transform.position, RAttackHurtBox.transform.localScale, RAttackHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);

        DashCooldown = DASHCD_RA;
        JumpCooldown = DASHCD_RA;

        

        for (int x = 0; x <= R_ATTACKDELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }
        

        if (RAttackAlternate == 1)
        {
            PA.PlayAnimation("EnrageAttack1");
            TSFX.PlayPunch1();
        }
            
        if (RAttackAlternate == 2)
        {
            TSFX.PlayPunch2();
            PA.PlayAnimation("EnrageAttack2");
        }

        //R_ATTACK_DMG.KBDuration = 3;
        //R_ATTACK_DMG.KBSpeed = 0.6f;
        //R_ATTACK_DMG2.KBDuration = 3;
        //R_ATTACK_DMG2.KBSpeed = 0.6f;

        for (int i = 0; i < AttackCast.Length; i++)
        {
            if (RAttackAlternate == 1)
            {
                //R_ATTACK_DMG.KnockBack = (VisualAim.transform.right);
                AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(R_ATTACK_DMG);
            }
            else
            {
                //R_ATTACK_DMG2.KnockBack = -(VisualAim.transform.right);
                AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(R_ATTACK_DMG2);
            }

            HitSomething = true;
            GameObject HitObject = Instantiate(HitEffectEnrageHit, AttackCast[i].point, Quaternion.identity);
            HitObject.GetComponent<HitEffect>().StartingVariables(AttackCast[i].transform.gameObject);
            DarkTherTarget = AttackCast[i].transform.GetComponentInParent<IEnemy>();
            DTTargetObject = AttackCast[i].transform.gameObject;
        }
        if (HitSomething)
        {
            HitCounter++;
            


        }


        if (RAttackAlternate == 1)
        {
            RAttackCooldown = R_ATTACKCD;
            RAttackAlternate++;

        }

        else if (RAttackAlternate == 2)
        {
            RAttackCooldown = R_ATTACKCD2;
            RAttackAlternate = 1;

        }
        

        for (int x = 0; x <= R_ATTACKFRAMES ; x++)
        {
            yield return new WaitForFixedUpdate();
        }



        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("EnrageIdle");

    }

    IEnumerator EnterChargeStance()
    {
        ActionSpeed = 0;
        PDS = PlayerDoingState.Charging;

        PA.PlayAnimation("PunchStance");

        bool Hold = true;
        int ChargeStanceCount = 0;

        

        while (Hold)
        {
            if ((LRight || LUp || LDown || LLeft) && !LStickHold && ChargeStanceCount > CHARGE_MIN_THRESH)
            {

                StartAction(DandyPunchAttackSwing(0));
            }

            if (ChargeStanceCount == CHARGE_UPPER_THRESH)
            {
                
                
                StartAction(RageBigSwing());

                yield break;
            }
            
            if (ChargeStanceCount == CHARGE_MIN_THRESH)
            {

                

                

                if (!AttackInputHold)
                {
                    StartAction(RageUppercutSwing());

                    yield break;
                }
                
            }

            ChargeStanceCount++;
            yield return new WaitForFixedUpdate();
        }


        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("EnrageIdle");

        
    }

    IEnumerator EnterKickStance()
    {
        ActionSpeed = 0;
        PDS = PlayerDoingState.KickStance;

        PA.PlayAnimation("KickStance");

        bool Hold = true;
        int ChargeStanceCount = 0;

        while (Hold)
        {
            if(ChargeStanceCount == LIGHTKICK_DELAY && !GunBatInputHold)
            {
                if (!GunBatInputHold)
                {
                    StartAction(LightKickAttackSwing());
                    yield break;
                }
                SR.color = Color.red;
            }
            if(ChargeStanceCount == KICKDELAY)
            {
                StartAction(HeavyKickAttackSwing());
                yield break;
            }
            if(ChargeStanceCount >= 8 && JumpInput)
            {
                StartAction(SlashKickAttackSwing());
                yield break;
            }

            ChargeStanceCount++;
            yield return new WaitForFixedUpdate();
        }


        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("EnrageIdle");


    }

    IEnumerator RageUppercutSwing()
    {
        PA.PlayAnimation("EnrageUppercut");
        TSFX.PlayAttack();
        PDS = PlayerDoingState.Uppercutting;

        ActionSpeed = UPPERCUTSLOW; // How much slower will the player be while attacking

        RAttackCooldown = ATTACKCD_UPPERCUT;
        DashCooldown = DASHCD_UPPERCUT;
        JumpCooldown = DASHCD_UPPERCUT;

        PA.SetMoveDir(AimDir);

        for(int i = 0; i < UPPERCUTDELAY; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(UpperCutHurtBox.transform.position, UpperCutHurtBox.transform.localScale, UpperCutHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(UPPERCUT_DMG, gameObject));
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            
        }

        KickComboCount = 0;
        KickComboTime = 0;

        for (int x = 0; x < UPPERCUTFRAMES; x++)
            yield return new WaitForFixedUpdate();

        PA.PlayAnimation("EnrageIdle");

        ActionSpeed = PLAYER_SPEED; // yes slow
        PDS = PlayerDoingState.Normal;
    }

    IEnumerator RageBigSwing()
    {
        PA.PlayAnimation("KOPunch");
        TSFX.PlayShoot();
        PDS = PlayerDoingState.BigPunching;

        ActionSpeed = 0; // How much slower will the player be while attacking

        RAttackCooldown = BIGPUNCH_ATTACKCD;
        DashCooldown = DASHCD_BIGPUNCH;
        JumpCooldown = DASHCD_BIGPUNCH;

        PA.SetMoveDir(AimDir);

        for (int i = 0; i < BIGPUNCHDELAY; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        bool HitSomething = false;

        

        AttackCast = Physics2D.BoxCastAll(ChargePunchHurtBox.transform.position, ChargePunchHurtBox.transform.localScale, ChargePunchHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(BIGPUNCH_DMG2, gameObject, VisualAim.transform.up, 1.2f, 6));
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            
        }

        KickComboCount = 0;
        KickComboTime = 0;

        for (int x = 0; x < BIGPUNCHFRAMES; x++)
            yield return new WaitForFixedUpdate();

        PA.PlayAnimation("EnrageIdle");

        ActionSpeed = PLAYER_SPEED; // yes slow
        PDS = PlayerDoingState.Normal;
    }

    IEnumerator HeavyKickAttackSwing() // big enrage kick
    {
        PDS = PlayerDoingState.HeavyKicking;

        
        ActionSpeed = KICKSLOW; // How much slower will the player be while attacking
        RAttackCooldown = KICK_ATTACKCD;
        DashCooldown = DASHCD_KICK;

        
        PA.SetMoveDir(AimDir);
        

        ActionMoveSpeed = 0;
        ActionMoveDir = VisualAim.transform.up;
        
        //for (int x = 0; x < KICKDELAY / 2; x++)
        //{
        //    yield return new WaitForFixedUpdate();
        //}

        SR.color = Color.white;
        PA.PlayAnimation("BigKick");

        for (int x = 0; x < KICK_LEADUP; x++)
        {
            if (x < KICK_THRESH1 && x > KICK_THRESH0)
            {
                ActionMoveSpeed = KICK_PUSH;
            }
            else if (x >= KICK_THRESH1 && x < KICK_THRESH2)
            {
                ActionMoveSpeed = KICK_PUSH2;
            }
            else
            {
                ActionMoveSpeed = KICK_PUSH3;
            }
            RaycastHit2D proximityCheck = Physics2D.BoxCast(transform.position, new Vector2(1, 1), KickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
            if (proximityCheck)
            {
                ActionMoveSpeed = KICK_PUSH3;
            }

            yield return new WaitForFixedUpdate();
        }
        


        TSFX.PlayAttack();

        CC.Shake(0.55f, 2);

        bool HitSomething = false;

        
        float PSpd = 1;
        int PDur = 200;

        AttackCast = Physics2D.BoxCastAll(KickHurtBox.transform.position, KickHurtBox.transform.localScale, KickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i<AttackCast.Length; i++)
        {
            KICK_DMG.KnockBack = Vector2.zero;
            
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(KICK_DMG);

            Instantiate(EnrageHKickProj, AttackCast[i].transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().
                InitializeImage(0, PDur, 0, new Damage(0, KICK_DMG.StunDuration, gameObject), 
                new MoveOrder[1] { new MoveOrder(VisualAim.transform.up, PSpd, PDur) });

            Instantiate(EnrageHKickProj, AttackCast[i].transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().
                InitializeImage(0, PDur, 0, new Damage(0, KICK_DMG.StunDuration, gameObject), 
                new MoveOrder[1] { new MoveOrder(VisualAim.transform.up - VisualAim.transform.right * 0.5f, PSpd, PDur) });

            Instantiate(EnrageHKickProj, AttackCast[i].transform.position, VisualAim.transform.rotation).GetComponent<FocusImage>().
                InitializeImage(0, PDur, 0, new Damage(0, KICK_DMG.StunDuration, gameObject), 
                new MoveOrder[1] { new MoveOrder(VisualAim.transform.up + VisualAim.transform.right * 0.5f, PSpd, PDur) });

            HitSomething = true;
        }
        


        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;

        if (HitSomething)
        {
            HitCounter++;


            KickComboCount++;
            KickComboTime = KICK_DMG.FlinchDuration + 20;

            Vector3 EndPos = transform.position + VisualAim.transform.up * (0.7f * 45);

            if (FloorUtilities.GetHighestFloor(Physics2D.LinecastAll(EndPos, EndPos + VisualAim.transform.up, FloorMask)) != FloorKey.Pit)
            {
                //transform.position = EndPos;

                //StartAction(BattingStance(0.7f));
                //yield break;
            }
        }
        CC.SetSize(5);

        for (int x = 0; x <= KICKFRAMES ; x++) // && !(LLeft || LRight || LUp || LDown)
        {

            PA.SetMoveDir(AimDir);


            yield return new WaitForFixedUpdate();
        }

        CC.ResetSize();

        

        KICK_DMG.KBDuration = 15;
        KICK_DMG.KBSpeed = 1.15f;

        for (int i = 0; i < AttackCast.Length; i++)
        {
            KICK_DMG.KnockBack = VisualAim.transform.up;

            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(KICK_DMG);
            
            HitSomething = true;
        }

        if ((LLeft || LRight || LUp || LDown))
        {
            StartAction(SlideKickAttackSwing());
            yield break;
        }

        PA.PlayAnimation("EnrageIdle");

        SetWalkDirToAimDir();
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
    }

    IEnumerator BattingStance(float LastSpeed)
    {
        ActionSpeed = 0;
        PDS = PlayerDoingState.BattingStance;

        CC.SetSize(15);

        PA.PlayAnimation("PunchStance");

        SetAimDirToVector(-VisualAim.transform.up);
        SetVAimToAimDir();
        SetWalkDirToAimDir();

        while (!AttackInputDown && !GunBatInputDown)
        {
            yield return new WaitForFixedUpdate();
        }

        while (AttackInput || GunBatInput)
        {
            yield return new WaitForFixedUpdate();
        }


        for(int i=0; i < 4; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        KickHurtBox.GetComponent<SpriteRenderer>().enabled = true;

        bool HitSomething = false;

        BATTING_DMG.KBDuration = (int)(35 / LastSpeed + 0.2f);
        BATTING_DMG.KBSpeed = LastSpeed + 0.2f;

        AttackCast = Physics2D.BoxCastAll(KickHurtBox.transform.position, KickHurtBox.transform.localScale, KickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            BATTING_DMG.KnockBack = VisualAim.transform.up;

            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(BATTING_DMG);
            
            HitSomething = true;
        }
        yield return new WaitForFixedUpdate();
        KickHurtBox.GetComponent<SpriteRenderer>().enabled = false;
        if (HitSomething)
        {
            Vector3 EndPos = transform.position + VisualAim.transform.up * ((BATTING_DMG.KBDuration - 6) * BATTING_DMG.KBSpeed);

            if (FloorUtilities.GetHighestFloor(Physics2D.LinecastAll(EndPos, EndPos + VisualAim.transform.up, FloorMask)) != FloorKey.Pit)
            {
                transform.position = EndPos;
                StartAction(BattingStance(LastSpeed + 0.2f));
                yield break;
            }
                
            
        }

        GunBatInputDown = false;
        AttackInputDown = false;

        CC.ResetSize();
        PA.PlayAnimation("EnrageIdle");

        SetWalkDirToAimDir();
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;

    }

    IEnumerator SlashKickAttackSwing() // big enrage follow up
    {
        PDS = PlayerDoingState.SlashKicking;
        PA.PlayAnimation("GKick");

        JumpInputDown = false;

        ActionSpeed = SLASHKICKSLOW; // How much slower will the player be while attacking
        RAttackCooldown = SLASHKICK_ATTACKCD;
        DashCooldown = DASHCD_SLASHKICK;

        SetAimDirectionToMovement();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        ActionMoveDir = VisualAim.transform.up;
        ActionMoveSpeed = SLASH_PUSH;

        PA.SetMoveDir(AimDir);
        //PA.SetSlashKicking(true);
        JumpInvul = true;

        for (int x = 0; x < SLASHKICKDELAY; x++)
        {
            if(x == SLASHKICK_THRESH)
            {
                ActionMoveDir = Vector3.zero;
            }
            yield return new WaitForFixedUpdate();
        }

        ActionMoveDir = Vector3.zero;

        TSFX.PlayAttack();

        CC.Shake(0.35f, 4);

        bool HitSomething = false;

        int PDur = 200;
        float PSpd = 1.2f;

        SLASHKICK_DMG.KBDuration = 6;
        SLASHKICK_DMG.KBSpeed = 0.8f;
        JumpInvul = false;
        AttackCast = Physics2D.BoxCastAll(SlashKickHurtBox.transform.position, SlashKickHurtBox.transform.localScale, SlashKickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            SLASHKICK_DMG.KnockBack = VisualAim.transform.right;
            if(Mathf.Abs(VisualAim.transform.right.x )  == 1 && AttackCast[i].transform.position.x < SlashKickHurtBox.transform.position.x)
            {
                SLASHKICK_DMG.KnockBack = -VisualAim.transform.right;
            }
            else if(AttackCast[i].transform.position.y < SlashKickHurtBox.transform.position.y)
            {
                SLASHKICK_DMG.KnockBack = -VisualAim.transform.right;
            }
            

            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(SLASHKICK_DMG);

            Instantiate(EnrageHKickProj, AttackCast[i].transform.position, Quaternion.Euler(VisualAim.transform.right)).GetComponent<FocusImage>().
                InitializeImage(0, PDur, 0, new Damage(0, SLASHKICK_DMG.StunDuration, gameObject),
                new MoveOrder[1] { new MoveOrder(VisualAim.transform.right, PSpd, PDur) });
            Instantiate(EnrageHKickProj, AttackCast[i].transform.position, Quaternion.Euler(-VisualAim.transform.right)).GetComponent<FocusImage>().
                InitializeImage(0, PDur, 0, new Damage(0, SLASHKICK_DMG.StunDuration, gameObject),
                new MoveOrder[1] { new MoveOrder(-VisualAim.transform.right, PSpd, PDur) });

            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            
            KickComboCount++;
            KickComboTime = SLASHKICK_DMG.FlinchDuration + 20;
        }

        

        
        
        
        for (int x = 0; x <= SLASHKICKFRAMES ; x++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        if ((LLeft || LRight || LUp || LDown))
        {
            StartAction(SlideKickAttackSwing());
            yield break;
        }

        SetWalkDirToAimDir();

        PA.PlayAnimation("EnrageIdle");
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
    }
    
    IEnumerator FlipAttackSwing() // big enrage kick
    {
        PA.PlayAnimation("FlipKick");

        PDS = PlayerDoingState.FlipKicking;

        ActionSpeed = FLIPSLOW; // How much slower will the player be while attacking
        RAttackCooldown = FLIP_ATTACKCD;
        DashCooldown = DASHCD_FLIP;

        

        

        PA.SetMoveDir(AimDir);

        SetAimDirectionToLook();
        SetVAimToAimDir();
        PA.SetMoveDir(AimDir);

        ActionMoveDir = -VisualAim.transform.up;
        ActionMoveSpeed = FLIPKNOCKBACK;

        for (int x = 0; x < FLIPDELAY; x++)
        {
            yield return new WaitForFixedUpdate();
        }
        TSFX.PlayAttack();

        bool HitSomething = false;

        AttackCast = Physics2D.BoxCastAll(FlipHurtBox.transform.position, FlipHurtBox.transform.localScale, FlipHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(FLIP_DMG);
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            
            KickComboCount = 0;
            KickComboTime = 0;
        }
        

        for (int x = 0; x <= FLIPFRAMES; x++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        
        
        ActionMoveSpeed = 0;


        for (int x = 0; x <= FLIPCD && !Interrupted; x++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }


        SetWalkDirToAimDir();
        PA.PlayAnimation("EnrageIdle");

        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;


    }

    IEnumerator LightKickAttackSwing() // small enrage kick
    {
        PA.PlayAnimation("LKick");

        PDS = PlayerDoingState.LightKicking;

        ActionSpeed = LIGHTKICK_SLOW; // How much slower will the player be while attacking
        RAttackCooldown = LIGHTKICK_ATTACKCD;
        DashCooldown = DASHCD_LIGHTKICK;

        

        PA.SetMoveDir(AimDir);


        //for (int x = 0; x < LIGHTKICK_DELAY; x++)
        //{
        //    yield return new WaitForFixedUpdate();
        //}
        TSFX.PlayAttack();



        bool HitSomething = false;



        AttackCast = Physics2D.BoxCastAll(LightKickHurtBox.transform.position, LightKickHurtBox.transform.localScale, LightKickHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(LIGHTKICK_DMG);
            HitSomething = true;
        }
        if (HitSomething)
        {
            HitCounter++;
            

            KickComboCount++;
            KickComboTime = LIGHTKICK_DMG.FlinchDuration + 20;
        }
        

        RAttackAlternate++;
        
        for (int x = 0; x <= LIGHTKICK_FRAMES && !Interrupted; x++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }

        if ((LLeft || LRight || LUp || LDown))
        {
            StartAction(SlideKickAttackSwing());
            yield break;
        }

        SetWalkDirToAimDir();
        PA.PlayAnimation("EnrageIdle");

        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;


    }

    IEnumerator ShotgunBlast()
    {
        PDS = PlayerDoingState.Shooting;

        PA.SetMoveDir(AimDir);

        PA.PlayAnimation("EnrageShotgun");
        
        ActionSpeed = SHOTGUN_SLOW;

        

        TSFX.PlayShotgun();

        for (int i = 0; i < SHOTGUNDELAY; i++)
        {
            PA.SetMoveDir(AimDir);
            yield return new WaitForFixedUpdate();
        }
        

        

        ActionMoveDir = -VisualAim.transform.up;
        ActionMoveSpeed = SHOTGUNKNOCKBACK;

        ShotgunCast = Physics2D.BoxCastAll(ShotgunHurtBoxBase.transform.position, ShotgunHurtBoxBase.transform.localScale, ShotgunHurtBoxBase.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        Instantiate(ShotgunExplosion, ShotgunProjectileBases[0].transform.position, ShotgunProjectileBases[0].transform.rotation);
        for (int i = 0; i < ShotgunCast.Length; i++) // the base of the shotgun explosion
        {
            ShotgunCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(SHOTGUN_DMG));
            PA.SetMoveDir(AimDir);
        }
        ShotgunCast = Physics2D.BoxCastAll(ShotgunHurtBoxTip.transform.position, ShotgunHurtBoxTip.transform.localScale, ShotgunHurtBoxTip.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < ShotgunCast.Length; i++) // the tip of the shotgun explosion
        {
            ShotgunCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(SHOTGUNTIP_DMG));
            PA.SetMoveDir(AimDir);
        }
        ShotgunComboCount++;
        for (int x = 0; x <= SHOTGUNFRAMES; x++)
            yield return new WaitForFixedUpdate();

        ActionMoveDir = Vector3.zero;
        ActionMoveSpeed = 0;

        if (ShotgunComboCount == 2)
        {
            ShotgunComboCount = 0;
            for (int x = 0; x < SHOTGUNCOOLDOWN; x++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        
        
        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
        PA.PlayAnimation("EnrageIdle");

        

        
    }

    //IEnumerator RageSpike()
    //{
    //    PDS = PlayerDoingState.Attack;
    //    PA.SetMoveDir(AimDir);
    //    GlobalVelMul = 0;

    //    for (int i = 0; i < 6; i++) // delay
    //    {
    //        PA.SetMoveDir(AimDir);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    Instantiate(GroundSpiker, transform.position, GunbatVAim.transform.rotation);

    //    for (int i = 0; i < 3; i++) // frames
    //    {
    //        PA.SetMoveDir(AimDir);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    GlobalVelMul = PLAYER_SPEED;
    //    PDS = PlayerDoingState.Normal;
    //}

    IEnumerator Reload()
    {
        PA.PlayStaplingReload(); // plays the start
        PA.PlayAnimation("StaplingReloadStart"); // tells the animator that the idle is still playing
        // set animator to reload
        ReloadMoveCD = HitCounter + RELOAD_CD;
        TSFX.PlayStapleReload2();
        ActionSpeed = STAPLING_RELOADSLOW;
        PDS = PlayerDoingState.Reloading;

        for (int i = 0; i < RELOAD_FRAMES && PDS == PlayerDoingState.Reloading; i++)
        {
            if (i < RELOAD_FILL_FRAME && i % 10 == 0)
            {
                TSFX.PlayStapleReload1();
            }
            if (i == RELOAD_FILL_FRAME)
            {
                PA.PlayAnimation("StaplingReloadFinish");
                
                TSFX.PlayStapleReload2();
                StaplingAmmo = STAPLE_MAXAMMO;
                SB.Reload();
                SB.SetStapleCount(StaplingAmmo);
            }
            yield return new WaitForFixedUpdate();
        }

        PA.PlayAnimation("Idle");

        ActionSpeed = PLAYER_SPEED;
        PDS = PlayerDoingState.Normal;
    }

    //CONTAINS MANY MOOORE COROUTINES
    //IEnumerator StaplerBlast()
    //{
    //    PDS = PlayerDoingState.Shooting;

    //    PA.SetMoveDir(AimDir);

    //    PA.SetEnrShotgun(true);

    //    GlobalVelMul = STAPLERBLAST_SLOW;
    //    for (int i = 0; i < STAPLERBLAST_DELAY; i++)
    //    {
    //        PA.SetMoveDir(AimDir);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    int SBFrames = 0;

    //    if (StaplingAmmo > 50)
    //    {
    //        SBFrames = STAPLERBLAST_FRAMES2;
    //    }
    //    else
    //    {
    //        SBFrames = STAPLERBLAST_FRAMES2;
    //    }

    //    GlobalVelMul = STAPLING_SLOW;

    //    TSFX.PlayShotgun();

    //    StaplerBlastPushback = true;

    //    AttackCast = Physics2D.BoxCastAll(StaplerBlastHurtBox.transform.position, StaplerBlastHurtBox.transform.localScale, StaplerBlastHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //    Instantiate(ShotgunExplosion, ShotgunProjectileBases[0].transform.position, ShotgunProjectileBases[0].transform.rotation);
    //    for (int i = 0; i < AttackCast.Length; i++) // the base of the shotgun explosion
    //    {
    //        AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(STAPLERBLAST_DMG, gameObject, VisualAim.transform.up, 0.65f, 10));
    //        PA.SetMoveDir(AimDir);
    //    }


    //    StaplingAmmo = 0;

    //    for (int x = 0; x <= SBFrames; x++)
    //        yield return new WaitForFixedUpdate();

    //    StaplerBlastPushback = false;
    //    PDS = PlayerDoingState.Normal;

    //    for (int x = 0; x < STAPLERBLAST_CD && PDS == PlayerDoingState.Normal; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    PA.SetEnrShotgun(false);
    //}

    //IEnumerator KatanaParry()
    //{
    //    PA.SetKatanaParry(true);
    //    GlobalVelMul = 0;
    //    PDS = PlayerDoingState.Parrying;
    //    KatanaParryCoolDown = PARRY_PARRYCD;
    //    AttackCooldown = PARRY_ATTACKCD;
    //    DashCooldown = PARRY_DASHCD;
    //    ParryDuration = PARRY_DURATION_MAX;
    //    if ((LLeft || LRight || LUp || LDown))
    //    {
    //        LStickHold = true;
    //    }

    //    for (int i = 0; i < PARRY_FRAMES; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    if (Interrupted)
    //    {
    //        PA.SetKatanaParry(false);
    //        if (PDS == PlayerDoingState.Parrying)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        yield break;
    //    }
    //    for (int i = 0; i < PARRY_CD && KatanaParryWindow <= 0 && !Interrupted; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    PA.SetKatanaParry(false);
    //    if (PDS == PlayerDoingState.Parrying)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = KATANA_SPEED;
    //    }
    //}

    //IEnumerator KatanaAttackSwing()
    //{
    //    if (Interrupted)
    //    {
    //        yield break;
    //    }

    //    GlobalVelMul = 0;
    //    PDS = PlayerDoingState.Attack;
    //    AttackCooldown = KATANA_ATTACK_ATTACKCD;
    //    DashCooldown = KATANA_ATTACK_DASHCD;
    //    KatanaParryCoolDown = KATANA_ATTACK_PARRYCD;
    //    PA.SetMoveDir(AimDir);
    //    PA.SetAttacking(true);
    //    int KatanaDelay = KATANA_ATTACK_DELAY;




    //    KatanaParryWindow = 0;
    //    for (int i = 0; i < KatanaDelay; i++)
    //    {
    //        if (Interrupted)
    //        {
    //            if (PDS == PlayerDoingState.Attack)
    //            {
    //                PDS = PlayerDoingState.Normal;
    //                GlobalVelMul = KATANA_SPEED;
    //            }
    //            PA.SetAttacking(false);
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }

    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        PA.SetAttacking(false);
    //        yield break;
    //    }
    //    PA.SetAttacking(false);
    //    if (AttackInputHold)
    //        StartCoroutine(KatanaSpin());
    //    else
    //        StartCoroutine(KatanaAttackShort());

    //}

    //IEnumerator KatanaAttackShort() // skips the delay
    //{
    //    GlobalVelMul = 0;
    //    PDS = PlayerDoingState.Attack;
    //    AttackCooldown = KATANA_ATTACK_ATTACKCD - KATANA_ATTACK_DELAY;
    //    DashCooldown = KATANA_ATTACK_DASHCD - KATANA_ATTACK_DELAY;
    //    KatanaParryCoolDown = KATANA_ATTACK_PARRYCD - KATANA_ATTACK_DELAY;
    //    PA.SetKatanaAttackShort(true);
    //    float KatanaDmg = 0;
    //    if (KatanaQuality > 0)
    //    {
    //        KatanaDmg = KATANA_DMG;
    //    }
    //    else
    //    {
    //        KatanaDmg = KATANA_DMG_MIN;
    //        TSFX.PlayLockOn();
    //    }

    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        PA.SetKatanaAttackShort(false);
    //        yield break;
    //    }

    //    bool HitSomething = false;
    //    if (KatanaDmg != KATANA_DMG_MIN)
    //    {
    //        AttackCast = Physics2D.BoxCastAll(KatanaHurtBox.transform.position, KatanaHurtBox.transform.localScale, KatanaHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //        for (int i = 0; i < AttackCast.Length; i++)
    //        {

    //            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(KatanaDmg, 25, gameObject, VisualAim.transform.up, 0.25f, 3));



    //            HitSomething = true;
    //        }
    //    }

    //    AttackCast = Physics2D.BoxCastAll(KatanaHurtBox2.transform.position, KatanaHurtBox2.transform.localScale, KatanaHurtBox2.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //    for (int i = 0; i < AttackCast.Length; i++) // small hitbox
    //    {

    //        AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(KatanaDmg, gameObject));
    //        HitSomething = true;
    //    }
    //    if (HitSomething)
    //    {
    //        RageCounter += RAGE_BUILD;
    //    }

    //    if (KatanaDmg != KATANA_DMG_MIN)
    //    {
    //        KatanaHurtBox.GetComponent<SpriteRenderer>().enabled = true;
    //    }


    //    yield return new WaitForFixedUpdate(); // DELETE THIS AFTER DELETING SPRITERENDER GET
    //    KatanaHurtBox.GetComponent<SpriteRenderer>().enabled = false;

    //    KatanaQuality--;
    //    if (KatanaQuality < 0)
    //    {
    //        KatanaQuality = 0;
    //    }

    //    for (int i = 0; i < KATANA_ATTACK_FRAMES; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    PDS = PlayerDoingState.Normal;
    //    GlobalVelMul = KATANA_SPEED;
    //    PA.SetKatanaAttackShort(false);
    //}

    //IEnumerator KatanaSpin()
    //{
    //    PDS = PlayerDoingState.Attack;
    //    TSFX.PlayAttack();

    //    AttackCooldown = KATANA_SPIN_ATTACKCD;
    //    DashCooldown = KATANA_SPIN_DASHCD;
    //    KatanaParryCoolDown = KATANA_SPIN_PARRYCD;

    //    GlobalVelMul = KATANA_SPIN_SLOW;


    //    PA.SetMoveDir(AimDirKey.Up);
    //    PA.SetSpinning(true);

    //    float KatanaDmg = 0;
    //    if (KatanaQuality > 0)
    //    {
    //        KatanaDmg = KATANA_SPIN_DMG_MAX;
    //    }
    //    else
    //    {
    //        KatanaDmg = KATANA_SPIN_DMG_MIN;
    //        TSFX.PlayLockOn();
    //    }
    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        PA.SetSpinning(false);
    //        yield break;
    //    }

    //    int KBDur = 10;
    //    bool HitSomething = false;
    //    if (KatanaDmg != KATANA_SPIN_DMG_MIN)
    //    {
    //        AttackCast = Physics2D.CircleCastAll(transform.position, 4, transform.up, 0, EnemyMask); // A circle with a radius of 2
    //        for (int i = 0; i < AttackCast.Length; i++)
    //        {
    //            if (Vector2.Distance(AttackCast[i].transform.position, transform.position) < 1)
    //            {
    //                KBDur = 15;
    //            }
    //            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(KatanaDmg, gameObject, (AttackCast[i].transform.position - transform.position).normalized, 0.45f, KBDur));
    //            HitSomething = true;
    //        }
    //    }
    //    else
    //    {
    //        AttackCast = Physics2D.CircleCastAll(transform.position, 2, transform.up, 0, EnemyMask); // A circle with a radius of 2
    //        for (int i = 0; i < AttackCast.Length; i++)
    //        {
    //            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(KatanaDmg, gameObject));
    //            HitSomething = true;
    //        }
    //    }

    //    if (HitSomething)
    //    {
    //        RageCounter += RAGE_BUILD;
    //    }

    //    for (int x = 0; x <= KATANA_SPIN_FRAMES; x++)
    //    {
    //        PA.SetMoveDir(AimDirKey.Up);
    //        yield return new WaitForFixedUpdate();
    //    }


    //    PA.SetSpinning(false);

    //    if (PDS == PlayerDoingState.Attack)
    //    {
    //        GlobalVelMul = KATANA_SPEED;
    //        PDS = PlayerDoingState.Normal;
    //    }
    //}

    //IEnumerator KatanaThrust()
    //{
    //    if ((LLeft || LRight || LUp || LDown))
    //    {
    //        LStickHold = true;

    //    }
    //    PDS = PlayerDoingState.Attack;
    //    TSFX.PlayAttack();
    //    PA.SetThrust(true);

    //    KatanaPushforward = true;
    //    KatanaThrustSpeed = KATANA_THRUST_PUSH;

    //    AttackCooldown = KATANA_THRUST_ATTACKCD;
    //    DashCooldown = KATANA_THRUST_DASHCD;

    //    GlobalVelMul = 0;

    //    SetAimDirectionToMovement();

    //    SetFreeAimToMovement();
    //    if (KeyBoardMouse)
    //    {
    //        SetFreeAimToMouse();

    //    }

    //    PA.SetMoveDir(AimDir);
    //    LightningBalls.Add(Instantiate(LightningBall, transform.position, GunbatVAim.transform.rotation * Quaternion.Euler(0, 0, 90)));
    //    KatanaThrustAmmo--;

    //    for (int x = 0; x < KATANA_THRUST_DELAY; x++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        KatanaPushforward = false;
    //        PA.SetThrust(false);
    //        yield break;
    //    }

    //    bool HitSomething = false;


    //    for (int x = 0; x <= KATANA_THRUST_FRAMES; x++)
    //    {
    //        if (!HitSomething)
    //        {
    //            AttackCast = Physics2D.BoxCastAll
    //                (KatanaThrustHurtBox.transform.position, KatanaThrustHurtBox.transform.localScale, KatanaThrustHurtBox.transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
    //            for (int i = 0; i < AttackCast.Length; i++)
    //            {
    //                AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(0, 250, gameObject));
    //                if (!KatanaThrustHits.Contains(AttackCast[i].transform.GetComponentInParent<IEnemy>()))
    //                {
    //                    KatanaThrustAmmo++;
    //                }
    //                KatanaThrustHits.Add(AttackCast[i].transform.GetComponentInParent<IEnemy>());
    //                HitSomething = true;
    //            }
    //        }


    //        if (x < KATANA_THRUST_THRESH1)
    //        {
    //            KatanaThrustSpeed = KATANA_THRUST_PUSH;
    //        }
    //        else
    //        {
    //            KatanaThrustSpeed = KATANA_THRUST_PUSH2;
    //        }

    //        PA.SetMoveDir(AimDir);
    //        yield return new WaitForFixedUpdate();
    //    }



    //    KatanaPushforward = false;

    //    if (Interrupted)
    //    {
    //        if (PDS == PlayerDoingState.Attack)
    //        {
    //            PDS = PlayerDoingState.Normal;
    //            GlobalVelMul = KATANA_SPEED;
    //        }
    //        KatanaPushforward = false;
    //        PA.SetThrust(false);
    //        yield break;
    //    }
    //    PA.SetThrust(false);

    //    for (int x = 0; x < KATANA_THRUST_CD; x++)
    //    {
    //        //if ((LLeft || LRight || LUp || LDown) && x > 7 && KatanaThrustAmmo > 0)
    //        //{
    //        //    StartCoroutine(KatanaThrust());
    //        //    yield break;
    //        //}
    //        if ((LLeft || LRight || LUp || LDown) && x > 5 && KatanaThrustAmmo > 0 && !LStickHold)
    //        {
    //            SetFreeAimToMovement();
    //            StartCoroutine(KatanaThrust());
    //            yield break;
    //        }

    //        yield return new WaitForFixedUpdate();
    //    }
    //    PA.SetKatanaThrustEnd(true);
    //    for (int x = 0; x < KATANA_THRUST_CD2; x++)
    //    {

    //        yield return new WaitForFixedUpdate();
    //    }

    //    for (int i = 0; i < LightningBalls.Count; i++)
    //    {
    //        LightningBalls[i].GetComponent<LightningBall>().Activate();
    //    }
    //    for (int i = 0; i < KatanaThrustHits.Count; i++)
    //    {
    //        if (KatanaQuality > 0)
    //        {
    //            KatanaThrustHits[i].GotHit(new Damage(KATANA_THRUST_DMG, 5, gameObject));
    //        }
    //        else
    //        {

    //            KatanaThrustHits[i].GotHit(new Damage(KATANA_THRUST_DMG_MIN, 5, gameObject));
    //        }
    //    }
    //    KatanaQuality--;
    //    LightningBalls.Clear();

    //    if (PDS == PlayerDoingState.Attack)
    //    {
    //        GlobalVelMul = KATANA_SPEED;

    //        SetWalkDirToAimDir();

    //        PDS = PlayerDoingState.Normal;
    //    }

    //    PA.SetKatanaThrustEnd(false);
    //}

    //IEnumerator KatanaTransform()
    //{
    //    GlobalVelMul = 0;
    //    PDS = PlayerDoingState.Transforming;
    //    PA.SetKatanaStart(true);
    //    PA.SetKatana(true);
    //    for (int i = 0; i < KATANA_TRANS_FRAMES && !Interrupted; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    InKatana = true;
    //    PA.SetKatanaStart(false);
    //    if (PDS == PlayerDoingState.Transforming)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = KATANA_SPEED;
    //    }

    //}

    //IEnumerator KatanaExit()
    //{
    //    GlobalVelMul = KATANA_EXIT_SPEED;
    //    for (int i = 0; i < KATANA_EXIT_FRAMES && !Interrupted; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    if (PDS == PlayerDoingState.Transforming)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = PLAYER_SPEED;
    //    }
    //}



    //IEnumerator GBCSlam()
    //{
    //    PA.SetGunFire(false);
    //    GlobalVelMul = 0;
    //    PA.SetGunBatSlamming(true);

    //    for (int i = 0; i < GBCDELAY; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    GBC.GetComponent<GunBatShotChained>().Slam(new Damage(GUNBATSLAM_DMG));


    //    for (int i = 0; i < GBCFRAMES; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    GlobalVelMul = PLAYER_SPEED;
    //    PDS = PlayerDoingState.Normal;
    //    PA.SetGunBatSlamming(false);
    //}

    //IEnumerator GBCExplosion()
    //{
    //    PA.SetGunFire(false);

    //    for (int i = 0; i < GBCDELAY; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    GBC.GetComponent<GunBatShotChained>().Explode();


    //    for (int i = 0; i < GBCFRAMES; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    GlobalVelMul = PLAYER_SPEED;
    //    PDS = PlayerDoingState.Normal;

    //}

    //IEnumerator Taunt()
    //{
    //    GlobalVelMul = 0;
    //    int i = 0;
    //    while (PDS == PlayerDoingState.Taunting && i < TAUNT_DURATION)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        i++;
    //    }
    //    if (PDS == PlayerDoingState.Taunting)
    //    {
    //        PDS = PlayerDoingState.Normal;
    //        GlobalVelMul = PLAYER_SPEED;
    //    }

    //}

    //IEnumerator RagingMove()
    //{
    //    for (int i = 0; i < RAGING_DURATION; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    PDS = PlayerDoingState.Normal;
    //    GlobalVelMul = PLAYER_SPEED;
    //}

    //IEnumerator GrabbingHand()
    //{
    //    GameObject Hand = Instantiate(GrabHand, transform.position + VisualAim.transform.up * 5, Quaternion.identity);
    //    while (RunDrawInput)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }
    //    if (KeyBoardMouse)
    //    {
    //        Hand.GetComponent<GrabHand>().PullHand(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
    //    }
    //    else
    //    {
    //        Hand.GetComponent<GrabHand>().PullHand(VisualAim.transform.up);
    //    }
    //    GlobalVelMul = PLAYER_SPEED;
    //    PDS = PlayerDoingState.Normal;
    //}
   
    
    #endregion

    public void HookShotGrab(Vector2 Pos) // when hookshot hits something it calls this function, this function will return where to send the hookshotted enemy/object
    {
        SetKnockback(Pos - (Vector2)transform.position, 8, 0.125f);
    }

    public AimDirKey GetAimDir()
    {
        return AimDir;
    }
    public AimDirKey GetMoveDir()
    {
        return WalkDir;
    }

    #region InputMethods
    public void OnMove(InputValue value)
    {
        
        LeftStickVal = value.Get<Vector2>();


        LLeft = LeftStickVal.x < -L_HOR_THRESH;
        LRight = LeftStickVal.x > L_HOR_THRESH;
        LUp = LeftStickVal.y > L_VER_THRESH;
        LDown = LeftStickVal.y < -L_VER_THRESH;
        LLeftDown = LLeft;
        LRightDown = LRight;
        LUpDown = LUp;
        LDownDown = LDown;
    }

    public void OnLook(InputValue value)
    {
        RightStickVal = value.Get<Vector2>();

        

        if (!IsNotFighting)
        {
            // R is for right stick input
            RLeft = RightStickVal.x < -R_HOR_THRESH;
            RRight = RightStickVal.x > R_HOR_THRESH;
            RUp = RightStickVal.y > R_VER_THRESH;
            RDown = RightStickVal.y < -R_VER_THRESH;
        }

    }

    public void OnAttack(InputValue value)
    {
        float AttackVal = value.Get<float>();

        if(AttackVal == 0)
        {
            AttackInput = false;
            AttackInputDown = false;
        }
        else
        {
            AttackInput = true;
            AttackInputDown = true;
        }
    }

    public void OnDodge(InputValue value)
    {
        float DodgeVal = value.Get<float>();
        if (DodgeVal == 0)
        {
            DashInput = false;
            DashInputDown = false;
        }
        else if(DodgeVal > 0)
        {
            DashInput = true;
            DashInputDown = true;
        }
    }

    public void OnJump(InputValue value)
    {
        float JumpVal = value.Get<float>();
        if (JumpVal == 0)
        {
            JumpInput = false;
            JumpInputDown = false;
        }
        else if (JumpVal > 0)
        {
            JumpInput = true;
            JumpInputDown = true;
        }
    }

    public void OnLockOn(InputValue value)
    {
        float LockOnVal = value.Get<float>();
        if(LockOnVal == 1)
        {
            //LockOnInput = true;
        }
        else
        {
            //LockOnInput = false;
        }
    }

    public void OnGunBat(InputValue value)
    {
        float Val = value.Get<float>();
        if (Val > 0)
        {
            
            GunBatInput = true;
            GunBatInputDown = true;
        }
        else
        {

            GunBatInput = false;
            GunBatInputDown = false;
        }
    }

    public void OnRunDraw(InputValue value)
    {
        float Val = value.Get<float>();
        if (Val > 0)
        {
            RunDrawInput = true;
            RunDrawInputDown = true;
        }
        else
        {
            RunDrawInput = false;
            RunDrawInputDown = false;
        }
    }

    public void OnAccessoryAction(InputValue value)
    {

        float Val = value.Get<float>();
        if (Val > 0)
        {
            AAInput = true;
            AAInputDown = true;
        }
        else
        {
            AAInput = false;
            AAInputDown = false;
        }
    }

    public void OnPause(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            PauseInputDown = true;
        }
        else
        {
            PauseInputDown = false;
        }
    }

    public void OnEnrage(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            EnrageInput = true;
        }
        else
        {
            EnrageInput = false;
        }
    }

    public void OnTaunt(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            TauntInput = true;
        }
        else
        {
            TauntInput = false;
        }
    }

    public void OnDeselect(InputValue value)
    {
        
    }
    public void OnKickAttack(InputValue value)
    {
        
        float val = value.Get<float>();
        if (val > 0)
        {
            KickAttackInput = true;
            KickAttackInputDown = true;
            
        }
        else
        {
            KickAttackInput = false;
            KickAttackInputDown = false;
        }
    }

    public void OnBauble(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            BaubleInput = true;
            BaubleInputDown = true;

        }
        else
        {
            BaubleInput = false;
            BaubleInputDown = false;
        }
    }
    public void OnClone(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            CloneInput = true;
            CloneInputDown = true;

        }
        else
        {
            CloneInput = false;
            CloneInputDown = false;
        }
    }
    public void OnModeSwitchUp(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            ModeSwitchUp = true;
        }
        else
        {
            ModeSwitchUp = false;
        }
    }
    public void OnModeSwitchDown(InputValue value)
    {
        float val = value.Get<float>();
        if (val > 0)
        {
            ModeSwitchDown = true;
        }
        else
        {
            ModeSwitchDown = false;
        }
    }

    #endregion


}
