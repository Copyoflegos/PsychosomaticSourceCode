using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaBook : MonoBehaviour
{
    public GameObject Ripple;
    public LayerMask EnemyMask;
    bool Landed;
    bool Intercepted;
    Vector3 FlightDir;
    const float FLIGHT_SPEED = 0.4f;
    Vector2 StartPos;
    SpriteRenderer SR;
    float LandingOffset = 1.65f; // Vertical Offset
    float ArcOffset = 0; // Horizontal Offset (magnitude)
    int ArcSign = 1; // Horizontal Direction (direction)

    const float UP_OFFSET = 2f;

    int FrameCountDown = 12;
    int FrameCountFloat = 8;
    // Start is called before the first frame update
    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        StartPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Intercepted)
        {
            if (FrameCountDown > 0)
            {
                transform.position += new Vector3(0, 0.04f);
                FrameCountDown--;

            }
            else if (FrameCountFloat > 0)
            {
                FrameCountFloat--;
            }
            else if (transform.position.y - (StartPos.y - LandingOffset) > 0.2f)
            {
                transform.position += new Vector3(0, -0.07f);
            }
            else if (Landed == false)
            {
                if(MasterMind.GetWaterLevel())
                    Instantiate(Ripple, transform.position, Quaternion.identity);
                Landed = true;
            }

            if (Mathf.Abs(transform.position.x - (StartPos.x + ArcOffset)) > 0.2f)
            {
                transform.position += new Vector3(0.035f * ArcSign, 0);
            }
        }
        else
        {
            transform.position += FlightDir * FLIGHT_SPEED;

            RaycastHit2D bookHurt = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, transform.up, 0, EnemyMask);
            if (bookHurt)
            {
                bookHurt.transform.GetComponentInParent<IEnemy>().GotHit(new Damage(30, gameObject, true));
            }

            FrameCountDown++;
            if(FrameCountDown > 700) // timeout
            {
                Destroy(gameObject);
            }
        }
        

        
        
    }

    public void SetDirection(AimDirKey ADK)
    {
        switch (ADK)
        {
            case AimDirKey.Down:
                break;
            case AimDirKey.Left:
                //move right
                ArcOffset = 1.5f;
                ArcSign = 1;
                break;
            case AimDirKey.Up:
                // fix sprite layer to be in front of therapist and down
                transform.position += new Vector3(0, -0.45f, 0);
                SR.sortingOrder = 6;
                LandingOffset = UP_OFFSET;
                break;
            case AimDirKey.Right:
                // move left
                ArcOffset = -1.5f;
                ArcSign = -1;
                break;
            case AimDirKey.DownRight:
                // move left and up
                transform.position += new Vector3(0, 0.05f, 0);
                ArcOffset = -0.9f;
                ArcSign = -1;
                FrameCountFloat = 4;
                break;
            case AimDirKey.DownLeft:
                //right and up
                transform.position += new Vector3(0, 0.05f, 0);
                ArcOffset = 0.9f;
                ArcSign = 1;
                FrameCountFloat = 4;
                break;
            case AimDirKey.UpRight:
                // move left and down and sprite
                transform.position += new Vector3(0,-0.05f,0);
                ArcOffset = -0.9f;
                ArcSign = -1;
                SR.sortingOrder = 6;
                FrameCountFloat = 4;
                LandingOffset = UP_OFFSET;
                break;
            case AimDirKey.UpLeft:
                // right and down and sprite
                transform.position += new Vector3(0, -0.05f, 0);
                ArcOffset = 0.9f;
                ArcSign = 1;
                SR.sortingOrder = 6;
                FrameCountFloat = 4;
                LandingOffset = UP_OFFSET;
                break;
        }
    }

    public void GotHit(Damage Damage)
    {
        if (FrameCountDown <= 0 && !Landed && !Intercepted) // if the book is coming down
        {
            Intercepted = true;
            FlightDir =  (transform.position - Damage.Source.transform.position).normalized;
        }
    }
    public void GotBleed(int dur, float dmg)
    {
        //nothing
    }
    public void GotStapled(int num)
    {

    }
}
