using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddictionAreaSFX : MonoBehaviour
{
    AudioSource AS;
    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }
}
