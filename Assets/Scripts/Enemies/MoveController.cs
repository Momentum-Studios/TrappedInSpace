/**
 * file: WhippingEnemy.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * purpose: This script controls the movement of an entity and provides functions
 * for checking certain conditions
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float initialDirection;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundedBoxCastDistance;
    [SerializeField] private bool stopAtEdge;
    [SerializeField] private float distanceToEdge;
    [SerializeField] private float edgeRaycastDistance;

    private Collider2D collider2d;
    private Rigidbody2D rigidbody2d;

    public float currentDirection { get; private set; }
    public bool isGrounded { get; private set; }
    public bool isNearEdge { get; private set; }

    void Start()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        // find collider component that is not a trigger
        foreach (Collider2D c in colliders)
        {
            if (!c.isTrigger)
            {
                collider2d = c;
                break;
            }
        }

        rigidbody2d = GetComponent<Rigidbody2D>();
        currentDirection = initialDirection;
    }

    void FixedUpdate()
    {
        checkGrounded();
        checkNearEdge();
    }

    // checks whether the entity is grounded
    private void checkGrounded()
    {
        // box cast downwards to check if grounded
        RaycastHit2D raycastHit = Physics2D.BoxCast(collider2d.bounds.center, collider2d.bounds.size,
                0f, Vector2.down, groundedBoxCastDistance, groundLayer);

        isGrounded = (raycastHit.collider != null);
    }

    // checks whether the entity is near an edge
    private void checkNearEdge()
    {
        float xDistance = Mathf.Sign(currentDirection) * (distanceToEdge + collider2d.bounds.extents.x);
        Vector2 origin = collider2d.bounds.center + new Vector3(xDistance, -collider2d.bounds.extents.y - edgeRaycastDistance, 0f);

        // raycast in front of current direction and downwards to determine if near edge
        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, edgeRaycastDistance * 2f, groundLayer);

        isNearEdge = (raycastHit.collider == null);
    }

    // moves the entity in the specified direction which is either
    // left (-1) or right (1)
    public void move(float direction)
    {
        currentDirection = Mathf.Sign(direction);

        if (!isGrounded) return;

        // stop at edges
        if (isNearEdge && stopAtEdge)
        {
            stopMoving();
            return;
        }

        float movementVelocity = movementSpeed * currentDirection;
        rigidbody2d.velocity = new Vector2(movementVelocity, rigidbody2d.velocity.y);
    }

    // moves the entity towards the specified position vector
    public void moveTowards(Vector2 pos)
    {
        move(pos.x - transform.position.x);
    }

    // makes the entity's x velocity zero and stops it
    public void stopMoving()
    {
        rigidbody2d.velocity = new Vector2(0f, rigidbody2d.velocity.y);
    }

    // sets the current direction the entity should be facing
    public void setCurrentDirection(float direction)
    {
        currentDirection = Mathf.Sign(direction);
    }
}
