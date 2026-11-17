using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaFallingTV : FallingTV
{

    bool destroyedTiles;

    public TileDestroyer TD;
    public GameObject FloorObject;
    // Start is called before the first frame update
    public override void Start()
    {
        //TD.SetBoundsPos( (int)transform.position.x, (int)transform.position.y);

        SetPoss(transform.position.y);

        transform.position += transform.up * 25;

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (stopHurt && !destroyedTiles)
        {
            FloorObject.SetActive(true);
            TD.DestroyTiles();
            destroyedTiles = true;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
