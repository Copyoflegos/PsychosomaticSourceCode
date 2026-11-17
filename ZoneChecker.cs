using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneChecker : MonoBehaviour
{
    // Keeps track of which zone that last contacted it has highest priority and then sends an update to the boss if the player moves zones

        // HOW TO USE: Set up zone boxes and child them to this object
        // Then assign a boss with method names alligned with each zone identifier
        // ????
        // profit

    public GameObject Boss;
    string LastIdentifier = " ";
    string CurrentIdentifier = " ";
    int CurrentPriority = -1;


    private void LateUpdate()
    {
       
        // Every frame the name is checked to see if it changed from last frame and if so then the boss is told which name is current
        if (!CurrentIdentifier.Equals(LastIdentifier))
        {
            if(Boss != null)
            {
                Boss.SendMessage(CurrentIdentifier);
            }
            //print("Change to: " + CurrentIdentifier);
            LastIdentifier = CurrentIdentifier;
        }


        CurrentPriority = -1;
        CurrentIdentifier = " ";


    }

    // Zone class sends info over, Name and Priority
    public void FoundPlayer(string Identifier, int Priority)
    {
        if(Priority > CurrentPriority )
        {
            CurrentPriority = Priority;
            CurrentIdentifier = Identifier;
            
        }
        else if(Priority == CurrentPriority && !Identifier.Equals(CurrentIdentifier))
        {
            Debug.LogError("Two adjacent zones with same priority: " + Identifier);
            
        }


        
    }

}
