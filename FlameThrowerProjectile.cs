using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerProjectile : MonoBehaviour
{
    const float GROWTH_RATE_X_1 = 0.10f;
    const float GROWTH_RATE_Y_1 = 0.20f;
    const float GROWTH_RATE_X_2 = 0.12f;
    const float GROWTH_RATE_Y_2 = 0.24f;
    const float GROWTH_RATE_X_3 = 0.075f;
    const float GROWTH_RATE_Y_3 = 0.15f;
    const float SPEED_1 = 0.45f;
    const float SPEED_2 = 0.35f;
    const float SPEED_3 = 0.25f;
    const int DIE_TIME = 40;
    const int END_TIME = 25;
    const int MID_TIME = 5;

    float timePassed = 0;

    public LayerMask EnemyMask;
    public Sprite[] Sprite_Array;
    public SpriteRenderer SR;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        RaycastHit2D[] AttackCast = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.rotation.eulerAngles.z, transform.up, 0, EnemyMask);
        for (int i = 0; i < AttackCast.Length; i++)
        {
            AttackCast[i].transform.GetComponentInParent<IEnemy>().GotHit(new Damage(5, 10, gameObject));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timePassed > END_TIME)
        {
            //if(timePassed == END_TIME + 1)
            //{
            //    transform.localScale = new Vector3(transform.localScale.x / 1.25f, transform.localScale.y / 1.25f);
            //}
            transform.localScale = new Vector3(transform.localScale.x + GROWTH_RATE_X_3, transform.localScale.y + GROWTH_RATE_Y_3);
            //SR.sprite = Sprite_Array[2];
            transform.position += transform.up * SPEED_3;
            if (timePassed > DIE_TIME)
            {
                Destroy(gameObject);
            }
        }
        else if(timePassed > MID_TIME)
        {
            transform.position += transform.up * SPEED_2;

            transform.localScale = new Vector3(transform.localScale.x + GROWTH_RATE_X_2, transform.localScale.y + GROWTH_RATE_Y_2);
            SR.sprite = Sprite_Array[1];
        }
        else
        {
            transform.position += transform.up * SPEED_1;

            transform.localScale = new Vector3(transform.localScale.x + GROWTH_RATE_X_1, transform.localScale.y + GROWTH_RATE_Y_1);
            SR.sprite = Sprite_Array[0];
        }

        timePassed++;
    }
}
