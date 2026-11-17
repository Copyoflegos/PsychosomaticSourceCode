using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KelpTree : MonoBehaviour, IEnemy
{
    private int KelpChildren;
    private const int MAX_CHILDREN = 35;
    private const float MAX_RANGE = 10;
    private const float MAX_CHILD_LOSS = 5;

    float RandomX;
    float RandomY;
    int RandomChild;

    int FlipFlop = 0;
    
    public void RemoveChild()
    {
        if(transform.childCount > 0)
        {
            RandomChild = Random.Range(0, transform.childCount);
            transform.GetChild(RandomChild).GetComponent<HandsCircle>().StartCoroutine("HandDie");
        }
    }

    public void AddChild(GameObject Kelp)
    {
        if(FlipFlop == 0)
        {
            RandomX = Random.Range(0, MAX_RANGE);
            RandomY = Random.Range(0, MAX_RANGE);
            FlipFlop++;
        }
        else if(FlipFlop == 1)
        {
            RandomX = Random.Range(0, MAX_RANGE);
            RandomY = Random.Range(-MAX_RANGE, 0);
            FlipFlop++;
        }
        else if (FlipFlop == 2)
        {
            RandomX = Random.Range(-MAX_RANGE, 0);
            RandomY = Random.Range(-MAX_RANGE, 0);
            FlipFlop++;
        }
        else if (FlipFlop == 3)
        {
            RandomX = Random.Range(-MAX_RANGE, 0);
            RandomY = Random.Range(0, MAX_RANGE);
            FlipFlop = 0;
        }
       
        Instantiate(Kelp, new Vector2(transform.position.x + RandomX, transform.position.y + RandomY), Quaternion.identity, transform);
    }

    public bool RoomForChildren()
    {
        return KelpChildren < MAX_CHILDREN;
    }

    public bool GetStunned()
    {
        return false;
    }
    public bool GetFlinchable()
    {
        return false;
    }

    public void GotHit(Damage Damage)
    {
        for(int i = 0; i < Damage.DmgAmount / 25 && i < MAX_CHILD_LOSS; i++)
        {
            RemoveChild();
        }
    }
    public void GotStapled(int num)
    {
        
    }
    public void GotBleed(int num, float damage)
    {
        
    }
    public void GotBurned(int num)
    {
        for (int i = 0; i < num && i < MAX_CHILD_LOSS; i++)
        {
            RemoveChild();
        }
    }
}
