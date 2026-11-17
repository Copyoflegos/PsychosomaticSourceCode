using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFloor : MonoBehaviour
{
    public float Damage;
    public int Time;
    int timeTick = 0;
    RaycastHit2D PlayerCast;
    RaycastHit2D[] FloorCast;
    public LayerMask FloorMask;
    public GameObject HB;

    bool Dying = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Dying)
        {
            PlayerCast = MasterMind.HitBoxToCast(HB, MasterMind.GetPlayerLayer());
            if (PlayerCast)
            {
                PlayerCast.transform.GetComponent<PlayerMovement>().GotHit(new Damage(5, true, gameObject));
            }
        }
        
        FloorCast = MasterMind.HitBoxToCastAll(gameObject, FloorMask);
        if (FloorUtilities.GetHighestFloor(FloorCast) == FloorKey.Pit)
        {
            Destroy(gameObject);
        }


        if (Time != 0)
        {
            timeTick++;
        }
        if(Time == timeTick)
        {

            StartCoroutine(DeathWait());
        }
    }

    public void SetTime(int TimeSet)
    {
        if(TimeSet == -1)
        {
            Time = 0;
        }
        else
        {
            Time = TimeSet;
        }
    }

    IEnumerator DeathWait()
    {
        Dying = true;
        GetComponent<Animator>().Play("StaticDeath");
        yield return new WaitForSeconds(0.417f);
        Destroy(gameObject);
    }
}
