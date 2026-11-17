using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusImage : MonoBehaviour
{
    public bool DEBUG; // will start the image without the need for code

    public bool CircleHB;
    public GameObject HB;

    public LayerMask EnemyMask;

    public float StartDelay = 0; // How many seconds to wait before becoming active
    public int Repeats = 1; // How many frames will the image remain damaging for
    public float InterDelay = 0; // How many seconds in between each frame
    public float DamageOverride = -1; // Use this if you want damage to be set within the prefab editor and not through code
    public float LingerTime = 0; // How long after dealing it's damage will it stick around (for death animations)
    Damage Damage; // The damage this image will deal

    bool Started = false;
    bool MovingImage = false;
    MoveOrder[] Destination;

    float Speed;
    Vector2 Direction;
    int Duration;
    int index = 0;

    private void Awake()
    {
        if (DEBUG)
        {
            InitializeImage();
        }
    }

    private void FixedUpdate()
    {
        if(MovingImage && Started)
        {
            if(Duration <= 0 && index < Destination.Length)
            {
                Direction = Destination[index].Direction;
                Speed = Destination[index].Speed;
                Duration = Destination[index].Duration;
                index++;
            }

            if(Duration > 0)
            {
                transform.position += (Vector3)Direction * Speed;
                Duration--;
            }
            else if (index >= Destination.Length)
            {
                MovingImage = false;
            }



        }
    }

    public void InitializeImage()
    {
        if(DamageOverride > -1)
        {
            Damage = new Damage(DamageOverride, gameObject);
        }

        StartCoroutine(FocusAttack());
    }
    public void InitializeImage(MoveOrder[] Destination)
    {
        this.Destination = Destination;
        MovingImage = true;

        InitializeImage();
    }
    public void InitializeImage(float StartDelay)
    {
        this.StartDelay = StartDelay;
        

        InitializeImage();
    }

    public void InitializeImage(float StartDelay, int Repeats, float InterDelay, Damage Dmg)
    {
        this.StartDelay = StartDelay;
        this.Repeats = Repeats;
        this.InterDelay = InterDelay;
        this.Damage = Dmg;
        DamageOverride = -1;

        InitializeImage();
    }
    public void InitializeImage(float StartDelay, int Repeats, float InterDelay, Damage Dmg, MoveOrder[] Destination)
    {
        this.Destination = Destination;
        MovingImage = true;

        InitializeImage(StartDelay, Repeats, InterDelay, Dmg);
    }

    IEnumerator FocusAttack()
    {
        yield return new WaitForSeconds(StartDelay);

        GetComponent<SpriteRenderer>().enabled = true;
        Started = true;
        // become visible

        if (CircleHB) // circle HB
        {
            for (int i = 0; i < Repeats; i++)
            {
                RaycastHit2D[] AttackCast = Physics2D.CircleCastAll(transform.position, HB.transform.lossyScale.x / 2, transform.up, 0, EnemyMask);

                for (int x = 0; x < AttackCast.Length; x++)
                {
                    AttackCast[x].transform.GetComponentInParent<IEnemy>().GotHit(Damage);
                }

                yield return new WaitForSeconds(InterDelay);
                yield return new WaitForFixedUpdate();
            }
        }
        else // square HB
        {
            for (int i = 0; i < Repeats; i++)
            {
                RaycastHit2D[] AttackCast = MasterMind.HitBoxToCastAll(HB, EnemyMask);

                for (int x = 0; x < AttackCast.Length; x++)
                {
                    AttackCast[x].transform.GetComponentInParent<IEnemy>().GotHit(Damage);
                }

                yield return new WaitForSeconds(InterDelay);
                yield return new WaitForFixedUpdate();
            }
        }

        yield return new WaitForSeconds(LingerTime);
        Destroy(gameObject);

    }
}
