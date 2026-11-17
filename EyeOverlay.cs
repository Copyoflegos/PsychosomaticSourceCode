using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOverlay : MonoBehaviour
{
    PlayerMovement PM;
    public GameObject EyeChild;
    // Start is called before the first frame update
    void Start()
    {
        PM = MasterMind.GetPlayer().GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        EyeChild.SetActive(false);
    }
}
