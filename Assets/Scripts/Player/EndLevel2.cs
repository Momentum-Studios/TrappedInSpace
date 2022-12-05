using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel2 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D c){
        if(c.tag == "EndLevel"){
            SceneManager.LoadScene("MainMenu");
        }
    }
}
