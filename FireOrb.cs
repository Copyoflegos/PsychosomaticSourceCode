using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrb : MonoBehaviour
{
    public LayerMask PlayerMask;
    public GameObject HitBox;
    RaycastHit2D PlayerCast;

    int TimeOut = 0;
    const int MAX_TIME = 100;
    // Update is called once per frame
    void Update()
    {
        PlayerCast = Physics2D.BoxCast(HitBox.transform.position, HitBox.transform.localScale, 0, transform.up, 0, PlayerMask);
        if (PlayerCast)
        {
            PlayerCast.transform.GetComponent<PlayerMovement>().PickUpFlameOrb();
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        TimeOut++;
        if(TimeOut > MAX_TIME)
        {
            Destroy(gameObject);
        }
    }
}
