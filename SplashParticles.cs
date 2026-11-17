using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashParticles : MonoBehaviour
{
    public PlayerAnimation PA;
    Animator Anim;
    int currentLayer = 0;

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Anim.SetInteger("AimDir", (int)PA.GetMoveDir());

        Anim.SetBool("Sliding", false);
        Anim.SetBool("Thrusting", false);
        
    }

    public void PlayerSplash(string Name)
    {
        
        Anim.Play(Name, -1);
    }
    public void AnimationSwitch(FloorKey IndexSet) // Animation Switch layer order is same as aimdir but +1
    {
        currentLayer = (int)IndexSet;
        for (int i = 0; i < Anim.layerCount; i++)
        {
            Anim.SetLayerWeight(i, 0);
        }
        Anim.SetLayerWeight((int)IndexSet + 1, 1);
    }
}
