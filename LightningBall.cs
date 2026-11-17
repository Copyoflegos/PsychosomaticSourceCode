using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : MonoBehaviour
{
    Animator ANIM;
    private void Awake()
    {
        ANIM = GetComponent<Animator>();
    }
    public void Activate() // make it animate instead
    {

        StartCoroutine(LightningWait());
    }
    IEnumerator LightningWait()
    {
        ANIM.Play("LightningFlash");
        for(int i =0; i < 35; i++)
            yield return null;
        Destroy(gameObject);
    }
}
