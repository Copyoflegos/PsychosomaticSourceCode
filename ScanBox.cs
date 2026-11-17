using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanBox : MonoBehaviour
{
    /*
     * ScanBox Script
     * 
     * Function:
     *    This script constantly checks if the player is in contact with BoxCast,
     *    and if so, it sets check to true, and when they leave, it will be false
     *    immediately
     *    
     * Notes:
     *    The size can be scaled and changed however for same effect
     * 
     */

    [HideInInspector]
    public bool check; //Whether collided or not


    public int identifier; // The number that the parent will be sent
    
    RaycastHit2D ZoneCast;
    IScanBoxController SBC; //ScanboxController
    LayerMask PlayerLayer;

    private void Start()
    {
        PlayerLayer = MasterMind.GetPlayerLayer();
        
        SBC = transform.GetComponentInParent<IScanBoxController>();
    }

    private void Update()
    {
        ZoneCast = MasterMind.HitBoxToCast(gameObject, PlayerLayer);

        if (ZoneCast)
        {
            check = true;
            SBC.FoundPlayer(identifier);
        }
        else
        {
            check = false;
            SBC.LostPlayer(identifier);
        }

    }

}
