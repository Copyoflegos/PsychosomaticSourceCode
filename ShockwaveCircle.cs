using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveCircle : MonoBehaviour
{
    const float GROWTH_RATE = 1.25f;
    const float LINE_GROWTH = 0.05f;
    const float LINE_WIDTH = 0.5f;
    float CurrentLineWidth;
    float CurrentWidth;
    RaycastHit2D[] CircleCast;
    LayerMask PlayerMask;
    public GameObject InnerCircle;
    public SpriteRenderer[] SR;

    

    void Start()
    {
        PlayerMask = MasterMind.GetPlayerLayer();
        CurrentLineWidth = LINE_WIDTH;
    }
    // Update is called once per frame
    void Update()
    {
        CircleCast = Physics2D.CircleCastAll(transform.position, transform.localScale.x / 2, transform.up, 0, PlayerMask);
        for(int i = 0; i < CircleCast.Length; i++)
        {
            if (Vector2.Distance(CircleCast[i].transform.position,transform.position) > CurrentWidth / 2 - CurrentLineWidth)
            {
                MasterMind.GetPlayer().GetComponent<PlayerMovement>().GotHit(new Damage(35, true, gameObject, DamageType.Projectile));
            }
        }

        if(CurrentWidth > 12)
        {
            SR[0].enabled = false;
            SR[1].enabled = true;
            SR[2].enabled = false;
        }
        if(CurrentWidth > 25)
        {
            SR[0].enabled = false;
            SR[1].enabled = false;
            SR[2].enabled = true;
        }

        if(CurrentWidth > 50)
        {
            Destroy(transform.parent.gameObject);
        }
    }
    private void FixedUpdate()
    {
        transform.localScale = new Vector3(transform.localScale.x + GROWTH_RATE, transform.localScale.y + GROWTH_RATE);
        CurrentWidth = transform.localScale.x;
        CurrentLineWidth += LINE_GROWTH ;
        InnerCircle.transform.localScale = new Vector3(CurrentWidth - CurrentLineWidth, CurrentWidth - CurrentLineWidth);
    }

}
