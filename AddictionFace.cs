using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionFace : MonoBehaviour
{
    SpriteRenderer SR;
    AddictionAnimator AA;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        AA = transform.parent.GetComponent<AddictionAnimator>();
    }
    void Update()
    {
        SR.sortingOrder = AA.GetComponent<SpriteRenderer>().sortingOrder + 1;
        SR.color = AA.GetComponent<SpriteRenderer>().color;
    }

    public void FaceState(bool State)
    {
        //SR.enabled = State;
    }
}
