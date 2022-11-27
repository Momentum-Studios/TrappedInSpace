using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

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
            
            body.gravityScale = 28;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 4;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        }

        if (isBoosting)
        {
            boostTimer += Time.deltaTime;

            if (boostTimer >= 4)
            {
                speed = 5f;

                if (boostTimer >= 3)
                {
                    boostTimer = 0;
                    isBoosting = false;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.tag == "SpeedBoost")
            {
                isBoosting = true;
                speed *= 1.5f;
                Destroy(other.gameObject);
            }
        }

        private void Jump()
        {
            if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
            else
            {
                if (isGrounded())
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                else
                {
                    //If not on the ground and coyote counter bigger than 0 do a normal jump
                    if (coyoteCounter > 0)
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

                //Reset coyote counter to 0 to avoid double jumps
                coyoteCounter = 0;
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
