using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    int BombLifeDuration = 0;
    const int MINLIFE = 200; // 50 is 1 second
    public Sprite LitBomb;
    public Sprite BlownBomb;
    SpriteRenderer SR;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        BombLifeDuration++;
        if(BombLifeDuration == MINLIFE)
        {
            SR.sprite = LitBomb;
        }
    }

    void BlowUp()
    {
        if(BombLifeDuration > MINLIFE)
        {
            StartCoroutine(BlowUpSequence());
        }
    }

    IEnumerator BlowUpSequence()
    {
        SR.sprite = BlownBomb;
        foreach(RaycastHit2D Hit in Physics2D.BoxCastAll(transform.position,new Vector2(1, 1), 0, transform.up))
        {
            Hit.transform.SendMessage("BombHit",SendMessageOptions.DontRequireReceiver);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
