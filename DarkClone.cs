using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkClone : MonoBehaviour
{
    public GameObject VisualAim;
    public GameObject SmallHB;
    public GameObject BigHB;
    public GameObject ZoneHB;
    public LayerMask EnemyMask;
    RaycastHit2D HitCast;
    Damage PunchDamage;
    Damage SwingDamage;
    Damage KickDamage;
    Damage ZoneDamage;


    private void Awake()
    {
        PunchDamage = new Damage(50, gameObject);
        SwingDamage = new Damage(75, gameObject, 50);
        KickDamage = new Damage(75, 150, gameObject);
        ZoneDamage = new Damage(100, 75, gameObject);
    }

    public void InitializePunch(Vector3 Position,  Vector3 Rotation)
    {
        transform.position = Position;
        VisualAim.transform.up = Rotation;
        StartCoroutine(PunchCombo());
    }

    public void InitializeSwing(Vector3 Position, Vector3 Rotation)
    {
        transform.position = Position;
        VisualAim.transform.up = Rotation;
        StartCoroutine(SwingCombo());
    }

    public void InitializeKick(Vector3 Position, Vector3 Rotation, int KDur, float KSpd, bool Windup)
    {
        transform.position = Position;
        VisualAim.transform.up = Rotation;

        KickDamage.KnockBack = Rotation;

        StartCoroutine(KickCombo(Windup));
    }
    public void InitializeZone(Vector3 Position)
    {
        transform.position = Position;
        
        StartCoroutine(ZoneCombo());
    }

    IEnumerator PunchCombo()
    {

        for(int i = 0; i < 7; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        HitCast = MasterMind.HitBoxToCast(SmallHB, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(PunchDamage);
        }

        SmallHB.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForFixedUpdate();

        SmallHB.GetComponent<SpriteRenderer>().enabled = false;

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        HitCast = MasterMind.HitBoxToCast(SmallHB, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(PunchDamage);
        }

        SmallHB.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForFixedUpdate();

        SmallHB.GetComponent<SpriteRenderer>().enabled = false;

        gameObject.SetActive(false);
    }

    IEnumerator SwingCombo()
    {

        for (int i = 0; i < 12; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        HitCast = MasterMind.HitBoxToCast(BigHB, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(SwingDamage);
        }

        BigHB.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForFixedUpdate();

        BigHB.GetComponent<SpriteRenderer>().enabled = false;

        gameObject.SetActive(false);
    }

    IEnumerator KickCombo(bool Wind)
    {
        if (Wind)
        {
            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        

        HitCast = MasterMind.HitBoxToCast(BigHB, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(SwingDamage);
        }

        BigHB.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForFixedUpdate();

        BigHB.GetComponent<SpriteRenderer>().enabled = false;

        gameObject.SetActive(false);
    }


    IEnumerator ZoneCombo()
    {

        for (int i = 0; i < 135; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        HitCast = MasterMind.HitBoxToCast(ZoneHB, EnemyMask);
        if (HitCast)
        {
            HitCast.transform.GetComponentInParent<IEnemy>().GotHit(ZoneDamage);
        }

        ZoneHB.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForFixedUpdate();

        ZoneHB.GetComponent<SpriteRenderer>().enabled = false;

        gameObject.SetActive(false);
    }
}
