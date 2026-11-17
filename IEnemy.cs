using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void GotHit(Damage dmg);

    void GotBleed(int Duration, float Damage);

    //void GotStapled(int StapleNum);

    //void GotBurned(int BurnNum);

    bool GetStunned();

    bool GetFlinchable();
    
}
