using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarBomb : MonoBehaviour
{
    Damage Dmg;
    
    Vector2 BasePos; // where the object was spawned
    Vector2 LandPos; // where it will land
    Vector2 AirPos;
    Vector3 FlightDir;
    

    int frameCount = 0;
    bool Landed = false;
    bool Intercepted = false;
    bool Exploded = false;

    const float FLIGHT_SPEED = 0.8f;

    public Sprite Explosion;
    public Sprite Falling;
    public GameObject ExplosionHB;
    public LayerMask EnemyMask;

    SpriteRenderer SR;

    // Start is called before the first frame update
    void Awake()
    {
        BasePos = transform.position;
        LandPos = new Vector2(BasePos.x, BasePos.y - 1);
        AirPos = new Vector2(BasePos.x, BasePos.y + 6);
        Dmg = new Damage(10, gameObject);

        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        frameCount++;
        if (!Intercepted)
        {
            if (frameCount < 5)
            {
                transform.position += new Vector3(0, 0.8f);
            }
            else if (frameCount < 10)
            {
                transform.position += new Vector3(0, 0.5f);
            }
            else if (frameCount < 20)
            {
                transform.position += new Vector3(0, 0.05f);
            }
            else if (frameCount < 30)
            {

            }
            else if (frameCount < 35)
            {
                SR.sprite = Falling;
                transform.position += new Vector3(0, -0.05f);
            }
            else if (frameCount < 40)
            {
                transform.position += new Vector3(0, -0.1f);
            }
            else if (frameCount < 45)
            {
                transform.position += new Vector3(0, -0.3f);
            }
            else if (frameCount < 51)
            {
                transform.position += new Vector3(0, -0.8f);
            }
            else if (frameCount == 51)
            {
                SR.sprite = Explosion;
                Landed = true;
                RaycastHit2D[] ExpCast = Physics2D.BoxCastAll(ExplosionHB.transform.position, ExplosionHB.transform.localScale, ExplosionHB.transform.rotation.eulerAngles.z, transform.up, 0);

                for (int i = 0; i < ExpCast.Length; i++)
                {
                    ExpCast[i].transform.SendMessage("GotHit", Dmg, SendMessageOptions.DontRequireReceiver);
                }

            }
            else if (frameCount == 55)
            {
                Destroy(gameObject);
            }
        }
        else if (Intercepted && !Exploded)
        {
            transform.position += FlightDir * FLIGHT_SPEED;

            RaycastHit2D bombHurt = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, transform.up, 0, EnemyMask);
            if (bombHurt)
            {
                SR.sprite = Explosion;
                Landed = true;
                RaycastHit2D[] ExpCast = Physics2D.BoxCastAll(ExplosionHB.transform.position, ExplosionHB.transform.localScale, ExplosionHB.transform.rotation.eulerAngles.z, transform.up, 0);

                for (int i = 0; i < ExpCast.Length; i++)
                {
                    ExpCast[i].transform.SendMessage("GotHit", Dmg, SendMessageOptions.DontRequireReceiver);
                }
                Exploded = true;
                StartCoroutine(DestroyWait());
                
            }
        }

        if(frameCount > 200)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    public void GotHit(Damage dmg)
    {
        if (frameCount >= 48 && !Landed) // if the bomb is coming down
        {
            Intercepted = true;
            FlightDir = (transform.position - dmg.Source.transform.position).normalized;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void GotBleed(int dur, float bleed)
    {
        //nothing
    }
    public void GotStapled(int num)
    {

    }
}
