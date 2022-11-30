using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWallTrap : MonoBehaviour
{
    [Header("level environment")]
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject trap;

    [Header("Enemies")]
    [SerializeField] private GameObject enemy1;
    [SerializeField] private GameObject enemy2;
    [SerializeField] private GameObject enemy3;
    [SerializeField] private GameObject enemy4;
    
    private void OnTriggerEnter2D(Collider2D c){
        if (c.tag == "Trap"){
            wall.SetActive(true);
            enemy1.SetActive(true);
            enemy2.SetActive(true);
            enemy3.SetActive(true);
            enemy4.SetActive(true);
            trap.SetActive(false);
        }
    }

    void Awake(){
        wall.SetActive(false);
    }
}
