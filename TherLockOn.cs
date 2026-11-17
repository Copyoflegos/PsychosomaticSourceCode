using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TherLockOn : MonoBehaviour
{
    Animator Anim;
    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    public void NewLockOn()
    {
        Anim.Play("LockOn");

    }
}
