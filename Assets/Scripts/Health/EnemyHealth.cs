using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private void Awake()
    {
        currentHealth = startingHealth;
    }
}
