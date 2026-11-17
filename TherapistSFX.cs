using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TherapistSFX : MonoBehaviour
{
    AudioSource AS;
    
    public AudioClip Attack, Hurt, LockOn, Dash1, Dash2, Heal, BigHeal, Shoot, Shotgun, Flamethrower, Flameblast, Staple, StapleLow, Enrage,Step;
    public AudioClip Quality1, Quality2, Quality3, AbilityReady;
    public AudioClip Slam, Spin, Thrust, StapleReload1, StapleReload2, StapleOutOfAmmo, Punch1, Punch2;

    


    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    public void PlaySlam()
    {
        AS.PlayOneShot(Slam);
    }
    public void PlayPunch1()
    {
        AS.PlayOneShot(Punch1);
    }
    public void PlayPunch2()
    {
        AS.PlayOneShot(Punch2);
    }
    public void PlaySpin()
    {
        AS.PlayOneShot(Spin);
    }
    public void PlayThrust()
    {
        AS.PlayOneShot(Thrust);
    }
    public void PlayStapleReload1()
    {
        AS.PlayOneShot(StapleReload1);
    }
    public void PlayStapleReload2()
    {
        AS.PlayOneShot(StapleReload2);
    }
    public void PlayStapleOutOfAmmo()
    {
        AS.PlayOneShot(StapleOutOfAmmo);
    }

    public void PlayAbilityReady()
    {
        AS.PlayOneShot(AbilityReady);
    }

    public void PlayAttack()
    {
        AS.PlayOneShot(Attack);
    }

    public void PlayHurt()
    {
        AS.PlayOneShot(Hurt);
    }
    public void PlayLockOn()
    {
        AS.PlayOneShot(LockOn);
    }
    public void PlayDash()
    {
        AS.PlayOneShot(Dash1);
    }
    public void PlayRoll()
    {
        AS.PlayOneShot(Dash2);
    }
    public void PlayHeal()
    {
        AS.PlayOneShot(Heal);
    }
    public void PlayBigHeal()
    {
        AS.PlayOneShot(BigHeal);
    }
    public void PlayShoot()
    {
        AS.PlayOneShot(Shoot);
    }
    public void PlayShotgun()
    {
        AS.PlayOneShot(Shotgun);
    }
    public void PlayFlamethrower()
    {
        AS.PlayOneShot(Flamethrower);
    }
    public void PlayFlameBlast()
    {
        AS.PlayOneShot(Flameblast);
    }
    public void PlayStaple()
    {
        AS.PlayOneShot(Staple);
    }
    public void PlayStapleLow()
    {
        AS.PlayOneShot(StapleLow);
    }
    public void PlayEnrage()
    {
        AS.PlayOneShot(Enrage);
    }
    public void PlayStep()
    {
        AS.PlayOneShot(Step);
    }

    public void PlayQuality1()
    {
        AS.PlayOneShot(Quality1);
    }
    public void PlayQuality2()
    {
        AS.PlayOneShot(Quality2);
    }
    public void PlayQuality3()
    {
        AS.PlayOneShot(Quality3);
    }
}
