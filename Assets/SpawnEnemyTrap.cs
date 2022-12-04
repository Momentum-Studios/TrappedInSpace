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
 * Enemies
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyTrap : MonoBehaviour
{
    [Header("level environment")]
    [SerializeField] private GameObject trap;

    [Header("Enemies")]
    [SerializeField] private GameObject enemy1;
    [SerializeField] private GameObject enemy2;
    [SerializeField] private GameObject enemy3;
    [SerializeField] private GameObject enemy4;
    [SerializeField] private GameObject enemy5;

    private void OnTriggerEnter2D(Collider2D c){
        //if it hits the right collider tagged as Trap, activate the walls and enemies
        //and enable Update
        if (c.tag == "Trap2"){
            enemy1.SetActive(true);
            enemy2.SetActive(true);
            enemy3.SetActive(true);
            enemy4.SetActive(true);
            enemy5.SetActive(true);
            trap.SetActive(false);
        }
    }
}
