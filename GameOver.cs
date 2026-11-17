using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOver : MonoBehaviour
{
    bool Ready = false;
    private void Awake()
    {
        StartCoroutine(WaitForLoadIn());
    }
    

    public void LoadScene( string Scene)
    {
        SceneManager.LoadScene(Scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitForLoadIn()
    {
        yield return new WaitForSeconds(0.25f);
        Ready = true;
    }

    public void OnRetry(InputValue value)
    {
        float DodgeVal = value.Get<float>();
        if (DodgeVal == 0)
        {
            
        }
        else if (DodgeVal == 1 && Ready)
        {
            LoadScene("ControlsRoom");
        }
    }
    public void OnQuit(InputValue value)
    {
        float DodgeVal = value.Get<float>();
        if (DodgeVal == 0)
        {

        }
        else if (DodgeVal == 1 && Ready)
        {
            QuitGame();
        }
    }
}
