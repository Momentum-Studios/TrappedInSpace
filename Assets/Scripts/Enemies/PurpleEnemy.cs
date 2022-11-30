/**
 * file: PurpleEnemy.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/28/2022
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
    [SerializeField] private float damageAmount;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float groundedBoxCastDistance;
    [SerializeField] private float distanceToEdge;
    [SerializeField] private float edgeRaycastDistance;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectilePoint;

    [SerializeField] private AudioClip deathSound;

    private Rigidbody2D objectRigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Animator muzzleFlashAnimator;
    private Transform playerTransform;
    private AIState currentAIState;
    private AudioSource audioSource;
    private EnemyHealth health;

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

    // setup purple enemy script
    void Start()
    {
        health = GetComponent<EnemyHealth>();
        objectRigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        muzzleFlashAnimator = muzzleFlash.GetComponent<Animator>();
        currentAIState = AIState.Idle;
        shootCooldownRemaining = shootCooldown;
        audioSource = GetComponent<AudioSource>();
    }

    // calculate the AI's next move in the state
    // machine
    void Update()
    {
        if (currentAIState == AIState.Dead) return;

        // check if should be dead
        if (health.currentHealth < 0.001)
            transitionToDeadState();

        if (playerTransform == null) return;

        handleShootCooldown();

        // handle state machine
        switch (currentAIState) {
            case AIState.Shooting:
                doShootingState();
                break;
            case AIState.Idle: case AIState.Chasing: case AIState.Dead:
                break;
        }

        handleAnimations();
    }

    // handle AI for moving and idle states
    void FixedUpdate()
    {
        if (currentAIState == AIState.Dead) return;

        // keep still if idle and on ground
        checkGrounded();
        if (isGrounded) objectRigidbody.velocity = new Vector2(0, objectRigidbody.velocity.y);

        if (playerTransform == null) return;

        checkCurrentDirection();

        // handle state machine
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
    }

    // activate this enemy once it detects the player within a 
    // certain range
    void OnTriggerEnter2D(Collider2D collider)
    {
        // detect player within circle collider trigger
        if (collider.gameObject.name == "Player" && playerTransform == null)
        {
            playerTransform = collider.gameObject.transform;
        }
    }

    // check if enemy is near an edge
    private void checkNearEdge()
    {
        float xDistance = Mathf.Sign(currentDirection) * (distanceToEdge + boxCollider.bounds.extents.x);
        Vector2 origin = boxCollider.bounds.center + new Vector3(xDistance, -boxCollider.bounds.extents.y - edgeRaycastDistance, 0f);

        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, edgeRaycastDistance * 2f, groundLayer);

        isNearEdge = (raycastHit.collider == null);
    }

    // check if enemy is grounded
    private void checkGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,
                0f, Vector2.down, groundedBoxCastDistance, groundLayer);

        isGrounded = (raycastHit.collider != null);
    }

    // check if enemy has clear line of sight to player and 
    // is within range
    private void checkShouldShoot()
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        LayerMask notCurrentLayer = ~(1 << gameObject.layer);

        RaycastHit2D raycastHit = Physics2D.Raycast(boxCollider.bounds.center, directionToPlayer, 
                shootDistance, notCurrentLayer);

        shouldShoot = (raycastHit.collider != null && raycastHit.collider.gameObject.name == "Player");
    }

    // shoot left or right depending on current direction
    private void shoot()
    {
        animator.SetTrigger("shoot");
        muzzleFlashAnimator.SetTrigger("muzzleFlash");

        GameObject projectileShot = Instantiate(projectile) as GameObject;
        Projectile p = projectileShot.GetComponent<Projectile>();
        p.SetDirection(currentDirection);
        p.setDamage(damageAmount);
        p.setTargetLayerMask(targetLayer);
        projectileShot.transform.position = projectilePoint.transform.position;
        projectileShot.SetActive(true);
    }

    // perform the idle state in the AI state machine
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

    // chase the player while in the chasing state in the
    // AI state machine
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

        // apply movement to rigidbody
        float movementVelocity = movementSpeed * currentDirection;
        objectRigidbody.velocity = new Vector2(movementVelocity, objectRigidbody.velocity.y);
    }

    // shoot at the player while in the shooting state
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

    // transition to the dead state when
    // the enemy dies
    private void transitionToDeadState()
    {
        // no transitions to other states
        // play death animation
        animator.SetTrigger("death");
        currentAIState = AIState.Dead;
        audioSource.PlayOneShot(deathSound);
        objectRigidbody.simulated = false;
    }

    // handle the animations for the enemy
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

    // check the current direction the enemy should
    // be facing
    private void checkCurrentDirection()
    {
        currentDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
    }

    // reset velocity in the x-axis to 0
    private void stopMoving()
    {
        objectRigidbody.velocity = new Vector2(0f, objectRigidbody.velocity.y);
    }

    // subtract delta time from the cooldown remaining
    // every frame
    private void handleShootCooldown()
    {
        if (shootCooldownRemaining > 0f)
        {
            shootCooldownRemaining -= Time.deltaTime;
        }
    }
}
