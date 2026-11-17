using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHand : MonoBehaviour
{
    public GameObject HB;
    RaycastHit2D[] PushCast;
    public LayerMask EnemyMask;
    public SpriteRenderer Visual;

    bool PushBoxEnabled = false;
    int TimeOut = 0;
    const int MAX_TIME = 15;

    const float SPEED1 = 0.55f;
    const float SPEED2 = 1.25f;
    float currentSpeed = 0;

    private void Awake()
    {
        currentSpeed = SPEED1;
    }
    void Update()
    {
        if (PushBoxEnabled)
        {
            PushCast = Physics2D.BoxCastAll(HB.transform.position, HB.transform.localScale, HB.transform.rotation.eulerAngles.z, HB.transform.up, 0, EnemyMask);
            for (int i = 0; i < PushCast.Length; i++)
            {
                PushCast[i].transform.SendMessage("GotHit", new Damage(0, gameObject, transform.up, currentSpeed, 1));
            }
        }
    }
    private void FixedUpdate()
    {
        if (PushBoxEnabled)
        {
            TimeOut++;

            if(TimeOut > 5)
            {
                currentSpeed = SPEED2;
            }

            if(TimeOut > MAX_TIME)
            {
                Destroy(gameObject);
            }
        }
    }

    public void PullHand(Vector2 MousePos) // for mouse & keyboard
    {
        transform.up = ((Vector3)MousePos - transform.position).normalized;
        PushBoxEnabled = true;
        Visual.enabled = true;
    }
    public void PullHand(Vector3 Direction) // for controller
    {
        transform.up = Direction;
        PushBoxEnabled = true;
        Visual.enabled = true;
    }
    
}
