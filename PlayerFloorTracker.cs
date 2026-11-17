using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloorTracker : MonoBehaviour
{

    public GameObject FloorHB;
    
    public LayerMask FloorMask;

    FloorKey highestFloor;
    

    // Update is called once per frame
    void Update()
    {
        highestFloor = FloorUtilities.GetHighestFloor(MasterMind.HitBoxToCastAll(FloorHB.gameObject, FloorMask));
        
    }

    public FloorKey GetCurrentFloor()
    {
        return highestFloor;
    }
    

}
