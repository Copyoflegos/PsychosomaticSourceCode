using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutLoadScene : MonoBehaviour
{
    bool FadeOut;

    SpriteRenderer SR;

    public float FADE_GAIN = 0.01f;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        FadeOut = true;
        
    }

    private void FixedUpdate()
    {
        if (FadeOut)
        {
            SR.color = new Color(0, 0, 0, SR.color.a + FADE_GAIN);

            if(SR.color.a >= 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
