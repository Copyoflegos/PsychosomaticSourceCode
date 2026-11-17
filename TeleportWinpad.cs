using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportWinpad : MonoBehaviour
{
    GameObject Player;
    
    public string nextScene;
    private void Start()
    {
        Player = MasterMind.GetPlayer();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < 1f)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
