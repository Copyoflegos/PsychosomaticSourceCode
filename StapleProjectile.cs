using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StapleProjectile : MonoBehaviour
{
    private void Start()
    {
        float scaleFactor = 1 / transform.lossyScale.x;
        transform.localScale = new Vector3(scaleFactor * transform.localScale.x, scaleFactor * transform.localScale.y);
    }
}
