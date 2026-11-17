using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamSpot : MonoBehaviour
{
    // This class waits a second and then hurts the player if he's within range and dissapears afterwards
    static float WAIT_TIME = 0.5f; // How long the player has to dodge the attack

    LayerMask PlayerOnly;

    public float Damage;

    private void Awake()
    {
        StartCoroutine(HitWait());
        PlayerOnly = LayerMask.GetMask("Player");
    }

    IEnumerator HitWait()
    {
        yield return new WaitForSeconds(WAIT_TIME);
        RaycastHit2D SlamHit = Physics2D.BoxCast(transform.position, new Vector2(2, 2), 0, transform.up, 0, PlayerOnly);
        if (SlamHit)
        {
            SlamHit.transform.SendMessage("Hit", Damage);
        }
        Destroy(gameObject);
    }
}
