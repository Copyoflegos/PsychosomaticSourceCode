using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodybox : MonoBehaviour
{
    Transform MyBoss;
    
    PolygonCollider2D Box;

    private void Awake()
    {
        MyBoss = transform.parent.parent;
        Box = GetComponent<PolygonCollider2D>();
    }

    void GotHit(Damage Damage)
    {
        MyBoss.SendMessage("GotHit", Damage);
    }
    

    public void BoxesToggle(bool V)
    {
        Box.enabled = V;
    }
}
