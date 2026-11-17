using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoomCDSpot : MonoBehaviour
{
    LayerMask PlayerMask;
    bool HitPlayer;
    RaycastHit2D PlayerCast;
    // Start is called before the first frame update
    void Start()
    {
        PlayerMask = MasterMind.GetPlayerLayer();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerCast = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, transform.up, 0, PlayerMask);

        if(PlayerCast && !HitPlayer)
        {
            PlayerCast.transform.GetComponent<PlayerMovement>().RefreshCooldowns();
            PlayerCast.transform.GetComponent<PlayerMovement>().RefreshBurst();
            PlayerCast.transform.GetComponent<PlayerMovement>().ClearConcentration();
            HitPlayer = true;
        }
        else if (!PlayerCast)
        {
            HitPlayer = false;
        }
    }
}
