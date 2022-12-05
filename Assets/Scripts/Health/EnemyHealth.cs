/**
 * file: EnemyHealth.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/12/2022
 * 
 * purpose: This script controls the health of the enemy which has this
 * script attached
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private bool damageable;

    // setup current health by setting it to starting health
    private void Awake()
    {
        currentHealth = startingHealth;
        damageable = false;
    }

    // handle damaging of enemy and check if damageable
    public override void TakeDamage(float damage)
    {
        if (damageable) {
            base.TakeDamage(damage);
        }
    }

    // set whether the enemy can be damaged
    public void setDamageable(bool d) {
        damageable = d;
    }
}
