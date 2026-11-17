using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericScanBoxController : MonoBehaviour, IScanBoxController
{

    public bool[] Checks; // length is determined in inspector

    
    public void FoundPlayer(int Identifier)
    {
        if(Identifier < Checks.Length)
        {
            Checks[Identifier] = true;
        }
        else
        {
            Debug.LogError("Scan Box Controller Identifier Out of Range");
        }
    }
    public void LostPlayer(int Identifier)
    {
        if (Identifier < Checks.Length)
        {
            Checks[Identifier] = false;
        }
        else
        {
            Debug.LogError("Scan Box Controller Identifier Out of Range");
        }
    }
    
}
