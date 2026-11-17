using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunShot : MonoBehaviour
{
    const float SPEED = 0.5f;
    RaycastHit2D HitCast;
    public LayerMask EnemyMask;
    int Time = 0;

    private void Update()
    {
        HitCast = Physics2D.BoxCast(transform.position, transform.localScale, 0, transform.up, 0, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(25, gameObject));
            Destroy(gameObject);
        }
        if(Time > 1000)
        {
            Destroy(gameObject);
        }

        Time++;
    }
    private void FixedUpdate()
    {

        transform.position -= transform.right * SPEED;
    }
}
