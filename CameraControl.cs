using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 refVel;
    Vector3 FocusPoint;
    public Vector3 BaseFocusPos; // used in case player is not assigned, I guess?
    public GameObject player; // Temporary
    GameObject FocusObject;
    public Vector3 OffSet; // offset to apply to camera
    public bool Static;
    public bool Dampen;
    Camera CAM;
    float FollowSpeed;
    const float START_SPEED = 0.28f;
    
    public float NormalSize;
    float IdealSize;
    // Start is called before the first frame update
    void Start()
    {
        FocusPoint = BaseFocusPos;
        CAM = GetComponent<Camera>();
        FollowSpeed = START_SPEED;
        FocusObject = player;
        IdealSize = NormalSize;
    }

    
    private void Update()
    {
        FocusPoint = FocusObject.transform.position + OffSet;
    }
    void FixedUpdate()
    {
        if (!Static)
        {
            if (Dampen)
                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(FocusPoint.x, FocusPoint.y, -10), ref refVel, FollowSpeed);
            else
            {
                transform.position = new Vector3(FocusPoint.x, FocusPoint.y, -10);
            }
        }
        CAM.orthographicSize = IdealSize;

        //if (IdealSize > CAM.orthographicSize)
        //    CAM.orthographicSize += 0.01f;
        //if (IdealSize < CAM.orthographicSize)
        //    CAM.orthographicSize -= 0.01f;
    }

    public void Shake(float Amount, float Repeats) // Amount is severity and Repeats is length (0.2f is a great amount of shake) and (2 repeats is a decent length)
    {
        
        StartCoroutine(ShakeIt(Repeats, Amount));

    }
    

    IEnumerator ShakeIt(float Repeats, float Amount)
    {
        for (int i = 0; i < Repeats; i++)
        {
            float ShakeX = Random.value * Amount * 2 - Amount;
            float ShakeY = Random.value * Amount * 2 - Amount;
            transform.position += new Vector3(ShakeX, ShakeY, 0);
            yield return null;
        }
    }
    

    public void StopShaking()
    {
        StopAllCoroutines();
    }

    

    
    public void DampenSet(bool SetTo) // a dampened camera does not snap to object
    {
        Dampen = SetTo;
    }

    public void SetSpeedTo(float ToSet) // lower is faster
    {
        FollowSpeed = ToSet;
    }
    public void SetSpeedTo() // overload that sets speed to default speed
    {
        FollowSpeed = START_SPEED;
    }

    public void SetSize(float Size)
    {
        IdealSize = Size;
    }

    public void ResetSize() // default
    {
        IdealSize = NormalSize;
    }

    public void SetStatic(bool StaticSet) // a static camera will not follow the player
    {
        Static = StaticSet;
    }

    public void SetOffset(Vector2 SetTo)
    {
        OffSet = SetTo;
    }

    public void SetFocusObject(GameObject ObjectSet)
    {
        FocusObject = ObjectSet;
    }
    public void ResetFocusObject()
    {
        FocusObject = player;
    }
}
