using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionLockOn : MonoBehaviour
{
    public GameObject Addiction;
    
    //bool Faded = false;

    void Update()
    {
        transform.position = new Vector3(Addiction.transform.position.x, Addiction.transform.position.y, 0);


    }

    
}
