using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMuncher : MonoBehaviour
{
    public TileDestroyer TD;
    public GameObject HB;
    public GameObject PitFloor;
    public LayerMask TileMask;
    public LayerMask PlayerMask;

    public float Speed;
    public float AliveTime;

    public GameObject MucherSprite;

    RaycastHit2D[] EatCast;
    RaycastHit2D PlayerCast;
    RaycastHit2D LeftTurnCast;
    RaycastHit2D RightTurnCast;

    int deathTimer = 0;
    float RealTimer = 0;

    public bool NewTiles;
    public bool NoTurn;

    private void FixedUpdate()
    {
        
        transform.position += transform.right * Speed;
        PitFloor.transform.position += transform.right * Speed;
        MucherSprite.transform.position = transform.position;
        MucherSprite.transform.rotation = transform.rotation;

        if (!NewTiles)
        {
            EatCast = Physics2D.BoxCastAll(HB.transform.position, HB.transform.localScale, HB.transform.rotation.eulerAngles.z, transform.right, 0, TileMask);

            if (EatCast.Length <= 0)
            {
                deathTimer++;
            }

            for (int i = 0; i < EatCast.Length; i++)
            {

                EatCast[i].transform.gameObject.SetActive(false);
                deathTimer = 0;
            }

            if (deathTimer > 100)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            RealTimer += Time.deltaTime;

            TD.DestroyTiles();

            if (RealTimer > AliveTime)
            {
                gameObject.SetActive(false);
            }
        }

        
        

        PlayerCast = Physics2D.BoxCast(HB.transform.position, HB.transform.localScale, HB.transform.rotation.eulerAngles.z, transform.right, 1, PlayerMask);
        if (PlayerCast)
        {
            PlayerCast.transform.GetComponent<PlayerMovement>().GotHit(new Damage(75, gameObject, transform.right, 0.75f, 20));
        }

        if (!NoTurn)
        {
            float HBX = HB.transform.position.x; float HBY = HB.transform.position.y;

            Vector2 StartPosL = new Vector3(HBX, HBY) + (transform.up * HB.transform.localScale.y) - transform.right * HB.transform.localScale.y / 2;
            Vector2 StartPosR = new Vector3(HBX, HBY) - (transform.up * HB.transform.localScale.y) - transform.right * HB.transform.localScale.y / 2;


            LeftTurnCast = Physics2D.Linecast(StartPosL, StartPosL, TileMask);
            RightTurnCast = Physics2D.Linecast(StartPosR, StartPosR, TileMask);

            if (LeftTurnCast)
            {
                transform.right = transform.up;
                transform.position -= transform.right * 2;
            }


            if (RightTurnCast)
            {
                transform.right = -transform.up;
                transform.position -= transform.right * 2;
            }
        }
            
    }
}
