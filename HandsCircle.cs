using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsCircle : MonoBehaviour
{
    public GameObject HB;
    public LayerMask PlayerMask;
    public LayerMask CircleMask;
    public LayerMask FloorMask;
    RaycastHit2D playerCast;

    int Counter = 0;
    
    int TimeCount = 0;

    GameObject Player;



    public bool InvincibleKelp;
    public Animator HandAnim;
    Animator CircleAnim;
    public GameObject LittleHand;
    
    GameObject[] LittleHandArray = new GameObject[4];

    

    AudioSource AS;

    const int MAX_COUNT = 150;

    bool Grabbed;
    bool LetGo;
    bool Dead;
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D OverlapCast = Physics2D.CircleCast(
            HB.transform.position,
            HB.transform.lossyScale.x / 2,
            HB.transform.up,
            0, CircleMask);
        
        

        RaycastHit2D FloorCast = Physics2D.CircleCast(
            HB.transform.position,
            HB.transform.lossyScale.x / 2,
            HB.transform.up,
            0, FloorMask);

        if (!FloorCast)
        {
            Destroy(gameObject);
        }

        Player = MasterMind.GetPlayer();
        
        CircleAnim = GetComponent<Animator>();
        GetComponent<CircleCollider2D>().enabled = true;
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float HBRadius = HB.transform.lossyScale.x / 2;
        playerCast = Physics2D.CircleCast(
            HB.transform.position,
            HBRadius,
            HB.transform.up,
            0, PlayerMask);

        HBRadius -= 2;

        if (Counter > MAX_COUNT / 5 * 4)
        {
            
            CircleAnim.Play("CircleDie");
            if (LittleHandArray[3] == null)
                LittleHandArray[3] = Instantiate(LittleHand, transform.position + new Vector3(Random.Range(0, HBRadius), -Random.Range(0, HBRadius)), Quaternion.identity, transform);
        }
        else if (Counter > MAX_COUNT / 5 * 3)
        {
            CircleAnim.Play("HandCircleIdle 4");
            if (LittleHandArray[2] == null)
                LittleHandArray[2] = Instantiate(LittleHand, transform.position + new Vector3(-Random.Range(0, HBRadius), -Random.Range(0, HBRadius)), Quaternion.identity, transform);
            if (LittleHandArray[3] != null)
            {
                Destroy(LittleHandArray[3]);
            }
        }
        else if (Counter > MAX_COUNT / 5 * 2)
        {
            CircleAnim.Play("HandCircleIdle 3");
            if (LittleHandArray[1] == null)
                LittleHandArray[1] = Instantiate(LittleHand, transform.position + new Vector3(-Random.Range(0, HBRadius), Random.Range(0, HBRadius)), Quaternion.identity, transform);
            if (LittleHandArray[2] != null)
            {
                Destroy(LittleHandArray[2]);
            }
        }
        else if (Counter > MAX_COUNT / 5 )
        {
            CircleAnim.Play("HandCircleIdle 2");
            if (LittleHandArray[0] == null)
                LittleHandArray[0]  = Instantiate(LittleHand, transform.position + new Vector3(Random.Range(0, HBRadius), Random.Range(0, HBRadius)), Quaternion.identity, transform);
            if(LittleHandArray[1] != null)
            {
                Destroy(LittleHandArray[1]);
            }
            
        }
        else
        {
            CircleAnim.Play("HandCircleIdle 1");
            if (LittleHandArray[0] != null)
            {
                Destroy(LittleHandArray[0]);
            }
        }
    }

    private void FixedUpdate()
    {
        TimeCount++;
        if (playerCast)
        {
            Counter++;
            TimeCount = 0;
        }
        else
        {
            Counter--;
        }
        if(Counter < 0)
        {
            Counter = 0;
        }


        if (Counter > MAX_COUNT && !Grabbed && !Dead)
        {
            AS.Play();
            Grabbed = Player.GetComponent<PlayerMovement>().UnlockGrabbed(transform.position, gameObject);
            
            if (Grabbed)
            {
                HandAnim.Play("HandGrab");
                
                TimeCount = 0;
                StartCoroutine(HandAnimationWait());
                GetComponent<CircleCollider2D>().enabled = false; 
            }
            if (!Grabbed)
            {
                Counter = 0;
                StartCoroutine(HandDie());
            }
        }

        

        
        if(!InvincibleKelp && TimeCount > 1050)
        {
            StartCoroutine(HandDie());
        }
    }

    void LetGoGrab()
    {
        Player.GetComponent<PlayerMovement>().LetGo();
        LetGo = true;
        StartCoroutine(HandDie());
    }

    IEnumerator HandAnimationWait()
    {
        yield return new WaitForSeconds(0.167f);
        HandAnim.Play("HandCaught");
        yield return new WaitForSeconds(3);
        if (!LetGo)
        {
            Player.GetComponent<PlayerMovement>().LetGo();
            StartCoroutine(HandDie());
        }
        LetGo = false;
    }
    IEnumerator HandDie()
    {
        if (!InvincibleKelp)
        {
            Dead = true;
            GetComponent<CircleCollider2D>().enabled = false;
            HandAnim.Play("HandDeath");
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
        else
        {
            Grabbed = false;
            
            Counter = 0;
            CircleAnim.Play("HandCircleIdle 1");
            HandAnim.Play("HandIdle");
            for(int i = 0; i < LittleHandArray.Length; i++)
            {
                if(LittleHandArray[i] != null)
                {
                    Destroy(LittleHandArray[i]);
                }
            }
        }
        
    }
}
