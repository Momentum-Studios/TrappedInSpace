/**
 * file: SpawnWallTrap.cs
 * studio: Momentum Studios
 * authors: Gregorius Avip
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/30/2022
 * 
 * purpose: Collider trigger to activate a trap that will spawn
 * a wall surrounding the player and remove the wall once the enemy is all dead
 */
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

    //Check if the player's collision hit a trigger collider
    private void OnTriggerEnter2D(Collider2D c){
        //if it hits the right collider tagged as Trap, activate the walls and enemies
        //and enable Update
        if (c.tag == "Trap"){
            wall.SetActive(true);
            enemy1.SetActive(true);
            enemy2.SetActive(true);
            enemy3.SetActive(true);
            enemy4.SetActive(true);
            trap.SetActive(false);
            enabled = true;
        }
    }

    //make sure that the wall is off since the trap is not yet activated
    void Awake(){
        wall.SetActive(false);
        enabled = false;    //optimize so Update is not called until the trap is activated
    }

    void Update() {
        //get all of the enemies states (hard coded) and disable the wall once all of the enemy is dead
        int countDead = 0;
        string[] enemyStates = {enemy1.GetComponent<PurpleEnemy>().getCurrentAIState(), enemy2.GetComponent<PurpleEnemy>().getCurrentAIState()
                                ,enemy3.GetComponent<MeleeEnemy>().getCurrentAIState(), enemy4.GetComponent<MeleeEnemy>().getCurrentAIState()};
        foreach(string enemyState in enemyStates){
            if(enemyState == "Dead")
                countDead += 1; 
            else{
                countDead = 0;
                break;
            }
        }
        if(countDead == 4){
            wall.SetActive(false);
            enabled = false;
        }
    }
}
