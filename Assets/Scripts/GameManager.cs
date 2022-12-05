/**
 * file: GameManager.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * purpose: Manage the overall game like reloading and restarting
 * the player at the correct checkpoint
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    private Vector3 currentCheckpoint;
    private bool valid;
    private string currentScene;

    // handle setting up the Game Manager
    void Start()
    {
        if (!gameManager)
            gameManager = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += onSceneLoad;
        valid = false;
    }

    // get the game manager instance
    public static GameManager getInstance() {
        return gameManager;
    }

    // restart the current level
    public void reloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    // handle scene loads
    void onSceneLoad(Scene scene, LoadSceneMode mode) 
    {
        if (scene.name != currentScene) valid = false;
        if (!valid) return;
        GameObject player = GameObject.Find("Player");
        if (!player) return;
        
        if (currentCheckpoint == null) return;
        player.transform.position = currentCheckpoint;
    }

    // set the current checkpoint position
    public void setCheckPoint(Vector3 pos) {
        currentCheckpoint = pos;
        valid = true;
        currentScene = SceneManager.GetActiveScene().name;
    }
}
