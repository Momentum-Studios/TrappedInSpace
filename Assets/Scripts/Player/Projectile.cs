/**
 * file: Projectile.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/26/2022
 * 
 * purpose: This script controls the movement of the blaster projectile
 * and handles damaging of other targets
 */
using UnityEngine;

public class Projectile : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private LayerMask hittableLayerMask;
   [SerializeField] private float despawnTime = 5;

    private LayerMask targetLayerMask;
    private float direction;
    private bool hit;
    private float damageAmount;

    private BoxCollider2D boxCollider;
    private Animator anim;

    // setups the script by getting components
    // destroys projectile at end of life
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        Destroy(gameObject, despawnTime);
    }


    // updates movement of projectile
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }


    // checks for hitting of other colliders
    // and checks if hittable or target
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ignore other triggers
        if (collision.isTrigger) return; 

        if (hit) return;
        // check if collider is hittable
        int layer = 1 << collision.gameObject.layer;
        if ((hittableLayerMask & layer) == 0 && (targetLayerMask & layer) == 0)
            return;
        
        if ((targetLayerMask & layer) != 0)
            damage(collision.gameObject);

        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");
    }


    // sets the direction of movement for the projectile
    // in the x-axis
    public void SetDirection(float _direction)
    {
        direction = _direction;
        hit = false;

        
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    // sets the amount of damage this projectile should do
    public void setDamage(float damage) {
        damageAmount = damage;
    }


    // sets the target layer mask which contains layers
    // which should be damaged
    public void setTargetLayerMask(LayerMask target) {
        targetLayerMask = target;
    }


    // deactivates the projectile at the end of its lifetime
    private void Deactivate() 
    {
        Destroy(gameObject);
    }


    // damages the GameObject target
    private void damage(GameObject target) {
        Health h = target.GetComponent<Health>();

        // check if null
        if (!h) return;

        h.TakeDamage(damageAmount);
    }

}
