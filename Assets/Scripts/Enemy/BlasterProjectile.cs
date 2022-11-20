/**
 * file: PurpleEnemy.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/12/2022
 * 
 * purpose: This script controls the movement of the blaster projectile
 * and handles damaging of other targets
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float despawnTime;
    
    private Rigidbody2D objectRigidbody;
    private float currentDirection;
    

    // Start is called before the first frame update
    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody2D>();

        if (despawnTime < 0f)
        {
            despawnTime = 0f;
        }

        Destroy(gameObject, despawnTime);
    }

    void FixedUpdate()
    {
        float movementVelocity = projectileSpeed * currentDirection;

        objectRigidbody.velocity = new Vector2(movementVelocity, 0f);
    }

    public void setCurrentDirection(float direction)
    {
        currentDirection = direction;

        if (direction < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
