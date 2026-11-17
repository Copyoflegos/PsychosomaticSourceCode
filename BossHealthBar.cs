using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    // This script manages the boss healthbar
    // Attach it to your boss of choice, tell it which bar is the boss', and then set the boss heatlh within the boss script
    float BossHealth = 0;
    float MaxHealth = 1000;
    int StapleCount;
    public Image HealthBarImg;
    public Image Container;
    public Sprite Original;
    public Sprite Static;
    float PreviousHealth;
    
    public Image StaplePoint;
    public Image StaplePoint2;
    Animator StapleANIM1;
    Animator StapleANIM2;

    private void Start()
    {
        StapleANIM1 = StaplePoint.GetComponent<Animator>();
        StapleANIM2 = StaplePoint2.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if(BossHealth < 0)
        {
            BossHealth = 0;
        }
        HealthBarImg.rectTransform.localScale = new Vector3 (BossHealth / MaxHealth, 1,1);
        

        
    }
    public void SetBossHealth(float Health)
    {
        BossHealth = Health;
        
        if(BossHealth != PreviousHealth)
        {
            StartCoroutine(StaticFlash());
        }
        PreviousHealth = BossHealth;
    }
    public void SetBossMaxHealth(float Health)
    {
        MaxHealth = Health;
    }
    public void SetStapleCount(int Staples)
    {
        if (Staples >= 30)
        {
            StapleANIM1.Play("StapleIdle3");
            StapleANIM2.Play("StapleIdle3");
        }
        else if (Staples >= 20)
        {
            StapleANIM1.Play("StapleIdle2");
            StapleANIM2.Play("StapleIdle2");
        }
        else if (Staples >= 10)
        {
            StapleANIM1.Play("StapleIdle1");
            StapleANIM2.Play("StapleIdle1");
            StaplePoint.enabled = true;
            StaplePoint2.enabled = true;
        }
        else
        {
            StaplePoint.enabled = false;
            StaplePoint2.enabled = false;
        }
    }
    public void BlowStaples()
    {
        StapleANIM1.Play("StaplePointExplosion");
        StapleANIM2.Play("StaplePointExplosion");
        StartCoroutine(BlowWait());
    }
    IEnumerator StaticFlash()
    {
        if(Container.sprite != Static)
        {
            Container.sprite = Static;
            HealthBarImg.enabled = false;
            yield return new WaitForSeconds(0.1f);
            HealthBarImg.enabled = true;
            Container.sprite = Original;
        }
        else
        {
            HealthBarImg.enabled = true;
            Container.sprite = Original;
        }
        
    }
    IEnumerator BlowWait()
    {
        yield return new WaitForSeconds(0.25f);
        SetStapleCount(0);
    }
}


