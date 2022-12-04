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
