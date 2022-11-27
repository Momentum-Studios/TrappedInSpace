/**
 * file: PurpleEnemy.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/12/2022
 * 
 * purpose: This script controls the movement, shooting, animations, and other
 * functions of the Purple Blaster Enemy.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleEnemy : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float targetDistance;
    [SerializeField] private float shootDistance;
    [SerializeField] private float shootCooldown;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float groundedBoxCastDistance;
    [SerializeField] private float distanceToEdge;
    [SerializeField] private float edgeRaycastDistance;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject projectile;

    [SerializeField] private AudioClip deathSound;

    private Rigidbody2D objectRigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Animator muzzleFlashAnimator;
    private Transform playerTransform;
    private AIState currentAIState;
    private AudioSource audioSource;

    private float currentDirection;
    private float shootCooldownRemaining;
    private bool isGrounded;
    private bool isNearEdge;
    private bool shouldShoot;

    private enum AIState {
        Idle,
        Chasing,
        Shooting,
        Dead
    }

    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        muzzleFlashAnimator = muzzleFlash.GetComponent<Animator>();
        currentAIState = AIState.Idle;
        shootCooldownRemaining = shootCooldown;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        handleShootCooldown();

        switch (currentAIState) {
            case AIState.Shooting:
                doShootingState();
                break;
            case AIState.Idle: case AIState.Chasing: case AIState.Dead:
                break;
        }

        handleAnimations();
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        checkCurrentDirection();

        switch (currentAIState) {
            case AIState.Idle:
                doIdleState();
                break;
            case AIState.Chasing:
                doChasingState();
                break;
            case AIState.Shooting: case AIState.Dead:
                break;
        }

        print(objectRigidbody.velocity.x + " " + objectRigidbody.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // detect player within circle collider trigger
        if (collider.gameObject.name == "Player" && playerTransform == null)
        {
            playerTransform = collider.gameObject.transform;
        }
    }

    private void checkNearEdge()
    {
        float xDistance = Mathf.Sign(currentDirection) * (distanceToEdge + boxCollider.bounds.extents.x);
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

    private void checkShouldShoot()
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        LayerMask notCurrentLayer = ~(1 << gameObject.layer);

        RaycastHit2D raycastHit = Physics2D.Raycast(boxCollider.bounds.center, directionToPlayer, 
                shootDistance, notCurrentLayer);

        shouldShoot = (raycastHit.collider != null && raycastHit.collider.gameObject.name == "Player");
    }

    private void shoot()
    {
        animator.SetTrigger("shoot");
        muzzleFlashAnimator.SetTrigger("muzzleFlash");

        GameObject projectileShot = Instantiate(projectile) as GameObject;
        projectileShot.GetComponent<BlasterProjectile>().setCurrentDirection(currentDirection);
        projectileShot.transform.position = projectile.transform.position;
        projectileShot.SetActive(true);
    }

    private void doIdleState()
    {
        // transition to shooting state if inside shoot distance
        checkShouldShoot();
        if (shouldShoot)
        {
            stopMoving();
            currentAIState = AIState.Shooting;
            return;
        }

        float distanceFromPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);

        // transition to chasing state if player farther than chase distance
        if (distanceFromPlayer > chaseDistance)
        {
            currentAIState = AIState.Chasing;
            doChasingState();
        }
    }

    private void doChasingState()
    {
        // transition to shooting state if inside shoot distance
        checkShouldShoot();
        if (shouldShoot)
        {
            stopMoving();
            currentAIState = AIState.Shooting;
            return;
        }

        float distanceFromPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);

        // transition to idle state if within target distance
        if (distanceFromPlayer <= targetDistance)
        {
            stopMoving();
            currentAIState = AIState.Idle;
            return;
        }

        checkGrounded();
        if (!isGrounded) return;

        checkNearEdge();
        if (isNearEdge)
        {
            stopMoving();
            return;
        }

        float movementVelocity = movementSpeed * currentDirection;
        objectRigidbody.velocity = new Vector2(movementVelocity, objectRigidbody.velocity.y);
    }

    private void doShootingState()
    {
        // transition if outside shoot distance
        checkShouldShoot();
        if (!shouldShoot)
        {
            float distanceFromPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);

            if (distanceFromPlayer < chaseDistance)
            {
                // transition to chasing state if farther than chase distance
                currentAIState = AIState.Chasing;
            }
            else
            {
                // transitionn to idle state otherwise
                stopMoving();
                currentAIState = AIState.Idle;
            }

            return;
        }

        if (shootCooldownRemaining > 0f) return;

        shoot();
        shootCooldownRemaining = shootCooldown;
    }

    private void transitionToDeadState()
    {
        // no transitions to other states
        // play death animation
        animator.SetTrigger("death");
        currentAIState = AIState.Dead;
        audioSource.PlayOneShot(deathSound);
    }

    private void handleAnimations()
    {
        if (currentAIState == AIState.Dead) return;

        animator.SetFloat("speed", Mathf.Abs(objectRigidbody.velocity.x));

        float velocityDirection = currentDirection;
        float scaleDirection = Mathf.Sign(transform.localScale.x);

        // flip sprite based on velocity direction
        if (velocityDirection != scaleDirection && playerTransform)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }

    private void checkCurrentDirection()
    {
        currentDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
    }

    private void stopMoving()
    {
        objectRigidbody.velocity = new Vector2(0f, objectRigidbody.velocity.y);
    }

    private void handleShootCooldown()
    {
        if (shootCooldownRemaining > 0f)
        {
            shootCooldownRemaining -= Time.deltaTime;
        }
    }
}
