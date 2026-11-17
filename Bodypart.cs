using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodypart : MonoBehaviour
{
    float Speed;
    int Duration;
    Vector2 Direction;
    string Animation;

    Animator ANIM;

    public LayerMask CollideMask;
    public LayerMask FloorMask;

    public GameObject CollideBox;

    Vector3 TransformAdd;

    private void Awake()
    {
        ANIM = GetComponent<Animator>();   
    }

    private void FixedUpdate()
    {
        if(Duration > 0)
        {
            TransformAdd = (Vector3)Direction * Speed;
            Duration--;
        }
        else
        {
            TransformAdd = Vector2.zero;
            ANIM.Play(Animation + "_STOP", 0);

            if(!MasterMind.HitBoxToCast(CollideBox, FloorMask))
            {
                Destroy(gameObject); // make fall later
            }
        }

        if (!MasterMind.HitBoxToCast(CollideBox, CollideMask))
        {
            transform.position += TransformAdd;
        }
        else
        {
            TransformAdd = Vector2.zero;
            Duration = 0;
        }

    }

    public void BodyPartInfo(float Speed, int Duration, Vector2 Direction, string Animation)
    {
        this.Speed = Speed;
        this.Duration = Duration;
        this.Direction = Direction;
        this.Animation = Animation;
        ANIM.Play(Animation, 0);
    }
}
