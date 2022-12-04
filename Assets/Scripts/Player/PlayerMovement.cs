/**
 * file: HealthCollectible.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Matthew Cheser
 * class: CS 4700 - Game Development
 * 
 * Assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * Purpose: Create a player game object that has the ability to interact with the game world. Running, Jumping, Take Damage etc.
 * 
 */
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Hang Time")]
    [SerializeField] private float hangTime; //How much time the player can hang in the air before jumping
    private float hangCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Audio Source")]
    [SerializeField] private AudioSource addHealthSoundEffect;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private float horizontalInput;
    private float boostTimer;
    private bool isBoosting;


    private void Start()
    {
        boostTimer = 0;
        isBoosting = false;
    }
    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        //Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            body.gravityScale = 25;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 4;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isGrounded())
            {
                hangCounter = hangTime; //Reset hang counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                hangCounter -= Time.deltaTime; //Start decreasing counter when not on the ground
        }

        if (isBoosting)
        {

            boostTimer += Time.deltaTime;

            if (boostTimer >= 3)
             {
                boostTimer = 0;
                isBoosting = false;
                speed /= 1.3f;
             }
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "SpeedBoost")
        {
            isBoosting = true;

            speed *= 1.3f;
            Destroy(collision.gameObject);
        }
    }  
    private void Jump()
    {
        if (hangCounter <= 0 && !onWall() && jumpCounter <= 0) return;
            else
            {
                if (isGrounded())
                    body.velocity = new Vector2(body.velocity.x, jumpPower);

                    //SoundManager.instance.PlaySound(jumpSound);
                else
                {
                    //If not on the ground and counter bigger than 0 do a normal jump
                    if (hangCounter > 0)
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                    else
                    {
                        if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                        {
                            body.velocity = new Vector2(body.velocity.x, jumpPower);
                            jumpCounter--;
                        }
                    }
                }

            //Reset hang counter to 0 to avoid double jumps
            hangCounter = 0;
            }
    }
        private bool isGrounded()
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
            return raycastHit.collider != null;
        }
        private bool onWall()
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
            return raycastHit.collider != null;
        }
        public bool canAttack() 
        {
            return horizontalInput == 0 && isGrounded() && !onWall();
        }
}
