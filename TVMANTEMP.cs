using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVMANTEMP : MonoBehaviour
{

    public int moveDirX;
    public int moveDirY;

    public float moveSpdX;
    public float moveSpdY;

    private void FixedUpdate()
    {
        gameObject.transform.position += new Vector3(moveDirX * moveSpdX, moveDirY * moveSpdY, 0);
    }

}
