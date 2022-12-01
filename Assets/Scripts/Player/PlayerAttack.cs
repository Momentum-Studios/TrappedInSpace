/**
 * file: PlayerAttack.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/12/2022
 * 
 * purpose: This script controls the attack action (blaster fire) of the player
 * by spawning in new projectiles upon player input.
 */
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] private float attackCoolDown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float damageAmount;
    [SerializeField] private LayerMask enemyLayer;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    // setup script by getting components
    private void Awake()
    {
        //Get references
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // check for player input every frame and update the cooldown timer
    private void Update()
    {
        if(!PauseMenu.isPaused){
            if (Input.GetMouseButton(0) && cooldownTimer > attackCoolDown )
            Attack();

            cooldownTimer += Time.deltaTime;
        }
        
    }

    // handle the attack action by firing the blaster
    // (spawn new projectile)
    private void Attack() {
        cooldownTimer = 0;

        GameObject projectileShot = Instantiate(projectile) as GameObject;
        Projectile p = projectileShot.GetComponent<Projectile>();
        p.SetDirection(Mathf.Sign(transform.localScale.x));
        p.setDamage(damageAmount);
        p.setTargetLayerMask(enemyLayer);
        projectileShot.transform.position = firePoint.position;
        projectileShot.SetActive(true);
    }

}
