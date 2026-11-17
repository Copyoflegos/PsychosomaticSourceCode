using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingArm : MonoBehaviour
{
    float SpeedTo;
    Vector3 EndPos;
    Vector3 Direction;
    public GameObject ThrowingHB;
    public LayerMask WallMask;
    public LayerMask FloorMask;
    Animator Anim;
    bool NotPlaying = true;
    PlayerMovement PlayerMove;
    float HitDamage;

    public GameObject HandCircle;
    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        


        if (Vector3.Distance(transform.position, EndPos) > 0.5f)
        {
            transform.position += Direction * SpeedTo;

            if (MasterMind.HitBoxToCast(ThrowingHB, MasterMind.GetPlayerLayer()))  
            {
                PlayerMove.GotHit(new Damage(HitDamage, gameObject));
            }
        }
        else if(NotPlaying)
        {
            //Anim.Play("ArmCD");
            NotPlaying = false;
            if(Physics2D.Linecast(transform.position, transform.position, FloorMask)) // if there is floor then spawn hand kelp
            {
                Instantiate(HandCircle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                EndPos = transform.position + Direction * 20;
            }
            
        }
    }

    public void Destination(float speed, Vector3 finalPos)
    {
        SpeedTo = speed;
        EndPos = finalPos;
        Direction = finalPos - transform.position;
        Direction = Direction.normalized;
    }
    public void SetDamage(float Damage, PlayerMovement PM)
    {
        PlayerMove = PM;
        HitDamage = Damage;
    }
}
