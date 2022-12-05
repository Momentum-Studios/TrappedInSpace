/**
 * file: PauseMenu.cs
 * studio: Momentum Studios
 * authors: Daniel Nam
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/30/2022
 * 
 * purpose: Allows user to pause the game with the escape key.
Game is frozen while it is paused. You can resume the game, go to the main menu, or quit the game from 
the pause screen. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private int currentSceneIndex;
    public GameObject pauseMenu;
    public static bool isPaused;

    //private PlayerMovement playerMove;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        //playerMove = GetComponent<PlayerMovement>();
        // you can now call methods like playerMove.CanAttack()
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu(){
        Time.timeScale = 1f;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("SavedScene", currentSceneIndex);
        SceneManager.LoadScene(0);
        isPaused = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

}
