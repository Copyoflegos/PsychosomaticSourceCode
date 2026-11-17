using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOcclusion : MonoBehaviour
{
    // 7, 3, 2, 1 basic values
    SpriteRenderer SR;
    public int HigherOrder; // The higher sort order that the enemy will switch to when the tolerance level is met
    public int LowerOrder;
    public float AlphaTolerance; // How close does player have to be for this enemy to turn transparent
    public float SortTolerance; // How high above does player have to be for sort layer to switch and transparency to apply (smaller for smaller sprites)
    public bool Transparency;

    bool Locked;
    int LockTo = 0;
    // Start is called before the first frame update
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Locked)
        {
            if (MasterMind.GetPlayer().transform.position.y + SortTolerance > transform.position.y)
            {
                SR.sortingOrder = HigherOrder;

                if (Vector3.Distance(MasterMind.GetPlayer().transform.position, transform.position) < AlphaTolerance && Transparency)
                {
                    SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 0.9f);
                }
                else
                {
                    SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1);
                }
            }
            else
            {
                SR.sortingOrder = LowerOrder;
                SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1);
            }
        }
        else
        {
            SR.sortingOrder = LockTo;
        }
    }


    public void LockOcclusion(int OrderLock)
    {
        Locked = true;
        LockTo = OrderLock;
    }
    public void UnlockOcclusion()
    {
        Locked = false;
    }
}
