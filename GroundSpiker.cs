using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpiker : MonoBehaviour
{
    public GameObject SmallHB1;
    public GameObject SmallHB2;
    public GameObject BigHB1;
    public GameObject BigHB2;

    public LayerMask EnemyLayer;

    RaycastHit2D[] SpikeCast; 

    const float SPEED = 0.125f; // constant speed as it moves up
    const int TIME_OUT = 600;
    int timer = 0;
    const int SPIKE_RATIO = 15;
    int spikeCount = 0;
    
    private void FixedUpdate()
    {
        timer++;
        if(timer > TIME_OUT)
        {
            Destroy(gameObject);
        }

        if(timer % SPIKE_RATIO == 0)
        {
            if(spikeCount == 0)
            {
                StartCoroutine(SpikeFlash(SmallHB1.GetComponent<SpriteRenderer>()));
                SpikeCast = Physics2D.BoxCastAll(SmallHB1.transform.position, SmallHB1.transform.localScale, 0, transform.up, 0, EnemyLayer);
                for(int i = 0; i < SpikeCast.Length; i++)
                {
                    SpikeCast[i].transform.SendMessage("GotHit", new Damage(25, gameObject));
                }
            }
            else if(spikeCount == 1)
            {
                StartCoroutine(SpikeFlash(SmallHB2.GetComponent<SpriteRenderer>()));
                SpikeCast = Physics2D.BoxCastAll(SmallHB2.transform.position, SmallHB2.transform.localScale, 0, transform.up, 0, EnemyLayer);
                for (int i = 0; i < SpikeCast.Length; i++)
                {
                    SpikeCast[i].transform.SendMessage("GotHit", new Damage(25, gameObject));
                }
            }
            else if(spikeCount % 2 == 0)
            {
                StartCoroutine(SpikeFlash(BigHB1.GetComponent<SpriteRenderer>()));
                SpikeCast = Physics2D.BoxCastAll(BigHB1.transform.position, BigHB1.transform.localScale, 0, transform.up, 0, EnemyLayer);
                for (int i = 0; i < SpikeCast.Length; i++)
                {
                    SpikeCast[i].transform.SendMessage("GotHit", new Damage(95, 60, gameObject));
                }
            }
            else
            {
                StartCoroutine(SpikeFlash(BigHB2.GetComponent<SpriteRenderer>()));
                SpikeCast = Physics2D.BoxCastAll(BigHB2.transform.position, BigHB2.transform.localScale, 0, transform.up, 0, EnemyLayer);
                for (int i = 0; i < SpikeCast.Length; i++)
                {
                    SpikeCast[i].transform.SendMessage("GotHit", new Damage(95, 60, gameObject));
                }
            }
            spikeCount++;
        }
        transform.position += transform.up * SPEED;
    }

    IEnumerator SpikeFlash(SpriteRenderer SR) // debug
    {
        SR.enabled = true;
        yield return new WaitForSeconds(0.25f);
        SR.enabled = false;
    }
}
