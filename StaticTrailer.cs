using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTrailer : MonoBehaviour
{
    public GameObject StaticTrail;

    public AddictionSFX ASFX;

    int TrailTime = -1; // -1 is infinite until manually killed
    public bool Active;
    Vector2 PreviousPos;

    List<GameObject> StaticList = new List<GameObject>();

    private void Awake()
    {
        StaticList.Clear();
        PreviousPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Active)
        {
            if(Vector2.Distance(PreviousPos, transform.position) >= 0.5f)
            {
                StaticList.Add(Instantiate(StaticTrail, transform.position, Quaternion.identity, transform.parent));
                StaticList[StaticList.Count - 1].GetComponent<StaticFloor>().SetTime(TrailTime);
                //StaticList[StaticList.Count - 1].transform.localScale = transform.localScale;
                PreviousPos = transform.position;
            }
        }
        if(StaticList.Count > 0 && StaticList[StaticList.Count - 1] == null)
        {
            ASFX.EnableStatic(false);
        }
    }

    public void StaticSet(bool SetState)
    {
        Active = SetState;
        if (SetState)
        {
            ASFX.EnableStatic(true);
        }
    }

    public void SetStaticTime(int TimeSet)
    {
        TrailTime = TimeSet;
    }

    public void SetStaticForever()
    {
        TrailTime = -1;
    }

    public void DeleteStatic()
    {
        for(int i = 0; i < StaticList.Count; i++)
        {
            StaticList[i].GetComponent<StaticFloor>().SetTime(-2); // tells it to destroy itself
        }
        StaticList.Clear();
    }
}
