using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionSFX : MonoBehaviour
{
    AudioSource AS;
    public AudioSource LS, GLS, MLS;
    public AudioSource ThreeDimAS; // three dimensional
    public AudioSource StaticSource;
    public AudioClip Roar, Hurt, Slam, Land, LaserCharge, LaserShot, LaserMixup, PoleSpin, GoSpin, HeadSlamWind, SwipeHit, TwoSwipeHit;
    public AudioClip Pace1, Pace2, ManicStep1, ManicStep2, StaticNoise, Spin, BurstLaserCharge, GigaLaserCharge, ManicLaserCharge;
    public AudioClip StaplerExplode;

    int HurtCD = 0;
    

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HurtCD--;   
    }
    public void PlayManicLaserCharge()
    {
        AS.PlayOneShot(ManicLaserCharge);
    }
    public void PlayBurstLaserCharge()
    {
        AS.PlayOneShot(BurstLaserCharge);
    }
    public void PlayGigaLaserCharge()
    {
        AS.PlayOneShot(GigaLaserCharge);
    }

    public void PlayHurt()
    {
        if(HurtCD <= 0)
        {
            AS.PlayOneShot(Hurt);
            HurtCD = 10;
        }
        
    }
    public void EnableStatic(bool value)
    {
        if (value)
        {
            StaticSource.Play();
        }
        else
        {
            StaticSource.Stop();
        }
    }

    public void PlayStep1()
    {
        AS.PlayOneShot(Pace1);
    }
    public void PlayStep2()
    {
        AS.PlayOneShot(Pace2);
    }
    public void PlayManicStep1()
    {
        ThreeDimAS.PlayOneShot(ManicStep1);
    }
    public void PlayManicStep2()
    {
        ThreeDimAS.PlayOneShot(ManicStep2);
    }
    public void PlaySpin()
    {
        AS.PlayOneShot(Spin);
    }

    public void PlayStapleExplode()
    {
        AS.PlayOneShot(StaplerExplode);
    }
    public void PlayPoleSpin()
    {
        AS.PlayOneShot(PoleSpin);
    }
    public void PlaySwipeHit()
    {
        AS.PlayOneShot(SwipeHit);
    }
    public void PlaySwipeNoHit()
    {
        AS.PlayOneShot(Roar);
    }
    public void PlayTwoSwipeHit()
    {
        AS.PlayOneShot(TwoSwipeHit);
    }
    public void PlayTwoSwipeNoHit()
    {
        AS.PlayOneShot(Roar);
    }
    public void PlayGoSpin()
    {
        AS.PlayOneShot(GoSpin);
    }
    public void PlayHeadSlamWind()
    {
        AS.PlayOneShot(HeadSlamWind);
    }

    public void PlaySlam()
    {
        AS.PlayOneShot(Slam);
    }
    public void PlayRoar()
    {
        AS.PlayOneShot(Roar);
    }

    public void PlayLand()
    {
        AS.PlayOneShot(Land);
    }
    public void PlayChargeUp()
    {
        LS.PlayOneShot(LaserCharge);
    }
    public void PlayLaserShot()
    {
        LS.PlayOneShot(LaserShot);
    }
    public void PlayLaserMixup()
    {
        LS.PlayOneShot(LaserMixup);
    }
    public void EnableLaser(bool value)
    {
        if (value)
        {
            LS.Play();
        }
        else
        {
            LS.Stop();
        }
    }
    public void EnableGigaLaser(bool value)
    {
        if (value)
        {
            GLS.Play();
        }
        else
        {
            GLS.Stop();
        }
    }
    public void EnableManicLaser(bool value)
    {
        if (value)
        {
            MLS.Play();
        }
        else
        {
            MLS.Stop();
        }
    }
}
