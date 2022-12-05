/**
 * file: MainMenu.cs
 * studio: Momentum Studios
 * authors: Daniel Nam
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 12/04/2022
 * 
 * purpose: Scripts for switching between scenes from the main menu. You can continue from the level
 you left off from with the continue button, start a new game with the start button, and quit game
 with the quit button.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private int continueScene;

    public void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ContinueGame(){
        continueScene = PlayerPrefs.GetInt("SavedScene");
        
        if (continueScene != 0){
            SceneManager.LoadScene(continueScene);
        }
        else{
            return;
        }
    }
}
