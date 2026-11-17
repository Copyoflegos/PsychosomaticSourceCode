using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCaster : MonoBehaviour
{
    public GameObject HB;
    public LayerMask PlayerMask;
    RaycastHit2D MemoryCast;
    public UnityEvent[] Events;

    public bool DisableOnActivate;
    bool FlipFlop = false;
    // Update is called once per frame
    void Update()
    {
        MemoryCast = Physics2D.BoxCast(HB.transform.position, HB.transform.localScale, 0, transform.up, 0, PlayerMask);
        if (MemoryCast && !FlipFlop)
        {
            for(int i = 0; i < Events.Length; i++)
            {
                Events[i].Invoke();
            }
            if (DisableOnActivate)
            {
                gameObject.SetActive(false);
            }
            FlipFlop = true;
        }
        else if(!MemoryCast && FlipFlop)
        {
            FlipFlop = false;
        }
    }

    
}
