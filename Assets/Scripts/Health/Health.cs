/**
 * file: Health.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * Assignment: Program 4
 * Date last modified: 11/28/2022
 * 
 * Purpose: Base class for manage health of both enemies and the player
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float startingHealth;
    public float currentHealth { get; protected set; }
    protected bool dead;

    // subtract damage from current health
    public virtual void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
    }

    // add health to current health
    public virtual void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
}
