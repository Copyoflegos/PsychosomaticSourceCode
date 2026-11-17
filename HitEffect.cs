using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    GameObject HitObject;
    HitDebris HitObjectHB;
    GameObject[] Debris;
    public GameObject[] BasicDebris;
    SpriteRenderer SR;
    public Transform[] ParticleSpawnsG1;
    public Transform[] ParticleSpawnsG2;
    public Transform[] ParticleSpawnsG3;
    public Transform[] ParticleSpawnsG4;
    public Transform[] ParticleSpawnsG5;
    public bool UseImpactSprite; // the middle one
    public bool DisallowDebrisOverride; // allow the debris to be decided by the enemy
    float RandomNumber = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartingVariables(GameObject ObjectHit)
    {
        HitObject = ObjectHit;
        
        if(ObjectHit.GetComponentInParent<HitDebris>() != null)
        {
            HitObjectHB = HitObject.GetComponentInParent<HitDebris>();
            if (HitObjectHB.Debris.Length > 0 && !DisallowDebrisOverride)
            {
                Debris = new GameObject[HitObjectHB.Debris.Length + BasicDebris.Length];
                for(int i = 0; i < BasicDebris.Length; i++)
                {
                    Debris[i] = BasicDebris[i];
                }
                for(int x = 0; x < HitObjectHB.Debris.Length; x++)
                {
                    Debris[x + BasicDebris.Length] = HitObjectHB.Debris[x];
                }
            }
            else
            {
                Debris = BasicDebris;
            }
            if (HitObjectHB.HitImpact != null) // change this
            {
                SR.sprite = HitObjectHB.HitImpact;
            }
            StartCoroutine(StartParticles());
        }
        else
        {
            Debris = BasicDebris;
            
            StartCoroutine(StartParticles());
        }
    }
    IEnumerator StartParticles()
    {
        SR.enabled = UseImpactSprite;
        SpawnGroup(ParticleSpawnsG1);
        SpawnGroup(ParticleSpawnsG2);
        SpawnGroup(ParticleSpawnsG3);
        SpawnGroup(ParticleSpawnsG4);
        SpawnGroup(ParticleSpawnsG5);
        yield return new WaitForSeconds(0.15f);
        SR.enabled = false;
        Destroy(gameObject);
    }
    void SpawnGroup(Transform[] ParticleArray)
    {
        RandomNumber = Random.Range(0, 9);
        int RandomNumber2 = Random.Range(0, Debris.Length);
        if(ParticleArray.Length > 1)
        {
            if (RandomNumber == 0)
            {
                return;
            }
            else if (RandomNumber == 8)
            {
                Instantiate(Debris[RandomNumber2], ParticleArray[0].position, ParticleArray[0].transform.rotation);
                RandomNumber2 = Random.Range(0, Debris.Length);
                Instantiate(Debris[RandomNumber2], ParticleArray[1].position, ParticleArray[1].transform.rotation);
            }
            else if (RandomNumber % 2 == 0)
            {
                Instantiate(Debris[RandomNumber2], ParticleArray[0].position, ParticleArray[0].transform.rotation);
            }
            else
            {
                Instantiate(Debris[RandomNumber2], ParticleArray[1].position, ParticleArray[1].transform.rotation);
            }
        }
        else if(ParticleArray.Length == 1)
        {
            Instantiate(Debris[RandomNumber2], ParticleArray[0].position, ParticleArray[0].transform.rotation);
        }
        
    }
}
