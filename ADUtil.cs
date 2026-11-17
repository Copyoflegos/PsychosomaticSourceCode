using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADUtil
{
    public static Vector2 GetAimDirVector(AimDirKey ADK) // Returns a vector 2 in front of the player (normalized)(in color)(remastered edition)
    {
        switch (ADK)
        {
            case AimDirKey.Down:
                return new Vector3(0, -1, 0);
                
            case AimDirKey.DownLeft:
                return new Vector3(-1, -1, 0);
                
            case AimDirKey.Left:
                return new Vector3(-1, 0, 0);
                
            case AimDirKey.UpLeft:
                return new Vector3(-1, 1, 0);
                
            case AimDirKey.Up:
                return new Vector3(0, 1, 0);
                
            case AimDirKey.UpRight:
                return new Vector3(1, 1, 0);
                
            case AimDirKey.Right:
                return new Vector3(1, 0, 0);
                
            case AimDirKey.DownRight:
                return new Vector3(1, -1, 0);
            default:
                
                return Vector2.zero;
        }
    }
}
