using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScanBoxController
{
    

    void FoundPlayer(int identifier);
    void LostPlayer(int identifier);
   
    
    
}
