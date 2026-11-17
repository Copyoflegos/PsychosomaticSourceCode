using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPad : MonoBehaviour
{
    GameObject Player;
    SceneInitializer SI;
    public string nextScene;
    private void Start()
    {
        Player = MasterMind.GetPlayer();

        SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < 1f)
        {
            Player.GetComponent<PlayerMovement>().RefreshCooldowns();
            Player.GetComponent<PlayerMovement>().ClearConcentration();
            Player.GetComponent<PlayerMovement>().RefreshBurst();
            SI = MasterMind.GetCurrentSI();
            Player.transform.position = SI.GetStartingPosition();
            SceneManager.MoveGameObjectToScene(Player.transform.parent.gameObject, SceneManager.GetSceneByName(nextScene));

            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            SI.StartScene();

            
        }
    }
}
