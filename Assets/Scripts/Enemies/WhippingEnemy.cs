/**
 * file: WhippingEnemy.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/29/2022
 * 
 * purpose: This script controls the movement, animations, melee, and other
 * functions of melee enemies
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhippingEnemy : MonoBehaviour
{
    [SerializeField] private float targetDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damageAmount;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    private MoveController moveController;
    private ColliderTriggerHandler attackTrigger;
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private Transform playerTransform;
    private AIState currentAIState;
    private float attackCooldownRemaining;
    private bool isAttacking;
    private AudioSource audioSource;
    private EnemyHealth health;

    private enum AIState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    // setup by getting components and setting initial AI state
    void Start()
    {
        health = GetComponent<EnemyHealth>();
        attackTrigger = GetComponentInChildren<ColliderTriggerHandler>();
        moveController = GetComponent<MoveController>();
        currentAIState = AIState.Idle;
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // update AI state and perform that state
    // excluding all movement AI states
    void Update()
    {
        if (currentAIState == AIState.Dead) return;

        if (health.currentHealth < 0.001)
            transitionToDeadState();

        if (!playerTransform) return;

        handleWhipCooldown();

        // handle different AI states
        switch (currentAIState)
        {
            case AIState.Attacking:
                performAttack();
                break;
            case AIState.Idle:
            case AIState.Chasing:
            case AIState.Dead:
                break;
        }

        handleAnimations();
    }

    // handle AI states for moving
    void FixedUpdate()
    {
        if (currentAIState == AIState.Dead) return;

        // stop moving if grounded and idle
        if (currentAIState == AIState.Idle && moveController.isGrounded)
            moveController.stopMoving();

        if (!playerTransform) return;

        // handle different AI states
        switch (currentAIState)
        {
            case AIState.Idle:
                doIdleState();
                break;
            case AIState.Chasing:
                doChasingState();
                break;
            case AIState.Dead:
            case AIState.Attacking:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        // detect player within circle collider trigger
        if (c.gameObject.name == "Player" && playerTransform == null)
        {
            playerTransform = c.gameObject.transform;
        }
    }

    // perform the idle state in the AI state machine
    private void doIdleState()
    {
        float directionToPlayer = playerTransform.position.x - transform.position.x;
        float distanceFromPlayer = Mathf.Abs(directionToPlayer);
        directionToPlayer = Mathf.Sign(directionToPlayer);

        // transition to chasing state if player farther than target distance
        if (distanceFromPlayer > targetDistance)
        {
            currentAIState = AIState.Chasing;
            doChasingState();
            return;
        }
        
        // transition to whipping state if close enough
        if (isAbleToAttack())  
        {
            currentAIState = AIState.Attacking;
            return;
        }

        // update the current direction of the enemy to always point at the player
        if (directionToPlayer != moveController.currentDirection)
        {
            moveController.setCurrentDirection(directionToPlayer);
        }
    }

    // perform the chasing state in the AI state machine which moves the enemy 
    // towards the player
    private void doChasingState()
    {
        float distanceFromPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);

        if (distanceFromPlayer <= targetDistance)
        {
            if (isAbleToAttack())   // transition to whipping state if close enough to player and can whip
            {
                currentAIState = AIState.Attacking;
            }
            else    // transition to idle state if within target distance
            {
                currentAIState = AIState.Idle;
            }
            moveController.stopMoving();
            return;
        }

        moveController.moveTowards(playerTransform.position);
    }

    // transition to the dead final state in the AI state machine which stop
    // future actions made by this enemy
    private void transitionToDeadState()
    {
        animator.SetTrigger("death");
        currentAIState = AIState.Dead;
        audioSource.PlayOneShot(deathSound);
        rigidbody2d.simulated = false;
    }

    // handle setting the different floats for the animation controller 
    // and handle flipping the sprite based on current direction
    private void handleAnimations()
    {
        animator.SetFloat("speed", Mathf.Abs(rigidbody2d.velocity.x));

        // flip sprite if current direction different from scale direction
        if (moveController.currentDirection != Mathf.Sign(transform.localScale.x))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    // trigger a whip animation and reset the whipping cooldown
    private void performAttack()
    {
        if (isAttacking) return;

        attackCooldownRemaining = attackCooldown;
        animator.SetTrigger("attack");
        isAttacking = true;
        audioSource.PlayOneShot(attackSound);
    }

    // subtract from the whip cooldown every update
    private void handleWhipCooldown()
    {
        if (attackCooldownRemaining > 0.0f)
        {
            attackCooldownRemaining -= Time.deltaTime;
        }
    }

    // this is an animation event that is triggered during the whipping animation
    // and when damage should be applied to the player (if within trigger collider)
    private void attackEvent()
    {
        // check if player is within whip range
        if (!attackTrigger.isInside) return;

        attackTrigger.gameObj.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
    }

    // this is an animation event that is triggered at the end of the whipping
    // animation and returns the AI state to the idle state
    private void attackFinishEvent()
    {
        // transition to idle state when whipping state is finished
        currentAIState = AIState.Idle;
        isAttacking = false;
    }

    // checks whether the enemy is able to whip by checking if the player is inside
    // the whip trigger collider and the cooldown has expired
    private bool isAbleToAttack()
    {
        return attackTrigger.isInside && attackCooldownRemaining <= 0f;
    }
}
