/**
 * file: HealthCollectible.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez
 * class: CS 4700 - Game Development
 * 
 * Assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * Purpose: allow the user to pick up the heart when it collides with the heart game object
 * 
 */

using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    [SerializeField] private float healthValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}