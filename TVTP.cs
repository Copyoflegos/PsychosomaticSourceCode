using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVTP : MonoBehaviour
{


    GameObject SiblingTV;
    AimDirKey FaceDirection;
    public Sprite[] CourageTheCowardlyDog; // sprites for tv facing direction 
    SpriteRenderer SR;
    GameObject Player;
    
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        
    }
    private void Start()
    {
        Player = MasterMind.GetPlayer();
    }



    public void InstantiationMethod(AimDirKey FacingDirection, Vector2 Destination)
    {
        FaceDirection = FacingDirection;
        transform.position = Destination;

        SR.sprite = CourageTheCowardlyDog[(int)FacingDirection];
        
    }
    public void SetSibling(GameObject OtherTV)
    {
        SiblingTV = OtherTV;
        // change sprite to glow
    }
    public void TpSiblingCheck(AimDirKey DirCheck) // sent by player to tv, if tv has a sibling and the direction is lined up, then tp player and destroy tv's
    {
        if(DirCheck == FaceDirection)
        {
            if(SiblingTV != null)
            {
                Player.transform.position = SiblingTV.transform.position;
                Destroy(SiblingTV);
                Destroy(gameObject);
            }
        }
    }
    public void ProjectileSiblingCheck(AimDirKey DirCheck, GameObject Projectile)
    {
        if(DirCheck == FaceDirection)
        {
            if(SiblingTV != null)
            {
                Projectile.transform.position = SiblingTV.transform.position;
                Projectile.SendMessage("ChangeDirection", SiblingTV.GetComponent<TVTP>().GetDirection());
                Projectile.SendMessage("Upgrade");
            }
        }
    }
    public AimDirKey GetDirection()
    {
        return FaceDirection;
    }
}
