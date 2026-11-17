using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    // This class is part of the larger zone checker, it scans for the player and notifies the parent every frame that the player is within them


    public string Identifier; // The name that the parent will be sent
    public int Priority; // If two zones overlap and the player is within one, which should take priority. The higher the better
    RaycastHit2D ZoneCast;
    ZoneChecker ZC;
    LayerMask PlayerLayer;

    private void Start()
    {
        PlayerLayer = MasterMind.GetPlayerLayer();
        ZC = transform.parent.GetComponent<ZoneChecker>();
    }

    private void Update()
    {
        ZoneCast = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0, transform.up, 0, PlayerLayer); // Cast is equal to size of game object (must be square)

        if (ZoneCast)
        {
            ZC.FoundPlayer(Identifier,Priority);
        }
    }
}
