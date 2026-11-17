using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalSectionPad : MonoBehaviour
{
    public Vector2 Position;
    

    public Vector2 CameraOffset;
    public float CameraSize;

    public GameObject HB;
    public LayerMask PlayerMask;
    RaycastHit2D PlayerCast;
    // Update is called once per frame
    void Update()
    {
        PlayerCast = MasterMind.HitBoxToCast(HB, PlayerMask);

        if (PlayerCast)
        {
            PlayerCast.transform.position = Position;
            Camera.main.GetComponent<CameraControl>().SetOffset(CameraOffset);
            Camera.main.GetComponent<CameraControl>().SetSize(CameraSize);
        }
    }
}
