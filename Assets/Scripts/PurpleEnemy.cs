/**
 * file: PurpleEnemy.cs
 * studio: Momentum Studios
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/10/2022
 * 
 * purpose: This script controls the movement, shooting, animations, and other
 * functions of the Purple Blaster Enemy.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleEnemy : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minimumDistanceFromPlayer;
    [SerializeField] private float maximumDistanceFromPlayer;
    [SerializeField] private float maximumShootingDistance;

    [SerializeField] private float groundedBoxCastDistance;
    [SerializeField] private float distanceToEdge;
    [SerializeField] private float edgeRaycastDistance;
    [SerializeField] private GameObject muzzleFlash;

    private Rigidbody2D objectRigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Transform playerTransform;

    private float currentDirection;
    private bool isGrounded;
    private bool isNearEdge;
    private bool chasingPlayer;


    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(objectRigidbody.velocity.x));

        float velocityDirection = currentDirection;
        float scaleDirection = Mathf.Sign(transform.localScale.x);

        if (velocityDirection != scaleDirection && playerTransform) {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        checkGrounded();

        // move only if is grounded and player was detected
        if (!isGrounded || playerTransform == null) return;

        float distanceFromPlayer = playerTransform.position.x - transform.position.x;
        currentDirection = Mathf.Sign(distanceFromPlayer);
        distanceFromPlayer = Mathf.Abs(distanceFromPlayer);

        checkNearEdge(currentDirection);

        float acceptableDistance = chasingPlayer ? minimumDistanceFromPlayer : maximumDistanceFromPlayer;

        // stop moving when near edge or too close to player
        if (isNearEdge || distanceFromPlayer <= acceptableDistance)
        {
            objectRigidbody.velocity = new Vector2(0f, objectRigidbody.velocity.y);
            chasingPlayer = false;
            return;
        } else if (distanceFromPlayer > acceptableDistance)
        {
            chasingPlayer = true;
        }

        float movementVelocity = movementSpeed * currentDirection;

        objectRigidbody.velocity = new Vector2(movementVelocity, objectRigidbody.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // detect player within circle collider trigger
        if (collider.gameObject.name == "Player" && playerTransform == null)
        {
            playerTransform = collider.gameObject.transform;
            chasingPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // detect player exiting range and "disable" enemy movement
        if (collider.gameObject.name == "Player" && playerTransform != null)
        {
            playerTransform = null;
            chasingPlayer = false;
        }
    }

    private void checkNearEdge(float xDirection)
    {
        float xDistance = Mathf.Sign(xDirection) * (distanceToEdge + boxCollider.bounds.extents.x);
        Vector2 origin = boxCollider.bounds.center + new Vector3(xDistance, -boxCollider.bounds.extents.y - edgeRaycastDistance, 0f);

        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, edgeRaycastDistance * 2f, groundLayer);

        isNearEdge = (raycastHit.collider == null);
    }

    private void checkGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,
                0f, Vector2.down, groundedBoxCastDistance, groundLayer);

        isGrounded = (raycastHit.collider != null);
    }

    private void shoot(float xDirection)
    {
        animator.SetTrigger("shoot");
    }
}
