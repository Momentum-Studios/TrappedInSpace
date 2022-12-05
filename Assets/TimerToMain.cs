/**
 * file: TimerToMain.cs
 * studio: Momentum Studios
 * authors: Gregorius Avip
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 12/04/2022
 * 
 * purpose: quick timer script that will transition from end level to main menu scene
 */

using UnityEngine;
using UnityEngine.SceneManagement;
public class TimerToMain : MonoBehaviour
{
    public float timer = 5.0f;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0.0f){
            SceneManager.LoadScene("MainMenu");
        }
    }
}
