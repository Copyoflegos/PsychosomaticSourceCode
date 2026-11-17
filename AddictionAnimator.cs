using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionAnimator : MonoBehaviour
{
    public GameObject AddictionMain;
    
    
    SpriteRenderer SR;
    Animator ANIM;
    SpriteMask SM;

    bool SetLockToAdd = true;
    int FaceDir = 0; // 0 Down, 1 Left, 2 Up, 3 Right

    

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        ANIM = GetComponent<Animator>();
        SM = GetComponent<SpriteMask>();
    }

    private void Update()
    {
        if(SetLockToAdd)
            transform.position = AddictionMain.transform.position;

        


        
        switch (FaceDir)
        {
            case 0:
                AnimationSwitch(1);
                break;
            case 1:
                AnimationSwitch(2);
                break;
            case 2:
                AnimationSwitch(3);
                break;
            case 3:
                AnimationSwitch(4);
                break;
        }
        
        
        
    }

    public void SetFaceDir(int Direction)
    {
        switch (FaceDir) // The sort of switch statement from your N I G H T M A R E S
        {
            case 0:
                switch (Direction)
                {
                    case 1:
                        //PlayAnimation("AddIdleDL");
                        break;
                    case 3:
                        //PlayAnimation("AddIdleDR");
                        break;
                    
                }
                break;
            case 1:
                switch (Direction)
                {
                    case 0:
                        //PlayAnimation("AddIdleDL");
                        break;
                    case 2:
                        //PlayAnimation("AddIdleUL");
                        break;
                    
                }
                break;
            case 2:
                switch (Direction)
                {
                    case 1:
                        //PlayAnimation("AddIdleUL");
                        break;
                    case 3:
                        //PlayAnimation("AddIdleUR");
                        break;
                }
                break;
            case 3:
                switch (Direction)
                {
                    case 2:
                        //PlayAnimation("AddIdleUR");
                        break;
                    case 0:
                        //PlayAnimation("AddIdleDR");
                        break;
                }
                break;


        }

        
        FaceDir = Direction;
    }
    public int GetFaceDir()
    {
        return FaceDir;
    }

    void AnimationSwitch(int IndexSet)
    {

        for (int i = 0; i < ANIM.layerCount; i++)
        {
            ANIM.SetLayerWeight(i, 0);
        }
        ANIM.SetLayerWeight(IndexSet, 1);
    }

    public void PlayAnimation(string AnimationName)
    {

        ANIM.Play(AnimationName, -1, 0);
    }
    public void PlayAnimation(string AnimationName, float time)
    {


        ANIM.Play(AnimationName,-1, time);
    }

    public void TakeDmg(bool Resist)
    {
        if (Resist)
        {
            StartCoroutine(ResistDamageFlash());
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
        
    }
    public void VulnerableStun()
    {
        StartCoroutine(VulnerableFlash());
    }
    public void SetLock(bool Value)
    {
        SetLockToAdd = Value;
    }

    public void SetPosition(Vector3 Value)
    {
        if(SetLockToAdd == false)
        {
            transform.position = Value;
        }
    }

    public void GiveDegree(float Degree)
    {
        AddictionMain.GetComponent<Addiction>().PoleMove(Degree);
    }

   public void WaitForAnimationEvent()
    {
        AddictionMain.GetComponent<Addiction>().WaitForAnimationEvent();
    }

    IEnumerator DamageFlash()
    {
        SR.color = Color.red;
        
        yield return new WaitForSeconds(0.2f);
        
        SR.color = Color.white;
    }
    IEnumerator ResistDamageFlash()
    {
        SR.color = Color.blue;

        yield return new WaitForSeconds(0.2f);

        SR.color = Color.white;
    }
    IEnumerator VulnerableFlash()
    {
        SR.color = Color.magenta;

        yield return new WaitForSeconds(0.2f);

        SR.color = Color.white;
    }
    public void SetSpriteMask(Sprite SpriteSet)
    {
        SM.sprite = SpriteSet;
    }
}
